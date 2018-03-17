using System;
using System.Linq;

namespace Parkitect_Mod_Bootstrap.Model.ParkitectObjects
{
    
    [ParkitectObjectTag("Wall")]
    [Serializable]
    public class WallParkitectObject : ParkitectObj
    {
        public override Type[] SupportedDecorators()
        {
            return new[] {
                typeof(BaseDecorator),
                typeof(ShopDecorator)
            };
        }
#if (PARKITECT)
        public override void BindToParkitect()
        {
            BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
            CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
            ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
            BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();


            Wall wall = Prefab.AddComponent<Wall>();
            wall.name = getKey;
            wall.categoryTag = categoryDecorator.category;
            wall.price = baseDecorator.price;
            wall.setDisplayName(baseDecorator.InGameName);
            wall.dontSerialize = true;
            wall.isPreview = true;

            if (colorDecorator.isRecolorable)
            {
                CustomColors colors = Prefab.AddComponent<CustomColors>();
                colors.setColors(colorDecorator.colors.ToArray());
            }

            foreach (var box in boxDecorator.boundingBoxes)
            {
                var b = Prefab.AddComponent<BoundingBox>();
                b.setBounds(box.bounds);
            }

            base.BindToParkitect();
        }
#endif
    }
}