using System;

[ParkitectObjectTag("Shop")]
[Serializable]
public class ShopParkitectObject : ParkitectObj
{
	public ShopParkitectObject()
	{
	}

	public override Type[] SupportedDecorators()
	{
		return new Type[] {
            typeof(BaseDecorator),
            typeof(ShopDecorator)
		};
	}

#if (!UNITY_EDITOR)
    public override void BindToParkitect()
    {
        var baseDecorator = this.DecoratorByInstance<BaseDecorator>();
        var shopDecorator = this.DecoratorByInstance<ShopDecorator>();

        var shop = Prefab.AddComponent<CustomShop>();

        foreach (var product in shopDecorator.products)
        {


        }

        base.BindToParkitect();
    }
#endif
}



