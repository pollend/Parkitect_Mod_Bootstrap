using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ParkitectObjectTag("Seating")]
[Serializable]
public class SeatingParkitectObject : ParkitectObj
{
    public override Type[] SupportedDecorators()
    {
        return new[]{
            typeof(BaseDecorator),
            typeof(SeatDecorator),
            typeof(ColorDecorator),
            typeof(BoundingBoxDecorator),
            typeof(CategoryDecorator)
        };
    }
#if PARKITECT
    private Seating _seat;
    public override void BindToParkitect(GameObject hider, AssetBundle bundle)
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        SeatDecorator seatDecorator= DecoratorByInstance<SeatDecorator>();
        ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();

        GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));;
        gameObject.transform.parent = hider.transform;

        _seat = gameObject.AddComponent<Seating>();
        _seat.name = Key;
        
        RemapUtility.RemapMaterials(gameObject);

        baseDecorator.Decorate(gameObject, hider, this,bundle);
        colorDecorator.Decorate(gameObject, hider, this,bundle);
        categoryDecorator.Decorate(gameObject,hider,this,bundle);
       
        foreach (var box in boxDecorator.BoundingBoxes)
        {
            var b = Prefab.AddComponent<BoundingBox>();
            b.setBounds(box.Bounds);
        }
        
        AssetManager.Instance.registerObject(_seat);
    }

    public override void UnBindToParkitect(GameObject hider)
    {
        AssetManager.Instance.unregisterObject(_seat);
    }
#endif
}

