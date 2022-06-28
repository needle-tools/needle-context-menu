﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Needle.Utils
{
	public class MenuItemElement
	{
		public string Name;
		public MenuItemElement Parent;
		public readonly List<MenuItemElement> Children = new List<MenuItemElement>();
		public string OriginalPath;

		public bool IsLeave => ChildCount <= 0;
		public int ChildCount => Children?.Count ?? 0;

		public string ComputeFullPath()
		{
			var str = Name;
			var par = Parent?.ComputeFullPath();
			if (par != null) return par + "/" + str;
			return str;
		}

		public void Add(MenuItemElement el)
		{
			el.RemoveFromParent();
			el.Parent = this;
			Children.Add(el);
		}

		public void RemoveFromParent()
		{
			if (Parent != null) Parent.Children?.Remove(this);
			Parent = null;
		}

		public static List<MenuItemElement> CreateHierarchy(IEnumerable<string> items)
		{
			var list = new List<MenuItemElement>();
			foreach (var item in items)
			{
				var elements = item.Split('/');
				MenuItemElement current = null;
				for (var index = 0; index < elements.Length; index++)
				{
					var part = elements[index];
					var currentList = current?.Children ?? list;
					if (current == null) current = currentList.FirstOrDefault(i => i.Name == part);

					var it = new MenuItemElement();
					it.Name = part;
					it.OriginalPath = item;
					if (current != null && index > 0) current.Add(it);
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