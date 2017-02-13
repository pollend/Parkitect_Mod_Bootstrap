using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class WaypointDecorator : Decorator
{
	private enum NodeState{Connecting, Dragging}


	[System.NonSerialized]
	private bool enableEditing = false;
	[System.NonSerialized]
	private float helperPlaneY = 0;
	[System.NonSerialized]
	public SPWaypoint selectedWaypoint;

	public List<SPWaypoint> waypoints = new List<SPWaypoint>();


	#if UNITY_EDITOR
	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
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
			//	currentTool = Tools.current;
				Tools.current = Tool.None;


			}
			else
			{
				//Tools.current = currentTool;
			}
		}
		if (enableEditing)
		{
			GUILayout.Label("S - Snap to axis of connected waypoints");
			helperPlaneY = EditorGUILayout.FloatField("Helper Plane Y", helperPlaneY);

			if (GUILayout.Button("Generate outer grid"))
			{
				//generateOuterGrid();
			}
			if (GUILayout.Button("(A)dd Waypoint"))
			{
				addWaypoint (sceneTransform.transform);
			}
			if (GUILayout.Button("Rotate 90°"))
			{
				//rotateWaypoints();
			}
			if (GUILayout.Button("Clear all"))
			{
				//ModManager.asset.waypoints.Clear();
			}
		}

		base.RenderInspectorGUI (parkitectObj);
	}

	private void addWaypoint(Transform transform)
	{
		selectedWaypoint = new SPWaypoint();

		if (Camera.current != null)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane plane = new Plane(Vector3.up, new Vector3(0, helperPlaneY, 0));
			float enter = 0;
			plane.Raycast(ray, out enter);
				selectedWaypoint.localPosition = ray.GetPoint(enter) - transform.position;
		}
		else
		{
			selectedWaypoint.localPosition = new Vector3(0, helperPlaneY, 0);
		}

		this.waypoints.Add(selectedWaypoint);
	}




	public override void RenderSceneGUI (ParkitectObj parkitectObj)
	{
		GameObject sceneTransform = parkitectObj.getGameObjectRef (false);
		if (sceneTransform == null)
			return;


		int i = 0;
		foreach (SPWaypoint waypoint in this.waypoints)
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
			
			if (Handles.Button (worldPos, Quaternion.identity, HandleUtility.GetHandleSize (worldPos) * 0.2f, HandleUtility.GetHandleSize (worldPos) * 0.2f, Handles.SphereCap)) {
					selectedWaypoint = waypoint;
			}
			

			Handles.color = Color.blue;
			foreach (int connectedIndex in waypoint.connectedTo)
			{
				Handles.DrawLine(worldPos, waypoints[connectedIndex].localPosition + sceneTransform.transform.position);
			}

		//	Handles.Label(worldPos, "#" + i, labelStyle);
			i++;
		}
		if (selectedWaypoint != null) {
				selectedWaypoint.localPosition = Handles.PositionHandle (selectedWaypoint.getWorldPosition (sceneTransform.transform), Quaternion.identity);
		}

		base.RenderSceneGUI (parkitectObj);
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
				//if (waypoints.waypoints.Count > 0) {
				//newWaypoint.connectedTo.Add(waypoints.waypoints.Count - 1);
				//}
				//ModManager.asset.waypoints.Add(newWaypoint);
			}
		}

	}
	#endif
}



