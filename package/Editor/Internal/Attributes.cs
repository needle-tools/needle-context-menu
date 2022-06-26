using System;

namespace Needle
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class LoadMenu : Attribute
	{
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class BeforeOpenMenu : Attribute
	{
	} 


}