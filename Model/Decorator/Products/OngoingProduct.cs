using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class OngoingProduct : Product
{
	[SerializeField]
	public int Duration;
	[SerializeField]
	public bool RemoveWhenDepleted;
	[SerializeField]
	public bool DestroyWhenDepleted;

	#if UNITY_EDITOR
	public override void RenderInspectorGUI ()
	{

		Duration = EditorGUILayout.IntField("Duration ", Duration);
		RemoveWhenDepleted = EditorGUILayout.Toggle ("Remove When Depleted", RemoveWhenDepleted);
		DestroyWhenDepleted = EditorGUILayout.Toggle ("Destroy When Depleted", DestroyWhenDepleted);

		base.RenderInspectorGUI ();
	}
	#endif
}