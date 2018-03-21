#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class FlatrideDecorator : Decorator
{
	public float Excitement;
	public float Intensity;
	public float Nausea;
	public int XSize = 1;
	public int ZSize = 1;
	public Vector3 ClosedAngleRetraints;

#if UNITY_EDITOR
public override void RenderInspectorGui (ParkitectObj parkitectObj)
{

	GUILayout.Space(10);
	GUILayout.Label("Rating", EditorStyles.boldLabel);
	Excitement = EditorGUILayout.Slider("Excitement (" + getRatingCategory(Excitement) + ")", Excitement, 0, 100);
	Intensity = EditorGUILayout.Slider("Intensity (" + getRatingCategory(Intensity) + ")", Intensity, 0, 100);
	Nausea = EditorGUILayout.Slider("Nausea (" + getRatingCategory(Nausea) + ")", Nausea, 0, 100);
	GUILayout.Space(10);
	ClosedAngleRetraints = EditorGUILayout.Vector3Field("Closed Restraints Angle", ClosedAngleRetraints);

	GUILayout.Space(10);
	GUI.color = Color.white;
	XSize = EditorGUILayout.IntField("X", XSize);
	ZSize = EditorGUILayout.IntField("Z", ZSize);

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

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{
		return new Dictionary<string, object>()
		{
			{"Excitement", Excitement},
			{"Intensity", Intensity},
			{"Nausea", Nausea},
			{"XSize", XSize},
			{"ZSize", ZSize}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("Excitement"))
			Excitement = Convert.ToSingle(elements["Excitement"]);
		if (elements.ContainsKey("Intensity"))
			Intensity = Convert.ToSingle(elements["Intensity"]);
		if (elements.ContainsKey("Nausea"))
			Nausea = Convert.ToSingle(elements["Nausea"]);
		if (elements.ContainsKey("XSize"))
			XSize = Convert.ToInt32(elements["XSize"]);
		if (elements.ContainsKey("ZSize"))
			ZSize = Convert.ToInt32(elements["ZSize"]);
		base.Deserialize (elements);
		
	}
}



