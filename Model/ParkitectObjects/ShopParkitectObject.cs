using System;
using System.Collections.Generic;
using UnityEngine;

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
	private CustomShop _shop;
	public override void BindToParkitect(GameObject hider, AssetBundle bundle)
	{

		var baseDecorator = DecoratorByInstance<BaseDecorator>();
		var shopDecorator = DecoratorByInstance<ShopDecorator>();

		GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));
		gameObject.transform.parent = hider.transform;

		shopDecorator.Decorate(gameObject, hider, this, bundle);
		baseDecorator.Decorate(gameObject, hider, this, bundle);

		CustomShop customShop = gameObject.GetComponent<CustomShop>();
		_shop = customShop;
		_shop.name = Key;
		AssetManager.Instance.registerObject(_shop);

		foreach (var prod in _shop.products)
		{
			AssetManager.Instance.registerObject(prod);
		}
	}

	public override void UnBindToParkitect(GameObject hider)
	{
		foreach (var prod in _shop.products)
		{
			AssetManager.Instance.unregisterObject(prod);
		}
		AssetManager.Instance.unregisterObject(_shop);
	}
#endif
}



