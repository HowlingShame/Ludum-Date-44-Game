using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
	public bool		m_AgeCondition;
	public bool		m_HealthCondition;
	public bool		m_HappinessCondition;
	public bool		m_WealthCondition;

	public Direction	m_SwapDirection;
	public bool			m_Open;
	public bool			Open
	{
		get
		{
			return m_Open;
		}
		set
		{
			GetComponent<Animator>().Play(value ? "Open" : "Close");
			m_Open = value;
		}
	}

	public Vector3	m_SelectScale;
	private Vector3	m_InitialScale;

	[SerializeField]	private GameObject	m_AgeSign;
	[SerializeField]	private GameObject	m_HealthSign;
	[SerializeField]	private GameObject  m_HappinessSign;
	[SerializeField]	private GameObject	m_WealthSign;

	[SerializeField]	private GameObject		m_DescriptionTooltip;


	[SerializeField]	private List<Transform>	m_Positions;
	protected int		m_PositionFillIndex;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		m_InitialScale = transform.localScale;
		
		m_DescriptionTooltip.SetActive(GameManager.Instance.m_AlwaysShowText);
	}

	private void OnMouseEnter()
	{
		if(GameManager.Instance.m_AlwaysShowText == false)
			m_DescriptionTooltip.SetActive(true);

		transform.localScale = m_SelectScale;
	}


	private void OnMouseExit()
	{
		if(GameManager.Instance.m_AlwaysShowText == false)
			m_DescriptionTooltip.SetActive(false);
		transform.localScale = m_InitialScale;
	}

	private void OnMouseDown()
	{
		if(GameManager.Instance.m_LockControll == false)
			GameManager.Instance.SwapRoom(m_SwapDirection);
		//LeanTween.moveLocalY( gameObject, 0.1f, 0.1f).setEase(LeanTweenType.easeInBounce ).setLoopClamp().setRepeat(2); 
		//LeanTween.moveLocalX( gameObject, 0.06f, 0.25f).setEase(LeanTweenType.easeShake ).setLoopClamp().setRepeat(2);
	}

	//////////////////////////////////////////////////////////////////////////
	public void PrepareSetConditions()
	{
		m_PositionFillIndex = 0;
//		m_AgeSign.SetActive(false);
//		m_HealthSign.SetActive(false);
//		m_HappinessSign.SetActive(false);
//		m_WealthSign.SetActive(false);
	}

	public void SetDescription(string text)
	{
		m_DescriptionTooltip.GetComponent<Text>().text = text;
	}

	public void ShowAgeCondition(bool show, bool succeeded)
	{
		if(show)
		{
			m_AgeSign.SetActive(true);

			var trans = m_Positions[m_PositionFillIndex].transform;
			m_AgeSign.transform.SetPositionAndRotation(trans.position, trans.rotation);
			m_AgeSign.GetComponent<ParameterCheck>().Check = succeeded;
			m_PositionFillIndex ++;
		}
		else
			m_AgeSign.SetActive(false);
	}

	public void ShowHealthCondition(bool show, bool succeeded)
	{
		if(show)
		{
			m_HealthSign.SetActive(true);

			var trans = m_Positions[m_PositionFillIndex].transform;
			m_HealthSign.transform.SetPositionAndRotation(trans.position, trans.rotation);
			m_HealthSign.GetComponent<ParameterCheck>().Check = succeeded;
			m_PositionFillIndex ++;
		}
		else
			m_HealthSign.SetActive(false);
	}

	public void ShowHappinessCondition(bool show, bool succeeded)
	{
		if(show)
		{
			m_HappinessSign.SetActive(true);

			var trans = m_Positions[m_PositionFillIndex].transform;
			m_HappinessSign.transform.SetPositionAndRotation(trans.position, trans.rotation);
			m_HappinessSign.GetComponent<ParameterCheck>().Check = succeeded;
			m_PositionFillIndex ++;
		}
		else
			m_HappinessSign.SetActive(false);
	}

	public void ShowWealthCondition(bool show, bool succeeded)
	{
		if(show)
		{
			m_WealthSign.SetActive(true);

			var trans = m_Positions[m_PositionFillIndex].transform;
			m_WealthSign.transform.SetPositionAndRotation(trans.position, trans.rotation);
			m_WealthSign.GetComponent<ParameterCheck>().Check = succeeded;
			m_PositionFillIndex ++;
		}
		else
			m_WealthSign.SetActive(false);
	}
}
