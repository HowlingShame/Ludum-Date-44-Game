using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : GLMonoBehaviour
{
	[SerializeField]    private int     m_Age;
	[SerializeField]    private int     m_Health;
	[SerializeField]    private int     m_Happiness;
	[SerializeField]    private int     m_Wealth;

	[SerializeField]    private ParameterVisualizer     m_AgeVisual;
	[SerializeField]    private ParameterVisualizer     m_HealthVisual;
	[SerializeField]    private ParameterVisualizer     m_HappinessVisual;
	[SerializeField]    private ParameterVisualizer     m_WealthVisual;

	public AnimationCurve	m_AgeChanseToDie;
	public AnimationCurve	m_HealthChanseToDie;

	public ParticleSystem                   m_DeathEffect;
	public ParticleSystem                   m_HitEffect;

	[Header("look")]
	public AnimatorOverrideController       m_Baby;
	public int      m_BabyAge;
	public AnimatorOverrideController       m_Peanut;
	public int      m_PeanutAge;
	public AnimatorOverrideController       m_Child;
	public int      m_ChildAge;
	public AnimatorOverrideController       m_Teenager;
	public int      m_TeenagerAge;
	public AnimatorOverrideController       m_Mature;
	public int      m_MatureAge;
	public AnimatorOverrideController       m_Old;
	public int      m_OldAge;


	private LTDescr m_LeanTweenDesc;
	[Header("Lean Tween Follow")]
//	[SerializeField]	private float m_SmoothTime = 1.0f;
	[SerializeField]    private float m_MaxSpeed = 1.0f;
	//	[SerializeField]	private float m_Friction = 1.0f;
	//	[SerializeField]	private float m_AccelRate = 0.5f;

	private Animator m_Animator;

	public float    m_MoveSpeed;
	private Vector3 m_LastFramePos;
	private bool m_UpdateMove = true;

	public int Age
	{
		get
		{
			return m_Age;
		}
		set
		{
			
			m_AgeVisual.Value = value;

			if(value > m_Age)
				if(UnityEngine.Random.Range(0.0f, 1.0f) < m_AgeChanseToDie.Evaluate((float)m_Age / 100.0f))
				{
					GameManager.Instance.m_Summary.AddNote("You died of old age.");
					GameManager.Instance.GameOver(GameManager.GameOverType.Explosion);
				}

			m_Age = m_AgeVisual.Value;
			UpdateAge();
		}
	}

	public int Health
	{
		get
		{
			return m_Health;
		}
		set
		{
			m_HealthVisual.Value = value;

			if(value < m_Health)
				if(UnityEngine.Random.Range(0.0f, 1.0f) < m_HealthChanseToDie.Evaluate((float)m_Health / 100.0f))
				{
					GameManager.Instance.m_Summary.AddNote("You died because of health problems.");
					GameManager.Instance.GameOver(GameManager.GameOverType.Explosion);
				}

			m_Health = m_HealthVisual.Value;
		}
	}

	public int Happiness
	{
		get
		{
			return m_Happiness;
		}
		set
		{
			m_HappinessVisual.Value = value;
			m_Happiness = m_HappinessVisual.Value;
		}
	}

	public int Wealth
	{
		get
		{
			return m_Wealth;
		}
		set
		{
			m_WealthVisual.Value = value;
			m_Wealth = m_WealthVisual.Value;
		}
	}


	//////////////////////////////////////////////////////////////////////////
	//	[Serializable]
	//	public enum Age
	//	{
	//		Yang,
	//		Teen,
	//		Mature,
	//		Old
	//	}

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		GameManager.Instance.m_Player = this;
		m_Animator = GetComponent<Animator>();

		m_LastFramePos = transform.position;

		Age = Age;
		Health = Health;
		Happiness = Happiness;
		Wealth = Wealth;
	}

	private void Update()
	{
		m_MoveSpeed = Vector3.Distance(m_LastFramePos, transform.position);

		if(m_UpdateMove)
			m_Animator.SetFloat("MoveSpeed", m_MoveSpeed);

		m_LastFramePos = transform.position;
	}


	//////////////////////////////////////////////////////////////////////////
	public void MoveToRoom(LifeRoom room)
	{
		Follow(room.transform);
	}

	public void Follow(Transform trans)
	{
		if(m_LeanTweenDesc == null)
			//m_LeanTweenDesc = LeanTween.followSpring(transform, room.transform, LeanProp.position, m_SmoothTime, m_MaxSpeed, m_Friction, m_AccelRate);
			//m_LeanTweenDesc = LeanTween.followBounceOut(transform, room.transform, LeanProp.position, m_SmoothTime, m_MaxSpeed, m_Friction, m_AccelRate);
			//m_LeanTweenDesc = LeanTween.followDamp(transform, room.transform, LeanProp.position, m_SmoothTime);
			m_LeanTweenDesc = LeanTween.followLinear(transform, trans, LeanProp.position, m_MaxSpeed).setEaseInOutSine();
			//m_LeanTweenDesc = LeanTween.move(gameObject, room.transform, m_SmoothTime);
		else
			m_LeanTweenDesc.setTarget(trans);
	}

	public void Death()
	{
		SoundManager.Instance.Play("Death");
		m_UpdateMove = false;
		if(m_LeanTweenDesc != null)
			LeanTween.cancel(m_LeanTweenDesc.id);

		if(m_DeathEffect != null)
		{
			m_DeathEffect.gameObject.SetActive(true);
			m_DeathEffect.Play();
		}

		transform.rotation = Quaternion.Euler(0.0f, 0.0f, 82.0f);
	}

	public void Hit()
	{
		SoundManager.Instance.Play("Hit");
		if(m_HitEffect != null)
		{
			m_HitEffect.gameObject.SetActive(true);
			m_HitEffect.Play();
		}
	}

	public void Leave()
	{
		if(m_LeanTweenDesc != null)
			LeanTween.cancel(m_LeanTweenDesc.id);
		LeanTween.move(gameObject, new Vector3(-6.0f, 0.0f, 0.0f), 6.0f);
	}

	public void Silent()
	{
		if(m_LeanTweenDesc != null)
			LeanTween.cancel(m_LeanTweenDesc.id);
	}

	public void UpdateAge()
	{
		AnimatorOverrideController overrideController = null;
		if(m_Age >= m_BabyAge)
			overrideController = m_Baby;
		if(m_Age >= m_PeanutAge)
			overrideController = m_Peanut;
		if(m_Age >= m_ChildAge)
			overrideController = m_Child;
		if(m_Age >= m_TeenagerAge)
			overrideController = m_Teenager;
		if(m_Age >= m_MatureAge)
			overrideController = m_Mature;
		if(m_Age >= m_OldAge)
			overrideController = m_Old;

		if(m_Animator.runtimeAnimatorController != overrideController)
		{   // change age
			m_Animator.runtimeAnimatorController = overrideController;
		}
	}

	[InspectorButton]
	void IncreaseAge()
	{
		Age += 1;
	}
}
