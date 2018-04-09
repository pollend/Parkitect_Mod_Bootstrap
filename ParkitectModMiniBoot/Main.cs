using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniJSON;

public class Main : IMod
{
    private String name = "MiniBoot";
    private String description = null;
    private String identifier = null;

    public string Path
    {
        get { return ModManager.Instance.getModEntries().First(x => x.mod == this).path; }
    }

    public void deserialize()
    {
        String[] files = Directory.GetFiles(Path, "*.spark", SearchOption.TopDirectoryOnly);
        foreach (var sparkModFile in files)
        {
            using (StreamReader reader = new StreamReader(sparkModFile))
            {

                Dictionary<string, object> entries = (Dictionary<string, object>) Json.Deserialize(reader.ReadToEnd());
                name = entries["ModName"] as string;
                description = entries["Description"] as string;
                break;

            }
        }

    }

    public void onEnabled()
    {
    }

    public void onDisabled()
    {
    }


    public string Name
    {
        get
        {
            if (name == null)
                deserialize();
            return name;
        }
    }

    public string Description
    {
        get
        {
            if (description == null)
                deserialize();
            return description;
        }
    }

    public string Identifier {
        get
        {
            return "Parkitect-Spark-"+ name;
        }
    }

}