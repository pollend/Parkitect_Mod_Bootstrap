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
	private float _lastTime;

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

	public override void Deserialize (Dictionary<string,object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex")) 
			Rotator = (RotateBetween) motors[Convert.ToInt32(elements["RotatorIndex"])];
		base.Deserialize (elements,motors);
	}


	public override Dictionary<string,object> Serialize (Transform root, Motor[] motors)
	{
		if (Rotator == null)
			return null;
		
		return new Dictionary<string, object>(){
			{"RotatorIndex", Array.IndexOf(motors,Rotator)}
		};
	}
}
