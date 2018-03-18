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
    public override void BindToParkitect(GameObject hider,List<SerializedMonoBehaviour> register)
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        SeatDecorator seatDecorator= DecoratorByInstance<SeatDecorator>();
        ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();

        
        GameObject gameObject = Instantiate(Prefab);
        gameObject.transform.parent = hider.transform;

        Seating seat = gameObject.AddComponent<Seating>();
        seat.name = Key;
        seat.setDisplayName(baseDecorator.InGameName);
        seat.price = baseDecorator.price;
        seat.categoryTag = "Patch Attachments/Benches";
        
        
        RemapUtility.RemapMaterials(gameObject);

        baseDecorator.Decorate(gameObject, hider, this,register);
        colorDecorator.Decorate(gameObject, hider, this,register);
        categoryDecorator.Decorate(gameObject,hider,this,register);

        if (colorDecorator.isRecolorable)
        {
            CustomColors colors = Prefab.AddComponent<CustomColors>();
            colors.setColors(colorDecorator.colors.ToArray());
        }

        foreach (var box in boxDecorator.boundingBoxes)
        {
            var b = Prefab.AddComponent<BoundingBox>();
            b.setBounds(box.bounds);
        }
        base.BindToParkitect(hider,register);
    }
#endif
}

