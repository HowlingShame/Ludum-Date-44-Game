using System;
using UnityEditor;
using UnityEngine;
using Gamelogic.Extensions;

public class GizmoDrawler
{
	static Vector3 InfoMapGizmoOffset = new Vector3(0, 0, -0.1f);
	static Vector3 InfoMapGizmoTextOffset = new Vector3(0, 0, -0.6f);

	//////////////////////////////////////////////////////////////////////////

	[DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
	public static void DrawScript(Core src, GizmoType gizmoType)
	{
		{
			if(gizmoType.HasFlag(GizmoType.Selected))
			{
				Gizmos.color = Color.gray;
				Gizmos.DrawSphere(src.m_MouseWorldPos.m_Position, Core.CellSize.magnitude * 0.10f);
			}
		}
	}

	
	//////////////////////////////////////////////////////////////////////////
	private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
	{
		float angle = 0f;
		Quaternion rot = Quaternion.LookRotation(forward, up);
		Vector3 lastPoint = Vector3.zero;
		Vector3 thisPoint = Vector3.zero;
 
		for (int i = 0; i < segments + 1; i++)
		{
			thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
			thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;
 
			if (i > 0)
			{
				Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
			}
 
			lastPoint = thisPoint;
			angle += 360f / segments;
		}
	}
}

