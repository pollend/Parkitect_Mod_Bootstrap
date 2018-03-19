﻿ //============[ The component added to the flatRide gameobject ]============;

using System.Collections.Generic;
using UnityEngine;
#if PARKITECT

public class CustomFlatRide : FlatRide
{
    public SPPhase currentPhase;
    int phaseNum;
    public bool animating;
    public List<SPPhase> phases = new List<SPPhase>();
    public List<SPMotor> motors = new List<SPMotor>();
    private bool _show;
    private Rect _windowPosition = new Rect(20, 20, 350, 320);
    private Vector2 motorsScrollPos;

    public override void onOpenInfoWindow()
    {
        if (!GetComponent<BuildableObject>().isPreview && !UIUtility.isMouseOverUIElement() &&
            (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)))
            _show = true;
        base.onOpenInfoWindow();
    }


    public override void onStartRide()
    {
        foreach (SPMotor m in motors)
        {
            m.Reset(transform);
        }

        base.onStartRide();
        foreach (SPMotor m in motors)
        {
            m.Enter(this.transform);
        }

        if (phases.Count <= 0)
        {
            animating = false;
            return;
        }

        foreach (SPMotor m in motors)
        {
            m.Enter(transform);
        }

        animating = true;
        phaseNum = 0;
        currentPhase = phases[phaseNum];
        currentPhase.running = true;
        currentPhase.Enter();
        currentPhase.Run(transform);
    }

    public override void tick(StationController stationController)
    {
        if (currentPhase != null && animating)
        {
            currentPhase.Run(transform);
            if (!currentPhase.running)
            {
                NextPhase();
            }
        }

    }

    void NextPhase()
    {

        currentPhase.Exit();
        currentPhase.running = false;
        phaseNum++;
        if (phases.Count > phaseNum)
        {
            currentPhase = phases[phaseNum];
            currentPhase.running = true;
            currentPhase.Enter();
            currentPhase.Run(transform);
            return;
        }

        animating = false;

    }

    public override bool shouldLetGuestsOut(StationController stationController)
    {
        return !animating;
    }
}

#endif