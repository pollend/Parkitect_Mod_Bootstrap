#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
public class SPFromToRot : SPRideAnimationEvent
{
	public SPRotateBetween Rotator;
	float _lastTime;

	public override string EventName {
		get {
			return "From-To Rot";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (Rotator) {
			ColorIdentifier = Rotator.ColorIdentifier;
		}
		foreach (SPRotateBetween R in motors.OfType<SPRotateBetween>().ToList()) {
			if (R == Rotator)
				GUI.color = Color.red;
			if (GUILayout.Button (R.Identifier)) {
				Rotator = R;
			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		_lastTime = Time.realtimeSinceStartup;

		Rotator.startFromTo();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Rotator) {
			Rotator.tick (Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Rotator.isStopped ()) {
				Done = true;
			}
			base.Run (root);
		}
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator")) {
			Rotator = new SPRotateBetween ();
			Rotator.Deserialize ((Dictionary<string, object>)elements["rotator"]);
		}

		base.Deserialize (elements);
	}


	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (Rotator == null)
			return null;
		
		return new Dictionary<string, object>(){
			{"rotator", Rotator.Serialize (root)}
		};
	}
}
