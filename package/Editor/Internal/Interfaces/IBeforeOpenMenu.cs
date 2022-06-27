using System;

namespace Needle
{
	public enum BeforeOpenMenuResponse
	{
		Continue = 0,
		Stop = 1,
	}
	
	public interface IBeforeOpenMenu
	{
		BeforeOpenMenuResponse OnOpenMenu(Context context);
	} 


}