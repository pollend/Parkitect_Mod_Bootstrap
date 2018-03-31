using System.Linq;
using UnityEngine;

#if (PARKITECT)
public static class RemapUtility
{
    public static void RemapMaterials(GameObject go)
    {
        for (int i = 0; i <  AssetManager.Instance.objectMaterials.Length; i++)
        {
            UnityEngine.Debug.Log(AssetManager.Instance.objectMaterials[i].name);
            
        }
        
        Renderer[] components = go.GetComponentsInChildren<Renderer>();
        foreach (var renderer in components)
        {
            Material[] sharedMaterails = renderer.sharedMaterials;
            for (int j = 0; j < sharedMaterails.Length; j++)
            {
                Material material = sharedMaterails[j];
                if (material != null)
                {
                    Debug.Log("Material Name:" + material.name);
                    if (material.name.StartsWith("Diffuse"))
                    {
                        Debug.Log("Diffuse: " + renderer.gameObject.name);
                        sharedMaterails[j] = AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("Diffuse"));
                    }
                    else if (material.name.StartsWith("CustomColorsDiffuse"))
                    {
                        Debug.Log("CustomColorsDiffuse: " + renderer.gameObject.name);

                        sharedMaterails[j] = AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("CustomColorsDiffuse"));
                    }
                    else if (material.name.StartsWith("Specular"))
                    {
                        Debug.Log("Specular: " + renderer.gameObject.name);
                        sharedMaterails[j] = AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("Specular"));
                    }
                    else if (material.name.StartsWith("CustomColorsSpecular"))
                    {
                        Debug.Log("CustomColorsSpecular: " + renderer.gameObject.name);
                        sharedMaterails[j] = AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("CustomColorsSpecular"));
                    }
                    else if (!material.name.StartsWith("SignTextMaterails") && !material.name.StartsWith("tv_image"))
                    {
                        Debug.Log("SignTextMaterails: " + renderer.gameObject.name);
                        sharedMaterails[j] = material;
                        sharedMaterails[j].shader = AssetManager.Instance.diffuseMaterial.shader;
                    }
                }
            }

            renderer.sharedMaterials = sharedMaterails;
        }
    }
    
}
   
#endif