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
public class SPSpinRotater : SPRideAnimationEvent
{
	[SerializeField] public SPRotator rotator;
	[SerializeField] public bool spin;
	[SerializeField] public float spins = 1;
	float lastTime;


	public override string EventName
	{
		get { return "SpinRotator"; }
	}
#if UNITY_EDITOR
	public override void RenderInspectorGUI(SPMotor[] motors)
	{
		if (rotator)
		{
			ColorIdentifier = rotator.ColorIdentifier;
			spin = EditorGUILayout.Toggle("amountOfSpins ", spin);
			if (spin)
				spins = EditorGUILayout.FloatField("spins ", spins);

			EditorGUILayout.LabelField("Amount " + rotator.getRotationsCount());
		}

		foreach (SPRotator R in motors.OfType<SPRotator>().ToList())
		{
			if (R == rotator)
				GUI.color = Color.red / 1.3f;
			if (GUILayout.Button(R.Identifier))
			{
				rotator = R;
			}

			GUI.color = Color.white;
		}

		base.RenderInspectorGUI(motors);
	}
#endif

	public override void Enter()
	{
		lastTime = Time.realtimeSinceStartup;
		rotator.resetRotations();
		base.Enter();
	}

	public override void Run(Transform root)
	{
		if (rotator)
		{
			rotator.tick(Time.realtimeSinceStartup - lastTime, root);
			lastTime = Time.realtimeSinceStartup;
			if (spin)
			{
				if (rotator.getRotationsCount() >= spins)
				{
					done = true;
				}
			}
			else
			{
				done = true;
			}

			base.Run(root);
		}
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("rotator"))
		{
			rotator = new SPRotator();
			rotator.Deserialize((Dictionary<string, object>) elements["rotator"]);
		}

		if (elements.ContainsKey("spin"))
		{
			spin = (bool) elements["spin"];
		}

		if (elements.ContainsKey("spins"))
		{
			spins = Convert.ToInt32(elements["spins"]);
		}

		base.Deserialize(elements);
	}

	public override Dictionary<string, object> Serialize(Transform root)
	{
		if (rotator == null)
			return null;

		return new Dictionary<string, object>
		{
			{"rotator", rotator.Serialize(root)},
			{"spin", spin},
			{"spins", spins}
		};
	}
}
