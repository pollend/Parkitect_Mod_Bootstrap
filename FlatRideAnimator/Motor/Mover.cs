
 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[MotorTag("Mover")]
public class Mover : Motor
{
	private enum State
	{
		RUNNING,
		STOPPED
	}
	[SerializeField]
	public RefrencedTransform Axis = new RefrencedTransform();

	[SerializeField]
	public Vector3 OriginalRotationValue;
	[SerializeField]
	private Vector3 _fromPosition;
	[SerializeField]
	public Vector3 ToPosition;
	[SerializeField]
	public float Duration = 10f;

	[SerializeField]
	private State _currentState = State.STOPPED;
	[SerializeField]
	private float _currentPosition = 1f;
	[SerializeField]
	private int _direction = -1;

	public override void Reset(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
			transform.localPosition = OriginalRotationValue;
		_currentPosition = 1f;
		_direction = -1;
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
		Axis.SetSceneTransform((Transform)EditorGUILayout.ObjectField("axis", Axis.FindSceneRefrence (root), typeof(Transform), true));
	    ToPosition = EditorGUILayout.Vector3Field("Move To", ToPosition );
		Duration = EditorGUILayout.FloatField("Time", Duration);
		base.InspectorGUI(root);

	}
#endif
	public override void Enter(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
			OriginalRotationValue = transform.localPosition;
		_currentPosition = 1f;

		_direction = -1;
		Initialize(root, Axis.FindSceneRefrence(root), transform.localPosition, ToPosition, Duration);
		base.Enter(root);
	}
	public void Initialize(Transform root, Transform axis, Vector3 fromPosition, Vector3 toPosition, float duration)
	{
		this.Axis.SetSceneTransform(axis);
		this._fromPosition = fromPosition;
		this.ToPosition = toPosition;
		this.Duration = duration;
		SetPosition(root);
	}

	public bool startFromTo()
	{
		if (_direction != 1)
		{
			_direction = 1;
			_currentPosition = 0f;
			_currentState = State.RUNNING;
			return true;
		}
		return false;
	}

	public bool StartToFrom()
	{
		if (_direction != -1)
		{
			_direction = -1;
			_currentPosition = 0f;
			_currentState = State.RUNNING;
			return true;
		}
		return false;
	}

	public bool ReachedTarget()
	{
		return _currentState == State.STOPPED && _currentPosition >= 1f;
	}

	public void Tick(float dt, Transform root)
	{
		_currentPosition += dt * 1f / Duration;
		if (_currentPosition >= 1f)
		{
			_currentPosition = 1f;
			_currentState = State.STOPPED;
		}
		SetPosition(root);
	}

	private void SetPosition(Transform root)
	{
		Vector3 a;
		Vector3 b;
		if (_direction == 1)
		{
			a = _fromPosition;
			b = ToPosition;
		}
		else
		{
			a = ToPosition;
			b = _fromPosition;
		}
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform != null)
			transform.localPosition = Vector3.Lerp(a, b, MathHelper.Hermite(0f, 1f, _currentPosition));
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		Axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}


	public override Dictionary<string,object> Serialize (Transform root)
	{
		return new Dictionary<string, object>{
			{"transform",Axis.Serialize(root)},
			{"from", Utility.SerializeVector(_fromPosition)},
			{"to", Utility.SerializeVector(ToPosition)},
			{"duration",Duration}
		};
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("transform"))
			Axis.Deserialize(elements["transform"] as List<object>);
		if (elements.ContainsKey("from"))
			_fromPosition = Utility.DeseralizeVector3(elements["from"] as Dictionary<string,object>);
		if (elements.ContainsKey("to"))
			ToPosition = Utility.DeseralizeVector3(elements["to"] as Dictionary<string,object>);
		if (elements.ContainsKey("duration"))
			Duration = Convert.ToSingle(elements["duration"]);

		base.Deserialize(elements);
	}
}

