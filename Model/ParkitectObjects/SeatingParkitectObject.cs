using System;

[ParkitectObjectTag("Seating")]
[Serializable]
public class SeatingParkitectObject : ParkitectObj
{
    public SeatingParkitectObject()
    {
    }

    public override Type[] SupportedDecorators()
    {
        return new Type[]{
            typeof(BaseDecorator),
            typeof(SeatDecorator),
            typeof(ColorDecorator),
            typeof(BoundingBoxDecorator)
        };
    }
    
#if (!UNITY_EDITOR)
    public override void BindToParkitect()
    {
        BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
        SeatDecorator seatDecorator= this.DecoratorByInstance<SeatDecorator>();
        ColorDecorator colorDecorator = this.DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();

        Seating seat = Prefab.AddComponent<Seating>();
        seat.name = this.getKey;
        seat.setDisplayName(baseDecorator.InGameName);
        seat.price = baseDecorator.price;
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
        base.BindToParkitect();
    }
#endif
}

