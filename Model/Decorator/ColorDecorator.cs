using System;
using UnityEngine;
using System.Xml.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

[Serializable]
public class ColorDecorator : Decorator
{
	public bool isRecolorable;

	public List<Color> colors = new List<Color>();

	public ColorDecorator()
	{

	}

#if UNITY_EDITOR
public override void RenderInspectorGUI (ParkitectObj parkitectObj)
{
	//ModManager.asset.Shader = (ParkitectObject.Shaders)EditorGUILayout.EnumPopup("Shader", ModManager.asset.Shader);
    isRecolorable = EditorGUILayout.BeginToggleGroup("Recolorable", isRecolorable);

    if (isRecolorable)
	{
		try
		{

			for(int x =0; x < colors.Count; x++)
			{
				this.colors[x] = EditorGUILayout.ColorField("Color "+x, this.colors[x]);
						
			}

			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();

			if(GUILayout.Button("Add Color"))
			{
					
					switch(colors.Count)
					{
						case 0:
							colors.Add(new Color(0.95f, 0, 0));
							break;
						case 1:
							colors.Add(new Color(0.32f, 0, 0));
							break;
						case 2:
							colors.Add(new Color(0.110f, 0.059f, 1f));
							break;
						case 3:
							colors.Add(new Color(1f, 0, 1f));
							break;
					}

			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			if(GUILayout.Button("Remove Color"))
			{
				colors.RemoveAt(colors.Count -1);
			}
			GUILayout.EndVertical();


			GUILayout.EndHorizontal();


		}
		catch (Exception)
		{
		}
	}
	EditorGUILayout.EndToggleGroup();

    base.RenderInspectorGUI (parkitectObj);
}
#endif

	public override List<XElement> Serialize ()
	{
		List<XElement> xmlcolors = new List<XElement> ();
		for (int x = 0; x < colors.Count; x++) {	
			xmlcolors.Add (new XElement("Color", Utility.SerializeColor (colors[x])));
		}
		return new List<XElement>{
			new XElement("Colors",xmlcolors),
			new XElement("IsRecolorable",isRecolorable),
		};
	}

	public override void Deserialize (XElement elements)
	{
		if (elements.Element ("Colors") != null) {
			foreach (XElement colorXml in elements.Element("Colors").Elements("Color")) {
				colors.Add(Utility.DeSerializeColor (colorXml));
			}
		}
		if(elements.Element ("IsRecolorable") != null)
			isRecolorable = bool.Parse (elements.Element ("IsRecolorable").Value);
		base.Deserialize (elements);
	}

}

