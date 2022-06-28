using System;
using UnityEngine;

namespace Needle
{
	internal class ColorScope : IDisposable
	{
		private readonly Color color;

		public ColorScope(Color col)
		{
			this.color = GUI.color;
			GUI.color = col;
		}
		
		public void Dispose()
		{
			GUI.color = this.color;
		}
	}
}