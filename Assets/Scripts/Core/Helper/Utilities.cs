using System;

namespace Utilities
{
	public static class Utilities
	{
		public static void EnumerateArray2D(this Array array, Action<int, int, Array> action)
		{
			for (var x = 0; x < array.GetLength(0); x++)
			for (var y = 0; y < array.GetLength(1); y++)
				action(x, y, array);
		}
	}
}