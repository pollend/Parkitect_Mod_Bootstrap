using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Object = System.Object;

#if PARKITECT

public class ParkitectModBootstrap : IMod
{
    private ParkitectModBootstrap()
    {
        foreach (String folder in Directory.GetDirectories(GameController.modsPath))
        {
            String[] files = Directory.GetFiles(folder, "*.spark", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                ModManager.Instance.addMod(new ParkitectMod(folder), folder,
                    AbstractGameContent.ContentSource.USER_CREATED, 0);
            }
        }
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
