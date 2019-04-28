using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverTolltip : MonoBehaviour
{

	private void OnMouseEnter()
	{
		transform.GetChild(0).gameObject.SetActive(true);
	}

	private void OnMouseExit()
	{
		transform.GetChild(0).gameObject.SetActive(false);
	}

}
