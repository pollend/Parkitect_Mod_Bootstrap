using System;

[ParkitectObjectTag("Shop")]
[Serializable]
public class ShopParkitectObject : ParkitectObj
{
	public override Type[] SupportedDecorators()
	{
		return new[] {
            typeof(BaseDecorator),
            typeof(ShopDecorator)
		};
	}
#if (PARKITECT)
	public override void BindToParkitect()
	{
		var baseDecorator = DecoratorByInstance<BaseDecorator>();
		var shopDecorator = DecoratorByInstance<ShopDecorator>();

		var shop = Prefab.AddComponent<CustomShop>();

		foreach (var product in shopDecorator.products)
		{


		}

		base.BindToParkitect();
	}
#endif
}



