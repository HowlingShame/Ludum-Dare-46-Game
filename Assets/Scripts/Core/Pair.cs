using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<T>
{
	public T	First;
	public T	Second;

	//////////////////////////////////////////////////////////////////////////
	protected bool Equals(Pair<T> other)
	{
		return EqualityComparer<T>.Default.Equals(First, other.First) && EqualityComparer<T>.Default.Equals(Second, other.Second)
			|| EqualityComparer<T>.Default.Equals(Second, other.First) && EqualityComparer<T>.Default.Equals(First, other.Second);
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Pair<T>) obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return EqualityComparer<T>.Default.GetHashCode(First) + EqualityComparer<T>.Default.GetHashCode(Second);
		}
	}

	public void Deconstruct(out T first, out T second)
	{
		first = First;
		second = Second;
	}

	public Pair(T first, T second)
	{
		First = first;
		Second = second;
	}
}
