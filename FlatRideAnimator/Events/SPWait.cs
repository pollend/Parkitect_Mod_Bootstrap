#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using System;

[Serializable]
[ExecuteInEditMode]
public class SPWait : SPRideAnimationEvent
{
	[SerializeField]
	public float seconds;
	float timeLimit;
	public override string EventName {
		get {
			return "Wait";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		seconds = EditorGUILayout.FloatField ("Seconds", seconds);
		if (isPlaying) {
			GUILayout.Label ("Time" + (timeLimit - Time.realtimeSinceStartup));
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		timeLimit = Time.realtimeSinceStartup + seconds;
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (Time.realtimeSinceStartup > timeLimit) {

			done = true;
		} else {

		}
		base.Run (root);
	}
	public override List<XElement> Serialize (Transform root)
	{
		return new List<XElement> (new XElement[] {
			new XElement ("timeLimit", timeLimit)
		});
	}
}

