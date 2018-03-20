
 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class SPMover : SPMotor
{

	private enum State
	{
		RUNNING,
		STOPPED
	}
	[SerializeField]
	public RefrencedTransform axis = new RefrencedTransform();

	[SerializeField]
	public Vector3 originalRotationValue;
	[SerializeField]
	private Vector3 fromPosition;
	[SerializeField]
	public Vector3 toPosition;
	[SerializeField]
	public float duration = 10f;

	[SerializeField]
	private State currentState = State.STOPPED;
	[SerializeField]
	private float currentPosition = 1f;
	[SerializeField]
	private int direction = -1;

	public override void Reset(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
			transform.localPosition = originalRotationValue;
		currentPosition = 1f;
		direction = -1;
		base.Reset(root);
	}
	public override string EventName
	{
		get
		{
			return "Mover";
		}
	}
#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{
		Identifier = EditorGUILayout.TextField("Name ", Identifier);
		axis.SetSceneTransform((Transform)EditorGUILayout.ObjectField("axis", axis.FindSceneRefrence (root), typeof(Transform), true));
	    toPosition = EditorGUILayout.Vector3Field("Move To", toPosition);
	    duration = EditorGUILayout.FloatField("Time", duration);
		base.InspectorGUI(root);

	}
#endif
	public override void Enter(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
			originalRotationValue = transform.localPosition;
		currentPosition = 1f;

		direction = -1;
		Initialize(root, axis.FindSceneRefrence(root), transform.localPosition, toPosition, duration);
		base.Enter(root);
	}
	public void Initialize(Transform root, Transform axis, Vector3 fromPosition, Vector3 toPosition, float duration)
	{
		this.axis.SetSceneTransform(axis);
		this.fromPosition = fromPosition;
		this.toPosition = toPosition;
		this.duration = duration;
		setPosition(root);
	}

	public bool startFromTo()
	{
		if (direction != 1)
		{
			direction = 1;
			currentPosition = 0f;
			currentState = State.RUNNING;
			return true;
		}
		return false;
	}

	public bool startToFrom()
	{
		if (direction != -1)
		{
			direction = -1;
			currentPosition = 0f;
			currentState = State.RUNNING;
			return true;
		}
		return false;
	}

	public bool reachedTarget()
	{
		return currentState == State.STOPPED && currentPosition >= 1f;
	}

	public void tick(float dt, Transform root)
	{
		currentPosition += dt * 1f / duration;
		if (currentPosition >= 1f)
		{
			currentPosition = 1f;
			currentState = State.STOPPED;
		}
		setPosition(root);
	}

	private void setPosition(Transform root)
	{
		Vector3 a;
		Vector3 b;
		if (direction == 1)
		{
			a = fromPosition;
			b = toPosition;
		}
		else
		{
			a = toPosition;
			b = fromPosition;
		}
		Transform transform = axis.FindSceneRefrence(root);
		if (transform != null)
			transform.localPosition = Vector3.Lerp(a, b, MathHelper.Hermite(0f, 1f, currentPosition));
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}


	public override Dictionary<string,object> Serialize (Transform root)
	{
		return new Dictionary<string, object>{
			{"transform",axis.Serialize(root)},
			{"from", Utility.SerializeVector(fromPosition)},
			{"to", Utility.SerializeVector(toPosition)},
			{"duration",duration}
		};
	}


}

