using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental;

namespace Needle.Tiny.Utils
{
	public interface ICanRebuild
	{
		void Rebuild();
	}

	/// <summary>
	/// Utility class to rebuild windows using uxml files on uxml change due to missing or not working live reload option
	/// </summary>
	public class UxmlWatcher
	{
		private static readonly Dictionary<string, List<ICanRebuild>> watching = new Dictionary<string, List<ICanRebuild>>();

		public static void RegisterGUID(string uxmlPath, ICanRebuild element)
		{
			Register(AssetDatabase.GUIDToAssetPath(uxmlPath), element);
		}
		
		public static void Register(string uxmlPath, ICanRebuild window)
		{
			if(!watching.ContainsKey(uxmlPath)) watching.Add(uxmlPath, new List<ICanRebuild>());
			watching[uxmlPath].Add(window);
		}

		[UsedImplicitly]
		private class UxmlImportWatcher : AssetsModifiedProcessor
		{
			protected override async void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
			{
				do await Task.Delay(50);
				while (EditorApplication.isUpdating);
				
				foreach (var ch in changedAssets)
				{
					if (ch.EndsWith(".uxml", StringComparison.Ordinal))
					{
						if (watching.TryGetValue(ch, out var watched))
						{
							foreach (var reb in watched)
							{
								reb.Rebuild();
							}
						}
					}
				}
			}
		}
	}
}