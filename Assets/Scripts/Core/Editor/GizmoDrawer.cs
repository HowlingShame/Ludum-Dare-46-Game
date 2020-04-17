using System;
using UnityEditor;
using UnityEngine;
using Gamelogic.Extensions;

public static partial class GizmoDrawer
{
	static Vector3 InfoMapGizmoOffset = new Vector3(0, 0, -0.1f);
	static Vector3 InfoMapGizmoTextOffset = new Vector3(0, 0, -0.6f);

	//////////////////////////////////////////////////////////////////////////
	[DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
	private static void DrawScript(Core src, GizmoType gizmoType)
	{
		{
			if(gizmoType.HasFlag(GizmoType.Selected))
			{
				Gizmos.color = Color.gray;
				Gizmos.DrawSphere(src.MouseWorldPosition, 0.10f);
			}
		}
	}
	
}

