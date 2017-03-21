using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class ParkitectModBootstrap
{
    #if (!UNITY_EDITOR)
        private static ParkitectModBootstrap instance;

    public static ParkitectModBootstrap Instance
    {
        get
        {
            if(instance == null)
                instance = new ParkitectModBootstrap();
            return instance;
        }
    }

    private ParkitectModBootstrap()
    {

    }

    public void RegisterMod(String modFolder,String assetbundle)
    {
        UnityEngine.Debug.Log("------------------------LOADING MOD------------------------");
        UnityEngine.Debug.Log("Mod Path:" + modFolder);

        try
        {
            AssetBundle bundle;
            using (WWW www = new WWW("file://" + modFolder + "/data/"+ assetbundle ))
            {

                if (www.error != null)
                {
                    Debug.Log("Loading had an error:" + www.error);
                    throw new Exception("Loading had an error:" + www.error);
                }
                if(www.assetBundle == null)
                {
                    Debug.Log("Loading had an error:" + www.error);
                    throw new Exception("assetBundle is null");

                }
                bundle = www.assetBundle;
            }

            ModPayload payload = new ModPayload();
            payload.Bundle = bundle;
            using (StreamReader reader = new StreamReader(modFolder + "/data/mod.xml"))
            {
                XElement element = XElement.Load(reader);
                payload.Deserialize(element.Element("mod"));
            }
            payload.bind();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        UnityEngine.Debug.Log("------------------------DONE LOADING MOD------------------------");
    }

    #endif
}
