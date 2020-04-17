// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Contains useful extension methods for vectors.
	/// </summary>
	[Version(1)]
	public static class VectorExtensions
	{
		#region Static Methods		

		/// <summary>
		/// Returns a copy of this vector with the given x-coordinate.
		/// </summary>
		[Version(2)]
		public static Vector2 WithX(this Vector2 vector, float x)
		{
			return new Vector2(x, vector.y);
		}

		/// <summary>
		/// Returns a copy of this vector with the given y-coordinate.
		/// </summary>
		[Version(2)]
		public static Vector2 WithY(this Vector2 vector, float y)
		{
			return new Vector2(vector.x, y);
		}

		/// <summary>
		/// Returns a copy of this vector with the given x-coordinate.
		/// </summary>
		[Version(2)]
		public static Vector3 WithX(this Vector3 vector, float x)
		{
			return new Vector3(x, vector.y, vector.z);
		}

		/// <summary>
		/// Returns a copy of this vector with the given y-coordinate.
		/// </summary>
		[Version(2)]
		public static Vector3 WithY(this Vector3 vector, float y)
		{
			return new Vector3(vector.x, y, vector.z);
		}

		/// <summary>
		/// Returns a copy of this vector with the given z-coordinate.
		/// </summary>
		[Version(2)]
		public static Vector3 WithZ(this Vector3 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		/// <summary>
		/// Returns a copy of the vector with the x-coordinate incremented
		/// with the given value.
		/// </summary>
		public static Vector2 WithIncX(this Vector2 vector, float xInc)
		{
			return new Vector2(vector.x + xInc, vector.y);
		}

		/// <summary>
		/// Returns a copy of the vector with the y-coordinate incremented
		/// with the given value.
		/// </summary>
		public static Vector2 WithIncY(this Vector2 vector, float yInc)
		{
			return new Vector2(vector.x, vector.y + yInc);
		}

		/// <summary>
		/// Returns a copy of the vector with the x-coordinate incremented
		/// with the given value.
		/// </summary>
		public static Vector3 WithIncX(this Vector3 vector, float xInc)
		{
			return new Vector3(vector.x + xInc, vector.y, vector.z);
		}

		/// <summary>
		/// Returns a copy of the vector with the y-coordinate incremented
		/// with the given value.
		/// </summary>
		public static Vector3 WithIncY(this Vector3 vector, float yInc)
		{
			return new Vector3(vector.x, vector.y + yInc, vector.z);
		}

		/// <summary>
		/// Returns a copy of the vector with the z-coordinate incremented
		/// with the given value.
		/// </summary>
		public static Vector3 WithIncZ(this Vector3 vector, float zInc)
		{
			return new Vector3(vector.x, vector.y, vector.z + zInc);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the x and z coordinates, and the given value for the y coordinate.
		/// </summary>
		public static Vector3 To3DXZ(this Vector2 vector, float y)
		{
			return new Vector3(vector.x, y, vector.y);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the x and z coordinates, and 0 for the y coordinate.
		/// </summary>
		public static Vector3 To3DXZ(this Vector2 vector)
		{
			return vector.To3DXZ(0);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the x and y coordinates, and the given value for the z coordinate.
		/// </summary>
		public static Vector3 To3DXY(this Vector2 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the x and y coordinates, and 0 for the z coordinate.
		/// </summary>
		public static Vector3 To3DXY(this Vector2 vector)
		{
			return vector.To3DXY(0);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the y and z coordinates, and the given value for the x coordinate.
		/// </summary>
		public static Vector3 To3DYZ(this Vector2 vector, float x)
		{
			return new Vector3(x, vector.x, vector.y);
		}

		/// <summary>
		/// Converts a 2D vector to a 3D vector using the vector 
		/// for the y and z coordinates, and 0 for the x coordinate.
		/// </summary>
		public static Vector3 To3DYZ(this Vector2 vector)
		{
			return vector.To3DYZ(0);
		}

		/// <summary>
		/// Converts a 3D vector to a 2D vector taking the x and z coordinates.
		/// </summary>
		public static Vector2 To2DXZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		/// <summary>
		/// Converts a 3D vector to a 2D vector taking the x and y coordinates.
		/// </summary>
		public static Vector2 To2DXY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		/// <summary>
		/// Converts a 3D vector to a 2D vector taking the y and z coordinates.
		/// </summary>
		public static Vector2 To2DYZ(this Vector3 vector)
		{
			return new Vector2(vector.y, vector.z);
		}

		/// <summary>
		/// Swaps the x and y coordinates of the vector.
		/// </summary>
		public static Vector2 YX(this Vector2 vector)
		{
			return new Vector2(vector.y, vector.x);
		}

		/// <summary>
		/// Creates a new vector by permuting the given vector's coordinates in the order YZX.
		/// </summary>
		public static Vector3 YZX(this Vector3 vector)
		{
			return new Vector3(vector.y, vector.z, vector.x);
		}

		/// <summary>
		/// Creates a new vector by permuting the given vector's coordinates in the order XZY.
		/// </summary>
		public static Vector3 XZY(this Vector3 vector)
		{
			return new Vector3(vector.x, vector.z, vector.y);
		}

		/// <summary>
		/// Creates a new vector by permuting the given vector's coordinates in the order ZXY.
		/// </summary>
		public static Vector3 ZXY(this Vector3 vector)
		{
			return new Vector3(vector.z, vector.x, vector.y);
		}

		/// <summary>
		/// Creates a new vector by permuting the given vector's coordinates in the order YXZ.
		/// </summary>
		[Version(1, 4, 3)]
		public static Vector3 YXZ(this Vector3 vector)
		{
			return new Vector3(vector.y, vector.x, vector.z);
		}

		/// <summary>
		/// Creates a new vector by permuting the given vector's coordinates in the order ZYX.
		/// </summary>
		[Version(1,4,3)]
		public static Vector3 ZYX(this Vector3 vector)
		{
			return new Vector3(vector.z, vector.y, vector.x);
		}

		/// <summary>
		/// Reflects the vector about x-axis.
		/// </summary>
		[Version(2, 1)]
		public static Vector2 ReflectAboutX(this Vector2 vector)
		{
			return new Vector2(vector.x, -vector.y);
		}

		/// <summary>
		/// Reflects the vector about y-axis.
		/// </summary>

		[Version(2, 1)]
		public static Vector2 ReflectAboutY(this Vector2 vector)
		{
			return new Vector2(-vector.x, vector.y);
		}
		
		/// <summary>
		/// Rotates a vector by a given angle.
		/// </summary>
		/// <param name="vector">vector to rotate</param>
		/// <param name="angleInDeg">angle in degrees.</param>
		/// <returns>Rotated vector.</returns>
		public static Vector2 Rotate(this Vector2 vector, float angleInDeg)
		{
			float angleInRad = Mathf.Deg2Rad * angleInDeg;
			float cosAngle = Mathf.Cos(angleInRad);
			float sinAngle = Mathf.Sin(angleInRad);

			float x = vector.x * cosAngle - vector.y * sinAngle;
			float y = vector.x * sinAngle + vector.y * cosAngle;

			return new Vector2(x, y);
		}

		/// <summary>
		/// Rotates a vector by a given angle around a given point.
		/// </summary>
		public static Vector2 RotateAround(this Vector2 vector, float angleInDeg, Vector2 axisPosition)
		{
			return (vector - axisPosition).Rotate(angleInDeg) + axisPosition;
		}

		/// <summary>
		/// Rotates a vector by a 90 degrees.
		/// </summary>
		public static Vector2 Rotate90(this Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x);
		}

		/// <summary>
		/// Rotates a vector by a 180 degrees.
		/// </summary>
		public static Vector2 Rotate180(this Vector2 vector)
		{
			return new Vector2(-vector.x, -vector.y);
		}

		/// <summary>
		/// Rotates a vector by a 270 degrees.
		/// </summary>
		public static Vector2 Rotate270(this Vector2 vector)
		{
			return new Vector2(vector.y, -vector.x);
		}

		/// <summary>
		/// Returns the vector rotated 90 degrees counter-clockwise.
		/// </summary>
		/// <remarks>
		/// 	<para>The returned vector is always perpendicular to the given vector. </para>
		/// 	<para>The perp dot product can be calculated using this: <c>var perpDotPorpduct = Vector2.Dot(v1.Perp(), v2);</c></para>
		/// </remarks>
		/// <param name="vector"></param>
		public static Vector2 Perp(this Vector2 vector)
		{
			return vector.Rotate90();
		}

		/// <summary>
		/// Equivalent to Vector2.Dot(v1.Perp(), v2).
		/// </summary>
		/// <param name="vector1">The first operand.</param>
		/// <param name="vector2">The second operand.</param>
		/// <returns>Vector2.</returns>
		public static float PerpDot(this Vector2 vector1, Vector2 vector2)
		{
			return -vector1.y*vector2.x + vector1.x*vector2.y;
		}

		/// <summary>
		/// Equivalent to Vector2.Dot(v1, v2).
		/// </summary>
		/// <param name="vector1">The first operand.</param>
		/// <param name="vector2">The second operand.</param>
		/// <returns>Vector2.</returns>
		public static float Dot(this Vector2 vector1, Vector2 vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y;
		}

		/// <summary>
		/// Equivalent to Vector3.Dot(v1, v2).
		/// </summary>
		/// <param name="vector1">The first operand.</param>
		/// <param name="vector2">The second operand.</param>
		/// <returns>Vector3.</returns>
		public static float Dot(this Vector3 vector1, Vector3 vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
		}

		/// <summary>
		/// Equivalent to Vector4.Dot(v1, v2).
		/// </summary>
		/// <param name="vector1">The first operand.</param>
		/// <param name="vector2">The second operand.</param>
		/// <returns>Vector4.</returns>
		public static float Dot(this Vector4 vector1, Vector4 vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z + vector1.w * vector2.w;
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector2 Proj(this Vector2 vector, Vector2 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// </summary>
		/// <remarks>
		/// 	<para>The sum of a vector's projection and rejection on a base is equal to
		/// the original vector.</para>
		/// </remarks>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector2 Rej(this Vector2 vector, Vector2 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>

		public static Vector3 Proj(this Vector3 vector, Vector3 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// </summary>
		/// <remarks>
		/// 	<para>The sum of a vector's projection and rejection on a base is equal to
		/// the original vector.</para>
		/// </remarks>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector3 Rej(this Vector3 vector, Vector3 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector4 Proj(this Vector4 vector, Vector4 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// The sum of a vector's projection and rejection on a base is
		/// equal to the original vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector4 Rej(this Vector4 vector, Vector4 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		/// <summary>
		/// Turns the vector 90 degrees anticlockwise as viewed from the top (keeping the y coordinate intact).
		/// Equivalent to <code>v.To2DXZ().Perp().To3DXZ(v.y);</code>
		/// </summary>
		public static Vector3 PerpXZ(this Vector3 v)
		{
			return new Vector3(-v.z, v.y, v.x);
		}

		/// <summary>
		/// Turns the vector 90 degrees anticlockwise as viewed from the front (keeping the z coordinate intact).
		/// Equivalent to <code>v.To2DXY().Perp().To3DXY(v.z);</code>
		/// </summary>

		public static Vector3 PerpXY(this Vector3 v)
		{
			return new Vector3(-v.y, v.x, v.z);
		}

		/// <summary>
		/// Multiplies component by component.
		/// </summary>
		/// <param name="thisVector">The this vector.</param>
		/// <param name="otherVector">The other vector.</param>
		[Version(1, 4, 4)]
		public static Vector2 HadamardMod(this Vector2 thisVector, Vector2 otherVector)
		{
			return new Vector2(
				GLMathf.FloorMod(thisVector.x, otherVector.x), 
				GLMathf.FloorMod(thisVector.y, otherVector.y));
		}

		/// <summary>
		/// Multiplies component by component.
		/// </summary>
		/// <param name="thisVector">The this vector.</param>
		/// <param name="otherVector">The other vector.</param>
		[Version(2)]
		public static Vector3 HadamardMod(this Vector3 thisVector, Vector3 otherVector)
		{
			return new Vector3(
				GLMathf.FloorMod(thisVector.x, otherVector.x), 
				GLMathf.FloorMod(thisVector.y, otherVector.y),
				GLMathf.FloorMod(thisVector.z, otherVector.z));
		}

		/// <summary>
		/// Multiplies component by component.
		/// </summary>
		/// <param name="thisVector">The this vector.</param>
		/// <param name="otherVector">The other vector.</param>
		[Version(2)]
		public static Vector4 HadamardMod(this Vector4 thisVector, Vector4 otherVector)
		{
			return new Vector4(
				GLMathf.FloorMod(thisVector.x, otherVector.x), 
				GLMathf.FloorMod(thisVector.y, otherVector.y),
				GLMathf.FloorMod(thisVector.z, otherVector.z),
				GLMathf.FloorMod(thisVector.w, otherVector.w));
		}

		/// <summary>
		/// Multiplies one vector componentwise by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector2 HadamardMul(this Vector2 thisVector, Vector2 otherVector)
		{
			return new Vector2(thisVector.x * otherVector.x, thisVector.y * otherVector.y);
		}

		/// <summary>
		/// Divides one vector component by component by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector2 HadamardDiv(this Vector2 thisVector, Vector2 otherVector)
		{
			return new Vector2(thisVector.x / otherVector.x, thisVector.y / otherVector.y);
		}

		/// <summary>
		/// Multiplies one vector component by component by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector3 HadamardMul(this Vector3 thisVector, Vector3 otherVector)
		{
			return new Vector3(
				thisVector.x * otherVector.x, 
				thisVector.y * otherVector.y,
				thisVector.z * otherVector.z);
		}

		/// <summary>
		/// Divides one vector component by component by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector3 HadamardDiv(this Vector3 thisVector, Vector3 otherVector)
		{
			return new Vector3(
				thisVector.x / otherVector.x, 
				thisVector.y / otherVector.y,
				thisVector.z / otherVector.z);
		}

		/// <summary>
		/// Multiplies one vector component by component by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector4 HadamardMul(this Vector4 thisVector, Vector4 otherVector)
		{
			return new Vector4(
				thisVector.x * otherVector.x,
				thisVector.y * otherVector.y,
				thisVector.z * otherVector.z,
				thisVector.w * otherVector.w);
		}

		/// <summary>
		/// Divides one vector component by component by another.
		/// </summary>
		[Version(1, 4, 4)]
		public static Vector4 HadamardDiv(this Vector4 thisVector, Vector4 otherVector)
		{
			return new Vector4(
				thisVector.x / otherVector.x,
				thisVector.y / otherVector.y,
				thisVector.z / otherVector.z,
				thisVector.w / otherVector.w);
		}

				
		public static Vector2Int To2DXY(this Vector3Int v)
		{
			return new Vector2Int(v.x, v.y);
		}

		public static Vector3Int To3DXY(this Vector2Int v)
		{
			return new Vector3Int(v.x, v.y, 0);
		}
		public static Vector3Int To3DXZ(this Vector2Int v)
		{
			return new Vector3Int(v.x, 0, v.y);
		}

		public static void SetMax(this Vector2Int v, Vector2Int max)
		{
			v.Set(v.x > max.x ? max.x : v.x,
				v.y > max.y ? max.y : v.y);
		}

		public static void SetMin(this Vector2Int v, Vector2Int min)
		{
			v.Set(v.x < min.x ? min.x : v.x, 
				v.y < min.y ? min.y : v.y);
		}

		public static Vector2Int Center(this Vector2Int v)
		{
			return new Vector2Int(v.x / 2, v.y / 2);
		}

		public static Vector2Int CenterRound(this Vector2Int v)
		{
			return new Vector2Int(Mathf.RoundToInt((float)v.x / 2.0f), Mathf.RoundToInt((float)v.y / 2.0f));
		}

		/// <summary>From inclusive, to exclusive </summary>
		public static bool InRange(this Vector2Int v, Vector2Int from, Vector2Int to)
		{
			return v.x >= from.x && v.y >= from.y 
				&& v.x < to.x && v.y < to.y;
		}
		
		// form zero to, exclusive 
		public static bool InRange(this Vector2Int v, Vector2Int to)
		{
			return v.x >= 0 && v.y >= 0 
				&& v.x < to.x && v.y < to.y;
		}

		/// <summary>Min max inclusive </summary>
		public static bool InRange(this Vector2Int v, int position)
		{
			return position >= v.x && position <= v.y;
		}
		
		/// <summary>Max value </summary>
		public static int Max(this Vector2Int v)
		{
			return v.x > v.y ? v.x : v.y;
		}

		// min value
		public static int Min(this Vector2Int v)
		{
			return v.x < v.y ? v.x : v.y;
		}
		
		// true if this vector in bounds(inclusive min max) of argument vector, none argument check
		public static bool InRangeOfInc(this Vector2 v, Vector2 range)
		{
			return v.x >= range.x && v.y <= range.y;
			// && v.x <= range.y && v.y >= range.x
		}

		public static bool InRangeOfInc(this Vector2 v, float pos)
		{
			return v.x <= pos && v.y >= pos;
		}

		public static float ClosesdValue(this Vector2 v, float pos)
		{
			return Mathf.Abs(v.x - pos) < Mathf.Abs(v.y - pos) ? v.x : v.y;
		}

		public static Vector2 Abs(this Vector2 v)
		{
			return new Vector2(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y);
		}

		public static Vector3 Abs(this Vector3 v)
		{
			return new Vector3(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y, v.z < 0.0f ? -v.z : v.z);
		}

		public static Vector2 ClampLenght(this Vector2 v, float lenghtAbs)
		{
			var lenght = v.magnitude;
			if (lenght > lenghtAbs)
				v *= lenghtAbs / lenght;

			return v;
		}

		public static Vector2Int Round(this Vector2 v)
		{
			return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		}
		public static Vector2Int Ceil(this Vector2 v)
		{
			return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
		}
		public static Vector2Int Floor(this Vector2 v)
		{
			return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		}

		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
		}

		public static Vector3 Clamp(this Vector3 v, float min, float max)
		{
			return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
		}

		public static Vector2Int Clamp(this Vector2Int v, int min, int max)
		{
			return new Vector2Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
		}

		public static Vector3Int Clamp(this Vector3Int v, int min, int max)
		{
			return new Vector3Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
		}
		
		public static Vector2 Normal(float angle)
		{
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}
		
		public static int ToInt(this Vector2Int v)
		{
			//return unchecked(v.x | (v.y << 15));
			return v.x | (v.y << 15);
		}

        public static int Sum(this Vector2Int v)
        {
            return v.x + v.y;
        }

        public static int SumAbs(this Vector2Int v)
        {
            return Mathf.Abs(v.x) + Mathf.Abs(v.y);
        }

		public static Vector2Int FromInt(int v)
		{
			return new Vector2Int(v & 0b0000_0000_0000_0000_1111_1111_1111_1111, v >> 15);
		}
		
		// in radians
		public static Vector3 NormalXY(float angle)
		{
			return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
		}
		public static Vector3 NormalYZ(float angle)
		{
			return new Vector3(0.0f, Mathf.Sin(angle), Mathf.Cos(angle));
		}
		public static Vector3 NormalXZ(float angle)
		{
			return new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
		}

		#endregion

	}

	/// <summary>
	/// Provides utility methods for doing geometry.
	/// </summary>
	[Version(2)]
	public static class Geometry
	{
		/// <summary>
		/// Determines whether a point is in the half plane described by a point and direction.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="halfPlanePoint">The half plane point.</param>
		/// <param name="halfPlaneDirection">The half plane direction.</param>
		/// <returns><c>true</c> if the point is in the half plane; otherwise, <c>false</c>.</returns>
		public static bool IsInHalfPlane(Vector2 point, Vector2 halfPlanePoint, Vector2 halfPlaneDirection)
		{
			return (point - halfPlanePoint).PerpDot(halfPlaneDirection) <= 0;
		}

		/// <summary>
		/// Determines whether a point is in the half (3D, hyper) plane described by a point and direction.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="halfPlanePoint">The half plane point.</param>
		/// <param name="halfPlaneDirection">The half plane direction.</param>
		/// <returns><c>true</c> if the point is in the half plane; otherwise, <c>false</c>.</returns>
		public static bool IsInHalfPlane(Vector3 point, Vector3 halfPlanePoint, Vector3 halfPlaneDirection)
		{
			return Vector3.Dot(point - halfPlanePoint, halfPlaneDirection) >= 0;
		}
	}
}
