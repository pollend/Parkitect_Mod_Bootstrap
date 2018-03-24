#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[RideAnimationEventTag("SpinRotator")]
public class SpinRotater : RideAnimationEvent
{
	[SerializeField] public Rotator Rotator;
	[SerializeField] public bool Spin;
	[SerializeField] public float Spins = 1;
	private float _lastTime;


	public override string EventName
	{
		get { return "SpinRotator"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(Motor[] motors)
	{
		if (Rotator)
		{
			ColorIdentifier = Rotator.ColorIdentifier;
			Spin = EditorGUILayout.Toggle("amountOfSpins ", Spin);
			if (Spin)
				Spins = EditorGUILayout.FloatField("spins ", Spins);

			EditorGUILayout.LabelField("Amount " + Rotator.GetRotationsCount());
		}

		foreach (Rotator R in motors.OfType<Rotator>().ToList())
		{
			if (R == Rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button(R.Identifier))
			{
				Rotator = R;
			}

			GUI.color = Color.white;
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		_lastTime = Time.realtimeSinceStartup;
		Rotator.ResetRotations();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (Rotator)
		{
			Rotator.Tick(Time.realtimeSinceStartup - _lastTime, root);
			_lastTime = Time.realtimeSinceStartup;
			if (Spin)
			{
				if (Rotator.GetRotationsCount() >= Spins)
				{
					Done = true;
				}
			}
			else
			{
				Done = true;
			}

			base.Run(root);
		}
	}

	public override void Deserialize(Dictionary<string, object> elements, Motor[] motors)
	{
		if (elements.ContainsKey("RotatorIndex"))
			Rotator = (Rotator) motors[Convert.ToInt32(elements["RotatorIndex"])];

		if (elements.ContainsKey("Spin"))
			Spin = (bool) elements["Spin"];

		if (elements.ContainsKey("Spins"))
			Spins = Convert.ToInt32(elements["Spins"]);

		base.Deserialize(elements, motors);
	}

	public override Dictionary<string, object> Serialize(Transform root, Motor[] motors)
	{
		if (Rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"RotatorIndex", Array.IndexOf(motors, Rotator)},
			{"Spin", Spin},
			{"Spins", Spins}
		};
	}
}
