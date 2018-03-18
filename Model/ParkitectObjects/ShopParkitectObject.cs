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
	public override void BindToParkitect(GameObject hider,List<SerializedMonoBehaviour> register)
	{
		var baseDecorator = DecoratorByInstance<BaseDecorator>();
		var shopDecorator = DecoratorByInstance<ShopDecorator>();

		
		GameObject gameObject = Instantiate(Prefab);
		gameObject.transform.parent = hider.transform;
		
		shopDecorator.Decorate(gameObject,hider,this,register);

		base.BindToParkitect(hider,register);
	}
#endif
}



