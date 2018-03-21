using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
[ExecuteInEditMode]
public class SPRideAnimation
{

	[SerializeField]
	public List<SPMotor> Motors = new List<SPMotor>();

	[SerializeField]
	public List<SPPhase> Phases = new List<SPPhase>();

	[SerializeField]
	public SPPhase CurrentPhase;
	private int _phaseNum;

	[SerializeField]
	public bool Animating;
	public void Animate(Transform root)
	{
		foreach (SPMotor m in Motors)
		{
			m.Enter(root);
		}
		if (Phases.Count <= 0)
		{
			Animating = false;
			foreach (SPMotor m in Motors)
			{
				m.Reset(root);
			}
			foreach (SPMultipleRotations R in Motors.OfType<SPMultipleRotations>().ToList())
			{
				R.Reset(root);
			}
			return;
		}
		foreach (SPMotor m in Motors)
		{
			m.Enter(root);
		}

		Animating = true;
		_phaseNum = 0;
		CurrentPhase = Phases[_phaseNum];
		CurrentPhase.running = true;
		CurrentPhase.Enter();
		CurrentPhase.Run(root);
	}
	void NextPhase(Transform root)
	{

		CurrentPhase.Exit();
		CurrentPhase.running = false;
		_phaseNum++;
		if (Phases.Count > _phaseNum)
		{
			CurrentPhase = Phases[_phaseNum];
			CurrentPhase.running = true;
			CurrentPhase.Enter();
			CurrentPhase.Run(root);
			return;
		}
		Animating = false;
		foreach (SPMotor m in Motors.OfType<SPRotator>().ToList())
		{
			m.Enter(root);

		}
		foreach (SPRotator m in Motors.OfType<SPRotator>().ToList())
		{

			m.Axis.FindSceneRefrence(root).localRotation = m.OriginalRotationValue;

		}
		foreach (SPRotateBetween m in Motors.OfType<SPRotateBetween>().ToList())
		{
			Transform transform = m.axis.FindSceneRefrence(root);
			if (transform)
				transform.localRotation = m.originalRotationValue;

		}
		foreach (SPMover m in Motors.OfType<SPMover>().ToList())
		{
			Transform transform = m.Axis.FindSceneRefrence(root);
			if (transform != null)
				transform.localPosition = m.OriginalRotationValue;

		}

		CurrentPhase = null;
	}
	public void Run(Transform root)
	{
		if (CurrentPhase != null)
		{
			CurrentPhase.Run(root);
			if (!CurrentPhase.running)
			{
				NextPhase(root);
			}
		}

	}
}


