using System;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;
#if (PARKITECT)
public class ParkitectMod : IMod
{
    private readonly String path ;

    private GameObject hider;
    private readonly ModPayload _payload;
    public String Path => path;

    public ParkitectMod()
    {
        
    }

   
    public ParkitectMod(String path)
    {
        this.path = path;
        String[] files = Directory.GetFiles(path, "*.spark", SearchOption.TopDirectoryOnly);
        foreach (var sparkModFile in files)
        {
            using (StreamReader reader = new StreamReader(sparkModFile))
            {
                ModPayload payload = ScriptableObject.CreateInstance<ModPayload>();

                payload.Deserialize((Dictionary<string, object>) Json.Deserialize(reader.ReadToEnd()));
                _payload = payload;
                break;

            }
        }
    }

    public void onEnabled()
    {
        if (path != null && _payload != null)
        {
            hider = new GameObject("hider");
            UnityEngine.Object.DontDestroyOnLoad(hider);

            Debug.Log("------------------------LOADING MOD------------------------");
            Debug.Log("Mod Path:" + path);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path + "/assetbundle");

            try
            {
                foreach (ParkitectObj obj in _payload.ParkitectObjs)
                {
                    Debug.Log("Loading Object:" + obj.Key);
                    obj.BindToParkitect(hider, assetBundle);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            assetBundle.Unload(false);

            Debug.Log("------------------------DONE LOADING MOD------------------------");
        }

    }

    public void onDisabled()
    {
        if (_payload != null)
        {
            try
            {
                foreach (ParkitectObj obj in _payload.ParkitectObjs)
                {
                    obj.UnBindToParkitect(hider);
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


    }

    public string Name
    {
        get
        {
            if (_payload == null)
                return null;
            return _payload.ModName;
        }
    }
    public string Description
    {
        get
        {
            if (_payload == null)
                return null;
            return _payload.Description;
        }
    }

    public string Identifier
    {
        get
        {
            if (_payload == null)
                return null;
            return _payload.ModName;
        }
    }

}
#endif