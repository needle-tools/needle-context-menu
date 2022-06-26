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
		private static readonly object[] beforeOpenArgs = new object[2];
		private static readonly IList<MenuItemInfo> items = new List<MenuItemInfo>();

		private static readonly string[] skipMenuItems =
		{
			"Assets/Create/Playables/Playable Asset C# Script "
		};

		[InitializeOnLoadMethod]
		private static void Init()
		{
			
			EditorApplication.projectWindowItemOnGUI += OnGUI;
			var allItems = Unsupported.GetSubmenus("Assets");
			var start = "Assets/".Length;
			foreach (var item in allItems)
			{
				if (skipMenuItems.Any(i => item.StartsWith(i))) continue;
				var display = item.Substring(start);
				items.Add(new MenuItemInfo(item, new GUIContent(display)));
			}
			var modifyArgs = new object[] { items };
			foreach (var method in TypeCache.GetMethodsWithAttribute<ModifyMenuAttribute>())
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
					beforeOpenArgs[0] = projectWindow;
					beforeOpenArgs[1] = items;
					foreach (var method in TypeCache.GetMethodsWithAttribute<BeforeOpenMenuAttribute>())
					{
						if (method.IsAbstract || !method.IsStatic) continue;
						var res = method.Invoke(null, beforeOpenArgs);
						if (res is bool b && !b)
						{
							return;
						}
					}
					Event.current.Use();
					var menu = new GenericMenu();
					for (var index = 0; index < items.Count; index++)
					{
						var it = items[index];
						if (!it.OnBeforeDisplay()) continue;

						var isActive = EditorApplication.ValidateMenuItem(it.Path);
						if (!isActive)
						{
							menu.AddDisabledItem(it.GUIContent, false);
						}
						else
						{
							menu.AddItem(it.GUIContent, false, () =>
							{
								if (!it.OnBeforeInvoke()) return;
								EditorApplication.ExecuteMenuItem(it.Path);
							});
						}
					}
					menu.ShowAsContext();
				}
			}
		}
	}
}