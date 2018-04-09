using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MiniJSON;
using Parkitect.Mods.AssetPacks;
using UnityEngine;
using Object = System.Object;

#if PARKITECT

public class ParkitectModBootstrap : IMod
{
    private List<ModPayload> _payloads;
    private GameObject _hider ;

    public ParkitectModBootstrap()
    {
    }


    public void onEnabled()
    {
        
        _payloads = new List<ModPayload>();
        
        _hider = new GameObject("hider");
        UnityEngine.Object.DontDestroyOnLoad(_hider);

        Debug.Log("------------------------Staring Bootstrap------------------------");
          
        foreach (var modEntry in ModManager.Instance.getModEntries())
        {
            if (modEntry.isActive())
            {

                String[] files = Directory.GetFiles(modEntry.path, "*.spark", SearchOption.TopDirectoryOnly);
                ModPayload payload = null;
                foreach (var sparkModFile in files)
                {
                    using (StreamReader reader = new StreamReader(sparkModFile))
                    {
                        payload = ScriptableObject.CreateInstance<ModPayload>();
                        payload.Deserialize((Dictionary<string, object>) Json.Deserialize(reader.ReadToEnd()));
                        break;

                    }
                }

                if (payload != null)
                {
                    Debug.Log("------------------------Loading Mod------------------------");
                    Debug.Log("Loading Mod From: " + modEntry.path);

                    AssetBundle assetBundle = AssetBundle.LoadFromFile(modEntry.path + "/assetbundle");

                    foreach (ParkitectObj obj in payload.ParkitectObjs)
                    {
                        Debug.Log("Loading Object:" + obj.Key);
                        obj.BindToParkitect(_hider, assetBundle);
                    }
                    assetBundle.Unload(false);

                    Debug.Log("------------------------Done Loading Mod------------------------");
                }

                _payloads.Add(payload);
            }

        }
        Debug.Log("------------------------Finished Bootstrap------------------------");
        _hider.SetActive(false);

    }

    public void onDisabled()
    {
        Debug.Log("------------------------Stopping Bootstrap------------------------");
        if (_payloads != null)
        {
            foreach (var payload in _payloads)
            {
                foreach (var parkitectObj in payload.ParkitectObjs)
                {
                    Debug.Log("unloading:" + parkitectObj.name);
                    parkitectObj.UnBindToParkitect(_hider);
                }

            }
        }

        if (_hider != (UnityEngine.Object)null)
        {
            UnityEngine.Object.Destroy(_hider);
        }

        Debug.Log("------------------------Stopping Bootstrap------------------------");
    }

    public string Name => "Parkitect Mod Bootstrap";
    public string Description => "bootstrapper used to run mods built by spark";
    public string Identifier => "ParkitectModBootstrap";

}
#endif
