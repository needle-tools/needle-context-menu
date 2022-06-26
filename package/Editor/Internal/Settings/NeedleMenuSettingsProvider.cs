using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Needle
{
	public class NeedleMenuSettingsProvider : SettingsProvider
	{
		public const string SettingsPath = "Project/Editor/Menu";

		[SettingsProvider]
		public static SettingsProvider CreateSettings()
		{
			try
			{
				NeedleMenuSettings.instance.Save();
				return new NeedleMenuSettingsProvider(SettingsPath, SettingsScope.Project);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			return null;
		}

		private NeedleMenuSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
		{
		}

		public override void OnGUI(string searchContext)
		{
			var settings = NeedleMenuSettings.instance;
			using (var ch = new EditorGUI.ChangeCheckScope())
			{
				settings.hideInactive = EditorGUILayout.ToggleLeft("Hide Inactive", settings.hideInactive);
				settings.sortAlphabetical = EditorGUILayout.ToggleLeft("Sort Alphabetical", settings.sortAlphabetical);
				if (ch.changed)
				{
					settings.Save();
				}
			}
		}
	}
}