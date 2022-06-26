using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Needle
{
	public class Context
	{
		public string Guid;
		public Rect Rect;
		public EditorWindow Window;
		public List<MenuItemInfo> Items;
	}
}