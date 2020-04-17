using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataContainerBase.DataElement))]
public class DataContainerElementDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		switch ((DataContainerBase.DataType)property.FindPropertyRelative("m_DataType").enumValueIndex)
		{
			case DataContainerBase.DataType.Int:
			case DataContainerBase.DataType.Float:
			case DataContainerBase.DataType.Boolean:
			case DataContainerBase.DataType.String:
			case DataContainerBase.DataType.Vector2:
			case DataContainerBase.DataType.Vector3:
			case DataContainerBase.DataType.Vector2Int:
			case DataContainerBase.DataType.Vector3Int:
				return EditorGUIUtility.singleLineHeight * 2.0f;

			case DataContainerBase.DataType.Null:
			case DataContainerBase.DataType.Object:
			default:
				return EditorGUIUtility.singleLineHeight;
		}

	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
//		int controlID = GUIUtility.GetControlID(FocusType.Keyboard, position);
//		switch (Event.current.GetTypeForControl(controlID))
//		{
//			case EventType.Repaint:
//			{
//
//			}	break;
//			default:
//				break;
//		}

		var dataTypeField = property.FindPropertyRelative("m_DataType");
		var enumValueIndex = (DataContainerBase.DataType)dataTypeField.enumValueIndex;
		
		var enumValueIndexChange = (DataContainerBase.DataType)EditorGUI.EnumPopup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), enumValueIndex);
		if (enumValueIndexChange != enumValueIndex)
			dataTypeField.enumValueIndex = (int)enumValueIndexChange;
		
		var dataBuffer = property.FindPropertyRelative("m_DataBuffer");
		var byteArray = new List<byte>(dataBuffer.arraySize);
		for (var n = 0; n < dataBuffer.arraySize; ++ n)
			byteArray.Add((byte)dataBuffer.GetArrayElementAtIndex(n).intValue);
		
		var byteArraySwap = new List<byte>(dataBuffer.arraySize);
		switch (enumValueIndex)
		{
			case DataContainerBase.DataType.Int:
			{
				var propValue = BitConverter.ToInt32(byteArray.ToArray(), 0);
				var value = EditorGUI.IntField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
						position.width, EditorGUIUtility.singleLineHeight), "IntValue", propValue);

				if (propValue != value)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(value));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Float:
			{
				var propValue = BitConverter.ToSingle(byteArray.ToArray(), 0);
				var value = EditorGUI.FloatField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
						position.width, EditorGUIUtility.singleLineHeight), "FloatValue", propValue);

				if (propValue != value)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(value));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Boolean:
			{
				var propValue = BitConverter.ToBoolean(byteArray.ToArray(), 0);
				var value = EditorGUI.Toggle(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
						position.width, EditorGUIUtility.singleLineHeight), "BoolValue", propValue);

				if (propValue != value)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(value));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.String:
			{
				var propValue = System.Text.Encoding.UTF8.GetString(byteArray.ToArray(), 0, byteArray.Count);
				var value = EditorGUI.TextField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
						position.width, EditorGUIUtility.singleLineHeight), "StringValue", propValue);

				if (propValue != value)
				{
					byteArraySwap.AddRange(System.Text.Encoding.UTF8.GetBytes(value));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Vector2:
			{
				var value = new Vector2(BitConverter.ToSingle(byteArray.ToArray(), 0), BitConverter.ToSingle(byteArray.ToArray(), 4));
				var vector = EditorGUI.Vector2Field(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
								position.width, EditorGUIUtility.singleLineHeight), "VectorValue",
								value);
				if (value != vector)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.x));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.y));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Vector3:
			{
				var value = new Vector3(BitConverter.ToSingle(byteArray.ToArray(), 0), BitConverter.ToSingle(byteArray.ToArray(), 4), BitConverter.ToSingle(byteArray.ToArray(), 8));
				var vector = EditorGUI.Vector3Field(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
								position.width, EditorGUIUtility.singleLineHeight), "VectorValue",
								value);
				if (value != vector)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.x));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.y));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.z));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Vector2Int:
			{
				var value = new Vector2Int(BitConverter.ToInt32(byteArray.ToArray(), 0), BitConverter.ToInt32(byteArray.ToArray(), 4));
				var vector = EditorGUI.Vector2IntField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
								position.width, EditorGUIUtility.singleLineHeight), "VectorValue",
								value);
				if (value != vector)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.x));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.y));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Vector3Int:
			{ 
				var value = new Vector3Int(BitConverter.ToInt32(byteArray.ToArray(), 0), BitConverter.ToInt32(byteArray.ToArray(), 4), BitConverter.ToInt32(byteArray.ToArray(), 8));
				var vector = EditorGUI.Vector3IntField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
								position.width, EditorGUIUtility.singleLineHeight), "VectorValue",
								value);
				if (value != vector)
				{
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.x));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.y));
					byteArraySwap.AddRange(BitConverter.GetBytes(vector.z));
					implApllySwapBuffer(dataBuffer, byteArraySwap);
				}
			}	break;
			case DataContainerBase.DataType.Object:
			{
			}	break;
		}

	}

	private static void implApllySwapBuffer(SerializedProperty dataBuffer, List<byte> byteArraySwap)
	{
		dataBuffer.ClearArray();
		dataBuffer.arraySize = byteArraySwap.Count;

		for (var n = 0; n < dataBuffer.arraySize; ++n)
			dataBuffer.GetArrayElementAtIndex(n).intValue = (int)byteArraySwap[n];
	}
}