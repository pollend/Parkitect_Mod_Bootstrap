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
	public Mover mover;

	float lastTime;
	public override string EventName {
		get {
			return "From-To Move";
		}
	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (mover) {
			ColorIdentifier = mover.ColorIdentifier;
		}
		foreach (Mover R in motors.OfType<Mover>().ToList()) {
			if (R == mover)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (R.Identifier)) {
				mover = R;
			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{
		lastTime = Time.realtimeSinceStartup;

		mover.startFromTo ();
		base.Enter ();
	}
	public override void Run(Transform root)
	{
		if (mover) {
			mover.Tick (Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (mover.ReachedTarget ()) {
				Done = true;
			}
			base.Run (root);
		}

	}


	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey ("mover") ) {
			this.mover = new Mover ();
			mover.Deserialize ((Dictionary<string, object>) elements["mover"]);
		}
		base.Deserialize (elements);
	}

	public override Dictionary<string,object> Serialize (Transform root)
	{
		if (mover == null)
			return null;
		return new Dictionary<string, object>(){
			{"mover", mover.Serialize (root)}
		};
	}
}

