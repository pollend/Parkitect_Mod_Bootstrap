using System.Collections.Generic;
using UnityEngine;

#if (PARKITECT)
public static class Extra
{

    public static GameObject Fence(GameObject Parent)
    {
        List<Transform> list = new List<Transform>();
        Utility.RecursiveFindTransformsStartingWith("Fence", Parent.transform, list);
        if (list.Count > 0)
        {
            return list[0].gameObject;
        }
        else
        {
            return AssetManager.Instance.rideFenceGO;
        }
    }
    public static Entrance FlatRideEntrance(GameObject Parent)
    {
        List<Transform> list = new List<Transform>();
        Utility.RecursiveFindTransformsStartingWith("Entrance", Parent.transform, list);
        if (list.Count > 0)
        {
            Entrance E = GameObject.Instantiate(AssetManager.Instance.attractionEntranceGO);
            E.boothGO = list[0].gameObject;
            return E;
        }
        else
        {
            return AssetManager.Instance.attractionEntranceGO;
        }
    }
    public static void FixSeats(GameObject Parent)
    {
        List<Transform> list = new List<Transform>();
        Utility.RecursiveFindTransformsStartingWith("Seat", Parent.transform, list);
        foreach (var item in list)
        {
            item.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public static void BasicFlatRideSetup(FlatRide flatRide)
    {
        flatRide.fenceStyle = AssetManager.Instance.rideFenceStyles.rideFenceStyles[0].identifier;
        flatRide.entranceGO = Extra.FlatRideEntrance(flatRide.gameObject);
        flatRide.exitGO = AssetManager.Instance.attractionExitGO;
        flatRide.categoryTag = "Attractions/Flat Ride";
        flatRide.defaultEntranceFee = 1f;
        flatRide.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
    }
}
#endif