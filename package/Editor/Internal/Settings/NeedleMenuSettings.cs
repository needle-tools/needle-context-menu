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
	}
}