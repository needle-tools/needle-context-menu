﻿using System;
using System.Collections.Generic;
using Needle.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Needle
{
	public class ReorderableMenuItems
	{
		private ReorderableList reorderableList;
		private List<MenuItemElement> items;
		private Rect listRect;

		public void Draw()
		{
			EnsureSetup();
			reorderableList.DoLayoutList();
			if (Event.current.type == EventType.Repaint)
			{
				listRect = GUILayoutUtility.GetLastRect();
			}
		}

		private void EnsureSetup()
		{
			if (reorderableList != null) return;
			var rawItems = MenuItemApi.GetProjectMenuItems();
			this.items = MenuItemElement.CreateHierarchy(rawItems).ToFlatList(true);

			// this.items = rawItems.ToList();
			reorderableList = new ReorderableList(this.items, typeof(MenuItemElement), true, false, false, false);
			reorderableList.drawElementCallback += OnDrawElement;
			reorderableList.onMouseDragCallback += OnDrag;
			reorderableList.onAddDropdownCallback += OnDrop;
			reorderableList.onReorderCallbackWithDetails += OnReorder;
			reorderableList.onChangedCallback += OnChange;
		}

		private void OnChange(ReorderableList list)
		{
		}

		private int indexClicked;

		private DateTime indexClickedTime;
		private int indexEditing = -1;
		private int currentMouseOver;

		private bool isDragging;
		private MenuItemElement draggingElement;

		private readonly List<Rect> rects = new List<Rect>();
		private readonly List<MenuItemElement> beforeReorder = new List<MenuItemElement>();
		private readonly Color hiddenColor = new Color(.7f, .7f, .7f);

		private void OnDrag(ReorderableList list)
		{
			if (!isDragging)
			{
				beforeReorder.Clear();
				beforeReorder.AddRange(items);
			}
			isDragging = true;
			draggingElement = items[list.index];
		}

		private void OnDrop(Rect rect, ReorderableList list)
		{
			isDragging = false;
		}

		private void OnReorder(ReorderableList list, int oldIndex, int newIndex)
		{
			if (draggingElement != null && newIndex > 0)
			{
				var item = draggingElement;
				var prev = this.beforeReorder[newIndex];
				prev.Parent?.Add(item);
			}
			draggingElement = null;
		}

		private void OnDrawElement(Rect rect, int index, bool active, bool focused)
		{
			if (Event.current.type == EventType.MouseUp)
			{
				currentMouseOver = -1;
				isDragging = false;
			}

			if (isDragging) indexEditing = -1;

			var item = this.items[index];

			// handle item being reordered

			if (Event.current.type == EventType.Layout)
			{
				if (rects.Count <= index) rects.Add(rect);
				else rects[index] = rect;
			}
			
			if (Event.current.type == EventType.Repaint && draggingElement == item)
			{
				var pos = Event.current.mousePosition.y - this.listRect.yMin;
				var intersect = rects.FindIndex(r => pos > r.yMin && pos < r.yMax);
				if (intersect > 0) currentMouseOver = intersect;
			}


			// if (Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseUp)
			if (indexEditing == index)
			{
				EditorGUI.TextField(rect, item.ComputeFullPath());
			}
			else
			{
				var label = default(string);
				var itemIsBeingDragged = isDragging && active;
				if (draggingElement == item && itemIsBeingDragged)
				{
					if (currentMouseOver > 0)
						label = items[currentMouseOver].Parent.ComputeFullPath() + "/" + item.Name;
				}

				if (label == null) label = item.ComputeFullPath();

				using (new ColorScope(item.Hidden && !itemIsBeingDragged ? hiddenColor : Color.white))
				{
					// GUI.Label(rect, label);
					item.Hidden = !GUI.Toggle(rect, !item.Hidden, label);
				}
			}


			switch (Event.current.type)
			{
				case EventType.MouseMove:
					if (MouseIntersects()) currentMouseOver = index;
					break;
				case EventType.MouseDown:
					if (MouseIntersects())
					{
						var isDoubleClick = DateTime.Now - indexClickedTime < TimeSpan.FromSeconds(.3);
						if (indexClicked == index && isDoubleClick)
						{
							indexClicked = -1;
							indexEditing = index;
						}
						else
						{
							indexEditing = -1;
							indexClicked = index;
							indexClickedTime = DateTime.Now;
						}
					}
					break;
				case EventType.ContextClick:
					if (MouseIntersects())
					{
						Event.current.Use();
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Duplicate"), false, () => { items.Insert(index, items[index]); });
						menu.ShowAsContext();
					}
					break;
			}

			bool MouseIntersects()
			{
				return rect.Contains(Event.current.mousePosition);
			}
		}
	}
}