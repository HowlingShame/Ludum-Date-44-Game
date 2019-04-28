using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
	public float	m_Delay = 1.0f;
	public float	m_RandomDelay = 0.0f;
	public float	m_Scale = 1.0f;
	public float	m_Time = 1.0f;

	public float	m_Up = 0.0f;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{		
		var randomDealy = Random.Range(0.0f, m_RandomDelay);
		LeanTween.sequence()
			.append(m_Delay + randomDealy)
			.append(LeanTween.scale(gameObject, transform.localScale * m_Scale, m_Time)
				.setRepeat(-1)
				.setLoopPingPong()
				.setEaseInOutSine());
		
		if(m_Up != 0.0f)
			LeanTween.sequence()
				.append(m_Delay + randomDealy)
				.append(LeanTween.moveLocalY(gameObject, m_Up, m_Time)
					.setRepeat(-1)
					.setLoopPingPong()
					.setEaseInOutSine());
	}
}
