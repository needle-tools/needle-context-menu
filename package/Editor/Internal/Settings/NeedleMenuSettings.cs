using System.Collections.Generic;
using Needle.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Needle
{
	[FilePath("ProjectSettings/NeedleMenuSettings.asset", FilePathAttribute.Location.ProjectFolder)]
	public class NeedleMenuSettings : ScriptableSingleton<NeedleMenuSettings>
	{
		public bool hideInactive = true;
		public bool sortAlphabetical = true;
		
		internal void Save()
		{
			Undo.RegisterCompleteObjectUndo(this, "Save Needle Exporter Settings");
			base.Save(true);
		}

		public List<string> hidden = new List<string>();

		public List<MenuItemModel> projectMenuItems = new List<MenuItemModel>();
	}
}