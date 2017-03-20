﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlatrideDecorator : Decorator
{
	public float Excitement;
	public float Intensity;
	public float Nausea;
	public float XSize = 1;
	public float ZSize = 1;
	public Vector3 closedAngleRetraints;

	public FlatrideDecorator()
	{
	}
#if UNITY_EDITOR
public override void RenderInspectorGUI (ParkitectObj parkitectObj)
{

	GUILayout.Space(10);
	GUILayout.Label("Rating", EditorStyles.boldLabel);
	this.Excitement = EditorGUILayout.Slider("Excitement (" + getRatingCategory(this.Excitement) + ")", this.Excitement, 0, 100);
	this.Intensity = EditorGUILayout.Slider("Intensity (" + getRatingCategory(this.Intensity) + ")", this.Intensity, 0, 100);
	this.Nausea = EditorGUILayout.Slider("Nausea (" + getRatingCategory(this.Nausea) + ")", this.Nausea, 0, 100);
	GUILayout.Space(10);
	this.closedAngleRetraints = EditorGUILayout.Vector3Field("Closed Restraints Angle", this.closedAngleRetraints);

	GUILayout.Space(10);
	GUI.color = Color.white;
	this.XSize = (float)EditorGUILayout.IntField("X", (int)Math.Floor(this.XSize));
	this.ZSize = (float)EditorGUILayout.IntField("Z", (int)Math.Floor(this.ZSize));

	base.RenderInspectorGUI (parkitectObj);
}

	public override void RenderSceneGUI (ParkitectObj parkitectObj)
	{
		GameObject refrence =  parkitectObj.getGameObjectRef (false);
		if (refrence == null)
			return;
		
		Vector3 topLeft = new Vector3(-this.XSize / 2.0f, 0, this.ZSize / 2.0f) + refrence.transform.position;
		Vector3 topRight = new Vector3(this.XSize / 2.0f, 0, this.ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomLeft = new Vector3(-this.XSize / 2.0f, 0, -this.ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomRight = new Vector3(this.XSize / 2.0f, 0, -this.ZSize / 2.0f) + refrence.transform.position;

		Color fill = Color.white;
		fill.a = 0.1f;
		Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, fill, Color.black);

		base.RenderSceneGUI (parkitectObj);
	}
#endif

	private string getRatingCategory(float ratingValue)
	{
		ratingValue /= 100f;
		if (ratingValue > 0.9f)
		{
			return "Very High";
		}
		if (ratingValue > 0.6f)
		{
			return "High";
		}
		if (ratingValue > 0.3f)
		{
			return "Medium";
		}
		return "Low";
	}

	public override List<XElement> Serialize (ParkitectObj parkitectObj)
	{
		return new List<XElement>(new XElement[]{
			new XElement("Excitement",Excitement),
			new XElement("Intensity",Intensity),
			new XElement("Nausea",Nausea),
			new XElement("XSize",XSize),
			new XElement("ZSize",ZSize)
		});
	}

	public override void Deserialize (XElement elements)
	{
		if (elements.Element ("Excitement") != null)
			Excitement = float.Parse(elements.Element ("Excitement").Value);
		if (elements.Element ("Intensity") != null)
			Intensity = float.Parse(elements.Element ("Intensity").Value);
		if (elements.Element ("Nausea") != null)
			Nausea = float.Parse(elements.Element ("Nausea").Value);
		if (elements.Element ("XSize") != null)
			XSize = float.Parse(elements.Element ("XSize").Value);
		if (elements.Element ("ZSize") != null)
			ZSize = float.Parse(elements.Element ("ZSize").Value);
		base.Deserialize (elements);
	}
}



