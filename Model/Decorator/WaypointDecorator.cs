
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class WaypointDecorator : Decorator
{
	private enum State
	{
		NONE,
		CONNECT
	}

	private State _state = State.NONE;

	public List<SPWaypoint> Waypoints = new List<SPWaypoint>();

#if UNITY_EDITOR
	public Tool currentTool = Tool.None;

	[NonSerialized] public bool EnableEditing;
	[NonSerialized] public bool Snap;
	[NonSerialized] public float HelperPlaneY;
	[NonSerialized] public SPWaypoint SelectedWaypoint;

	public override void RenderInspectorGui(ParkitectObj parkitectObj)
	{
		GameObject sceneTransform = parkitectObj.GetGameObjectRef(false);
		if (sceneTransform == null)
			return;

		//FlatrideDecorator flatRideDecorator = parkitectObj.GetDecorator (typeof(FlatrideDecorator));

		string caption = "Enable Editing Waypoints";
		if (EnableEditing)
		{
			caption = "Disable Editing Waypoints";
		}

		if (EnableEditing)
		{
			GUI.color = Color.green;
		}

		bool currentEnableEditing = EnableEditing;
		if (GUILayout.Button(caption))
		{
			SelectedWaypoint = null;
			EnableEditing = !EnableEditing;
		}

		if (EnableEditing)
		{
			GUI.color = Color.white;
		}

		if (currentEnableEditing != EnableEditing)
		{
			if (EnableEditing)
			{
				currentTool = Tools.current;
				Tools.current = Tool.None;
			}
			else
			{
				Tools.current = currentTool;
			}
		}

		if (EnableEditing)
		{

			GUILayout.Label("S - Snap to axis of connected waypoints");
			HelperPlaneY = EditorGUILayout.FloatField("Helper Plane Y", HelperPlaneY);

			if (GUILayout.Button("Add Waypoint"))
			{
				addWaypoint(sceneTransform.transform);
			}

			if (GUILayout.Button("Rotate 90°"))
			{
				rotateWaypoints(sceneTransform.transform);
			}

			if (GUILayout.Button("Clear all"))
			{
				Waypoints.Clear();
			}
		}

		base.RenderInspectorGui(parkitectObj);
	}





	public override void RenderSceneGui(ParkitectObj parkitectObj)
	{
		GameObject sceneTransform = parkitectObj.GetGameObjectRef(false);
		if (sceneTransform == null)
			return;

		GUIStyle labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;

		if (EnableEditing)
		{
			switch (Event.current.type)
			{
				case EventType.Layout:
					break;
				case EventType.KeyDown:
					if (Event.current.keyCode == KeyCode.S)
					{
						Snap = true;
					}

					break;
				case EventType.KeyUp:
					if (Event.current.keyCode == KeyCode.C)
					{
						if (_state != State.CONNECT)
						{
							_state = State.CONNECT;
						}
						else
						{
							_state = State.NONE;
						}
					}
					else if (Event.current.keyCode == KeyCode.R)
					{
						removeSelectedWaypoint();
					}
					/*else if (Event.current.keyCode == KeyCode.A)
					{
						addWaypoint(sceneTransform.transform);
					}*/
					else if (Event.current.keyCode == KeyCode.O && SelectedWaypoint != null)
					{
						SelectedWaypoint.isOuter = !SelectedWaypoint.isOuter;
					}
					else if (Event.current.keyCode == KeyCode.I && SelectedWaypoint != null)
					{
						SelectedWaypoint.isRabbitHoleGoal = !SelectedWaypoint.isRabbitHoleGoal;
					}
					else if (Event.current.keyCode == KeyCode.S)
					{
						Snap = false;
					}

					SceneView.RepaintAll();
					HandleUtility.Repaint();
					break;
			}
		}


		int i = 0;
		foreach (SPWaypoint waypoint in Waypoints)
		{
			if (waypoint == SelectedWaypoint)
			{
				Handles.color = Color.red;
			}
			else if (waypoint.isOuter)
			{
				Handles.color = Color.green;
			}
			else if (waypoint.isRabbitHoleGoal)
			{
				Handles.color = Color.blue;
			}
			else
			{
				Handles.color = Color.yellow;
			}

			Vector3 worldPos = waypoint.localPosition + sceneTransform.transform.position;

			Handles.zTest = CompareFunction.LessEqual;
			if (Handles.Button(worldPos, Quaternion.identity, HandleUtility.GetHandleSize(worldPos) * 0.2f,
				HandleUtility.GetHandleSize(worldPos) * 0.2f, Handles.SphereCap))
			{
				if (EnableEditing)
				{
					handleClick(waypoint);
				}
			}


			Handles.color = Color.blue;
			foreach (int connectedIndex in waypoint.connectedTo)
			{
				Handles.zTest = CompareFunction.Always;
				Handles.DrawLine(worldPos, Waypoints[connectedIndex].localPosition + sceneTransform.transform.position);
			}

			Handles.Label(worldPos, "#" + i, labelStyle);
			i++;
		}

		if (SelectedWaypoint != null)
		{
			Vector3 worldPos = SelectedWaypoint.localPosition + sceneTransform.transform.position;

			if (_state == State.CONNECT)
			{
				Handles.Label(worldPos, "\nConnecting...", labelStyle);
			}
			else
			{
				Vector3 newPos =
					Handles.PositionHandle(SelectedWaypoint.GetWorldPosition(sceneTransform.transform), Quaternion.identity);
				SelectedWaypoint.localPosition = handleSnap(newPos, SelectedWaypoint);

				SelectedWaypoint.localPosition = handleSnap(newPos, SelectedWaypoint);

				Handles.Label(worldPos, "\n(C)onnect\n(R)emove\n(O)uter\nRabb(i)t Hole", labelStyle);
			}
		}

		base.RenderSceneGui(parkitectObj);
	}


	public void handleClick(SPWaypoint waypoint)
	{

		if (_state == State.NONE && waypoint != null)
		{
			SelectedWaypoint = waypoint;
		}
		else if (_state == State.CONNECT && SelectedWaypoint != null)
		{
			int closestWaypointIndex = Waypoints.FindIndex(delegate(SPWaypoint wp) { return wp == waypoint; });
			int selectedWaypointIndex = Waypoints.FindIndex(delegate(SPWaypoint wp) { return wp == SelectedWaypoint; });
			if (closestWaypointIndex >= 0 && selectedWaypointIndex >= 0)
			{
				if (!SelectedWaypoint.connectedTo.Contains(closestWaypointIndex))
				{
					SelectedWaypoint.connectedTo.Add(closestWaypointIndex);
					waypoint.connectedTo.Add(selectedWaypointIndex);
				}
				else
				{
					SelectedWaypoint.connectedTo.Remove(closestWaypointIndex);
					waypoint.connectedTo.Remove(selectedWaypointIndex);
				}
			}
		}

	}

	public void addWaypoint(Transform transform)
	{
		Vector3 pos = transform.position;
		SelectedWaypoint = new SPWaypoint();

		if (Camera.current != null)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane plane = new Plane(Vector3.up, new Vector3(0, HelperPlaneY, 0));
			float enter = 0;
			plane.Raycast(ray, out enter);
			SelectedWaypoint.localPosition = ray.GetPoint(enter) - pos;
		}
		else
		{
			SelectedWaypoint.localPosition = new Vector3(0, HelperPlaneY, 0);
		}

		Waypoints.Add(SelectedWaypoint);
	}

	public void removeSelectedWaypoint()
	{

		int selectedWaypointIndex = Waypoints.FindIndex(delegate(SPWaypoint wp) { return wp == SelectedWaypoint; });
		foreach (SPWaypoint waypoint in Waypoints)
		{
			waypoint.connectedTo.Remove(selectedWaypointIndex);
		}

		Waypoints.Remove(SelectedWaypoint);

		foreach (SPWaypoint waypoint in Waypoints)
		{
			for (int i = 0; i < waypoint.connectedTo.Count; i++)
			{
				if (waypoint.connectedTo[i] > selectedWaypointIndex)
				{
					waypoint.connectedTo[i]--;
				}
			}
		}

		SelectedWaypoint = null;

	}

	public void rotateWaypoints(Transform transform)
	{
		Vector3 pos = transform.position;

		foreach (SPWaypoint waypoint in Waypoints)
		{
			Vector3 dir = waypoint.localPosition - pos;
			dir.y = 0;
			float phi = Mathf.Atan2(dir.z, dir.x);
			phi += Mathf.PI / 2;
			float x = pos.x + dir.magnitude * Mathf.Cos(phi);
			float z = pos.z + dir.magnitude * Mathf.Sin(phi);
			waypoint.localPosition = new Vector3(x, waypoint.localPosition.y, z);
		}

	}

	public Vector3 handleSnap(Vector3 newPos, SPWaypoint waypoint)
	{
		Vector3 oldPos = waypoint.localPosition;

		if (Snap && (newPos - oldPos).magnitude > Mathf.Epsilon)
		{
			if (Mathf.Abs(newPos.x - oldPos.x) > Mathf.Epsilon)
			{
				newPos = handleAxisSnap(newPos, waypoint, 0);
			}

			if (Mathf.Abs(newPos.y - oldPos.y) > Mathf.Epsilon)
			{
				newPos = handleAxisSnap(newPos, waypoint, 1);
			}

			if (Mathf.Abs(newPos.z - oldPos.z) > Mathf.Epsilon)
			{
				newPos = handleAxisSnap(newPos, waypoint, 2);
			}
		}

		return newPos;
	}

	public Vector3 handleAxisSnap(Vector3 newPos, SPWaypoint waypoint, int axisIndex)
	{

		foreach (int connectedIndex in waypoint.connectedTo)
		{
			SPWaypoint connectedWaypoint = Waypoints[connectedIndex];
			if (Mathf.Abs(newPos[axisIndex] - connectedWaypoint.localPosition[axisIndex]) < 0.1f)
			{
				newPos[axisIndex] = connectedWaypoint.localPosition[axisIndex];
			}
		}

		return newPos;
	}
#endif
#if PARKITECT

	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		Waypoints w = go.AddComponent<Waypoints>();
		foreach (var t in Waypoints)
		{
			Waypoint waypoint = new Waypoint
			{
				connectedTo = t.connectedTo,
				isOuter = t.isOuter,
				isRabbitHoleGoal = t.isRabbitHoleGoal,
				localPosition = t.localPosition
			};
			Debug.Log("added waypoint" + t.localPosition);
			w.waypoints.Add(waypoint);
		}
	}
#endif

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{
		List<object> wp = new List<object>();
		for (int i = 0; i < Waypoints.Count; i++)
		{
			wp.Add(Waypoints[i].Serialize());
		}

		return new Dictionary<string, object>()
		{
			{"Waypoints", wp}
		};
	}

	public override void Deserialize(Dictionary<string, object> elements)
	{

		if (elements.ContainsKey("Waypoints"))
		{
			foreach (var wp in (List<object>) elements["Waypoints"])
			{
				Waypoints.Add(SPWaypoint.Deserialize(wp as Dictionary<string, object>));
			}
		}

		base.Deserialize(elements);
	}

}



