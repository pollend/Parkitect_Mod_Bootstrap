using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = System.Object;

#if PARKITECT

public class ParkitectModBootstrap : IMod
{
    public ParkitectModBootstrap()
    {
        Debug.Log("------------------------Staring Bootstrap------------------------");
        
        foreach (String folder in Directory.GetDirectories(GameController.modsPath))
        {
            String[] files = Directory.GetFiles(folder, "*.spark", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                Debug.Log("------------------------Adding Mod------------------------");
                Debug.Log("Loading Mod From: " + folder);
                try
                {
                    ModManager.Instance.addMod(new ParkitectMod(folder), folder,
                        AbstractGameContent.ContentSource.USER_CREATED, 0);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                Debug.Log("------------------------Finished Adding Mod------------------------");


            }
        }
        Debug.Log("------------------------Finished Bootstrap------------------------");
    }


    public void onEnabled()
    {
        

    }

    public void onDisabled()
    {
    }

    public string Name => "Parkitect Mod Bootsrap";
    public string Description => "bootstrapper used to run mods built by spark";
    public string Identifier => "ParkitectModBootsrap";
}
#endif
