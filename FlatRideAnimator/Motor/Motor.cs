﻿using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class Motor : ScriptableObject
{
	public bool showSettings;
	public string Identifier = "";
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }
	public void Awake()
	{
		ColorIdentifier = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
	}
	#if UNITY_EDITOR
	public virtual void InspectorGUI(Transform root)
	{
		ColorIdentifier = EditorGUILayout.ColorField("Color ", ColorIdentifier);
	}
	#endif
	public virtual void Enter(Transform root)
	{

	}
	public virtual void Reset(Transform root)
	{

	}

	public virtual void PrepareExport(ParkitectObj parkitectObj)
	{
	}
}