#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[MotorTag("PendulumRotator")]
public class PendulumRotator : Rotator
{
	[SerializeField] public float ArmLength;
	[SerializeField] public float Gravity;
	[SerializeField] public float AngularFriction;
	[SerializeField] public bool Pendulum;

#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{
		ArmLength = EditorGUILayout.FloatField("armLength ", ArmLength);
		Gravity = EditorGUILayout.FloatField("gravity", Gravity);
		AngularFriction = EditorGUILayout.FloatField("angularFriction", AngularFriction);
		Pendulum = EditorGUILayout.Toggle("pendulum", Pendulum);
		base.InspectorGUI(root);
	}
#endif

	public override string EventName
	{
		get { return ""; }
	}

	public void SetActAsPendulum(bool pendulum)
	{
		Pendulum = pendulum;
	}

	public override void tick(float dt, Transform root)
	{
		Transform transformAxis = Axis.FindSceneRefrence(root);
		if (transformAxis)
			return;

		if (!Pendulum)
		{
			base.tick(dt, root);
			return;
		}

		float num = -1f * Gravity * Mathf.Sin(transformAxis.localEulerAngles[RotationAxisIndex] * 0.0174532924f) /
		            ArmLength * 157.29578f;
		num = Mathf.Clamp(num, -AccelerationSpeed, AccelerationSpeed);
		CurrentSpeed += num * dt;
		CurrentRotation += num * dt;
		CurrentSpeed -= CurrentSpeed * AngularFriction * dt;
		CurrentSpeed = Mathf.Clamp(CurrentSpeed, -MaxSpeed, MaxSpeed);
		Vector3 localEulerAngles = transformAxis.localEulerAngles;
		int rotationAxisIndex;
		int expr_C6 = rotationAxisIndex = RotationAxisIndex;
		float num2 = localEulerAngles[rotationAxisIndex];
		localEulerAngles[expr_C6] = num2 + CurrentSpeed * dt;
		transformAxis.localEulerAngles = localEulerAngles;
		if (CurrentState == State.REQUEST_STOP && Mathf.Abs(CurrentSpeed) <= 0.5f &&
		    Mathf.Abs(num) <= 0.3f)
		{
			ChangeState(State.STOPPING);
			transformAxis.localRotation = InitialRotation;
			CurrentSpeed = 0f;
		}
	}


	public override Dictionary<string,object> Serialize(Transform root)
	{
		Dictionary<string,object> result = base.Serialize(root);
		result.Add("armLength",ArmLength);
		result.Add("gravity",Gravity);
		result.Add("angularFriction",AngularFriction);
		result.Add("pendulum",Pendulum);

		return result;
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("armLength"))
			ArmLength = Convert.ToSingle(elements["armLength"]);
		if (elements.ContainsKey("gravity"))
			Gravity = Convert.ToSingle(elements["gravity"]);
		if (elements.ContainsKey("angularFriction"))
			AngularFriction = Convert.ToSingle(elements["angularFriction"]);
		if (elements.ContainsKey("pendulum"))
			Pendulum = (bool) elements["pendulum"];


		base.Deserialize(elements);
	}
}
