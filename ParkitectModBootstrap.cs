using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Object = System.Object;

#if PARKITECT
    
public class ParkitectModBootstrap : IMod
{
 
    private GameObject hider;
    private List<SerializedMonoBehaviour> registeredObjects = new List<SerializedMonoBehaviour>();
    
    private ParkitectModBootstrap()
    {

    }


    public void onEnabled()
    {
        hider = new GameObject("hider");
        UnityEngine.Object.DontDestroyOnLoad(hider);
        
        foreach (String folder in Directory.GetDirectories(GameController.modsPath))
        {
            String[] files = Directory.GetFiles(folder, "*.spark", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                foreach (var sparkModFile in files)
                {
                    using (StreamReader reader = new StreamReader(sparkModFile))
                    {
                        Debug.Log("------------------------LOADING MOD------------------------");
                        Debug.Log("Mod Path:" + folder);

                        AssetBundle assetBundle = AssetBundle.LoadFromFile(folder + "/assetbundle");
                        
                        try
                        {
                            ModPayload payload = new ModPayload();
                            payload.Bundle = assetBundle;

                            XElement element = XElement.Load(reader);
                            payload.Deserialize(element.Element("mod"),assetBundle);
                            
                            foreach(ParkitectObj obj in payload.ParkitectObjs)
                            {
                                BaseDecorator dec = obj.DecoratorByInstance<BaseDecorator>();
                                if (dec != null)
                                    Debug.Log("---------------------------" + dec.InGameName + "---------------------------");
                                obj.BindToParkitect(hider,registeredObjects);
                            }
                                
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                        
                        Debug.Log("------------------------DONE LOADING MOD------------------------");
                    }
                }
                
            }
        }

      /*  try
        {
            AssetBundle bundle;
            using (WWW www = new WWW("file://" + modFolder + "/data/" + assetbundle))
            {

                if (www.error != null)
                {
                    Debug.Log("Loading had an error:" + www.error);
                    throw new Exception("Loading had an error:" + www.error);
                }

                if (www.assetBundle == null)
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
            Debug.LogException(e);
        }*/

    }

    public void onDisabled()
    {
    }

    public string Name => "Parkitect Mod Bootsrap";
    public string Description => "bootstrapper used to run mods built by spark";
    public string Identifier => "ParkitectModBootsrap";
}
#endif
