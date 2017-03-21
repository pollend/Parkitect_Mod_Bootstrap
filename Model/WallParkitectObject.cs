using System;

[ParkitectObjectTag("Wall")]
[Serializable]
public class WallParkitectObject : ParkitectObj
{
	public WallParkitectObject()
	{
	}

	public override Type[] SupportedDecorators()
	{
		return new Type[] {
            typeof(BaseDecorator),
            typeof(CategoryDecorator),
            typeof(ColorDecorator),
            typeof(BoundingBoxDecorator)
	    };
	}

#if (!UNITY_EDITOR)
    public override void BindToParkitect()
    {
        BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
        CategoryDecorator categoryDecorator = this.DecoratorByInstance<CategoryDecorator>();
        ColorDecorator colorDecorator = this.DecoratorByInstance<ColorDecorator>();
        BoundingBoxDecorator boxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();


        Wall wall = Prefab.AddComponent<Wall>();
        wall.name = this.getKey;
        wall.categoryTag = categoryDecorator.category;
        wall.price = baseDecorator.price;
        wall.setDisplayName(baseDecorator.InGameName);
        wall.dontSerialize = true;
        wall.isPreview = true;

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
