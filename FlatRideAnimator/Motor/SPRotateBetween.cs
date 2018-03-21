
﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class SPRotateBetween : SPMotor
{
	[SerializeField] public RefrencedTransform axis = new RefrencedTransform();
	[SerializeField] public Quaternion fromRotation;
	[SerializeField] public Vector3 rotationAxis = Vector3.up;
	[SerializeField] public Quaternion toRotation;
	[SerializeField] public Quaternion originalRotationValue;
	[SerializeField] public float duration = 1f;

	[SerializeField] private float currentPosition;

	[SerializeField] private float direction;
	public override string EventName
	{
		get { return "RotateBetween"; }
	}
#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{

		Identifier = EditorGUILayout.TextField("Name ", Identifier);
		axis.SetSceneTransform(
			(Transform) EditorGUILayout.ObjectField("axis", axis.FindSceneRefrence(root), typeof(Transform), true));
		rotationAxis = EditorGUILayout.Vector3Field("Rotate To", rotationAxis);
		duration = EditorGUILayout.FloatField("Time", duration);
		base.InspectorGUI(root);
	}
#endif
	public override void Reset(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation = originalRotationValue;
		currentPosition = 0f;


		base.Reset(root);
	}

	public override void Enter(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
		{
			originalRotationValue = transform.localRotation;
			Initialize(transform, transform.localRotation, Quaternion.Euler(transform.localEulerAngles + rotationAxis),
				duration);
		}
	}

	public void Initialize(Transform axis, Quaternion fromRotation, Quaternion toRotation, float duration)
	{
		this.axis.SetSceneTransform(axis);
		this.fromRotation = fromRotation;
		this.toRotation = toRotation;
		this.duration = duration;
		axis.localRotation = Quaternion.Lerp(fromRotation, toRotation, 0);
	}

	public bool startFromTo()
	{
		if (direction != 1f)
		{
			direction = 1f;
			return true;
		}

		return false;
	}

	public bool startToFrom()
	{
		if (direction != -1f)
		{
			direction = -1f;
			return true;
		}

		return false;
	}

	public bool isStopped()
	{
		if (direction == 1f)
		{
			return currentPosition >= 0.99f;
		}

		return currentPosition <= 0.01f;
	}

	public void tick(float dt, Transform root)
	{
		currentPosition += dt * direction * 1f / duration;
		currentPosition = Mathf.Clamp01(currentPosition);
		var transform = axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation =
				Quaternion.Lerp(fromRotation, toRotation, MathHelper.Hermite(0f, 1f, currentPosition));
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}


	public override Dictionary<string, object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"axis", axis.Serialize(root)},
			{"fromRotation", Utility.SerializeQuaternion(fromRotation)},
			{"rotationAxis", Utility.SerializeVector(rotationAxis)},
			{"toRotation", Utility.SerializeQuaternion(toRotation)},
			{"duration", duration}
		};
	}
}
