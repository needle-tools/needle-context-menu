using System.Collections.Generic;
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
	private static bool OpenFinder(EditorWindow window, IList<MenuItemInfo> items)
	{
		if (Event.current.modifiers.HasFlag(EventModifiers.Alt) == false) return true;
		
		if (searcher == null)
		{
			var db = new List<SearcherDatabaseBase>();
			var list = new List<SearcherItem>();
			foreach (var item in items)
				list.Add(new SearcherItem(item.GUIContent.text));
			db.Add(new SearcherDatabase(SearcherTreeUtility.CreateFromFlatList(list)));
			searcher = new Searcher(db.ToArray(), new MyAdapter(""));
		}

		typeof(SearcherWindow).GetField("s_Searcher", BindingFlags.Static | BindingFlags.NonPublic)?
			.SetValue(null, searcher);
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