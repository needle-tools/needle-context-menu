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
		public event Func<string, bool>? BeforeInvoke;

		public void Execute()
		{
			EditorApplication.ExecuteMenuItem(Path);
		}

		public MenuItemInfo(string path, GUIContent guiContent)
		{
			Path = path;
			GUIContent = guiContent;
		}

		internal virtual bool OnBeforeInvoke(string guid)
		{
			return BeforeInvoke?.Invoke(guid) ?? true;
		}

		internal virtual bool OnBeforeDisplay()
		{
			return BeforeDisplay?.Invoke(this) ?? true;
		}

		internal bool Validate()
		{
			if (BeforeInvoke != null) return true;
			return EditorApplication.ValidateMenuItem(Path);
		}
	}
}