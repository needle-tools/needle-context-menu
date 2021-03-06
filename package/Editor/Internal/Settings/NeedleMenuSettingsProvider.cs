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

		private bool hiddenItemsFoldout
		{
			get => SessionState.GetBool("NeedleMenuSettingsProvider.hiddenItemsFoldout", false);
			set => SessionState.SetBool("NeedleMenuSettingsProvider.hiddenItemsFoldout", value);
		}

		public override void OnGUI(string searchContext) 
		{
			var settings = NeedleMenuSettings.instance;
			using (var ch = new EditorGUI.ChangeCheckScope())
			{
				var changed = false;
				settings.enabled = EditorGUILayout.ToggleLeft("Enabled", settings.enabled);
				settings.hideInactive = EditorGUILayout.ToggleLeft("Hide Inactive", settings.hideInactive);
				settings.sortAlphabetical = EditorGUILayout.ToggleLeft("Sort Alphabetical", settings.sortAlphabetical);

				using (new EditorGUILayout.HorizontalScope())
				{
					hiddenItemsFoldout = EditorGUILayout.Foldout(hiddenItemsFoldout, "Menu Items");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Recommended"))
					{
						changed = true;
						settings.hidden.Clear();
						settings.hidden.AddRange(Recommended.Hidden);
					}
					if (GUILayout.Button("All"))
					{
						changed = true;
						foreach (var it in MenuItemApi.GetProjectMenuItems())
							if (!settings.hidden.Contains(it))
								settings.hidden.Add(it);
					}
					if (GUILayout.Button("None"))
					{
						changed = true;
						settings.hidden.Clear();
					}
				}
				if (hiddenItemsFoldout)
				{
					EditorGUI.indentLevel += 1;
					var disabledColor = new Color(.7f, .7f, .7f);
					foreach (var it in MenuItemApi.GetProjectMenuItems())
					{
						var visible = !settings.hidden.Contains(it);
						using (new ColorScope(visible ? Color.white : disabledColor))
						{
							var newVisible = EditorGUILayout.ToggleLeft(it, visible);
							if (visible != newVisible)
							{
								if (!newVisible)
									settings.hidden.Add(it);
								else
									settings.hidden.Remove(it);
							}
						}
					}
					EditorGUI.indentLevel -= 1;
				}

				if (ch.changed || changed)
				{
					settings.Save();
				}
			}
		}
	}
}