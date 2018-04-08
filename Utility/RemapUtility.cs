using System.Linq;
using UnityEngine;

#if (PARKITECT)
public static class RemapUtility
{
    public static void RemapMaterials(GameObject go)
    {

        Renderer[] components = go.GetComponentsInChildren<Renderer>();
        foreach (var renderer in components)
        {
            Material[] sharedMaterails = renderer.sharedMaterials;
            for (int j = 0; j < sharedMaterails.Length; j++)
            {
                Material material = sharedMaterails[j];
                if (material != null)
                {
                    if (material.name.StartsWith("Diffuse"))
                    {
                        sharedMaterails[j] =
                            AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("Diffuse"));
                    }
                    else if (material.name.StartsWith("CustomColorsDiffuse"))
                    {
                        sharedMaterails[j] =
                            AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("CustomColorsDiffuse"));
                    }
                    else if (material.name.StartsWith("Specular"))
                    {
                        sharedMaterails[j] =
                            AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("Specular"));
                    }
                    else if (material.name.StartsWith("CustomColorsSpecular"))
                    {
                        sharedMaterails[j] =
                            AssetManager.Instance.objectMaterials.First(x => x.name.StartsWith("CustomColorsSpecular"));
                    }
                    else if (!material.name.StartsWith("SignTextMaterails") && !material.name.StartsWith("tv_image"))
                    {
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