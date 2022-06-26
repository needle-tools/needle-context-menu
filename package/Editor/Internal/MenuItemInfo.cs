#nullable enable

using System;
using UnityEditor;
using UnityEngine;

namespace Needle
{
	public class MenuItemInfo
	{
		public readonly string Path;
		public GUIContent GUIContent;
		public event Func<MenuItemInfo, bool>? BeforeDisplay;
		public event Func<bool>? BeforeInvoke;

		public void Execute()
		{
			EditorApplication.ExecuteMenuItem(Path);
		}

		public MenuItemInfo(string path, GUIContent guiContent)
		{
			Path = path;
			GUIContent = guiContent;
		}

		internal virtual bool OnBeforeInvoke()
		{
			return BeforeInvoke?.Invoke() ?? true;
		}

		internal virtual bool OnBeforeDisplay()
		{
			return BeforeDisplay?.Invoke(this) ?? true;
		}
	}
}