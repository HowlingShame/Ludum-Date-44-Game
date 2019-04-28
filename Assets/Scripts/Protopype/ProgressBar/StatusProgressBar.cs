using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusProgressBar : MonoBehaviour
{
	[SerializeField]	private float						m_Status = 1.0f;
	[SerializeField]	private float						m_MinValue = 0.0f;
	[SerializeField]	private float						m_MaxValue = 1.0f;

	public BaseVisualizerProgressBar		m_Visualizer;

	public float		Status
	{
		get
		{
			return m_Status;
		}
		set
		{
			m_Status = value;
			m_Visualizer?.UpdateStatus(this);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_Visualizer = GetComponent<BaseVisualizerProgressBar>();
	}
}
