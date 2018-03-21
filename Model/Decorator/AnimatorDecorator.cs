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
		private List<SPMotor> _motors = new List<SPMotor>();

		[SerializeField]
		private List<SPPhase> _phases = new List<SPPhase>();

		[SerializeField]
		public SPPhase CurrentPhase;
		private int _phaseNum;

		[SerializeField]
		public bool Animating;

		public ReadOnlyCollection<SPMotor> Motors { get { return _motors.AsReadOnly(); } }

		public ReadOnlyCollection<SPPhase> Phases { get { return _phases.AsReadOnly(); } }
#if UNITY_EDITOR
	public void AddMotor(SPMotor motor)
	{
		AssetDatabase.AddObjectToAsset (motor, this);
		EditorUtility.SetDirty (this);
		AssetDatabase.SaveAssets ();

		_motors.Add (motor);
	}

	public void RemoveMotor(SPMotor motor)
	{
		_motors.Remove (motor);
		DestroyImmediate (motor, true);
	}

	public void AddPhase(SPPhase phase)
	{
		AssetDatabase.AddObjectToAsset (phase,this);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();

		_phases.Add (phase);
	}

	public void RemovePhase(SPPhase phase)
	{
		_phases.Remove (phase);
		DestroyImmediate (phase, true);
	}
#endif

	public override void PrepareExport(ParkitectObj parkitectObj)
	{

		for (int x = 0; x < _motors.Count; x++) {
			_motors [x].PrepareExport (parkitectObj);
		}
		base.PrepareExport (parkitectObj);
	}

	public override void CleanUp(ParkitectObj parkitectObj)
	{
		for (int x = 0; x < _phases.Count; x++) {
			if (_phases [x] != null) {
				_phases [x].CleanUp ();
				DestroyImmediate (_phases [x], true);
			}
		}
		for (int x = 0; x < _motors.Count; x++) {
			if (_motors [x] != null) {
				DestroyImmediate (_motors [x], true);
			}
		}
		_motors.Clear ();
		_phases.Clear ();

		base.CleanUp (parkitectObj);
	}

	public void Animate(Transform root)
	{
		_motors.RemoveAll (x => x == null);
		_phases.RemoveAll (x => x == null);

		foreach (SPMotor m in _motors) {
			m.Enter (root);
		}
		if (_phases.Count <= 0) {
			Animating = false;
			foreach (SPMotor m in _motors) {
				m.Reset (root);
			}
			foreach (SPMultipleRotations R in _motors.OfType<SPMultipleRotations>()) {
				R.Reset (root);
			}
			return;
		}
		foreach (SPMotor m in _motors) {
			m.Enter (root);
		}

		Animating = true;
		_phaseNum = 0;
		CurrentPhase = _phases [_phaseNum];
		CurrentPhase.running = true;
		CurrentPhase.Enter ();
		CurrentPhase.Run (root);
	}



	void NextPhase(Transform root)
	{

		CurrentPhase.Exit ();
		CurrentPhase.running = false;
		_phaseNum++;
		if (_phases.Count > _phaseNum) {
			CurrentPhase = _phases [_phaseNum];
			CurrentPhase.running = true;
			CurrentPhase.Enter ();
			CurrentPhase.Run (root);
			return;
		}
		Animating = false;
		foreach (SPMotor m in _motors.OfType<SPRotator>()) {
			m.Enter (root);

		}
		foreach (SPRotator m in _motors.OfType<SPRotator>()) {
			Transform transform = m.Axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localRotation = m.OriginalRotationValue;

		}
		foreach (SPRotateBetween m in _motors.OfType<SPRotateBetween>()) {
			Transform transform = m.axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localRotation = m.originalRotationValue;

		}
		foreach (SPMover m in _motors.OfType<SPMover>()) {
			Transform transform = m.Axis.FindSceneRefrence (root);
			if (transform != null)
				transform.localPosition = m.OriginalRotationValue;

		}

		CurrentPhase = null;
	}

	public void Run(Transform transform)
	{
		if (CurrentPhase != null) {
			CurrentPhase.Run (transform);
			if (!CurrentPhase.running) {
				NextPhase (transform);
			}
		}
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		foreach (var ele in (Dictionary<string,object>)elements["phases"]) {
			SPPhase phase = Utility.GetByTypeName<SPPhase> (ele.Key);
			phase.Deserialize ((Dictionary<string, object>) ele.Value);
			_phases.Add (phase);
		}
	}


	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{

		List<Dictionary<string, object>> ph = new List<Dictionary<string, object>>();
		foreach (var t in _phases)
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

