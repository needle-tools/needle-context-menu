using UnityEditor;

namespace Needle
{
	public static class MenuItemApi
	{
		public static string[] GetProjectMenuItems()
		{
			return Unsupported.GetSubmenus("Assets");
		}
		
		public static string[] GetProjectMenuItemsCommands()
		{
			return Unsupported.GetSubmenusCommands("Assets");
		}
	}
}