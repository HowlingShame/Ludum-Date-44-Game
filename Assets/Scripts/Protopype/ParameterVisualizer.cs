using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ParameterVisualizer : GLMonoBehaviour
{
	[SerializeField]
	private int				m_Value;
	[Tooltip("Inclusive range")]
	public Vector2Int		m_Range;
	public GameObject		m_IncreaseEffect;
	public GameObject		m_DecreaseEffect;

	private Animator		m_Animator;
	public Text				m_Text;

//	[Header("LeanTween")]
//	public float			m_LeanTweenDelay;
//	public float			m_ScaleTime;
//	public Vector3			m_ScaleA;
//	public Vector3			m_ScaleB;


	public int Value
	{
		get
		{
			return m_Value;
		}

		set
		{
			var inc = value - m_Value;
			Increment(inc);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
//		transform.localScale = m_ScaleA;
//		
//		LeanTween.sequence()
//			.append(m_LeanTweenDelay)
//			.append(LeanTween.scale(gameObject, m_ScaleB, m_ScaleTime)
//				.setRepeat(-1)
//				.setLoopPingPong()
//				.setEaseInOutSine());
	}

	//////////////////////////////////////////////////////////////////////////
	public void Increment(int val)
	{
		var clampValue = Mathf.Clamp(m_Value + val, m_Range.x, m_Range.y);

		if(clampValue == m_Value)
			return;

		if(clampValue < m_Value)
		{
			if(m_DecreaseEffect != null)	
			{
				Instantiate(m_DecreaseEffect, transform); 
			}
		}
		else
		{	
			if(m_IncreaseEffect != null)
			{
				Instantiate(m_IncreaseEffect, transform); 
			}
		}

		m_Value = clampValue;
		m_Animator.SetFloat("Value", (float)m_Value / (m_Range.y - m_Range.x));

		if(m_Text != null)
			m_Text.text = m_Value.ToString();
	}

	[InspectorButton]
	void Increace()
	{
		Increment(1);
	}

	[InspectorButton]
	void Decrease()
	{
		Increment(-1);
	}
}
