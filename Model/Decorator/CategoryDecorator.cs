using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CategoryDecorator : Decorator
{
	public string category;
	public String subCategory;

#if UNITY_EDITOR
	public override void RenderInspectorGUI(ParkitectObj parkitectObj)
	{
		category = EditorGUILayout.TextField("Category: ", category);
		subCategory = EditorGUILayout.TextField("Sub-Category: ", this.subCategory);

		base.RenderInspectorGUI(parkitectObj);
	}
#endif

	public override List<XElement> Serialize(ParkitectObj parkitectObj)
	{
		return new List<XElement>(new[]
		{
			new XElement("Category", category),
			new XElement("SubCategory", subCategory)
		});
	}

	public override void Deserialize(XElement elements)
	{
		category = elements.Element("Category").Value;
		subCategory = elements.Element("SubCategory").Value;

		base.Deserialize(elements);
	}
	
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj,List<SerializedMonoBehaviour> register)
	{
		BuildableObject buildableObject = go.GetComponent<BuildableObject>();
		if (String.IsNullOrEmpty(subCategory))
		{
			buildableObject.categoryTag = category + "/" + subCategory;
		}
		else
		{
			buildableObject.categoryTag = category;
		}
	}
#endif
}

