using System.Collections.Generic;
using Needle;
using UnityEngine;

internal class AddCopyGuid : ILoadMenu
{
	public void OnModifyCollectedItems(List<MenuItemInfo> items)
	{
		var it = new MenuItemInfo("Add Copy Guid", new GUIContent("Copy Guid"));
		it.BeforeInvoke += guid =>
		{
			GUIUtility.systemCopyBuffer = guid;
			return false;
		};
		items.Add(it);
	}
}