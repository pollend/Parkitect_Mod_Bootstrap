﻿using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Spark
{
	public class PendulumRotator : Rotator
	{
		[SerializeField]
		public float armLength;
		[SerializeField]
		public float gravity;
		[SerializeField]
		public float angularFriction;
		[SerializeField]
		public bool pendulum;
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
			get
			{
				return "";
			}
		}
		public void setActAsPendulum(bool pendulum)
		{
			this.pendulum = pendulum;
		}

		public override void tick(float dt, Transform root)
		{
			Transform transformAxis = axis.FindSceneRefrence(root);
			if (transformAxis)
				return;

			if (!this.pendulum)
			{
				base.tick(dt, root);
				return;
			}
			float num = -1f * this.gravity * Mathf.Sin(transformAxis.localEulerAngles[this.rotationAxisIndex] * 0.0174532924f) / this.armLength * 157.29578f;
			num = Mathf.Clamp(num, -this.accelerationSpeed, this.accelerationSpeed);
			this.currentSpeed += num * dt;
			this.currentRotation += num * dt;
			this.currentSpeed -= this.currentSpeed * this.angularFriction * dt;
			this.currentSpeed = Mathf.Clamp(this.currentSpeed, -this.maxSpeed, this.maxSpeed);
			Vector3 localEulerAngles = transformAxis.localEulerAngles;
			int rotationAxisIndex;
			int expr_C6 = rotationAxisIndex = this.rotationAxisIndex;
			float num2 = localEulerAngles[rotationAxisIndex];
			localEulerAngles[expr_C6] = num2 + this.currentSpeed * dt;
			transformAxis.localEulerAngles = localEulerAngles;
			if (this.currentState == Rotator.State.REQUEST_STOP && Mathf.Abs(this.currentSpeed) <= 0.5f && Mathf.Abs(num) <= 0.3f)
			{
				base.changeState(Rotator.State.STOPPING);
				transformAxis.localRotation = this.initialRotation;
				this.currentSpeed = 0f;
			}
		}
	}
}