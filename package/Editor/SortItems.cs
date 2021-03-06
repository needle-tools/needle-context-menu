using System;
using System.Collections.Generic;
using System.Linq;
using Needle;

internal class SortItems : ILoadMenu
{
	public void OnModifyCollectedItems(List<MenuItemInfo> items)
	{
		if (NeedleMenuSettings.instance.sortAlphabetical)
			items.Sort((a, b) => GetOrder(a.Path, b.Path));
	}

	private static int GetOrder(string str1, string str2)
	{ 
		return string.Compare(str1, str2, StringComparison.Ordinal);
	}
}