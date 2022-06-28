using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.APIUpdaterExtensions;
using UnityEngine.UIElements;

namespace Needle.Utils
{
	public class MenuItemElement
	{
		public string Name;
		public bool Hidden;
		public string Key => OriginalPath;
		public MenuItemElement Parent;
		public readonly List<MenuItemElement> Children = new List<MenuItemElement>();
		public string OriginalPath { get; private set; }

		public bool IsLeave => ChildCount <= 0;
		public int ChildCount => Children?.Count ?? 0;

		public int GetIndex()
		{
			return Parent?.Children.IndexOf(this) ?? 0;
		}

		public string ComputeFullPath()
		{
			if (Name == "Assets" && Parent == null) return null;
			var str = Name;
			var par = Parent?.ComputeFullPath();
			if (par != null) return par + "/" + str;
			return str;
		} 

		public void Add(MenuItemElement el, int index = -1)
		{
			el.RemoveFromParent();
			el.Parent = this;
			if (index >= 0 && index < Children.Count)
			{
				Children.Insert(index, el);
			}
			else
			{
				Children.Add(el);
			}
		}

		public void RemoveFromParent()
		{
			if (Parent != null) Parent.Children?.Remove(this);
			Parent = null;
		}

		public static List<MenuItemElement> CreateProjectMenuItems()
		{
			var dict = new Dictionary<string, MenuItemModel>();
			var saved = NeedleMenuSettings.instance.projectMenuItems;
			foreach (var model in saved)
			{
				if (!string.IsNullOrEmpty(model.Key) && !dict.ContainsKey(model.Key))
				{
					dict.Add(model.Key, model);
				}
			}
			var allOptions = MenuItemApi.GetProjectMenuItems();
			var list = CreateHierarchy(allOptions, dict).ToFlatList(true);

			return list;
		}
		
		private static readonly string[] skipMenuItems =
		{
			"Assets/Create/Playables/Playable Asset C# Script "
		};

		public static List<MenuItemElement> CreateHierarchy(IEnumerable<string> items, IDictionary<string, MenuItemModel> models = null)
		{
			var list = new List<MenuItemElement>();
			foreach (var item in items)
			{
				if (skipMenuItems.Contains(item)) continue;
				var model = default(MenuItemModel);
				if (models != null)
					models.TryGetValue(item, out model);
				var path = model?.Path ?? item;

				var elements = path.Split('/');
				MenuItemElement current = null;
				for (var index = 0; index < elements.Length; index++)
				{
					var part = elements[index]; 
					var currentList = current?.Children ?? list;
					if (current == null) current = currentList.FirstOrDefault(i => i.Name == part);
					var isLastElement = index == elements.Length - 1;

					var it = new MenuItemElement(); 
					it.Name = part;
					it.OriginalPath = item;
					it.Hidden = model?.Hidden ?? false;
					if (current != null && index > 0)
					{
						var insertAt = -1;
						if(isLastElement && model != null) insertAt = model.Index;
						current.Add(it, insertAt);
					}
					else list.Add(it);
					current = it;
				}
				
			}
			return list;
		}
	}

	public static class MenuItemElementExtensions
	{
		public static List<MenuItemElement> ToFlatList(this List<MenuItemElement> list, bool leavesOnly)
		{
			var flatList = new List<MenuItemElement>();
			foreach (var it in list) RecursiveFlatten(it, flatList, leavesOnly);
			return flatList;  
		}

		public static void SaveAsProjectMenuItems(this IEnumerable<MenuItemElement> list)
		{
			var models = new List<MenuItemModel>();
			foreach (var el in list)
			{
				var model = new MenuItemModel();
				model.Key = el.Key;
				model.Hidden = el.Hidden;
				model.Path = el.ComputeFullPath();
				model.Index = el.GetIndex();
				models.Add(model);
			}
			NeedleMenuSettings.instance.projectMenuItems = models;
			NeedleMenuSettings.instance.Save();
		}

		private static void RecursiveFlatten(MenuItemElement element, IList<MenuItemElement> flatList, bool leavesOnly)
		{
			if (!leavesOnly || element.IsLeave)
				flatList.Add(element);
			if (element.Children != null)
			{
				foreach (var ch in element.Children)
				{
					RecursiveFlatten(ch, flatList, leavesOnly);
				}
			}
		}
	}
}