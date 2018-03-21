using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ParkitectObjectTag("Deco")]
[Serializable]
public class DecoParkitectObject : ParkitectObj
{

    public override Type[] SupportedDecorators()
    {
        return new[]
        {
            typeof(BaseDecorator),
            typeof(GridDecorator),
            typeof(CategoryDecorator),
            typeof(ColorDecorator),
            typeof(BoundingBoxDecorator)
        };
    }

#if (PARKITECT)
    private Deco _deco;
    
    public override void BindToParkitect(GameObject hider, AssetBundle bundle)
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        GridDecorator gridDecorator = DecoratorByInstance<GridDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
        ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();

        GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));
        gameObject.transform.parent = hider.transform;

        _deco = gameObject.AddComponent<Deco>();
        _deco.name = Key;

        _deco.buildOnLayerMask = 4096;
        _deco.heightChangeDelta = gridDecorator.HeightDelta;
        _deco.defaultGridSubdivision = gridDecorator.GridSubdivision;
        _deco.buildOnGrid = gridDecorator.Grid;
        _deco.defaultSnapToGridCenter = gridDecorator.Snap;

        _deco.isPreview = true;
        _deco.isStatic = true;
        _deco.dontSerialize = true;
        
        RemapUtility.RemapMaterials(gameObject);

        baseDecorator.Decorate(gameObject, hider, this,bundle);
        colorDecorator.Decorate(gameObject, hider, this,bundle);
        categoryDecorator.Decorate(gameObject,hider,this,bundle);


        foreach (var box in boxDecorator.BoundingBoxes)
        {
            var b = gameObject.AddComponent<BoundingBox>();
            b.setBounds(box.Bounds);
        }
        
        AssetManager.Instance.registerObject(_deco);
    }

    public override void UnBindToParkitect(GameObject hider)
    {
        AssetManager.Instance.unregisterObject(_deco);
    }
#endif
}