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
    }


    private ParkitectMod tryLoadPath(String folder)
    {
        String[] files = Directory.GetFiles(folder, "*.spark", SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
        {
            Debug.Log("Loading Mod From: " + folder);
            try
            {
                ParkitectMod mod = new ParkitectMod(folder);              
                return mod;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        
        return null;
    }

    public void onEnabled()
    {
        Debug.Log("------------------------Staring Bootstrap------------------------");

        List<ModManager.ModEntry> toRemove = new List<ModManager.ModEntry>();
        List<ParkitectMod> toAdd = new List<ParkitectMod>();

        foreach (var modEntry in ModManager.Instance.getModEntries())
        {
            if (!(modEntry.mod is ParkitectMod))
            {
                Debug.Log(modEntry.path);
                ParkitectMod mod = tryLoadPath(modEntry.path);
                if (mod != null)
                {
                    toAdd.Add(mod);
                    toRemove.Add(modEntry);
                }
            }

        }
        Debug.Log("------------------------Finished Bootstrap------------------------");

        foreach (var mod in toRemove)
        {
            ModManager.Instance.removeMod(mod);   
        }
        foreach (var mod in toAdd)
        {
            ModManager.Instance.addMod(mod, mod.Path, AbstractGameContent.ContentSource.USER_CREATED, 0);
            mod.onEnabled();

        }
    }

    public void onDisabled()
    {
    }

    public string Name => "Parkitect Mod Bootstrap";
    public string Description => "bootstrapper used to run mods built by spark";
    public string Identifier => "ParkitectModBootstrap";
}
#endif
