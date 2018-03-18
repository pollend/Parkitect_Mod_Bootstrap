using System;
using System.Collections.Generic;
using UnityEngine;


[ParkitectObjectTag("FlatRide")]
[Serializable]
public class FlatRideParkitectObject : ParkitectObj
{
	public FlatRideParkitectObject()
	{
	}

	public override Type[] SupportedDecorators()
	{
		return new Type[]
		{
			typeof(BaseDecorator),
			typeof(WaypointDecorator),
			typeof(FlatrideDecorator),
			typeof(BoundingBoxDecorator),
			typeof(AnimatorDecorator),
			typeof(SeatDecorator)
		};
	}
#if (PARKITECT)
	public override void BindToParkitect(GameObject hider,List<SerializedMonoBehaviour> register)
	{
		BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
		WaypointDecorator waypointDecorator = this.DecoratorByInstance<WaypointDecorator>();
		FlatrideDecorator flatrideDecorator = this.DecoratorByInstance<FlatrideDecorator>();
		BoundingBoxDecorator boundingBoxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();
		AnimatorDecorator animatorDecorator = this.DecoratorByInstance<AnimatorDecorator>();

		base.BindToParkitect(hider,register);
	}
#endif

}


