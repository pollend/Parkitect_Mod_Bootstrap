
 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
[MotorTag("Rotator")]
public class Rotator : Motor
{
	public enum State
	{
		STARTING,
		RUNNING,
		PAUSING,
		REQUEST_STOP,
		STOPPING
	}
	[SerializeField]
	public Quaternion OriginalRotationValue;
	[SerializeField]
	public float AccelerationSpeed = 12f;
	[SerializeField]
	public float MaxSpeed = 180f;
	[SerializeField]
	private Vector3 _rotationAxis = Vector3.up;
	[SerializeField]
	public int RotationAxisIndex = 1;
	[SerializeField]
	public float MinRotationSpeedPercent = 0.3f;
	[SerializeField]
	public Quaternion InitialRotation;
	[SerializeField]
	public State CurrentState = State.STOPPING;
	[SerializeField]
	public float CurrentSpeed;
	[SerializeField]
	public float CurrentRotation;
	[SerializeField]
	public int Direction = 1;
	[SerializeField]
	public RefrencedTransform Axis = new RefrencedTransform();

	
	public Vector3 RotationAxis
	{
		get { return _rotationAxis; }
		set
		{
			_rotationAxis = value;
			if (value.x != 0f)
			{
				RotationAxisIndex = 0;
			}
			else if (value.y != 0f)
			{
				RotationAxisIndex = 1;
			}
			else if (value.z != 0f)
			{
				RotationAxisIndex = 2;
			}
		}
	}

	public override void Reset(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation = OriginalRotationValue;
		base.Reset(root);
	}
	
	
	
	public override string EventName
	{
		get
		{
			return "Rotator";
		}
	}
#if UNITY_EDITOR
public override void InspectorGUI(Transform root)
{

    Identifier = EditorGUILayout.TextField("Name ", Identifier);
	Axis.SetSceneTransform((Transform)EditorGUILayout.ObjectField("axis", Axis.FindSceneRefrence(root), typeof(Transform),true));
	MaxSpeed = EditorGUILayout.FloatField("maxSpeed", MaxSpeed);
    AccelerationSpeed = EditorGUILayout.FloatField("accelerationSpeed", AccelerationSpeed);
	RotationAxis = EditorGUILayout.Vector3Field("rotationAxis", RotationAxis);
	base.InspectorGUI(root);
}
#endif
	public override void Enter(Transform root)
	{
		Transform transform = Axis.FindSceneRefrence(root);
		if (transform)
			OriginalRotationValue = transform.localRotation;
		ResetRotations();
		CurrentRotation = 0;
		CurrentSpeed = 0;
		ChangeState(State.STARTING);
		Initialize(root, Axis.FindSceneRefrence(root), AccelerationSpeed, MaxSpeed, _rotationAxis);
		base.Enter(root);
	}

	public void Initialize(Transform root, Transform axis, float accelerationSpeed, float maxSpeed)
	{
		Initialize(root, axis, accelerationSpeed, maxSpeed, Vector3.up);
	}

	public void Initialize(Transform root, Transform axis, float accelerationSpeed, float maxSpeed, Vector3 rotationAxis)
	{
		Axis.SetSceneTransform(axis);
		AccelerationSpeed = accelerationSpeed;
		MaxSpeed = maxSpeed;
		RotationAxis = rotationAxis;
		InitialRotation = axis.localRotation;
		axis.Rotate(rotationAxis, CurrentRotation);
	}

	public bool Start()
	{
		if (CurrentState != State.STARTING && CurrentState != State.RUNNING)
		{
			ChangeState(State.STARTING);
			CurrentSpeed = 0f;
			CurrentRotation = 0f;
			return true;
		}
		return false;
	}

	public void Stop()
	{
		ChangeState(State.REQUEST_STOP);
	}

	public void Pause()
	{
		ChangeState(State.PAUSING);
	}

	public bool IsStopped()
	{
		return CurrentState == State.STOPPING && Mathf.Approximately(CurrentSpeed, 0f);
	}

	public void ResetRotations()
	{
		CurrentRotation = 0f;
	}

	public float GetRotationsCount()
	{
		return Mathf.Abs(CurrentRotation) / 360f;
	}

	public int GetCompletedRotationsCount()
	{
		return Mathf.FloorToInt(GetRotationsCount());
	}

	public bool IsInAngleRange(Transform root, float fromAngle, float toAngle)
	{
		fromAngle %= 360f;
		toAngle %= 360f;
		Transform transform = Axis.FindSceneRefrence(root);
		float num = transform.localEulerAngles[RotationAxisIndex];
		if (fromAngle >= toAngle)
		{
			return num >= fromAngle || num <= toAngle;
		}
		return num < toAngle && num > fromAngle;
	}

	public bool ReachedFullSpeed()
	{
		return CurrentState != State.STARTING;
	}

	public void ChangeState(State newState)
	{
		CurrentState = newState;

	}

	public virtual void tick(float dt, Transform root)
	{
		Transform transformAxis = Axis.FindSceneRefrence(root);

		float num = CurrentSpeed * dt;
		CurrentRotation += num;
		if (CurrentState == State.STARTING || CurrentState == State.RUNNING || CurrentState == State.PAUSING)
		{
			transformAxis.Rotate(_rotationAxis, num * Direction);
		}
		if (CurrentState == State.STARTING)
		{
			if (CurrentSpeed < MaxSpeed)
			{
				CurrentSpeed += dt * AccelerationSpeed;
			}
			else
			{
				ChangeState(State.RUNNING);
			}
		}
		else if (CurrentState == State.PAUSING)
		{
			CurrentSpeed -= dt * AccelerationSpeed;
			if (CurrentSpeed < 0f)
			{
				CurrentSpeed = 0f;
			}
		}
		else if (CurrentState == State.REQUEST_STOP)
		{
			CurrentSpeed -= dt * AccelerationSpeed;
			CurrentSpeed = Mathf.Max(MaxSpeed * MinRotationSpeedPercent - 0.01f, CurrentSpeed);
			if (CurrentSpeed < MaxSpeed * MinRotationSpeedPercent)
			{
				float num2 = transformAxis.localEulerAngles[RotationAxisIndex] - InitialRotation.eulerAngles[RotationAxisIndex] + 180f;
				float num3 = num2 - 360f * Mathf.Round(num2 / 360f);
				if ((num3 > 0f && Direction > 0) || (num3 < 0f && Direction < 0))
				{
					ChangeState(State.STOPPING);
				}
			}
			transformAxis.Rotate(_rotationAxis, num * Direction);
		}
		else if (CurrentState == State.STOPPING && CurrentSpeed != 0f)
		{
			float b = Quaternion.Angle(transformAxis.localRotation, InitialRotation);
			CurrentSpeed = Mathf.Min(CurrentSpeed, b);
			transformAxis.localRotation = Quaternion.RotateTowards(Axis.FindSceneRefrence(root).localRotation, InitialRotation, Mathf.Max(1f, CurrentSpeed) * dt);
			float num4 = transformAxis.localEulerAngles[RotationAxisIndex] - InitialRotation.eulerAngles[RotationAxisIndex];
			float num5 = num4 - 360f * Mathf.Round(num4 / 360f);
			if ((num5 > 0f && Direction > 0) || (num5 < 0f && Direction < 0))
			{
				transformAxis.localRotation = InitialRotation;
				CurrentSpeed = 0f;
			}
		}
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
			{"minRotationSpeedPercent", MinRotationSpeedPercent},
			{"rotationAxisIndex", RotationAxisIndex},
			{"rotationAxis", Utility.SerializeVector(_rotationAxis)},
			{"maxSpeed", MaxSpeed},
			{"accelerationSpeed", AccelerationSpeed}

		};
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{
		if (elements.ContainsKey("axis"))
			Axis.Deserialize(elements["axis"] as List<object>);
		if (elements.ContainsKey("minRotationSpeedPercent"))
			MinRotationSpeedPercent = Convert.ToSingle(elements["minRotationSpeedPercent"]);
		if (elements.ContainsKey("rotationAxisIndex"))
			RotationAxisIndex = Convert.ToInt32(elements["rotationAxisIndex"]);
		if (elements.ContainsKey("rotationAxis"))
			_rotationAxis = Utility.DeseralizeVector3(elements["rotationAxis"] as Dictionary<string,object>);
		if (elements.ContainsKey("maxSpeed"))
			MaxSpeed = Convert.ToSingle(elements["maxSpeed"]);
		if (elements.ContainsKey("accelerationSpeed"))
			AccelerationSpeed = Convert.ToSingle(elements["minRotationSpeedPercent"]);
			
		base.Deserialize(elements);
	}
}
