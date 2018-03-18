using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BaseDecorator : Decorator
{
	[SerializeField]
		public string InGameName;

		[SerializeField]
		public float price;

#if UNITY_EDITOR

    public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
		InGameName = EditorGUILayout.TextField("In Game name: ", InGameName);
		price = EditorGUILayout.FloatField("Price: ", price);
        base.RenderInspectorGUI (parkitectObj);
	}
#endif

	public override List<XElement> Serialize (ParkitectObj parkitectObj)
	{
		List<XElement> element = new List<XElement> ();
		element.Add (new XElement ("InGameName", InGameName));
		element.Add (new XElement ("Price", price));
		return element;
	}

	public override void Deserialize (XElement elements)
	{
		if (elements.Element ("InGameName") != null)
			InGameName = elements.Element ("InGameName").Value;
		if (elements.Element ("Price") != null)
			price = float.Parse(elements.Element ("Price").Value);
		
		base.Deserialize (elements);
	}

#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj,List<SerializedMonoBehaviour> register)
	{
		BuildableObject component = go.GetComponent<BuildableObject>();
		component.price = price;
		component.setDisplayName(InGameName);
	}
#endif
	
}

