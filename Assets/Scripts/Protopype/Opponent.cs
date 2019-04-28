using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : GLMonoBehaviour
{
	public AnimationCurve		m_HealthChanseToDie;
	public int m_Health;
	private ParameterVisualizer m_HealthVisual;
	public bool m_HitNow;

	public Vector2Int	m_ToBlockDamage;
	public Vector2Int	m_ToRecoveryDamage;
	public Vector2Int   m_ToHitDamage;
	public Vector2Int	m_ToDoudgeDamage;

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
					GameManager.Instance.m_Summary.AddNote("You Win!");
					GameManager.Instance.GameOver(GameManager.GameOverType.Silent);
				}

			m_Health = m_HealthVisual.Value;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		m_HealthVisual = GetComponent<ParameterVisualizer>();
		m_HealthVisual.Value = m_Health;
	}

	//////////////////////////////////////////////////////////////////////////
	public void IncrementHealth(int inc)
	{
		Health += inc;
	}

	public void HitToBlock()
	{
		implMakeHit(Random.Range(m_ToBlockDamage.x, m_ToBlockDamage.y));
	}

	public void HitToDoudge()
	{
		implMakeHit(Random.Range(m_ToRecoveryDamage.x, m_ToRecoveryDamage.y));
	}
	
	public void HitToHit()
	{
		implMakeHit(Random.Range(m_ToHitDamage.x, m_ToHitDamage.y));
	}
	
	public void HitToRecovery()
	{
		implMakeHit(Random.Range(m_ToDoudgeDamage.x, m_ToDoudgeDamage.y));
	}
	
	//////////////////////////////////////////////////////////////////////////
	private void implMakeHit(int damage)
	{
		m_HitNow = !m_HitNow;

		if(m_HitNow)
		{
			GameManager.Instance.m_Player.Hit();

			GameManager.Instance.m_Player.Health -= damage;
		}
	}
}
