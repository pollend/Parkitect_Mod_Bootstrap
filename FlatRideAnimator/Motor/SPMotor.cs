
 #if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SPMotor : ScriptableObject
{
	[NonSerialized]
	public bool showSettings;
	public string Identifier = "";
	public Color ColorIdentifier;
	public virtual string EventName { set; get; }
	public void Awake()
	{
		ColorIdentifier = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
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
		
	public virtual Dictionary<string,object> Serialize(Transform root){return null;}
	public virtual void Deserialize(Dictionary<string,object> elements){}

}
