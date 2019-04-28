using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoot : MonoBehaviour
{
	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		GameManager.Instance.m_LevelRoot = gameObject;
	}

	private void OnDestroy()
	{
		if(GameManager.Instance != null)
			GameManager.Instance.m_LevelRoot = GameManager.Instance.gameObject;
	}
}
