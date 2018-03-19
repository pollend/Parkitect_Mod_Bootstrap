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
	private CustomFlatRide _flatRide;
	public override void BindToParkitect(GameObject hider, AssetBundle bundle)
	{
		BaseDecorator baseDecorator = this.DecoratorByInstance<BaseDecorator>();
		WaypointDecorator waypointDecorator = this.DecoratorByInstance<WaypointDecorator>();
		FlatrideDecorator flatrideDecorator = this.DecoratorByInstance<FlatrideDecorator>();
		BoundingBoxDecorator boundingBoxDecorator = this.DecoratorByInstance<BoundingBoxDecorator>();
		AnimatorDecorator animatorDecorator = this.DecoratorByInstance<AnimatorDecorator>();

		GameObject gameObject = Instantiate(bundle.LoadAsset<GameObject>(Key));

		waypointDecorator.Decorate(gameObject, hider, this, bundle);
		flatrideDecorator.Decorate(gameObject, hider, this, bundle);


		CustomFlatRide flatride = gameObject.AddComponent<CustomFlatRide>();
		_flatRide = flatride;
		_flatRide.name = Key;
		flatride.xSize = flatrideDecorator.XSize;
		flatride.zSize = flatrideDecorator.ZSize;
		flatride.excitementRating = flatrideDecorator.Excitement;
		flatride.intensityRating = flatrideDecorator.Intensity;
		flatride.nauseaRating = flatrideDecorator.Nausea;


		//Setup Animation
		flatride.motors = new List<SPMotor>(animatorDecorator.Motors);
		flatride.phases = new List<SPPhase>(animatorDecorator.Phases);


		//Basic FlatRide Settings
		flatride.fenceStyle = AssetManager.Instance.rideFenceStyles.rideFenceStyles[0].identifier;
		flatride.entranceGO = Extra.FlatRideEntrance(flatride.gameObject);
		flatride.exitGO = AssetManager.Instance.attractionExitGO;
		flatride.categoryTag = "Attractions/Flat Ride";
		flatride.defaultEntranceFee = 1f;
		flatride.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
		AssetManager.Instance.registerObject(_flatRide);
	}

	public override void UnBindToParkitect(GameObject hider)
	{
		AssetManager.Instance.unregisterObject(_flatRide);
	}
#endif

}


