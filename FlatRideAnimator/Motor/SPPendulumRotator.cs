
 #if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

public class SPPendulumRotator : SPRotator
{
	[SerializeField] public float armLength;
	[SerializeField] public float gravity;
	[SerializeField] public float angularFriction;
	[SerializeField] public bool pendulum;

#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{
		armLength = EditorGUILayout.FloatField("armLength ", armLength);
		gravity = EditorGUILayout.FloatField("gravity", gravity);
		angularFriction = EditorGUILayout.FloatField("angularFriction", angularFriction);
		pendulum = EditorGUILayout.Toggle("pendulum", pendulum);
		base.InspectorGUI(root);
	}
#endif

	public override string EventName
	{
		get { return ""; }
	}

	public void setActAsPendulum(bool pendulum)
	{
		this.pendulum = pendulum;
	}

	public override void tick(float dt, Transform root)
	{
		Transform transformAxis = Axis.FindSceneRefrence(root);
		if (transformAxis)
			return;

		if (!this.pendulum)
		{
			base.tick(dt, root);
			return;
		}

		float num = -1f * this.gravity * Mathf.Sin(transformAxis.localEulerAngles[this.RotationAxisIndex] * 0.0174532924f) /
		            this.armLength * 157.29578f;
		num = Mathf.Clamp(num, -this.AccelerationSpeed, this.AccelerationSpeed);
		this.CurrentSpeed += num * dt;
		this.CurrentRotation += num * dt;
		this.CurrentSpeed -= this.CurrentSpeed * this.angularFriction * dt;
		this.CurrentSpeed = Mathf.Clamp(this.CurrentSpeed, -this.MaxSpeed, this.MaxSpeed);
		Vector3 localEulerAngles = transformAxis.localEulerAngles;
		int rotationAxisIndex;
		int expr_C6 = rotationAxisIndex = this.RotationAxisIndex;
		float num2 = localEulerAngles[rotationAxisIndex];
		localEulerAngles[expr_C6] = num2 + this.CurrentSpeed * dt;
		transformAxis.localEulerAngles = localEulerAngles;
		if (this.CurrentState == SPRotator.State.REQUEST_STOP && Mathf.Abs(this.CurrentSpeed) <= 0.5f &&
		    Mathf.Abs(num) <= 0.3f)
		{
			base.ChangeState(SPRotator.State.STOPPING);
			transformAxis.localRotation = this.InitialRotation;
			this.CurrentSpeed = 0f;
		}
	}


	public override Dictionary<string,object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"armLength", armLength},
			{"gravity", gravity},
			{"angularFriction", angularFriction},
			{"pendulum", pendulum},
			{"axis", Axis.Serialize(root)},
			{"minRotationSpeedPercent", MinRotationSpeedPercent},
			{"rotationAxisIndex", RotationAxisIndex},
			{"rotationAxis", Utility.SerializeVector(RotationAxis)},
			{"maxSpeed", MaxSpeed},
			{"accelerationSpeed", AccelerationSpeed}
		};
	}
}
