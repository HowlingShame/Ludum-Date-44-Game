using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterCheck : MonoBehaviour
{
	public Sprite	m_Ok;
	public Sprite	m_Failed;

	public bool		Check
	{
		set
		{
			GetComponent<SpriteRenderer>().sprite = value ? m_Ok : m_Failed;
		}
	}
}
