using System;
using UnityEngine;

[ParkitectObjectTag("Path")]
[Serializable]
public class PathStyleParkitectObject : ParkitectObj
{
    public PathStyleParkitectObject()
    {
    }

    public override Type[] SupportedDecorators()
    {
        return new Type[]{
            typeof(BaseDecorator),
            typeof(CategoryDecorator),
            typeof(PathDecorator),
            typeof(BoundingBoxDecorator)
        };
    }
    
    
#if (!UNITY_EDITOR)
        public override void BindToParkitect()
        {

            BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
            CategoryDecorator categoryDecorator = this.DecoratorByInstance<CategoryDecorator>();
            PathDecorator pathDecorator = this.DecoratorByInstance<PathDecorator>();
            BoundingBoxDecorator boxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();

            PathStyle c = AssetManager.Instance.pathStyles.getPathStyle("concrete");
            PathStyle ps = new PathStyle();


            ps.handRailGO = c.handRailGO;
            ps.handRailRampGO = c.handRailRampGO;
            Material Mat = GameObject.Instantiate(c.material);
            Mat.mainTexture = pathDecorator.PathTexture;
            ps.material = Mat;
            ps.platformTileMapper = AssetManager.Instance.platformTileMapper;
            ps.identifier = this.getKey;
            ps.spawnSound = c.spawnSound;
            ps.despawnSoundEvent = c.despawnSoundEvent;
            ps.spawnLastSound = c.spawnLastSound;
            ps.spawnTilesOnPlatforms = true;


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


            //base.BindToParkitect();
        }
    #endif
}

