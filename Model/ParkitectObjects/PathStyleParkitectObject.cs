#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

public enum PathType { Normal, Queue, Employee }

[ParkitectObjectTag("Path")]
[Serializable]
public class PathStyleParkitectObject : ParkitectObj
{
    
    public PathType PathType;
    public Texture2D PathTexture;
    public String PathTexturePath;
    
    public override Type[] SupportedDecorators()
    {
        return new[]{
            typeof(BaseDecorator),
            typeof(CategoryDecorator),
            typeof(BoundingBoxDecorator)
        };
    }
    
    
    public override Dictionary<string, object> Serialize()
    {
        Dictionary<string,object> elements = base.Serialize();
        elements.Add("PathType", PathType);
        elements.Add("texture", PathTexturePath);


        return new Dictionary<string, object>
        {
            {"PathType", PathType},
            {"texture", PathTexturePath}
        };
    }

    public override void DeSerialize (Dictionary<string,object> elements)
    {
        if (elements.ContainsKey("PathType") )
            PathType = (PathType)Enum.Parse (typeof(PathType), (string) elements["PathType"]);
        if (elements.ContainsKey ("texture"))
            PathTexturePath = (string) elements["texture"];
        base.DeSerialize (elements);
    }
    
#if UNITY_EDITOR
    public override void RenderInspectorGui ()
    {
        
        base.RenderInspectorGui ();
        
        PathTexture = (Texture2D)EditorGUILayout.ObjectField("Texture",PathTexture, typeof(Texture2D), true);
        PathTexturePath = AssetDatabase.GetAssetPath(PathTexture);
        if(GUILayout.Button("Create") && PathTexture)
        {
            PathTexture.alphaIsTransparency = true;
            PathTexture.wrapMode = TextureWrapMode.Repeat;
            PathTexture.filterMode = FilterMode.Point;

            AssetDatabase.DeleteAsset("Assets/Materials/Paths/" + Key + ".mat");
            Prefab.AddComponent<MeshRenderer>();
            MeshRenderer MR = Prefab.GetComponent<MeshRenderer>();

            //Check Folder for the mat
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Paths"))
                AssetDatabase.CreateFolder("Assets/Materials", "Paths");
            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            material.mainTexture = PathTexture;
            AssetDatabase.CreateAsset(material, "Assets/Materials/Paths/" + Key + ".mat");
            MR.material = material;

            Prefab.AddComponent<MeshFilter>();
            MeshFilter MF = Prefab.GetComponent<MeshFilter>();
            GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            MF.mesh = GO.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(GO);
            Prefab.transform.eulerAngles = new Vector3(90,0,0);
        }

    }

    public override string[] getAssets()
    {
        return new[]{AssetDatabase.GetAssetPath(PathTexture)};
    }
    
#endif
    
    
#if (PARKITECT)
    private PathStyle _pathStyle;
    public override void BindToParkitect(GameObject hider, AssetBundle bundle)
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
        PathDecorator pathDecorator = DecoratorByInstance<PathDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();

        PathStyle c = AssetManager.Instance.pathStyles.getPathStyle("concrete");
        PathStyle ps = new PathStyle();

        ps.handRailGO = c.handRailGO;
        ps.handRailRampGO = c.handRailRampGO;
        Material mat = Instantiate(c.material);
        mat.mainTexture = bundle.LoadAsset<Texture>(pathDecorator.PathTexturePath);
        ps.material = mat;
        ps.platformTileMapper = AssetManager.Instance.platformTileMapper;
        ps.identifier = Key;
        ps.spawnSound = c.spawnSound;
        ps.despawnSoundEvent = c.despawnSoundEvent;
        ps.spawnLastSound = c.spawnLastSound;
        ps.spawnTilesOnPlatforms = true;

        _pathStyle = ps;

        switch (pathDecorator.PathType)
        {
            case PathType.Normal:
                AssetManager.Instance.pathStyles.registerPathStyle(ps);
                break;
            case PathType.Queue:
                AssetManager.Instance.queueStyles.registerPathStyle(ps);
                break;
            case PathType.Employee:
                AssetManager.Instance.employeePathStyles.registerPathStyle(ps);
                break;
        }
    }

    public override void UnBindToParkitect(GameObject hider)
    {
        PathDecorator pathDecorator = DecoratorByInstance<PathDecorator>();

        switch (pathDecorator.PathType)
        {
            case PathType.Normal:
                AssetManager.Instance.pathStyles.unregisterPathStyle(_pathStyle);
                break;
            case PathType.Queue:
                AssetManager.Instance.queueStyles.unregisterPathStyle(_pathStyle);
                break;
            case PathType.Employee:
                AssetManager.Instance.employeePathStyles.unregisterPathStyle(_pathStyle);
                break;
        }
    }
#endif
}

