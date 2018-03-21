#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;


public class BoundingBoxDecorator : Decorator
{

	[NonSerialized]
	private bool enableEditing;
	private bool snap;
	private Vector2 scrollPos2;
	public SPBoundingBox selected;

	public List<SPBoundingBox> BoundingBoxes = new List<SPBoundingBox> ();

#if UNITY_EDITOR
    public override void RenderInspectorGui (ParkitectObj parkitectObj)
    {
        Event e = Event.current;

        scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, "GroupBox", GUILayout.Height(100));
        for (int i = 0; i < BoundingBoxes.Count; i++)
        {
            Color gui = GUI.color;
            if (BoundingBoxes[i] == selected)
            { GUI.color = Color.red; }

            if (GUILayout.Button("BoudingBox" + (i + 1)))
            {
                if (e.button == 1)
                {
                    BoundingBoxes.RemoveAt(i);
                    return;
                }

                if (selected == BoundingBoxes[i])
                {
                    selected = null;
                    return;
                }
                selected = BoundingBoxes[i];
            }
            GUI.color = gui;
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add BoudingBox"))
        {
			BoundingBoxes.Add(new SPBoundingBox());
        }
        string caption = "Enable Editing";
        if (enableEditing)
        {
            caption = "Disable Editing";
        }
        if (GUILayout.Button(caption))
        {
            enableEditing = !enableEditing;
        }
        if (enableEditing)
        {
            GUILayout.Label("Hold S - Snap to 0.25");
        }

        base.RenderInspectorGui (parkitectObj);
    }

    public override void RenderSceneGui (ParkitectObj parkitectObj)
    {
        drawBox(parkitectObj);

        if (!enableEditing)
        {
            return;
        }

       // EditorUtility.SetDirty(PMM);
        Tools.current = Tool.None;

        int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
        switch (Event.current.type)
        {
        case EventType.Layout:
            HandleUtility.AddDefaultControl(controlID);
            break;
        case EventType.KeyDown:
            if (Event.current.keyCode == KeyCode.S)
            {
                snap = true;
            }
            break;
        case EventType.KeyUp:
            if (Event.current.keyCode == KeyCode.S)
            {
                snap = false;
            }
            break;
        }

        base.RenderInspectorGui (parkitectObj);
    }

    private void drawBox(ParkitectObj parkitectObj)
    {
 
		foreach (SPBoundingBox box in BoundingBoxes)
        {

            Vector3 diff = box.Bounds.max - box.Bounds.min;
            Vector3 diffX = new Vector3(diff.x, 0, 0);
            Vector3 diffY = new Vector3(0, diff.y, 0);
            Vector3 diffZ = new Vector3(0, 0, diff.z);

            Color fill = Color.white;
            fill.a = 0.025f;
            Color outer = Color.gray;
            if (enableEditing && box == selected)
            {
                fill = Color.magenta;
                fill.a = 0.1f;
                outer = Color.black;
            }

	        
	        // left
            drawPlane(box.Bounds.min, box.Bounds.min + diffZ, box.Bounds.min + diffZ + diffY, box.Bounds.min + diffY, fill, outer, parkitectObj);

	        //back
            drawPlane(box.Bounds.min, box.Bounds.min + diffX, box.Bounds.min + diffX + diffY, box.Bounds.min + diffY, fill, outer, parkitectObj);

	        //right
            drawPlane(box.Bounds.max, box.Bounds.max - diffY, box.Bounds.max - diffY - diffZ, box.Bounds.max - diffZ, fill, outer, parkitectObj);

	        //forward
            drawPlane(box.Bounds.max, box.Bounds.max - diffY, box.Bounds.max - diffY - diffX, box.Bounds.max - diffX, fill, outer, parkitectObj);

            //up
            drawPlane(box.Bounds.max, box.Bounds.max - diffX, box.Bounds.max - diffX - diffZ, box.Bounds.max - diffZ, fill, outer, parkitectObj);
            
	        //down
            drawPlane(box.Bounds.min, box.Bounds.min + diffX, box.Bounds.min + diffX + diffZ, box.Bounds.min + diffZ, fill, outer, parkitectObj);

            if (enableEditing && box == selected)
            {
                box.Bounds.min = handleModifyValue(box.Bounds.min, parkitectObj.Prefab.transform.InverseTransformPoint(Handles.PositionHandle(parkitectObj.Prefab.transform.TransformPoint(box.Bounds.min), Quaternion.LookRotation(Vector3.left, Vector3.down))));
                box.Bounds.max = handleModifyValue(box.Bounds.max, parkitectObj.Prefab.transform.InverseTransformPoint(Handles.PositionHandle(parkitectObj.Prefab.transform.TransformPoint(box.Bounds.max), Quaternion.LookRotation(Vector3.forward))));
                Handles.Label(parkitectObj.Prefab.transform.position + box.Bounds.min, box.Bounds.min.ToString("F2"));
                Handles.Label(parkitectObj.Prefab.transform.position +  box.Bounds.max, box.Bounds.max.ToString("F2"));
            }
        }
    }
	private Vector3 handleModifyValue(Vector3 value, Vector3 newValue)
	{
		if (snap && (newValue - value).magnitude > Mathf.Epsilon)
		{
			if (Mathf.Abs(newValue.x - value.x) > Mathf.Epsilon)
			{
				newValue.x = Mathf.Round(newValue.x * 4) / 4;
			}
			if (Mathf.Abs(newValue.y - value.y) > Mathf.Epsilon)
			{
				newValue.y = Mathf.Round(newValue.y * 4) / 4;
			}
			if (Mathf.Abs(newValue.z - value.z) > Mathf.Epsilon)
			{
				newValue.z = Mathf.Round(newValue.z * 4) / 4;
			}
		}
		return newValue;
	}

	private void drawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color fill, Color outer, ParkitectObj PO)
	{
		Handles.DrawSolidRectangleWithOutline(new[] { PO.Prefab.transform.TransformPoint(p1), PO.Prefab.transform.TransformPoint(p2), PO.Prefab.transform.TransformPoint(p3), PO.Prefab.transform.TransformPoint(p4) }, fill, outer);
	}
#endif


	public override Dictionary<string,object> Serialize (ParkitectObj parkitectObj)
	{
		List<object> boxes = new List<object> ();
		foreach (var b in BoundingBoxes)
		{
			boxes.Add (b.Serialize());
		}
		return new Dictionary<string, object>
		{
			{"BoundingBoxes",boxes}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("BoundingBoxes")) {
			foreach (var box in (List<object>) elements["BoundingBoxes"]) {
				BoundingBoxes.Add (SPBoundingBox.Deserialize (box as Dictionary<string,object>));
			}
		}
		base.Deserialize (elements);
	}

}

