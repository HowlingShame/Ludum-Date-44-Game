using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StatusProgressBar))]
public class StatusProgressBarInspector : Editor
{
	public override VisualElement CreateInspectorGUI()
	{
		var container = new VisualElement();

		//UnityEngine.UIElements.
		//UnityEditor.UIElements.
		//var c = new UnityEditor.UIElements.
		//var rangeSlider = new MinMaxSlider("range", 1, 2, -10, 20);


		var rangeSlider = new Slider("Range", serializedObject.FindProperty("m_MinValue").floatValue, serializedObject.FindProperty("m_MaxValue").floatValue);
		var minValue = new FloatField("MinValue");
		var maxValue = new FloatField("MaxValue");
		var status = new FloatField("Status");

//		maxValue.isReadOnly = true; 
//		maxValue.pickingMode = PickingMode.Ignore;
//		maxValue.focusable = false;
//		status.style.overflow = Overflow.Hidden;

		{	// Slider
			rangeSlider.RegisterValueChangedCallback((e) =>
			{
				serializedObject.FindProperty("m_Status").floatValue = e.newValue;
				status.value = e.newValue;
				serializedObject.ApplyModifiedProperties();
			});
			container.Add(rangeSlider);
		}

		{	// MinValue
			minValue.value = serializedObject.FindProperty("m_MinValue").floatValue;
			minValue.RegisterValueChangedCallback((e) =>
			{
				rangeSlider.lowValue = e.newValue;
				serializedObject.FindProperty("m_MinValue").floatValue = e.newValue;
				serializedObject.ApplyModifiedProperties();
			});
			container.Add(minValue);
		}

		{	// MaxValue
			maxValue.value = serializedObject.FindProperty("m_MaxValue").floatValue;
			maxValue.RegisterValueChangedCallback((e) =>
			{
				rangeSlider.highValue = e.newValue;
				serializedObject.FindProperty("m_MaxValue").floatValue = e.newValue;
				serializedObject.ApplyModifiedProperties();
			});
			container.Add(maxValue);
		}
		
		{	// Status
			status.RegisterValueChangedCallback((e) =>
			{
				var newValue = e.newValue;
				if(e.newValue <= rangeSlider.lowValue)
					newValue = rangeSlider.lowValue;

				if(e.newValue >= rangeSlider.highValue)
					newValue = rangeSlider.highValue;
				
				status.value = newValue;
				rangeSlider.value = status.value;

				serializedObject.FindProperty("m_Status").floatValue = e.newValue;
				serializedObject.ApplyModifiedProperties();
				

			});
			container.Add(status);
		}
		return container;
	}
}
