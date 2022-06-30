using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Needle
{
	[FilePath("ProjectSettings/NeedleMenuSettings.asset", FilePathAttribute.Location.ProjectFolder)]
	public class NeedleMenuSettings : ScriptableSingleton<NeedleMenuSettings>
	{
		public bool enabled = true;
		public bool hideInactive = true;
		public bool sortAlphabetical = true;
		
		internal void Save()
		{
			Undo.RegisterCompleteObjectUndo(this, "Save Needle Exporter Settings");
			base.Save(true);
		}

		public List<string> hidden = new List<string>();
	}
}