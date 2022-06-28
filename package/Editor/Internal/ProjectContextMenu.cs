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
		private static IList<ILoadMenu> loadMenu;
		private static IList<IBeforeOpenMenu> beforeOpenMenu;
		internal static List<MenuItemElement> menuItems { get; private set; }


		[InitializeOnLoadMethod]
		private static async void Init()
		{
			// wait for editor to finish loading
			while(EditorApplication.isUpdating || EditorApplication.isCompiling) await Task.Delay(100);
			
			menuItems = MenuItemElement.CreateProjectMenuItems();
			EditorApplication.projectWindowItemOnGUI += OnGUI;
			
			beforeOpenMenu = InstanceUtils.GetInstances<IBeforeOpenMenu>();
			loadMenu = InstanceUtils.GetInstances<ILoadMenu>();

			// foreach (var load in loadMenu)
			// {
			// 	load.OnModifyCollectedItems(items);
			// } 
		}


		private static void OnGUI(string guid, Rect rect)
		{
			if (Event.current.button == 1 && Event.current.type == EventType.ContextClick)
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					var projectWindow = ProjectBrowser.s_LastInteractedProjectBrowser;
					// var context = new Context()
					// {
					// 	Guid = guid,
					// 	Rect = rect,
					// 	Window = projectWindow,
					// 	Items = items
					// };
					// foreach (var beforeOpen in beforeOpenMenu)
					// {
					// 	var res = beforeOpen.OnOpenMenu(context);
					// 	if (res == BeforeOpenMenuResponse.Stop)
					// 		return;
					// }
					Event.current.Use();
					var settings = NeedleMenuSettings.instance;
					var menu = new GenericMenu();
					char? lastChar = null;
					for (var index = 0; index < menuItems.Count; index++)
					{
						var it = menuItems[index];
						if (it.Hidden) continue;
						// if (!it.OnBeforeDisplay()) continue;
						 
						// var isActive = it.Validate();
						// if (!isActive)
						// { 
						// 	if (!settings.hideInactive)
						// 	{
						// 		OnBeforeAddItem();
						// 		menu.AddDisabledItem(it.GUIContent, false);
						// 	}
						// }
						// else
						{
							// OnBeforeAddItem();
							menu.AddItem(new GUIContent(it.ComputeFullPath()), false, () =>
							{
								// if (!it.OnBeforeInvoke(guid)) return;
								EditorApplication.ExecuteMenuItem(it.OriginalPath); 
							});
						}

						void OnBeforeAddItem()
						{
							// var displayPath = it.GUIContent.text;
							// var slashIndex = displayPath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
							// if (slashIndex > 0 && slashIndex + 1 < displayPath.Length)
							// {
							// 	var nextChar = displayPath[slashIndex + 1];
							// 	if (lastChar != null && lastChar != nextChar)
							// 	{
							// 		var sep = displayPath.Substring(0, slashIndex +1 );
							// 		menu.AddSeparator(sep);
							// 	}
							// 	lastChar = nextChar;
							// }
						
						}
					}
					menu.ShowAsContext();
				}
			}
		}
	}
}