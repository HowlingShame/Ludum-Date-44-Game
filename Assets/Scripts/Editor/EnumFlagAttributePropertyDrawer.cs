using System;
using UnityEngine;
using UnityEditor;
 
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
class EnumFlagAttributePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
 
		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
 
		EditorGUI.EndProperty();
	}
}
/*public static object GetValue(this SerializedProperty property)
{
	System.Type parentType = property.serializedObject.targetObject.GetType();
	System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);  
	return fi.GetValue(property.serializedObject.targetObject);
}
public static void SetValue(this SerializedProperty property,object value)
{
	System.Type parentType = property.serializedObject.targetObject.GetType();
	System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
	fi.SetValue(property.serializedObject.targetObject, value);
}*/