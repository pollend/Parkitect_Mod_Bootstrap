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
    public override void BindToParkitect(GameObject hider,List<SerializedMonoBehaviour> register)
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        GridDecorator gridDecorator = DecoratorByInstance<GridDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
        ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();

        GameObject gameObject = Instantiate(Prefab);
        gameObject.transform.parent = hider.transform;

        Deco deco = gameObject.AddComponent<Deco>();
        register.Add(deco);
        deco.name = Key;

        deco.buildOnLayerMask = 4096;
        deco.heightChangeDelta = gridDecorator.heightDelta;
        deco.defaultGridSubdivision = gridDecorator.gridSubdivision;
        deco.buildOnGrid = gridDecorator.grid;
        deco.defaultSnapToGridCenter = gridDecorator.snap;

        deco.isPreview = true;
        deco.isStatic = true;
        deco.dontSerialize = true;
        
        RemapUtility.RemapMaterials(gameObject);

        baseDecorator.Decorate(gameObject, hider, this,register);
        colorDecorator.Decorate(gameObject, hider, this,register);
        categoryDecorator.Decorate(gameObject,hider,this,register);


        foreach (var box in boxDecorator.boundingBoxes)
        {
            var b = gameObject.AddComponent<BoundingBox>();
            b.setBounds(box.bounds);
        }

        base.BindToParkitect(hider,register);
    }
#endif
}