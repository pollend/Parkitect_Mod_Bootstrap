#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class AnimatorDecorator : Decorator
{
		[SerializeField]
		private List<SPMotor> motors = new List<SPMotor>();

		[SerializeField]
		private List<SPPhase> phases = new List<SPPhase>();

		[SerializeField]
		public SPPhase currentPhase;
		int phaseNum;

		[SerializeField]
		public bool animating;

		public ReadOnlyCollection<SPMotor> Motors { get { return motors.AsReadOnly(); } }

		public ReadOnlyCollection<SPPhase> Phases { get { return phases.AsReadOnly(); } }
#if UNITY_EDITOR
	public void AddMotor(SPMotor motor)
	{
		AssetDatabase.AddObjectToAsset (motor, this);
		EditorUtility.SetDirty (this);
		AssetDatabase.SaveAssets ();

		motors.Add (motor);
	}

	public void RemoveMotor(SPMotor motor)
	{
		motors.Remove (motor);
		DestroyImmediate (motor, true);
	}

	public void AddPhase(SPPhase phase)
	{
		AssetDatabase.AddObjectToAsset (phase,this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();

		phases.Add (phase);
	}

	public void RemovePhase(SPPhase phase)
	{
		phases.Remove (phase);
		DestroyImmediate (phase, true);
	}
#endif

	public override void PrepareExport(ParkitectObj parkitectObj)
	{

		for (int x = 0; x < motors.Count; x++) {
			motors [x].PrepareExport (parkitectObj);
		}
		base.PrepareExport (parkitectObj);
	}

	public override void CleanUp(ParkitectObj parkitectObj)
	{
		for (int x = 0; x < phases.Count; x++) {
			if (phases [x] != null) {
				phases [x].CleanUp ();
				DestroyImmediate (phases [x], true);
			}
		}
		for (int x = 0; x < motors.Count; x++) {
			if (motors [x] != null) {
				DestroyImmediate (motors [x], true);
			}
		}
		motors.Clear ();
		phases.Clear ();

		base.CleanUp (parkitectObj);
	}

	public void Animate(Transform root)
	{
		motors.RemoveAll (x => x == null);
		phases.RemoveAll (x => x == null);

		foreach (SPMotor m in motors) {
			m.Enter (root);
		}
		if (phases.Count <= 0) {
			animating = false;
			foreach (SPMotor m in motors) {
				m.Reset (root);
			}
			foreach (SPMultipleRotations R in motors.OfType<SPMultipleRotations>()) {
				R.Reset (root);
			}
			return;
		}
		foreach (SPMotor m in motors) {
			m.Enter (root);
		}

		animating = true;
		phaseNum = 0;
		currentPhase = phases [phaseNum];
		currentPhase.running = true;
		currentPhase.Enter ();
		currentPhase.Run (root);
	}



	void NextPhase(Transform root)
	{

		currentPhase.Exit ();
		currentPhase.running = false;
		phaseNum++;
		if (phases.Count > phaseNum) {
			currentPhase = phases [phaseNum];
			currentPhase.running = true;
			currentPhase.Enter ();
			currentPhase.Run (root);
			return;
		}
		animating = false;
		foreach (SPMotor m in motors.OfType<SPRotator>()) {
			m.Enter (root);

		}
		foreach (SPRotator m in motors.OfType<SPRotator>()) {
			Transform transform = m.axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localRotation = m.originalRotationValue;

		}
		foreach (SPRotateBetween m in motors.OfType<SPRotateBetween>()) {
			Transform transform = m.axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localRotation = m.originalRotationValue;

		}
		foreach (SPMover m in motors.OfType<SPMover>()) {
			Transform transform = m.axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localPosition = m.originalRotationValue;

		}

		currentPhase = null;
	}

	public void Run(Transform transform)
	{
		if (currentPhase != null) {
			currentPhase.Run (transform);
			if (!currentPhase.running) {
				NextPhase (transform);
			}
		}
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		foreach (var ele in (Dictionary<string,object>)elements["phases"]) {
			SPPhase phase = Utility.GetByTypeName<SPPhase> (ele.Key);
			phase.Deserialize ((Dictionary<string, object>) ele.Value);
			phases.Add (phase);
		}
	}


	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{

		List<Dictionary<string, object>> ph = new List<Dictionary<string, object>>();
		foreach (var t in phases)
		{
			ph.Add(new Dictionary<string, object> {{t.GetType().ToString(), t.Serialize(parkitectObj.Prefab.transform)}});
		}

		return new Dictionary<string, object>
		{
			{
				"phases", ph
			}
		};
	}
}

