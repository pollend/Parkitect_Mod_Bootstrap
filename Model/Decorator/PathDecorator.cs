#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

public enum PathType { Normal, Queue, Employee }

public class PathDecorator : Decorator
{
	public PathType PathType;
	public Texture2D PathTexture;
	public String PathTexturePath;
#if UNITY_EDITOR
	public override void RenderInspectorGui (ParkitectObj parkitectObj)
	{
		PathTexture = (Texture2D)EditorGUILayout.ObjectField("Texture",PathTexture, typeof(Texture2D), true);
		PathTexturePath = AssetDatabase.GetAssetPath(PathTexture);
		if(GUILayout.Button("Create") && PathTexture)
		{
			PathTexture.alphaIsTransparency = true;
			PathTexture.wrapMode = TextureWrapMode.Repeat;
			PathTexture.filterMode = FilterMode.Point;

			AssetDatabase.DeleteAsset("Assets/Materials/Paths/" + parkitectObj.Key + ".mat");
			parkitectObj.Prefab.AddComponent<MeshRenderer>();
			MeshRenderer MR = parkitectObj.Prefab.GetComponent<MeshRenderer>();

			//Check Folder for the mat
			if (!AssetDatabase.IsValidFolder("Assets/Materials"))
				AssetDatabase.CreateFolder("Assets", "Materials");
			if (!AssetDatabase.IsValidFolder("Assets/Materials/Paths"))
				AssetDatabase.CreateFolder("Assets/Materials", "Paths");
			Material material = new Material(Shader.Find("Transparent/Diffuse"));
			material.mainTexture = PathTexture;
			AssetDatabase.CreateAsset(material, "Assets/Materials/Paths/" + parkitectObj.Key + ".mat");
			MR.material = material;

			parkitectObj.Prefab.AddComponent<MeshFilter>();
			MeshFilter MF = parkitectObj.Prefab.GetComponent<MeshFilter>();
			GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Quad);
			MF.mesh = GO.GetComponent<MeshFilter>().sharedMesh;
			DestroyImmediate(GO);
			parkitectObj.Prefab.transform.eulerAngles = new Vector3(90,0,0);
		}

		base.RenderInspectorGui (parkitectObj);
	}

	public override string[] getAssets()
	{
		return new[]{AssetDatabase.GetAssetPath(PathTexture)};
	}
#endif

	public override Dictionary<string, object> Serialize(ParkitectObj parkitectObj)
	{

		return new Dictionary<string, object>
		{
			{"PathType", PathType},
			{"texture", PathTexturePath}
		};
	}

	public override void Deserialize (Dictionary<string,object> elements)
	{
		if (elements.ContainsKey("PathType") )
			PathType = (PathType)Enum.Parse (typeof(PathType), (string) elements["PathType"]);
		if (elements.ContainsKey ("texture"))
			PathTexturePath = (string) elements["texture"];
		base.Deserialize (elements);
	}

}


