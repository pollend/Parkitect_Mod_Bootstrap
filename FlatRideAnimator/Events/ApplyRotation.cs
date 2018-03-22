#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("ApplyRotations")]
public class ApplyRotation : RideAnimationEvent
{
	[SerializeField]
	public MultipleRotations Rotator;
	private float _lastTime;


	public override string EventName {
		get {
			return "ApplyRotations";
		}
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{

		if (rotator) {
			ColorIdentifier = rotator.ColorIdentifier;
		}
		foreach (MultipleRotations R in motors.OfType<MultipleRotations>().ToList()) {
			if (R == rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button (R.Identifier)) {
				rotator = R;

			}
			GUI.color = Color.white;
		}
		base.RenderInspectorGUI (motors);
	}
#endif

	public override void Enter()
	{

	}
	public override void Run(Transform root)
	{
		if (Rotator)
		{
			Rotator.Tick(Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			Done = true;
			base.Run(root);
		}

	}

	public override void Deserialize(Dictionary<string, object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex"))
			Rotator = (MultipleRotations) motors[Convert.ToInt32(elements["RotatorIndex"])];
		base.Deserialize(elements, motors);
	}

	public override Dictionary<string, object> Serialize(Transform root,Motor[] motors)
	{
		if (Rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"RotatorIndex", Array.IndexOf(motors,Rotator)}
		};
	}

}
