
 #if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[MotorTag("RotateBetween")]
public class RotateBetween : Motor
{
	[SerializeField] public RefrencedTransform Axis = new RefrencedTransform();
	[SerializeField] public Quaternion FromRotation;
	[SerializeField] public Vector3 RotationAxis = Vector3.up;
	[SerializeField] public Quaternion ToRotation;
	[SerializeField] public Quaternion OriginalRotationValue;
	[SerializeField] public float Duration = 1f;

	[SerializeField] private float _currentPosition;

	[SerializeField] private float _direction;
	public override string EventName
	{
		get { return "RotateBetween"; }
	}
#if UNITY_EDITOR
	public override void InspectorGUI(Transform root)
	{

		Identifier = EditorGUILayout.TextField("Name ", Identifier);
		Axis.SetSceneTransform(
			(Transform) EditorGUILayout.ObjectField("axis", Axis.FindSceneRefrence(root), typeof(Transform), true));
		RotationAxis = EditorGUILayout.Vector3Field("Rotate To", RotationAxis);
		Duration = EditorGUILayout.FloatField("Time", Duration);
		base.InspectorGUI(root);
	}
#endif
	public override void Reset(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation = OriginalRotationValue;
		_currentPosition = 0f;


		base.Reset(root);
	}

	public override void Enter(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
		{
			OriginalRotationValue = transform.localRotation;
			Initialize(transform, transform.localRotation, Quaternion.Euler(transform.localEulerAngles + RotationAxis),
				Duration);
		}
	}

	public void Initialize(Transform axis, Quaternion fromRotation, Quaternion toRotation, float duration)
	{
		this.Axis.SetSceneTransform(axis);
		this.FromRotation = fromRotation;
		this.ToRotation = toRotation;
		this.Duration = duration;
		axis.localRotation = Quaternion.Lerp(fromRotation, toRotation, 0);
	}

	public bool StartFromTo()
	{
		if (_direction != 1f)
		{
			_direction = 1f;
			return true;
		}

		return false;
	}

	public bool StartToFrom()
	{
		if (_direction != -1f)
		{
			_direction = -1f;
			return true;
		}

		return false;
	}

	public bool IsStopped()
	{
		if (_direction == 1f)
		{
			return _currentPosition >= 0.99f;
		}

		return _currentPosition <= 0.01f;
	}

	public void Tick(float dt, Transform root)
	{
		_currentPosition += dt * _direction * 1f / Duration;
		_currentPosition = Mathf.Clamp01(_currentPosition);
		var transform = Axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation =
				Quaternion.Lerp(FromRotation, ToRotation, MathHelper.Hermite(0f, 1f, _currentPosition));
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		Axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}


	public override Dictionary<string, object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"axis", Axis.Serialize(root)},
			{"fromRotation", Utility.SerializeQuaternion(FromRotation)},
			{"rotationAxis", Utility.SerializeVector(RotationAxis)},
			{"toRotation", Utility.SerializeQuaternion(ToRotation)},
			{"duration", Duration}
		};
	}
	
	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("axis"))
			Axis.Deserialize(elements["axis"] as List<object>);
		if (elements.ContainsKey("fromRotation"))
			FromRotation = Utility.DeSerializeQuaternion(elements["fromRotation"] as Dictionary<string,object>);
		if (elements.ContainsKey("rotationAxis"))
			RotationAxis = Utility.DeseralizeVector3(elements["rotationAxis"] as Dictionary<string,object>);
		if (elements.ContainsKey("toRotation"))
			ToRotation = Utility.DeSerializeQuaternion(elements["toRotation"] as Dictionary<string,object>);
		if (elements.ContainsKey("duration"))
			Duration = Convert.ToSingle(elements["duration"]);
			
		base.Deserialize(elements);
	}
}
