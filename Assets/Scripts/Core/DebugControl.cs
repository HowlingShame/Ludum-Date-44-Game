using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugControl : GLMonoBehaviour 
{
	public float m_MouseScrollScale = 1.0f;
	public float m_MouseMoveScale = 0.06f;

	public float m_KeyboardMoveScale = 10.0f;

	protected Vector3 m_MousePosLast;


	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		hideFlags = HideFlags.HideInInspector;
	}

	private void Update()
	{
		Vector3 translateVector = Vector3.zero;

		if(Input.GetKey(KeyCode.UpArrow))
			translateVector += Vector3.up;
		if(Input.GetKey(KeyCode.DownArrow))
			translateVector += Vector3.down;
		if(Input.GetKey(KeyCode.LeftArrow))
			translateVector += Vector3.left;
		if(Input.GetKey(KeyCode.RightArrow))
			translateVector += Vector3.right;
		
		if(translateVector != Vector3.zero)
		{
			translateVector.Normalize();
			Core.Instance.m_Camera.gameObject.transform.position += ((translateVector * m_KeyboardMoveScale * Time.deltaTime).WithZ(0.0f));
		}

		var view = Core.Instance.m_Camera.ScreenToViewportPoint(Input.mousePosition);

		if((view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1) == false)
		{
			if(Input.GetMouseButton(0))
			{
				Vector3 offset = m_MousePosLast - Input.mousePosition;
				if(offset.magnitude < 40.0f)
					Core.Instance.m_Camera.transform.position += ((offset * m_MouseMoveScale).WithZ(0.0f));
			}

			//if(Input.GetKey(KeyCode.LeftShift))
			Core.Instance.m_Camera.transform.TranslateZ(Input.mouseScrollDelta.y * m_MouseScrollScale);
		}

		m_MousePosLast = Input.mousePosition;
	}

	//
	private void OnGUI()
	{
	}
}
