using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ColorDecorator : Decorator
{
	public bool IsRecolorable;
	public Color[] Colors = {new Color(0.95f, 0, 0), new Color(0.32f, 0, 0), new Color(0.110f, 0.059f, 1f), new Color(1f, 0, 1f)};
	public int ColorCount = 1;

	public ColorDecorator()
	{
		
	}

#if UNITY_EDITOR
	public override void RenderInspectorGui(ParkitectObj parkitectObj)
	{
		//ModManager.asset.Shader = (ParkitectObject.Shaders)EditorGUILayout.EnumPopup("Shader", ModManager.asset.Shader);
		IsRecolorable = EditorGUILayout.BeginToggleGroup("Recolorable", IsRecolorable);

		if (IsRecolorable)
		{
			GUILayout.BeginVertical();
			try
			{
				ColorCount = Mathf.RoundToInt(EditorGUILayout.Slider("Color Count: ", ColorCount, 1, 4));
				for (int x = 0; x < ColorCount; x++)
				{
					Colors[x] = EditorGUILayout.ColorField("Color " + x, Colors[x]);

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
		for (int x = 0; x < ColorCount; x++)
		{
			c.Add( Utility.SerializeColor(Colors[x]));
		}

		return new Dictionary<string, object>
		{
			{"Colors", c},
			{"IsRecolorable", IsRecolorable}
		};
	}

	public override void Deserialize(Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("Colors"))
		{
			List<object> deserializeColors = (List<object>) elements["Colors"];

			int index = 0;
			foreach (var colordeserialize in deserializeColors)
			{
				Colors[index] = Utility.DeSerializeColor(colordeserialize as Dictionary<string,object>);
				index++;
			}

			ColorCount = deserializeColors.Count;
		}

		if (elements.ContainsKey("IsRecolorable"))
			IsRecolorable = (bool)elements["IsRecolorable"];
		base.Deserialize(elements);
	}
	
#if PARKITECT
	public override void Decorate(GameObject go, GameObject hider, ParkitectObj parkitectObj, AssetBundle bundle)
	{
		if (IsRecolorable)
		{
			CustomColors customColors = go.AddComponent<CustomColors>();
			List<Color> final = new List<Color>();
			for (int x = 0; x < ColorCount; x++)
			{
				final.Add(Colors[x]);
			}
			customColors.setColors(final.ToArray());
		}
	}
#endif
}

