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

	public CategoryDecorator()
	{
	}
	#if UNITY_EDITOR
	public override void RenderInspectorGUI (ParkitectObj parkitectObj)
	{
		this.category = EditorGUILayout.TextField("Category: ", this.category);

	    base.RenderInspectorGUI (parkitectObj);
	}
	#endif

	public override List<XElement> Serialize ()
	{
		return new List<XElement>(new XElement[]{
			new XElement("Category",category)
		});
	}
}

