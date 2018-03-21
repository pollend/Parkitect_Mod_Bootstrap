using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ColorDecorator : Decorator
{
	public bool isRecolorable;
	public Color[] colors = {new Color(0.95f, 0, 0), new Color(0.32f, 0, 0), new Color(0.110f, 0.059f, 1f), new Color(1f, 0, 1f)};
	public int colorCount = 1;

	public ColorDecorator()
	{
		
	}

#if UNITY_EDITOR
	public override void RenderInspectorGui(ParkitectObj parkitectObj)
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
					colors[x] = EditorGUILayout.ColorField("Color " + x, colors[x]);

				}
			}
			catch (Exception)
			{
			}

			GUILayout.EndVertical();
		}
		EditorGUILayout.EndToggleGroup();


		base.RenderInspectorGui(parkitectObj);
	}
#endif

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{
		List<object> c = new List<object>();
		for (int x = 0; x < colorCount; x++)
		{
			c.Add( Utility.SerializeColor(colors[x]));
		}

		return new Dictionary<string, object>
		{
			{"Colors", c},
			{"IsRecolorable", isRecolorable}
		};
	}

	public override void Deserialize(Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("Colors"))
		{
			int index = 0;
			foreach (var colordeserialize in (List<object>)elements["Colors"])
			{
				colors[index] = Utility.DeSerializeColor(colordeserialize as Dictionary<string,object>);
				index++;
			}
		}

		if (elements.ContainsKey("IsRecolorable"))
			isRecolorable = (bool)elements["IsRecolorable"];
		base.Deserialize(elements);
	}
	
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		if (isRecolorable)
		{
			CustomColors customColors = go.AddComponent<CustomColors>();
			List<Color> final = new List<Color>();
			for (int x = 0; x < colorCount; x++)
			{
				final.Add(colors[x]);
			}
			customColors.setColors(final.ToArray());
		}
	}
#endif
}

