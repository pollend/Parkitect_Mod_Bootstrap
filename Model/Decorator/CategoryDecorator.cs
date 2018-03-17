using System;
using System.Collections.Generic;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CategoryDecorator : Decorator
{
	public string category;
	public String subCategory;

	public CategoryDecorator()
	{
	}
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
		return new List<XElement>(new XElement[]
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
}

