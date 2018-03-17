using System;

namespace Parkitect_Mod_Bootstrap.Model.ParkitectObjects
{
    
    [ParkitectObjectTag("Wall")]
    [Serializable]
    public class WallParkitectObject : ParkitectObj
    {
        public WallParkitectObject()
        {
        }

        public override Type[] SupportedDecorators()
        {
            return new Type[] {
                typeof(BaseDecorator),
                typeof(ShopDecorator)
            };
        }
    }
}