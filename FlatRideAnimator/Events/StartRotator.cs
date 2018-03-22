#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("StartRotator")]
public class StartRotator : RideAnimationEvent
{
	public Rotator Rotator;
	private float _lastTime;
	public override string EventName {
		get {
			return "StartRotator";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Rotator) {
			ColorIdentifier = Rotator.ColorIdentifier;
		}
		foreach (Rotator R in motors.OfType<Rotator>().ToList()) {
			if (R == Rotator)
				GUI.color = Color.red / 1.3f;
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

		Rotator.Start ();
		base.Enter ();
	}

	public override void Run(Transform root)
	{
		if (Rotator) {

			Rotator.tick (Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Rotator.ReachedFullSpeed ()) {
				Done = true;
			}
			base.Run (root);
		}
	}

	public override void Deserialize (Dictionary<string,object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex"))
			Rotator = (Rotator) motors[Convert.ToInt32(elements["RotatorIndex"])];


		base.Deserialize (elements,motors);
	}

	public override Dictionary<string,object> Serialize (Transform root, Motor[] motors)
	{
		if (Rotator == null)
			return null;
		
		return new Dictionary<string,object> {
			{"RotatorIndex", Array.IndexOf(motors, Rotator)}
		};
	}


}

