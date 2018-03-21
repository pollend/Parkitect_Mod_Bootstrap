
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
	public bool SnapCenter = true;
	public bool Snap;
	public bool Grid;
	public float HeightDelta;
	public float GridSubdivision = 1;

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
			{"SnapCenter", SnapCenter},
			{"Snap", Snap},
			{"Grid", Grid},
			{"HeightDelta", HeightDelta},
			{"GridSubdivisons", GridSubdivision}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if(elements.ContainsKey("SnapCenter"))
			SnapCenter = (bool)elements["SnapCenter"];
		if(elements.ContainsKey ("Snap") )
			Snap = (bool)elements["Snap"];
		if(elements.ContainsKey ("Grid") )
			Grid = (bool)elements["Grid"];
		if(elements.ContainsKey ("HeightDelta"))
			HeightDelta = Convert.ToSingle(elements["HeightDelta"]);
		if(elements.ContainsKey ("GridSubdivisons"))
			GridSubdivision = Convert.ToSingle(elements["GridSubdivisons"]);
		base.Deserialize (elements);
	}
	
}


