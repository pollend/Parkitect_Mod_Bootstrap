using System;
using System.Linq;

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
    public override void BindToParkitect()
    {
        BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
        GridDecorator gridDecorator = DecoratorByInstance<GridDecorator>();
        CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
        ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();


        Deco deco = Prefab.AddComponent<Deco>();
        deco.name = getKey;
        deco.heightChangeDelta = gridDecorator.heightDelta;
        deco.defaultGridSubdivision = gridDecorator.gridSubdivision;
        deco.buildOnGrid = gridDecorator.grid;
        deco.defaultSnapToGridCenter = gridDecorator.snap;
        deco.categoryTag = categoryDecorator.category;
        deco.price = baseDecorator.price;
        deco.setDisplayName(baseDecorator.InGameName);
        deco.dontSerialize = true;
        deco.isPreview = true;

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