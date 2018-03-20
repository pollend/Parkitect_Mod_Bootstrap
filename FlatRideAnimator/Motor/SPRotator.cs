
 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class SPRotator : SPMotor
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
	public Quaternion originalRotationValue;
	[SerializeField]
	public float accelerationSpeed = 12f;
	[SerializeField]
	public float maxSpeed = 180f;
	[SerializeField]
	public Vector3 rotationAxis = Vector3.up;
	[SerializeField]
	public int rotationAxisIndex = 1;
	[SerializeField]
	public float minRotationSpeedPercent = 0.3f;
	[SerializeField]
	public Quaternion initialRotation;

	[SerializeField]
	public State currentState = State.STOPPING;

	[SerializeField]
	public float currentSpeed;

	[SerializeField]
	public float currentRotation;

	[SerializeField]
	public int direction = 1;

	[SerializeField]
	public RefrencedTransform axis = new RefrencedTransform();


	public override void Reset(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
			transform.localRotation = originalRotationValue;
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
	axis.SetSceneTransform((Transform)EditorGUILayout.ObjectField("axis", axis.FindSceneRefrence(root), typeof(Transform),true));
    maxSpeed = EditorGUILayout.FloatField("maxSpeed", maxSpeed);
    accelerationSpeed = EditorGUILayout.FloatField("accelerationSpeed", accelerationSpeed);
    rotationAxis = EditorGUILayout.Vector3Field("rotationAxis", rotationAxis);
	base.InspectorGUI(root);
}
#endif
	public override void Enter(Transform root)
	{
		Transform transform = axis.FindSceneRefrence(root);
		if (transform)
			originalRotationValue = transform.localRotation;
		resetRotations();
		currentRotation = 0;
		currentSpeed = 0;
		changeState(State.STARTING);
		Initialize(root, axis.FindSceneRefrence(root), accelerationSpeed, maxSpeed, rotationAxis);
		base.Enter(root);
	}

	public void Initialize(Transform root, Transform axis, float accelerationSpeed, float maxSpeed)
	{
		Initialize(root, axis, accelerationSpeed, maxSpeed, Vector3.up);
	}

	public void Initialize(Transform root, Transform axis, float accelerationSpeed, float maxSpeed, Vector3 rotationAxis)
	{
		this.axis.SetSceneTransform(axis);
		this.accelerationSpeed = accelerationSpeed;
		this.maxSpeed = maxSpeed;
		setRotationAxis(rotationAxis);
		setInitialRotation(axis.localRotation);
		axis.Rotate(rotationAxis, currentRotation);
	}

	public void setInitialRotation(Quaternion initialLocalRotation)
	{
		initialRotation = initialLocalRotation;
	}

	public void setMinRotationSpeedPercent(float minRotationSpeedPercent)
	{
		this.minRotationSpeedPercent = minRotationSpeedPercent;
	}

	private void setRotationAxis(Vector3 rotationAxis)
	{
		this.rotationAxis = rotationAxis;
		if (rotationAxis.x != 0f)
		{
			rotationAxisIndex = 0;
		}
		else if (rotationAxis.y != 0f)
		{
			rotationAxisIndex = 1;
		}
		else if (rotationAxis.z != 0f)
		{
			rotationAxisIndex = 2;
		}
	}

	public bool start()
	{
		if (currentState != State.STARTING && currentState != State.RUNNING)
		{
			changeState(State.STARTING);
			currentSpeed = 0f;
			currentRotation = 0f;
			return true;
		}
		return false;
	}

	public void stop()
	{
		changeState(State.REQUEST_STOP);
	}

	public void pause()
	{
		changeState(State.PAUSING);
	}

	public bool isStopped()
	{
		return currentState == State.STOPPING && Mathf.Approximately(currentSpeed, 0f);
	}

	public State getState()
	{
		return currentState;
	}

	public void resetRotations()
	{
		currentRotation = 0f;
	}

	public float getRotationsCount()
	{
		return Mathf.Abs(currentRotation) / 360f;
	}

	public int getCompletedRotationsCount()
	{
		return Mathf.FloorToInt(getRotationsCount());
	}

	public bool isInAngleRange(Transform root, float fromAngle, float toAngle)
	{
		fromAngle %= 360f;
		toAngle %= 360f;
		Transform transform = axis.FindSceneRefrence(root);
		float num = transform.localEulerAngles[rotationAxisIndex];
		if (fromAngle >= toAngle)
		{
			return num >= fromAngle || num <= toAngle;
		}
		return num < toAngle && num > fromAngle;
	}

	public bool reachedFullSpeed()
	{
		return currentState != State.STARTING;
	}

	public float getCurrentSpeed()
	{
		return currentSpeed;
	}

	public float getMaxSpeed()
	{
		return maxSpeed;
	}

	public void setDirection(int direction)
	{
		this.direction = direction;
	}

	public int getDirection()
	{
		return direction;
	}

	public void changeState(State newState)
	{
		currentState = newState;

	}

	public virtual void tick(float dt, Transform root)
	{
		Transform transformAxis = axis.FindSceneRefrence(root);

		float num = currentSpeed * dt;
		currentRotation += num;
		if (currentState == State.STARTING || currentState == State.RUNNING || currentState == State.PAUSING)
		{
			transformAxis.Rotate(rotationAxis, num * direction);
		}
		if (currentState == State.STARTING)
		{
			if (currentSpeed < maxSpeed)
			{
				currentSpeed += dt * accelerationSpeed;
			}
			else
			{
				changeState(State.RUNNING);
			}
		}
		else if (currentState == State.PAUSING)
		{
			currentSpeed -= dt * accelerationSpeed;
			if (currentSpeed < 0f)
			{
				currentSpeed = 0f;
			}
		}
		else if (currentState == State.REQUEST_STOP)
		{
			currentSpeed -= dt * accelerationSpeed;
			currentSpeed = Mathf.Max(maxSpeed * minRotationSpeedPercent - 0.01f, currentSpeed);
			if (currentSpeed < maxSpeed * minRotationSpeedPercent)
			{
				float num2 = transformAxis.localEulerAngles[rotationAxisIndex] - initialRotation.eulerAngles[rotationAxisIndex] + 180f;
				float num3 = num2 - 360f * Mathf.Round(num2 / 360f);
				if ((num3 > 0f && direction > 0) || (num3 < 0f && direction < 0))
				{
					changeState(State.STOPPING);
				}
			}
			transformAxis.Rotate(rotationAxis, num * direction);
		}
		else if (currentState == State.STOPPING && currentSpeed != 0f)
		{
			float b = Quaternion.Angle(transformAxis.localRotation, initialRotation);
			currentSpeed = Mathf.Min(currentSpeed, b);
			transformAxis.localRotation = Quaternion.RotateTowards(axis.FindSceneRefrence(root).localRotation, initialRotation, Mathf.Max(1f, currentSpeed) * dt);
			float num4 = transformAxis.localEulerAngles[rotationAxisIndex] - initialRotation.eulerAngles[rotationAxisIndex];
			float num5 = num4 - 360f * Mathf.Round(num4 / 360f);
			if ((num5 > 0f && direction > 0) || (num5 < 0f && direction < 0))
			{
				transformAxis.localRotation = initialRotation;
				currentSpeed = 0f;
			}
		}
	}

	public override void PrepareExport(ParkitectObj parkitectObj)
	{
		axis.UpdatePrefabRefrence(parkitectObj.Prefab.transform);
		base.PrepareExport(parkitectObj);
	}


	public virtual Dictionary<string, object> Serialize(Transform root)
	{
		return new Dictionary<string, object>
		{
			{"axis", axis.Serialize(root)},
			{"minRotationSpeedPercent", minRotationSpeedPercent},
			{"rotationAxisIndex", rotationAxisIndex},
			{"rotationAxis", Utility.SerializeVector(rotationAxis)},
			{"maxSpeed", maxSpeed},
			{"accelerationSpeed", accelerationSpeed}

		};
	}
}
