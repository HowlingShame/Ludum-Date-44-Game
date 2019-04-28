using Gamelogic.Extensions;
using Malee;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LifeRoom : GLMonoBehaviour
{
	public string			m_VisualName;
	[Header("Transitions")]

	[Reorderable(elementNameOverride = null, paginate = false), SerializeField ]
	private TransitionVariants		m_Left;
	[Reorderable(elementNameOverride = null, paginate = false), SerializeField]
	private TransitionVariants		m_Right;
	[Reorderable(elementNameOverride = null, paginate = false), SerializeField]
	private TransitionVariants		m_Up;
	[Reorderable(elementNameOverride = null, paginate = false), SerializeField]
	private TransitionVariants		m_Down;
	
	[NonSerialized]	public Transition		m_LeftRoomTrans;
	[NonSerialized]	public Transition		m_RightRoomTrans;
	[NonSerialized]	public Transition		m_UpRoomTrans;
	[NonSerialized]	public Transition		m_DownRoomTrans;
	
	public LifeRoom		m_LeftRoom;
	public LifeRoom		m_RightRoom;
	public LifeRoom		m_UpRoom;
	public LifeRoom		m_DownRoom;
	
	[HideInInspector]
	public Direction	m_LeaveDirection;
	[HideInInspector]
	public Direction	m_EnterDirection;


	[Header("Events")]
	public UnityEvent		m_OnEnter;
	public UnityEvent		m_OnLeave;
		
	public Sign				m_LeftSign;
	public Sign				m_RightSign;
	public Sign             m_UpSign;
	public Sign				m_DownSign;


	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class Transition
	{
		public string				m_Description;
		public ConditionShowMode	m_ShowMod;
		public float				m_Priority = 1.0f;

		public StatCondition	m_Condition;

		public LifeRoom			m_Room;

		public Cost				m_Cost;

		public UnityEvent		m_ActivateEvent;


		//////////////////////////////////////////////////////////////////////////
		[Serializable]
		public enum ConditionShowMode
		{
			ShowNever = 0,
			ShowIfFailed = 1,
			ShowIfSucceeded = 2,
			ShowAlvays = ShowIfFailed | ShowIfSucceeded,


		}

		[Serializable]
		public class StatCondition
		{
			public bool				m_Enable = false;
			
			public Vector2Int		m_AgeCondition = new Vector2Int(0, 0);			
			public Vector2Int		m_HealfCondition = new Vector2Int(0, 0);			
			public Vector2Int		m_HappinessCondition = new Vector2Int(0, 0);			
			public Vector2Int		m_WealthCondition = new Vector2Int(0, 0);

			//////////////////////////////////////////////////////////////////////////
			public bool СonditionFulfilled()
			{
				if(m_Enable == false)
					return true;


				var player = GameManager.Instance.m_Player;
				return true;
			}
		}
		[Serializable]
		public class Cost
		{			
			public int		m_Age;
			public int		m_Health;
			public int		m_Happiness;
			public int		m_Wealth;

			//////////////////////////////////////////////////////////////////////////
			public void Pay()
			{
				GameManager.Instance.m_Player.Age		+= m_Age;
				GameManager.Instance.m_Player.Health	+= m_Health;
				GameManager.Instance.m_Player.Happiness	+= m_Happiness;
				GameManager.Instance.m_Player.Wealth	+= m_Wealth;
			}
		}
	}

	[Serializable]
	public class TransitionVariants : ReorderableArray<Transition> {}

	//////////////////////////////////////////////////////////////////////////
	public Transition GetRoom(Direction dir)
	{
		TransitionVariants variants = null;
		switch(dir)
		{
			case Direction.Left:		variants = m_Left;	break;
			case Direction.Right:		variants = m_Right;	break;
			case Direction.Up:			variants = m_Up;	break;
			case Direction.Down:		variants = m_Down;	break;
		}

		if(variants == null)
			return null;

		var options = variants
			.Where((t) => t.m_Condition.СonditionFulfilled());
		var weights = options
			.Select((t) => t.m_Priority);

		
		var bag = GameManager.Instance.m_Random.WeightedBag(options.ToArray(), weights.ToArray());
		if(bag.Data.Count == 0)
			return null;

		var result = bag.Next();
		if(result.m_Room == null)	// for random events
			return null;

		return result;
	}
	
	public bool CanMove(Direction dir)
	{
		switch(dir)
		{
			case Direction.Left:		return m_LeftRoomTrans != null && m_LeftSign.Open;
			case Direction.Right:		return m_RightRoomTrans != null && m_RightSign.Open;
			case Direction.Up:			return m_UpRoomTrans != null && m_UpSign.Open;
			case Direction.Down:		return m_DownRoomTrans != null && m_DownSign.Open;
		}

		return false;
	}

	public void Enter(Direction dirFrom)
	{
		// selsect rooms
		m_LeftRoomTrans = GetRoom(Direction.Left);
		m_RightRoomTrans = GetRoom(Direction.Right);
		m_UpRoomTrans = GetRoom(Direction.Up);
		m_DownRoomTrans = GetRoom(Direction.Down);
		
		// play animation
		m_EnterDirection = dirFrom;
		switch(dirFrom)
		{
			case Direction.Left:	GetComponent<Animator>().Play("Enter_Left");	break;
			case Direction.Right:	GetComponent<Animator>().Play("Enter_Right");	break;
			case Direction.Up:		GetComponent<Animator>().Play("Enter_Up");		break;
			case Direction.Down:	GetComponent<Animator>().Play("Enter_Down");	break;
		}
		
		// update rooms
		m_LeftRoom = m_LeftRoomTrans?.m_Room;
		m_RightRoom = m_RightRoomTrans?.m_Room;
		m_UpRoom = m_UpRoomTrans?.m_Room;
		m_DownRoom = m_DownRoomTrans?.m_Room;

		// fill condition signs
		implFillSign(m_LeftSign, m_LeftRoomTrans);
		implFillSign(m_RightSign, m_RightRoomTrans);
		implFillSign(m_UpSign, m_UpRoomTrans);
		implFillSign(m_DownSign, m_DownRoomTrans);


		// invoke events
		m_OnEnter.Invoke();

		// check if valid vays exists
		if(CanMove(Direction.Left) == false &&
			CanMove(Direction.Right) == false &&
			CanMove(Direction.Up) == false &&
			CanMove(Direction.Down) == false)
			GameManager.Instance.CantMoveAnyDirecton();

		// set room name
		GameManager.Instance.m_RoomName.SetRoomName(m_VisualName);
	}

	public void Leave(Direction	dirTo)
	{
		// play animation
		m_LeaveDirection = dirTo;
		switch(dirTo)
		{
			case Direction.Left:	GetComponent<Animator>().Play("Leave_Left");	break;
			case Direction.Right:	GetComponent<Animator>().Play("Leave_Right");	break;
			case Direction.Up:		GetComponent<Animator>().Play("Leave_Up");		break;
			case Direction.Down:	GetComponent<Animator>().Play("Leave_Down");	break;
		}
		
		// run events, pay cost
		Transition	trans = null;
		switch(dirTo)
		{
			case Direction.Left:		trans = m_LeftRoomTrans;	break;
			case Direction.Right:		trans = m_RightRoomTrans;	break;
			case Direction.Up:			trans = m_UpRoomTrans;		break;
			case Direction.Down:		trans = m_DownRoomTrans;	break;
		}
		trans.m_Cost.Pay();
		trans.m_ActivateEvent.Invoke();

		// invoke events
		m_OnLeave.Invoke();
	}

	private void implFillSign(Sign sign, Transition trans)
	{
		if(trans != null)
		{
			sign.transform.parent.gameObject.SetActive(true);
			sign.SetDescription(trans.m_Description);

			bool open = true;
			if(trans.m_Condition.m_Enable)
			{
				sign.PrepareSetConditions();

				{	// health
					var check = trans.m_Condition.m_HealfCondition.Lenght() != 0;
					var succeeded = trans.m_Condition.m_HealfCondition.InRangeInclusive(GameManager.Instance.m_Player.Health);

					if(succeeded == false && check)		open = false;

					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfSucceeded) && check && succeeded)
						sign.ShowHealthCondition(true, succeeded);
					else
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfFailed) && check && !succeeded)		
						sign.ShowHealthCondition(true, succeeded);
					else
						sign.ShowHealthCondition(false, false);
				}
				{	// happiness
					var check = trans.m_Condition.m_HappinessCondition.Lenght() != 0;
					var succeeded = trans.m_Condition.m_HappinessCondition.InRangeInclusive(GameManager.Instance.m_Player.Happiness);

					if(succeeded == false && check)		open = false;
					
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfSucceeded) && check && succeeded)
						sign.ShowHappinessCondition(true, succeeded);
					else
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfFailed) && check && !succeeded)		
						sign.ShowHappinessCondition(true, succeeded);
					else
						sign.ShowHappinessCondition(false, false);
				}
				{	// wealth
					var check = trans.m_Condition.m_WealthCondition.Lenght() != 0;
					var succeeded = trans.m_Condition.m_WealthCondition.InRangeInclusive(GameManager.Instance.m_Player.Wealth);

					if(succeeded == false && check)		open = false;
					
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfSucceeded) && check && succeeded)
						sign.ShowWealthCondition(true, succeeded);
					else
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfFailed) && check && !succeeded)		
						sign.ShowWealthCondition(true, succeeded);
					else
						sign.ShowWealthCondition(false, false);
				}
				{	// age
					var check = trans.m_Condition.m_AgeCondition.Lenght() != 0;
					var succeeded = trans.m_Condition.m_AgeCondition.InRangeInclusive(GameManager.Instance.m_Player.Age);

					if(succeeded == false && check)		open = false;
					
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfSucceeded) && check && succeeded)
						sign.ShowAgeCondition(true, succeeded);
					else
					if(trans.m_ShowMod.HasFlag(Transition.ConditionShowMode.ShowIfFailed) && check && !succeeded)		
						sign.ShowAgeCondition(true, succeeded);
					else
						sign.ShowAgeCondition(false, false);
				}
			}
			else
			{
				sign.ShowHealthCondition(false, false);
				sign.ShowHappinessCondition(false, false);
				sign.ShowWealthCondition(false, false);
				sign.ShowAgeCondition(false, false);
			}

			sign.Open = open;
		}
		else
		{
			sign.transform.parent.gameObject.SetActive(false);
			
//			sign.GetComponent<Animator>().Play("Blocked");

//			sign.ShowHealthCondition(false, false);
//			sign.ShowHappinessCondition(false, false);
//			sign.ShowWealthCondition(false, false);
//			sign.ShowAgeCondition(false, false);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public void LockControll()
	{
		GameManager.Instance.LockControll();
	}

	public void UnlockControll()
	{
		GameManager.Instance.UnlockControll();
	}

	public void SetBackgroundColor(Color color)
	{
		Core.Instance.m_Camera.backgroundColor = color;
	}


	public void GameOverLeave()
	{
		GameManager.Instance.GameOver(GameManager.GameOverType.Silent);
	}
	public void GameOverDeath()
	{
		GameManager.Instance.GameOver(GameManager.GameOverType.Explosion);
	}

	public void AddNote(string note)
	{
		GameManager.Instance.m_Summary.AddNote(note);
	}

	public void IncrementAge(int inc)
	{
		GameManager.Instance.m_Player.Age += inc;
	}

	public void IncrementHealth(int inc)
	{
		GameManager.Instance.m_Player.Health += inc;
	}

	public void IncrementHappiness(int inc)
	{
		GameManager.Instance.m_Player.Happiness += inc;
	}


	public void IncrementWealth(int inc)
	{
		GameManager.Instance.m_Player.Wealth += inc;
	}

}
