using System.Collections.Generic;
using System.Xml.Linq;

public interface BaseProduct
{
#if UNITY_EDITOR
    void RenderInspectorGUI();
#endif
    
    List<XElement> Serialize();
    
    void DeSerialize(XElement element);
}
