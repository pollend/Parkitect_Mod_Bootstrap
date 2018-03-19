using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlatrideDecorator : Decorator
{
	public float Excitement;
	public float Intensity;
	public float Nausea;
	public int XSize = 1;
	public int ZSize = 1;
	public Vector3 closedAngleRetraints;

#if UNITY_EDITOR
public override void RenderInspectorGui (ParkitectObj parkitectObj)
{

	GUILayout.Space(10);
	GUILayout.Label("Rating", EditorStyles.boldLabel);
	Excitement = EditorGUILayout.Slider("Excitement (" + getRatingCategory(Excitement) + ")", Excitement, 0, 100);
	Intensity = EditorGUILayout.Slider("Intensity (" + getRatingCategory(Intensity) + ")", Intensity, 0, 100);
	Nausea = EditorGUILayout.Slider("Nausea (" + getRatingCategory(Nausea) + ")", Nausea, 0, 100);
	GUILayout.Space(10);
	closedAngleRetraints = EditorGUILayout.Vector3Field("Closed Restraints Angle", closedAngleRetraints);

	GUILayout.Space(10);
	GUI.color = Color.white;
	XSize = EditorGUILayout.IntField("X", (int)Math.Floor(XSize));
	ZSize = EditorGUILayout.IntField("Z", (int)Math.Floor(ZSize));

	base.RenderInspectorGui (parkitectObj);
}

	public override void RenderSceneGui (ParkitectObj parkitectObj)
	{
		GameObject refrence =  parkitectObj.getGameObjectRef (false);
		if (refrence == null)
			return;
		
		Vector3 topLeft = new Vector3(-XSize / 2.0f, 0, ZSize / 2.0f) + refrence.transform.position;
		Vector3 topRight = new Vector3(XSize / 2.0f, 0, ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomLeft = new Vector3(-XSize / 2.0f, 0, -ZSize / 2.0f) + refrence.transform.position;
		Vector3 bottomRight = new Vector3(XSize / 2.0f, 0, -ZSize / 2.0f) + refrence.transform.position;

		Color fill = Color.white;
		fill.a = 0.1f;
		Handles.zTest = CompareFunction.LessEqual;
		Handles.DrawSolidRectangleWithOutline(new[] { topLeft, topRight, bottomRight, bottomLeft }, fill, Color.black);

		base.RenderSceneGui (parkitectObj);
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
		return new List<XElement>(new[]{
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
			XSize = int.Parse(elements.Element ("XSize").Value);
		if (elements.Element ("ZSize") != null)
			ZSize = int.Parse(elements.Element ("ZSize").Value);
		base.Deserialize (elements);
	}
}



