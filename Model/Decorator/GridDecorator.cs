using System;
using System.Collections.Generic;
using System.Xml.Linq;


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

	public GridDecorator()
	{
	}

	#if UNITY_EDITOR
	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
		this.grid = EditorGUILayout.Toggle("GridSnap: ", this.grid);
		this.heightDelta = EditorGUILayout.FloatField("HeightDelta: ", this.heightDelta);
		this.snapCenter = EditorGUILayout.Toggle("SnapCenter: ", this.snapCenter);
		this.gridSubdivision = EditorGUILayout.FloatField("Grid Subdivision", this.gridSubdivision);

	    base.RenderInspectorGUI (parkitectObj);
	}
	#endif

	public override List<XElement> Serialize ()
	{
		return new List<XElement>(new XElement[]{
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
			this.snapCenter = bool.Parse(elements.Element ("SnapCenter").Value);
		if(elements.Element ("Snap") != null)
			this.snap = bool.Parse(elements.Element ("Snap").Value);
		if(elements.Element ("Grid") != null)
			this.grid = bool.Parse(elements.Element ("Grid").Value);
		if(elements.Element ("HeightDelta") != null)
			this.heightDelta = float.Parse(elements.Element ("HeightDelta").Value);
		if(elements.Element ("GridSubdivisons") != null)
			this.gridSubdivision = float.Parse(elements.Element ("GridSubdivisons").Value);
		base.Deserialize (elements);
	}
}


