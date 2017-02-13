using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
[ExecuteInEditMode]
public class SPRideAnimation
{

	[SerializeField]
	public List<SPMotor> motors = new List<SPMotor>();

	[SerializeField]
	public List<SPPhase> phases = new List<SPPhase>();

	[SerializeField]
	public SPPhase currentPhase;
	int phaseNum;

	[SerializeField]
	public bool animating;
	public void Animate(Transform root)
	{
		foreach (SPMotor m in motors)
		{
			m.Enter(root);
		}
		if (phases.Count <= 0)
		{
			animating = false;
			foreach (SPMotor m in motors)
			{
				m.Reset(root);
			}
			foreach (SPMultipleRotations R in motors.OfType<SPMultipleRotations>().ToList())
			{
				R.Reset(root);
			}
			return;
		}
		foreach (SPMotor m in motors)
		{
			m.Enter(root);
		}

		animating = true;
		phaseNum = 0;
		currentPhase = phases[phaseNum];
		currentPhase.running = true;
		currentPhase.Enter();
		currentPhase.Run(root);
	}
	void NextPhase(Transform root)
	{

		currentPhase.Exit();
		currentPhase.running = false;
		phaseNum++;
		if (phases.Count > phaseNum)
		{
			currentPhase = phases[phaseNum];
			currentPhase.running = true;
			currentPhase.Enter();
			currentPhase.Run(root);
			return;
		}
		animating = false;
		foreach (SPMotor m in motors.OfType<SPRotator>().ToList())
		{
			m.Enter(root);

		}
		foreach (SPRotator m in motors.OfType<SPRotator>().ToList())
		{

			m.axis.FindSceneRefrence(root).localRotation = m.originalRotationValue;

		}
		foreach (SPRotateBetween m in motors.OfType<SPRotateBetween>().ToList())
		{
			Transform transform = m.axis.FindSceneRefrence(root);
			if (transform)
				transform.localRotation = m.originalRotationValue;

		}
		foreach (SPMover m in motors.OfType<SPMover>().ToList())
		{
			Transform transform = m.axis.FindSceneRefrence(root);
			if (transform != null)
				transform.localPosition = m.originalRotationValue;

		}

		currentPhase = null;
	}
	public void Run(Transform root)
	{
		if (currentPhase != null)
		{
			currentPhase.Run(root);
			if (!currentPhase.running)
			{
				NextPhase(root);
			}
		}

	}
}


