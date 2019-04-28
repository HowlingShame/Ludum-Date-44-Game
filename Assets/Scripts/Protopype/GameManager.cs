using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : GLMonoBehaviour
{
	public static GameManager			Instance;
	
	public LifeRoom						m_CurrentRoom;
	public Player						m_Player;
	public RoomName						m_RoomName;
	public UnityRandom					m_Random = new UnityRandom();
	public GameObject					m_StartMenu;
	public GameObject					m_PauseMenu;
	public GameObject					m_SummaryMenu;
	public GameObject					m_LevelRoot;

	public Summary						m_Summary;

	protected LTSeq					m_LTSeq;

	[Header("States")]
	public string						m_CurrentScene;
	public bool							m_LockControll;
	public bool							m_GameRunning;
	public bool							m_InPauseMenu;
	public bool							m_InStartMenu;
	public bool							m_AlwaysShowText;
	private bool						m_ResumeControll;
	public bool							m_ButtonDownSwap = true;
	private bool m_InStatistic;
	private bool m_GameOver;
	public bool m_PlayMusic;
	public string						m_MainMenuMusic;

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public enum GameOverType
	{
		Silent,
		Explosion,
	}

	[Serializable]
	public class Summary
	{
		public string			m_Summary;

		//////////////////////////////////////////////////////////////////////////
		public void AddNote(string note)
		{
			m_Summary += note + " ";
		}

		public void Clear()
		{
			m_Summary = "";
		}

		public string GetSummary()
		{
			return m_Summary;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		if(Instance != null)
			Debug.LogWarning("Manager have more then one instance");
		
		Instance = this;
		EnterStartMenu();
	}

	private void Update()
	{
		if(m_GameRunning)
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				if(m_InPauseMenu)	LeavePauseMenu();
				else				EnterPauseMenu();
				if(m_InStatistic)	LeaveStatistic();
			}			
		}

		if(m_LockControll == false && m_GameOver == false)
		{
			var moveDirection = Direction.None;

			if(m_ButtonDownSwap)
			{
				if(Input.GetKeyDown(KeyCode.LeftArrow))
					moveDirection = Direction.Left;
				if(Input.GetKeyDown(KeyCode.RightArrow))
					moveDirection = Direction.Right;
				if(Input.GetKeyDown(KeyCode.UpArrow))
					moveDirection = Direction.Up;
				if(Input.GetKeyDown(KeyCode.DownArrow))
					moveDirection = Direction.Down;
			}
			else
			{
				if(Input.GetKey(KeyCode.LeftArrow))
					moveDirection = Direction.Left;
				if(Input.GetKey(KeyCode.RightArrow))
					moveDirection = Direction.Right;
				if(Input.GetKey(KeyCode.UpArrow))
					moveDirection = Direction.Up;
				if(Input.GetKey(KeyCode.DownArrow))
					moveDirection = Direction.Down;
			}

			if(moveDirection != Direction.None)
			{
				SwapRoom(moveDirection);
			}
		}
	}


	//////////////////////////////////////////////////////////////////////////
	public void StartGame(LifeRoom startRoom)
	{
		m_Summary.Clear();
		m_GameOver = false;
		m_GameRunning = true;
		m_CurrentRoom = startRoom;
		m_CurrentRoom.Enter(Direction.Down);

		UnlockControll();
		if(m_InStartMenu)
			LeaveStartMenu();
	}

	public void LeaveGame()
	{
		m_GameRunning = false;
		m_ResumeControll = true;
		SceneManager.UnloadSceneAsync(m_CurrentScene);
		LeavePauseMenu();
		EnterStartMenu();
	}

	public void GameOver(GameOverType gameOverType)
	{
		m_CurrentRoom.transform.parent.gameObject.SetActive(false);

		m_GameOver = true;
		switch(gameOverType)
		{
			case GameOverType.Silent:
				m_Player.Leave();
				ShowStatistic();
				break;
			case GameOverType.Explosion:
				m_Player.Death();
				ShowStatistic();
				break;
		}
	}

	public void PlayMusic(bool play)
	{
		m_PlayMusic = play;
		if(play)	SoundManager.Instance.m_MusicSource.Play();
		else		SoundManager.Instance.m_MusicSource.Stop();
	}

	public void AlwaysShowDescriptionText(bool show)
	{
		m_AlwaysShowText = show;
	}

	public void ShowStatistic()
	{
		m_InStatistic = true;
		m_SummaryMenu.SetActive(true);
		m_SummaryMenu.GetComponentInChildren<Text>().text = m_Summary.GetSummary();
		
		LeanTween.delayedCall(6.2f, () =>
		{
			if(m_GameRunning)
			{
				var psp = transform.Find("PlayerSummaryPos", true);
				if(psp != null)
					m_Player.Follow(psp.transform);
			}
		});
	}

	public void LeaveStatistic()
	{
		if(m_InStatistic == false)
			return;

		m_InStatistic = false;
		m_SummaryMenu.SetActive(false);
		LeaveGame();
	}

	private void EnterPauseMenu()
	{
		m_ResumeControll = m_LockControll ? false : true;
		
		foreach(var n in m_LevelRoot.transform.GetComponentsInChildren<Animator>())
			n.speed = 0.0f;

		LockControll();
		m_InPauseMenu = true;
		m_PauseMenu.SetActive(true);
	}

	private void LeavePauseMenu()
	{
		if(m_InPauseMenu == false)
			return;

		if(m_ResumeControll)
			UnlockControll();
		
		foreach(var n in m_LevelRoot.transform.GetComponentsInChildren<Animator>())
			n.speed = 1.0f;

		m_InPauseMenu = false;
		m_PauseMenu.SetActive(false);
	}
	
	private void EnterStartMenu()
	{
		m_InStartMenu = true;
		m_StartMenu.SetActive(true);
		
		if(SoundManager.Instance != null)
		{
			SoundManager.Instance.SetMusic(m_MainMenuMusic);
			if(GameManager.Instance.m_PlayMusic)
				SoundManager.Instance.m_MusicSource.Play();
			else
				SoundManager.Instance.m_MusicSource.Stop();
		}
	}

	private void LeaveStartMenu()
	{
		if(m_InStartMenu == false)
			return;
		m_InStartMenu = false;
		m_StartMenu.SetActive(false);
	}

	public void LoadLevelScene(string sceneName)
	{
		m_CurrentScene = sceneName;
		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
	}

	public void SwapRoom(Direction dir)
	{

		LifeRoom.Transition variant = null;
		switch(dir)
		{
			case Direction.Left:		variant = m_CurrentRoom.m_LeftRoomTrans;	break;
			case Direction.Right:		variant = m_CurrentRoom.m_RightRoomTrans;	break;
			case Direction.Up:			variant = m_CurrentRoom.m_UpRoomTrans;		break;
			case Direction.Down:		variant = m_CurrentRoom.m_DownRoomTrans;	break;
		}

		if(m_CurrentRoom.CanMove(dir))
		{
			SoundManager.Instance.Play("Swap");

			m_CurrentRoom.Leave(dir);

			m_CurrentRoom = variant.m_Room;
			
			m_CurrentRoom.Enter(DirectionHelper.g_OppositeDirection[dir]);

			m_Player.MoveToRoom(m_CurrentRoom);
		}
		else
		{
			// transition unvalid
			Debug.Log("Unvalid Transition");
		}
	}

	public void CantMoveAnyDirecton()
	{
		Debug.Log("Cant move any direction");
		//LeaveGame();
	}

	public void LockControll()
	{
		m_LockControll = true;
	}

	public void UnlockControll()
	{
		m_LockControll = false;
	}

	//////////////////////////////////////////////////////////////////////////
	[InspectorButton]
	void EnterCall()
	{
		m_CurrentRoom.Enter(Direction.Left);
	}

	[InspectorButton]
	void SwapLeft()
	{
		SwapRoom(Direction.Left);
	}

	[InspectorButton]
	void SwapRight()
	{
		SwapRoom(Direction.Right);
	}
	
	[InspectorButton]
	void SwapUp()
	{
		SwapRoom(Direction.Up);
	}
	
	[InspectorButton]
	void SwapDown()
	{
		SwapRoom(Direction.Down);
	}
}
