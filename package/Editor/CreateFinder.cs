using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Needle;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

internal static class CreateFinder
{
	private static Searcher searcher;

	[BeforeOpenMenu]
	[UsedImplicitly]
	private static bool OpenFinder(Context context)
	{
		if (Event.current.modifiers.HasFlag(EventModifiers.Alt) == false) return true;
		
		if (searcher == null)
		{
			var db = new List<SearcherDatabaseBase>();
			var list = new List<SearcherItem>();
			foreach (var item in context.Items)
			{
				var si = new SearcherItem(item.GUIContent.text);
				si.UserData = item;
				list.Add(si);
			}
			list = SearcherTreeUtility.CreateFromFlatList(list);
			db.Add(new SearcherDatabase(list));
			searcher = new Searcher(db.ToArray(), new MyAdapter(string.Empty));
		}

		typeof(SearcherWindow).GetField("s_Searcher", BindingFlags.Static | BindingFlags.NonPublic)?
			.SetValue(null, searcher);
		typeof(SearcherWindow).GetField("s_ItemSelectedDelegate", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, new Func<SearcherItem, bool>(
			item =>
			{
				if (item != null && item.UserData is MenuItemInfo mi)
				{
					mi.Execute();
					return true;
				}
				return false;
			}));
		var w = ScriptableObject.CreateInstance<SearcherWindow>();
		var rect = w.position;
		var p = Event.current.mousePosition;
		p.x += rect.width * .42f;
		p.y += rect.height * .25f;
		rect.position = p;
		w.position = rect;
		w.ShowPopup();
		const int bw = 4;
		w.rootVisualElement.style.borderBottomWidth = bw;
		w.rootVisualElement.style.borderLeftWidth = bw;
		w.rootVisualElement.style.borderRightWidth = bw;
		w.rootVisualElement.style.borderTopWidth = bw;
		w.rootVisualElement.Query<VisualElement>("searcherVisualContainer").First().style.paddingTop = 6;
		w.rootVisualElement.Query<VisualElement>("smartSearchItem").ForEach(i =>
		{
			Debug.Log(i.style.height);
			i.style.height = 15;
		});
		w.Focus();
		
		Event.current.Use();
		return false;
	}

	private class MyAdapter : SearcherAdapter
	{
		public MyAdapter(string title) : base(title)
		{
		}

		public override bool HasDetailsPanel => false;
		public override bool MultiSelectEnabled => false;
	}
}