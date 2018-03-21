
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GridDecorator : Decorator
{
	public bool snapCenter = true;
	public bool snap;
	public bool grid;
	public float heightDelta;
	public float gridSubdivision = 1;

	public GridDecorator()
	{
		
	}

#if UNITY_EDITOR
	public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{
		
		grid = EditorGUILayout.Toggle("GridSnap: ", grid);
		snapCenter = EditorGUILayout.Toggle("SnapCenter: ", snapCenter);
		
		heightDelta = Mathf.RoundToInt(EditorGUILayout.Slider("Height delta: ", heightDelta, 0.05f, 1) * 200f) / 200f;
		gridSubdivision = Mathf.RoundToInt(EditorGUILayout.Slider("Grid subdivision: ", gridSubdivision, 1, 9));
		
	    base.RenderInspectorGui (parkitectObj);
	}

	public override void RenderSceneGui(ParkitectObj parkitectObj)
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

		base.RenderSceneGui(parkitectObj);
	}
#endif

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{
		return new Dictionary<string, object>
		{
			{"SnapCenter", snapCenter},
			{"Snap", snap},
			{"Grid", grid},
			{"HeightDelta", heightDelta},
			{"GridSubdivisons", gridSubdivision}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if(elements.ContainsKey("SnapCenter"))
			snapCenter = (bool)elements["SnapCenter"];
		if(elements.ContainsKey ("Snap") )
			snap = (bool)elements["Snap"];
		if(elements.ContainsKey ("Grid") )
			grid = (bool)elements["Grid"];
		if(elements.ContainsKey ("HeightDelta"))
			heightDelta = (float)(double)elements["HeightDelta"];
		if(elements.ContainsKey ("GridSubdivisons"))
			gridSubdivision = (float)(double)elements["GridSubdivisons"];
		base.Deserialize (elements);
	}
	
}


