using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class GridDecorator : Decorator
{
	public bool snapCenter = true;
	public bool snap;
	public bool grid;
	public float heightDelta;
	public float gridSubdivision = 1;

#if UNITY_EDITOR
	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
		
		grid = EditorGUILayout.Toggle("GridSnap: ", this.grid);
		snapCenter = EditorGUILayout.Toggle("SnapCenter: ", this.snapCenter);
		
		heightDelta = Mathf.RoundToInt(EditorGUILayout.Slider("Height delta: ", heightDelta, 0.05f, 1) * 200f) / 200f;
		gridSubdivision = Mathf.RoundToInt(EditorGUILayout.Slider("Grid subdivision: ", gridSubdivision, 1, 9));
		
	    base.RenderInspectorGUI (parkitectObj);
	}

	public override void RenderSceneGUI(ParkitectObj parkitectObj)
	{
		GameObject gameObject = parkitectObj.getGameObjectRef(false);
		if (gameObject)
		{
			var min = snapCenter ? -2.5f : -3f;
			var max = snapCenter ? 2.5f : 3f;

			for (float x = min; x <= max; x += 1 / gridSubdivision)
			{
				Handles.DrawLine(gameObject.transform.position + new Vector3(x, 0, min),gameObject.transform.position + new Vector3(x, 0, max));
			}

			for (float z = min; z <= max; z += 1 / gridSubdivision)
			{
				Handles.DrawLine(gameObject.transform.position + new Vector3(min, 0, z),gameObject.transform.position + new Vector3(max, 0, z));
			}
		}

		base.RenderSceneGUI(parkitectObj);
	}
#endif

	public override List<XElement> Serialize (ParkitectObj parkitectObj)
	{
		return new List<XElement>(new[]{
			new XElement("SnapCenter",snapCenter),
			new XElement("Snap",snap),
			new XElement("Grid",grid),
			new XElement("HeightDelta",heightDelta),
			new XElement("GridSubdivisons",gridSubdivision)
		
		});
	}

	public override void Deserialize (XElement elements)
	{
		if(elements.Element ("SnapCenter") != null)
			snapCenter = bool.Parse(elements.Element ("SnapCenter").Value);
		if(elements.Element ("Snap") != null)
			snap = bool.Parse(elements.Element ("Snap").Value);
		if(elements.Element ("Grid") != null)
			grid = bool.Parse(elements.Element ("Grid").Value);
		if(elements.Element ("HeightDelta") != null)
			heightDelta = float.Parse(elements.Element ("HeightDelta").Value);
		if(elements.Element ("GridSubdivisons") != null)
			gridSubdivision = float.Parse(elements.Element ("GridSubdivisons").Value);
		base.Deserialize (elements);
	}
	
}


