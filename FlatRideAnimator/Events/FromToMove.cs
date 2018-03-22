#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("From-ToMove")]
public class FromToMove : RideAnimationEvent
{
	public Mover Mover;

	float lastTime;
	public override string EventName {
		get {
			return "From-To Move";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Mover) {
			ColorIdentifier = Mover.ColorIdentifier;
		}
		foreach (Mover R in motors.OfType<Mover>().ToList()) {
			if (R == Mover)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (R.Identifier)) {
				Mover = R;
			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		lastTime = Time.realtimeSinceStartup;

		Mover.startFromTo ();
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (Mover) {
			Mover.Tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (Mover.ReachedTarget ()) {
				Done = true;
			}
			base.Run (root);
		}

	}


	public override void Deserialize (Dictionary<string,object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("MoverIndex"))
			Mover = (Mover) motors[Convert.ToInt32(elements["MoverIndex"])];
		
		base.Deserialize (elements,motors);
	}

	public override Dictionary<string,object> Serialize (Transform root, Motor[] motors)
	{
		if (Mover == null)
			return null;
		return new Dictionary<string, object>(){
			{"MoverIndex", Array.IndexOf(motors,Mover)}
		};
	}
}

