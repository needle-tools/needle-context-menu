using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Needle.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Needle
{
	public static class ProjectContextMenu
	{
		private static readonly List<MenuItemInfo> items = new List<MenuItemInfo>();
		private static IList<ILoadMenu> loadMenu;
		private static IList<IBeforeOpenMenu> beforeOpenMenu;

		internal static List<MenuItemElement> menuItems = new List<MenuItemElement>();

		private static readonly string[] skipMenuItems =
		{
			"Assets/Create/Playables/Playable Asset C# Script "
		};

		[InitializeOnLoadMethod]
		private static async void Init()
		{
			while(EditorApplication.isUpdating || EditorApplication.isCompiling) await Task.Delay(100);
			EditorApplication.projectWindowItemOnGUI += OnGUI;
			var allItems = MenuItemApi.GetProjectMenuItems();

			var start = "Assets/".Length;
			for (var index = 0; index < allItems.Length; index++)
			{
				var item = allItems[index];
				if (skipMenuItems.Any(i => item.StartsWith(i))) continue;
				var display = item.Substring(start);
				items.Add(new MenuItemInfo(item, new GUIContent(display)));
			}

			beforeOpenMenu = InstanceUtils.GetInstances<IBeforeOpenMenu>();
			loadMenu = InstanceUtils.GetInstances<ILoadMenu>();

			foreach (var load in loadMenu)
			{
				load.OnModifyCollectedItems(items);
			} 
		}


		private static void OnGUI(string guid, Rect rect)
		{
			if (Event.current.button == 1 && Event.current.type == EventType.ContextClick)
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					var projectWindow = ProjectBrowser.s_LastInteractedProjectBrowser;
					var context = new Context()
					{
						Guid = guid,
						Rect = rect,
						Window = projectWindow,
						Items = items
					};
					foreach (var beforeOpen in beforeOpenMenu)
					{
						var res = beforeOpen.OnOpenMenu(context);
						if (res == BeforeOpenMenuResponse.Stop)
							return;
					}
					Event.current.Use();
					var settings = NeedleMenuSettings.instance;
					var menu = new GenericMenu();
					char? lastChar = null;
					for (var index = 0; index < items.Count; index++)
					{
						var it = items[index];
						if (settings.hidden.Contains(it.Path)) continue;
						if (!it.OnBeforeDisplay()) continue;
						
						var isActive = it.Validate();
						if (!isActive)
						{
							if (!settings.hideInactive)
							{
								OnBeforeAddItem();
								menu.AddDisabledItem(it.GUIContent, false);
							}
						}
						else
						{
							OnBeforeAddItem();
							menu.AddItem(it.GUIContent, false, () =>
							{
								if (!it.OnBeforeInvoke(guid)) return;
								EditorApplication.ExecuteMenuItem(it.Path);
							});
						}

						void OnBeforeAddItem()
						{
							var displayPath = it.GUIContent.text;
							var slashIndex = displayPath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
							if (slashIndex > 0 && slashIndex + 1 < displayPath.Length)
							{
								var nextChar = displayPath[slashIndex + 1];
								if (lastChar != null && lastChar != nextChar)
								{
									var sep = displayPath.Substring(0, slashIndex +1 );
									menu.AddSeparator(sep);
								}
								lastChar = nextChar;
							}

						}
					}
					menu.ShowAsContext();
				}
			}
		}
	}
}