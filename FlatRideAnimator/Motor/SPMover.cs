﻿using System;
using System.Xml.Linq;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif
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
	private SPMover.State currentState = SPMover.State.STOPPED;
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
		this.currentPosition = 1f;

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
		this.setPosition(root);
	}

	public bool startFromTo()
	{
		if (this.direction != 1)
		{
			this.direction = 1;
			this.currentPosition = 0f;
			this.currentState = SPMover.State.RUNNING;
			return true;
		}
		return false;
	}

	public bool startToFrom()
	{
		if (this.direction != -1)
		{
			this.direction = -1;
			this.currentPosition = 0f;
			this.currentState = SPMover.State.RUNNING;
			return true;
		}
		return false;
	}

	public bool reachedTarget()
	{
		return this.currentState == SPMover.State.STOPPED && this.currentPosition >= 1f;
	}

	public void tick(float dt, Transform root)
	{
		this.currentPosition += dt * 1f / this.duration;
		if (this.currentPosition >= 1f)
		{
			this.currentPosition = 1f;
			this.currentState = SPMover.State.STOPPED;
		}
		this.setPosition(root);
	}

	private void setPosition(Transform root)
	{
		Vector3 a;
		Vector3 b;
		if (this.direction == 1)
		{
			a = this.fromPosition;
			b = this.toPosition;
		}
		else
		{
			a = this.toPosition;
			b = this.fromPosition;
		}
		Transform transform = this.axis.FindSceneRefrence(root);
		if (transform != null)
			transform.localPosition = Vector3.Lerp(a, b, MathHelper.Hermite(0f, 1f, this.currentPosition));
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}

	public override List<XElement> Serialize ()
	{
		
		return new System.Collections.Generic.List<XElement>(){
			new XElement("Identifier",Identifier),
			new XElement("Axis",null),
			new XElement("FromPosition",Utility.SerializeVector(this.fromPosition)),
			new XElement("ToPosition",Utility.SerializeVector(this.toPosition)),
			new XElement("Duration",this.duration)

		};
	}


}

