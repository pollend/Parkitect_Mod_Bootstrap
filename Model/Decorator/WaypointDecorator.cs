﻿
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
        NONE, CONNECT
    }
    private State _state = State.NONE;

	public List<SPWaypoint> Waypoints = new List<SPWaypoint>();

	#if UNITY_EDITOR
	public Tool currentTool = Tool.None;

	[NonSerialized]
	private bool enableEditing;
	[NonSerialized]
	private bool snap;
	[NonSerialized]
	private float helperPlaneY;
	[NonSerialized]
	public SPWaypoint selectedWaypoint;

	public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{
		GameObject sceneTransform = parkitectObj.getGameObjectRef (false);
		if (sceneTransform == null)
			return;
		
		//FlatrideDecorator flatRideDecorator = parkitectObj.GetDecorator (typeof(FlatrideDecorator));

		string caption = "Enable Editing Waypoints";
		if (enableEditing)
		{
			caption = "Disable Editing Waypoints";
		}
		if (enableEditing)
		{
			GUI.color = Color.green;
		}
		bool currentEnableEditing = enableEditing;
		if (GUILayout.Button(caption))
		{
			selectedWaypoint = null;
			enableEditing = !enableEditing;
		}
		if (enableEditing)
		{
			GUI.color = Color.white;
		}
		if (currentEnableEditing != enableEditing)
		{
			if (enableEditing)
			{
				currentTool = Tools.current;
				Tools.current = Tool.None;
			}
			else
			{
				Tools.current = currentTool;
			}
		}
		if (enableEditing)
		{
            
            GUILayout.Label("S - Snap to axis of connected waypoints");
			helperPlaneY = EditorGUILayout.FloatField("Helper Plane Y", helperPlaneY);

            FlatrideDecorator flatRideDecorator = (FlatrideDecorator)parkitectObj.GetDecorator(typeof(FlatrideDecorator), false);

            if (GUILayout.Button("Generate outer grid"))
			{
                generateOuterGrid(flatRideDecorator);
            }
            if (GUILayout.Button("Add Waypoint"))
			{
				addWaypoint (sceneTransform.transform);
			}
			if (GUILayout.Button("Rotate 90°"))
			{
			    rotateWaypoints(sceneTransform.transform);
			}
			if (GUILayout.Button("Clear all"))
			{
				waypoints.Clear();
			}
		}

		base.RenderInspectorGui (parkitectObj);
	}
    




	public override void RenderSceneGui (ParkitectObj parkitectObj)
	{
		GameObject sceneTransform = parkitectObj.getGameObjectRef (false);
		if (sceneTransform == null)
			return;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.black;

		if (enableEditing)
		{
			switch (Event.current.type)
			{
				case EventType.Layout:
					break;
				case EventType.KeyDown:
					if (Event.current.keyCode == KeyCode.S)
					{
						snap = true;
					}

					break;
				case EventType.KeyUp:
					if (Event.current.keyCode == KeyCode.C)
					{
						if (state != State.CONNECT)
						{
							state = State.CONNECT;
						}
						else
						{
							state = State.NONE;
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
					else if (Event.current.keyCode == KeyCode.O && selectedWaypoint != null)
					{
						selectedWaypoint.isOuter = !selectedWaypoint.isOuter;
					}
					else if (Event.current.keyCode == KeyCode.I && selectedWaypoint != null)
					{
						selectedWaypoint.isRabbitHoleGoal = !selectedWaypoint.isRabbitHoleGoal;
					}
					else if (Event.current.keyCode == KeyCode.S)
					{
						snap = false;
					}

					SceneView.RepaintAll();
					HandleUtility.Repaint();
					break;
			}
		}


		int i = 0;
		foreach (SPWaypoint waypoint in waypoints)
		{
			if (waypoint == selectedWaypoint)
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
            if (Handles.Button(worldPos, Quaternion.identity, HandleUtility.GetHandleSize(worldPos) * 0.2f, HandleUtility.GetHandleSize(worldPos) * 0.2f, Handles.SphereCap))
            {
	            if (enableEditing)
	            {
		            handleClick(waypoint);
	            }
            }
		
			
            Handles.color = Color.blue;
			foreach (int connectedIndex in waypoint.connectedTo)
			{
				Handles.zTest = CompareFunction.Always;
				Handles.DrawLine(worldPos, waypoints[connectedIndex].localPosition + sceneTransform.transform.position);
			}

			Handles.Label(worldPos, "#" + i, labelStyle);
			i++;
		}

		if (selectedWaypoint != null)
        {
            Vector3 worldPos = selectedWaypoint.localPosition + sceneTransform.transform.position;
            Vector3 newPos = Handles.PositionHandle (selectedWaypoint.getWorldPosition (sceneTransform.transform), Quaternion.identity);
            selectedWaypoint.localPosition = handleSnap(newPos, selectedWaypoint);

            selectedWaypoint.localPosition = handleSnap(newPos, selectedWaypoint);

            if (state == State.CONNECT)
            {
                Handles.Label(worldPos, "\nConnecting...", labelStyle);
            }
            else
            {
                Handles.Label(worldPos, "\n(C)onnect\n(R)emove\n(O)uter\nRabb(i)t Hole", labelStyle);
            }
        }

		base.RenderSceneGui (parkitectObj);
    }


    public void handleClick(SPWaypoint waypoint)
    {

        if (state == State.NONE && waypoint != null)
        {
            selectedWaypoint = waypoint;
        }
        else if (state == State.CONNECT && selectedWaypoint != null)
        {
            int closestWaypointIndex =waypoints.FindIndex(delegate (SPWaypoint wp)
            {
                return wp == waypoint;
            });
            int selectedWaypointIndex = waypoints.FindIndex(delegate (SPWaypoint wp)
            {
                return wp == selectedWaypoint;
            });
            if (closestWaypointIndex >= 0 && selectedWaypointIndex >= 0)
            {
                if (!selectedWaypoint.connectedTo.Contains(closestWaypointIndex))
                {
                    selectedWaypoint.connectedTo.Add(closestWaypointIndex);
                    waypoint.connectedTo.Add(selectedWaypointIndex);
                }
                else
                {
                    selectedWaypoint.connectedTo.Remove(closestWaypointIndex);
                    waypoint.connectedTo.Remove(selectedWaypointIndex);
                }
            }
        }

    }
    public void addWaypoint(Transform transform)
    {
        Vector3 pos = transform.position;
        selectedWaypoint = new SPWaypoint();

        if (Camera.current != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0, helperPlaneY, 0));
            float enter = 0;
            plane.Raycast(ray, out enter);
            selectedWaypoint.localPosition = ray.GetPoint(enter) - pos;
        }
        else
        {
            selectedWaypoint.localPosition = new Vector3(0, helperPlaneY, 0);
        }

        waypoints.Add(selectedWaypoint);
    }

    private void generateOuterGrid(FlatrideDecorator flatRideDecorator)
    {
        float minX = -flatRideDecorator.XSize / 2;
		float maxX = flatRideDecorator.XSize / 2;
		float minZ = -flatRideDecorator.ZSize / 2;
		float maxZ = flatRideDecorator.ZSize / 2;
		for (int xi = 0; xi < Mathf.RoundToInt(maxX - minX); xi++)
		{
			for (int zi = 0; zi < Mathf.RoundToInt(maxZ - minZ); zi++)
			{
				float x = minX + xi;
				float z = minZ + zi;
				if (!(x == minX || x == maxX - 1) && !(z == minZ || z == maxZ - 1))
				{
					continue;
				}
				SPWaypoint newWaypoint = new SPWaypoint();
				newWaypoint.localPosition = new Vector3(x + 0.5f, helperPlaneY, z + 0.5f);
				newWaypoint.isOuter = true;
				//if (waypoints.Count > 0) {
				//    newWaypoint.connectedTo.Add(waypoints.Count - 1);
				//}
				waypoints.Add(newWaypoint);
			}
		}

    }
    public void removeSelectedWaypoint()
    {

        int selectedWaypointIndex = waypoints.FindIndex(delegate (SPWaypoint wp)
        {
            return wp == selectedWaypoint;
        });
        foreach (SPWaypoint waypoint in waypoints)
        {
            waypoint.connectedTo.Remove(selectedWaypointIndex);
        }
        waypoints.Remove(selectedWaypoint);

        foreach (SPWaypoint waypoint in waypoints)
        {
            for (int i = 0; i < waypoint.connectedTo.Count; i++)
            {
                if (waypoint.connectedTo[i] > selectedWaypointIndex)
                {
                    waypoint.connectedTo[i]--;
                }
            }
        }

        selectedWaypoint = null;

    }

    public void rotateWaypoints(Transform transform)
    {
        Vector3 pos = transform.position;

        foreach (SPWaypoint waypoint in waypoints)
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

        if (snap && (newPos - oldPos).magnitude > Mathf.Epsilon)
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
            SPWaypoint connectedWaypoint = waypoints[connectedIndex];
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
			Waypoint waypoint = new Waypoint();
			waypoint.connectedTo = t.connectedTo;
			waypoint.isOuter = t.isOuter;
			waypoint.isRabbitHoleGoal = t.isRabbitHoleGoal;
			waypoint.localPosition = t.localPosition;
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

	public override void Deserialize (Dictionary<string,object> elements)
	{
		
		if (elements.ContainsKey("Waypoints")) {
			foreach (var wp in (List<object> )elements["Waypoints"]) {
				Waypoints.Add (SPWaypoint.Deserialize (wp as Dictionary<string,object>));
			}
		}
		base.Deserialize (elements);
	}

}



