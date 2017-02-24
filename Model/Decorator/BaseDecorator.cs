using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class BaseDecorator : Decorator
{
		public BaseDecorator()
		{
		}


		[SerializeField]
		public string InGameName;

		[SerializeField]
		public float price;

#if UNITY_EDITOR

    public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{

		this.InGameName = EditorGUILayout.TextField("In Game name: ", this.InGameName);
		this.price = EditorGUILayout.FloatField("Price: ", this.price);
        base.RenderInspectorGUI (parkitectObj);
	}
#endif

	public override List<XElement> Serialize ()
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

}

