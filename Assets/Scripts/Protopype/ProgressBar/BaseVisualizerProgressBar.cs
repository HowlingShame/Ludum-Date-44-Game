using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseVisualizerProgressBar : MonoBehaviour
{
	public abstract void UpdateStatus(StatusProgressBar status);
}
