#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("From-ToRot")]
public class FromToRot : RideAnimationEvent
{
	public RotateBetween Rotator;
	float _lastTime;

	public override string EventName {
		get {
			return "From-To Rot";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Rotator) {
			ColorIdentifier = Rotator.ColorIdentifier;
		}
		foreach (RotateBetween R in motors.OfType<RotateBetween>().ToList()) {
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

		Rotator.StartFromTo();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Rotator) {
			Rotator.Tick (Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Rotator.IsStopped ()) {
				Done = true;
			}
			base.Run (root);
		}
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("rotator")) {
			Rotator = new RotateBetween ();
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
