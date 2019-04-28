using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomName : MonoBehaviour
{
	private Text m_Text;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		GameManager.Instance.m_RoomName = this;
		m_Text = GetComponent<Text>();
	}

	//////////////////////////////////////////////////////////////////////////
	public void SetRoomName(string text)
	{
//		LeanTween.moveLocalX(gameObject, 10.0f, 0.5f)
//			.setLoopPingPong(2);

		m_Text.text = text;
	}
}
