﻿
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System;

public class SeatDecorator : Decorator
{
	private const string SEAT = "Seat";

	[System.NonSerialized]
	private List<GameObject> seats = new List<GameObject>();

	#if UNITY_EDITOR
	public override void Load (ParkitectObj parkitectObj)
	{
	seats.Clear ();
	Utility.findAllChildrenWithName (parkitectObj.getGameObjectRef (true).transform, SEAT, seats);
	base.Load (parkitectObj);
	}


	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
	GUILayout.BeginHorizontal();
	if (GUILayout.Button("Create 1 Seats"))
	{
		GameObject seat1 = new GameObject(SEAT);
		seat1.transform.parent = parkitectObj.getGameObjectRef(true).transform;

		seat1.transform.localPosition = new Vector3(0, 0.1f, 0);
		seat1.transform.localRotation = Quaternion.Euler(Vector3.zero);

		seats.Clear ();
		Utility.findAllChildrenWithName (parkitectObj.getGameObjectRef (true).transform, SEAT, seats);
	}
	if (GUILayout.Button("Create 2 Seat"))
	{
		GameObject seat1 = new GameObject(SEAT);
		GameObject seat2 = new GameObject(SEAT);



		seat1.transform.parent = parkitectObj.getGameObjectRef(true).transform;
		seat2.transform.parent = parkitectObj.getGameObjectRef(true).transform;

		seat1.transform.localPosition = new Vector3(0.1f, 0.1f, 0);
		seat1.transform.localRotation = Quaternion.Euler(Vector3.zero);
		seat2.transform.localPosition = new Vector3(-0.1f, 0.1f, 0);
		seat2.transform.localRotation = Quaternion.Euler(Vector3.zero);
		seats.Clear ();
		Utility.findAllChildrenWithName (parkitectObj.getGameObjectRef (true).transform,SEAT, seats);

	}
	GUILayout.EndHorizontal();

	base.RenderInspectorGUI (parkitectObj);
	}


	public override void RenderSceneGUI (ParkitectObj parkitectObj)
	{
	//Debug.Log (parkitectObj.Prefab.transform.GetInstanceID ());
	//Debug.Log (parkitectObj.getGameObjectRef(true).transform.GetInstanceID ());


	seats.RemoveAll (x => x == null);

	if(seats != null)
		for (int x = 0; x < seats.Count; x++) {
			if (seats [x] != null) {
				var transform = seats [x].transform;

				Handles.SphereCap (200, transform.position, transform.rotation, 0.05f);
				Vector3 vector = transform.position - transform.up * 0.02f + transform.forward * 0.078f - transform.right * 0.045f;
				Handles.SphereCap (201, vector, transform.rotation, 0.03f);
				Vector3 vector2 = transform.position - transform.up * 0.02f + transform.forward * 0.078f + transform.right * 0.045f;
				Handles.SphereCap (202, vector2, transform.rotation, 0.03f);
				Vector3 center = transform.position + transform.up * 0.305f + transform.forward * 0.03f;
				Handles.SphereCap (203, center, transform.rotation, 0.1f);
				Vector3 center2 = vector + transform.forward * 0.015f - transform.up * 0.07f;
				Handles.SphereCap (204, center2, transform.rotation, 0.02f);
				Vector3 center3 = vector2 + transform.forward * 0.015f - transform.up * 0.07f;
				Handles.SphereCap (205, center3, transform.rotation, 0.02f);
			}
	}


	base.RenderSceneGUI (parkitectObj);
	}
	#endif

}



