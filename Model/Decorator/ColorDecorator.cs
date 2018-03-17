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
	public Color[] colors =
		{new Color(0.95f, 0, 0), new Color(0.32f, 0, 0), new Color(0.110f, 0.059f, 1f), new Color(1f, 0, 1f)};
	public int colorCount = 1;

	public ColorDecorator()
	{

	}

#if UNITY_EDITOR
	public override void RenderInspectorGUI(ParkitectObj parkitectObj)
	{
		//ModManager.asset.Shader = (ParkitectObject.Shaders)EditorGUILayout.EnumPopup("Shader", ModManager.asset.Shader);
		isRecolorable = EditorGUILayout.BeginToggleGroup("Recolorable", isRecolorable);

		if (isRecolorable)
		{
			GUILayout.BeginVertical();
			try
			{
				colorCount = Mathf.RoundToInt(EditorGUILayout.Slider("Color Count: ", colorCount, 1, 4));
				for (int x = 0; x < colorCount; x++)
				{
					colors[x] = EditorGUILayout.ColorField("Color " + x, this.colors[x]);

				}
			}
			catch (Exception)
			{
			}

			GUILayout.EndVertical();
		}
		EditorGUILayout.EndToggleGroup();


		base.RenderInspectorGUI(parkitectObj);
	}
#endif

	public override List<XElement> Serialize(ParkitectObj parkitectObj)
	{
		List<XElement> xmlcolors = new List<XElement>();
		for (int x = 0; x < colorCount; x++)
		{
			xmlcolors.Add(new XElement("Color", Utility.SerializeColor(colors[x])));
		}

		return new List<XElement>
		{
			new XElement("Colors", xmlcolors),
			new XElement("IsRecolorable", isRecolorable),
		};
	}

	public override void Deserialize(XElement elements)
	{
		if (elements.Element("Colors") != null)
		{
			int index = 0;
			foreach (XElement colorXml in elements.Element("Colors").Elements("Color"))
			{
				colors[index] = Utility.DeSerializeColor(colorXml);
				index++;
			}
		}

		if (elements.Element("IsRecolorable") != null)
			isRecolorable = bool.Parse(elements.Element("IsRecolorable").Value);
		base.Deserialize(elements);
	}
}

