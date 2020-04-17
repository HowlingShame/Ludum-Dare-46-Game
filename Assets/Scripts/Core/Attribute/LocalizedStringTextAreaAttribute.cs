using System;
using UnityEngine;

[Serializable]
public class LocalizedStringTextAreaAttribute : PropertyAttribute
{
	public int	m_LinesCount;

	public LocalizedStringTextAreaAttribute(int linesCount)
	{
		m_LinesCount = linesCount;
	}
}