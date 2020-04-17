using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Gamelogic.Extensions
{
	public static class PhysicsExtensions
	{
		public static Vector2 XAxisProj(this Bounds b)
		{
			return new Vector2(b.center.x - b.extents.x, b.center.x + b.extents.x);
		}

		public static Vector2 YAxisProj(this Bounds b)
		{
			return new Vector2(b.center.y - b.extents.y, b.center.y + b.extents.y);
		}

		public static Vector2 ZAxisProj(this Bounds b)
		{
			return new Vector2(b.center.z - b.extents.z, b.center.z + b.extents.z);
		}

		public static bool Contains(this Bounds b, Bounds bounds)
		{
			return b.XAxisProj().InRangeOfInc(bounds.XAxisProj()) &&
				b.YAxisProj().InRangeOfInc(bounds.YAxisProj()) &&
				b.ZAxisProj().InRangeOfInc(bounds.ZAxisProj());
		}

		public static bool ContainsXY(this Bounds b, Bounds bounds)
		{
//			return b.XAxisProj().InRangeOfInc(bounds.XAxisProj()) &&
//				b.YAxisProj().InRangeOfInc(bounds.YAxisProj());
			return bounds.XAxisProj().InRangeOfInc(b.XAxisProj()) &&
				bounds.YAxisProj().InRangeOfInc(b.YAxisProj());
		}
		
		public static bool ContainsXZ(this Bounds b, Bounds bounds)
		{
			return bounds.XAxisProj().InRangeOfInc(b.XAxisProj()) &&
				bounds.ZAxisProj().InRangeOfInc(b.ZAxisProj());
		}
	}
}