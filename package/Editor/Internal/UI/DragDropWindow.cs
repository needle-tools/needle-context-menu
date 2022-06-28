using System;
using System.Collections.Generic;
using Needle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropWindow : EditorWindow, ICanRebuild
{
	[MenuItem("Needle/DrawDrop")]
	public static void ShowExample()
	{
		var wnd = GetWindow<DragAndDropWindow>();
		wnd.titleContent = new GUIContent("Drag And Drop");
	}

	private static string UxmlPath => AssetDatabase.GUIDToAssetPath("a6eb1fa81c044ee1b62844c5ce3a19f4");

	private void OnEnable()
	{
		UxmlWatcher.Register(UxmlPath, this);
	}

	public void CreateGUI()
	{
		Rebuild();
	}

	private VisualElement element = null;
	private DragAndDropManipulator manipulator;

	public void Rebuild()
	{
		Debug.Log("Build window");
		
		if(element != null) element.RemoveFromHierarchy();

		var root = rootVisualElement;

		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
		element = visualTree.Instantiate();
		root.Add(element);
		
		new DragAndDropManipulator(rootVisualElement.Q<VisualElement>("object"), element);
		new DragAndDropManipulator(rootVisualElement.Q<VisualElement>("object2"), element);
	}

	private void BuildItems(List<string> menuItems)
	{
		
	}
}