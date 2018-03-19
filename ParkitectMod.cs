﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class ParkitectMod : IMod
{
    private String name;
    private String description;
    private String identifier;

    private String path;

    private GameObject hider;
    private ModPayload _payload;
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

                XElement element = XElement.Load(reader);
                payload.Deserialize(element.Element("mod"));
                _payload = payload;
                break;

            }
        }
    }

    public void onEnabled()
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
                    BaseDecorator dec = obj.DecoratorByInstance<BaseDecorator>();
                    if (dec != null)
                        Debug.Log("---------------------------" + dec.InGameName + "---------------------------");
                    obj.BindToParkitect(hider,assetBundle);
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        

   

        Debug.Log("------------------------DONE LOADING MOD------------------------");


    }

    public void onDisabled()
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

    public string Name => _payload?.Identifier;
    public string Description => _payload?.Description;
    public string Identifier => _payload?.Identifier;
}