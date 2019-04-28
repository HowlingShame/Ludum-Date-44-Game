using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager		Instance;
	public SoundDictionary			m_SoundDic;
	public GameObject				m_AudioSourcePrefab;
	public AudioSource				m_MusicSource;
	public Dictionary<string, AudioSource>	m_AudioDic = new Dictionary<string, AudioSource>();

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class SoundDictionary : SerializableDictionaryBase<string, AudioClip> { }

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		Instance = this;
		m_MusicSource = GetComponent<AudioSource>();

		foreach(var n in m_SoundDic)
		{
			if(n.Value != null)
			{
				var obj = GameObject.Instantiate(m_AudioSourcePrefab, transform);
				obj.name = n.Key;
				var audio = obj.GetComponent<AudioSource>();
				audio.clip = n.Value;
				m_AudioDic.Add(n.Key, audio);
			}
		}
	}
	//////////////////////////////////////////////////////////////////////////
	public void Play(string audio)
	{
		AudioSource value;
		if(m_AudioDic.TryGetValue(audio, out value))
			value.Play();
	}
	public void SetMusic(string audio)
	{
		AudioClip value;
		if(m_SoundDic.TryGetValue(audio, out value))
			m_MusicSource.clip = value;
	}
}
