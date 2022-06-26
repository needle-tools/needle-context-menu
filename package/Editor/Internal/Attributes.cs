using System;

namespace Needle
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ModifyMenuAttribute : Attribute
	{
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class BeforeOpenMenuAttribute : Attribute
	{
	} 


}