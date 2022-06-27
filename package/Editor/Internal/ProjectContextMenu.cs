using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Needle
{
	public static class ProjectContextMenu
	{
		private static readonly object[] beforeOpenArgs = new object[1];
		private static readonly List<MenuItemInfo> items = new List<MenuItemInfo>();

		private static readonly string[] skipMenuItems =
		{
			"Assets/Create/Playables/Playable Asset C# Script "
		};

		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorApplication.projectWindowItemOnGUI += OnGUI;
			var allItems = MenuItemApi.GetProjectMenuItems();
			var start = "Assets/".Length;
			foreach (var item in allItems)
			{
				if (skipMenuItems.Any(i => item.StartsWith(i))) continue;
				var display = item.Substring(start);
				items.Add(new MenuItemInfo(item, new GUIContent(display)));
			}
			var modifyArgs = new object[] { items };
			foreach (var method in TypeCache.GetMethodsWithAttribute<LoadMenu>())
			{
				if (method.IsAbstract || !method.IsStatic) continue;
				method.Invoke(null, modifyArgs);
			}
		}

		private static void OnGUI(string guid, Rect rect)
		{
			if (Event.current.button == 1 && Event.current.type == EventType.ContextClick)
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					var projectWindow = ProjectBrowser.s_LastInteractedProjectBrowser;
					beforeOpenArgs[0] = new Context()
					{
						Guid = guid,
						Rect = rect,
						Window = projectWindow,
						Items = items
					};
					foreach (var method in TypeCache.GetMethodsWithAttribute<BeforeOpenMenu>())
					{
						if (method.IsAbstract || !method.IsStatic) continue;
						var res = method.Invoke(null, beforeOpenArgs);
						if (res is bool b && !b)
						{
							return;
						}
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