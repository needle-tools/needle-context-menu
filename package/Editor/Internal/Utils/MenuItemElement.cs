using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Needle.Utils
{
	public class MenuItemElement
	{
		public string Name;
		public MenuItemElement Parent;
		public readonly List<MenuItemElement> Children = new List<MenuItemElement>();

		public string ComputeFullPath()
		{
			var str = Name;
			var par = Parent;
			while (par != null)
			{
				str = par.Name + "/" + str;
				par = par.Parent;
			}
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
	}
}