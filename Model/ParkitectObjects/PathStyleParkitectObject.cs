using System;
using System.Collections.Generic;
using UnityEngine;

[ParkitectObjectTag("Path")]
[Serializable]
public class PathStyleParkitectObject : ParkitectObj
{
    public override Type[] SupportedDecorators()
    {
        return new[]{
            typeof(BaseDecorator),
            typeof(CategoryDecorator),
            typeof(PathDecorator),
            typeof(BoundingBoxDecorator)
        };
    }
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

