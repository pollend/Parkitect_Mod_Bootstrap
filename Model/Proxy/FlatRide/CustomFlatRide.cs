 //============[ The component added to the flatRide gameobject ]============;

using System.Collections.Generic;
using UnityEngine;

#if PARKITECT

public class CustomFlatRide : FlatRide
{

    private bool isAnimating = false;
    public override void onStartRide()
    {
        base.onStartRide();
        Animator animator =  GetComponent<Animator>();
        animator.SetTrigger("Play");
        isAnimating = true;
    }

    public override void tick(StationController stationController)
    {
        
        Animator animator =  GetComponent<Animator>();
        animator.Update(Time.deltaTime);
        if (animator.GetAnimatorTransitionInfo(0).IsName("Play -> Idle"))
            isAnimating = false;
    }

    public override bool shouldLetGuestsOut(StationController stationController)
    {
        return !isAnimating;
    }
}

#endif