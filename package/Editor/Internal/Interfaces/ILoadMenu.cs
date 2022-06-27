using System.Collections.Generic;

namespace Needle
{
	public interface ILoadMenu
	{
		void OnModifyCollectedItems(List<MenuItemInfo> items);
	}
}