using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;

namespace Needle
{
	internal static class InstanceUtils
	{
		public static IList<T> GetInstances<T>()
		{
			var res = new List<T>();
			var types = TypeCache.GetTypesDerivedFrom<T>();
			foreach (var type in types)
			{
				if (type.IsAbstract || type.IsInterface) continue;
				if (typeof(UnityEngine.Object).IsAssignableFrom(type)) continue;
				// create instance without calling the constructor
				var inst = (T)FormatterServices.GetUninitializedObject(type);
				res.Add(inst);
			}
			return res;
		}
	}
}