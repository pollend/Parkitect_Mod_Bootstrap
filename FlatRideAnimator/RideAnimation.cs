using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
[ExecuteInEditMode]
public class RideAnimation
{

	[SerializeField]
	public List<Motor> Motors = new List<Motor>();

	[SerializeField]
	public List<Phase> Phases = new List<Phase>();

	[SerializeField]
	public Phase CurrentPhase;
	private int _phaseNum;

	[SerializeField]
	public bool Animating;
	
	public void Animate(Transform root)
	{
		foreach (Motor m in Motors)
		{
			m.Enter(root);
		}
		if (Phases.Count <= 0)
		{
			Animating = false;
			foreach (Motor m in Motors)
			{
				m.Reset(root);
			}
			foreach (MultipleRotations R in Motors.OfType<MultipleRotations>().ToList())
			{
				R.Reset(root);
			}
			return;
		}
		foreach (Motor m in Motors)
		{
			m.Enter(root);
		}

		Animating = true;
		_phaseNum = 0;
		CurrentPhase = Phases[_phaseNum];
		CurrentPhase.Running = true;
		CurrentPhase.Enter();
		CurrentPhase.Run(root);
	}
	void NextPhase(Transform root)
	{

		CurrentPhase.Exit();
		CurrentPhase.Running = false;
		_phaseNum++;
		if (Phases.Count > _phaseNum)
		{
			CurrentPhase = Phases[_phaseNum];
			CurrentPhase.Running = true;
			CurrentPhase.Enter();
			CurrentPhase.Run(root);
			return;
		}
		Animating = false;
		foreach (Rotator m in Motors.OfType<Rotator>().ToList())
		{
			m.Enter(root);

		}
		foreach (Rotator m in Motors.OfType<Rotator>().ToList())
		{

			m.Axis.FindSceneRefrence(root).localRotation = m.OriginalRotationValue;

		}
		foreach (RotateBetween m in Motors.OfType<RotateBetween>().ToList())
		{
			Transform transform = m.Axis.FindSceneRefrence(root);
			if (transform)
				transform.localRotation = m.OriginalRotationValue;

		}
		foreach (Mover m in Motors.OfType<Mover>().ToList())
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
			if (!CurrentPhase.Running)
			{
				NextPhase(root);
			}
		}

	}
}


