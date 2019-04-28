using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeRoom))]
public class StartRoom : MonoBehaviour
{
	public Color			m_BackgrounColor;
	public string			m_Music;

	//////////////////////////////////////////////////////////////////////////
	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();		// start game after player variable will be set

		SoundManager.Instance.SetMusic(m_Music);
		if(GameManager.Instance.m_PlayMusic)
			SoundManager.Instance.m_MusicSource.Play();
		else
			SoundManager.Instance.m_MusicSource.Stop();

		GameManager.Instance.StartGame(GetComponent<LifeRoom>());
		Core.Instance.m_Camera.backgroundColor = m_BackgrounColor;
	}

}
