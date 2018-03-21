﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public override void BindToParkitect(GameObject hider, AssetBundle bundle)
        {
            base.BindToParkitect(hider,bundle);
            
            BaseDecorator baseDecorator = DecoratorByInstance<BaseDecorator>();
            CategoryDecorator categoryDecorator = DecoratorByInstance<CategoryDecorator>();
            ColorDecorator colorDecorator = DecoratorByInstance<ColorDecorator>();
            BoundingBoxDecorator boxDecorator = DecoratorByInstance<BoundingBoxDecorator>();

            GameObject go = Instantiate(bundle.LoadAsset<GameObject>(Key));
          
            Wall wall = go.AddComponent<Wall>();
            wall.name = Key;
            wall.categoryTag = categoryDecorator.category;
            wall.price = baseDecorator.Price;
            wall.setDisplayName(baseDecorator.InGameName);
            wall.dontSerialize = true;
            wall.isPreview = true;

            if (colorDecorator.isRecolorable)
            {
                CustomColors colors = go.AddComponent<CustomColors>();
                colors.setColors(colorDecorator.colors.ToArray());
            }

            foreach (var box in boxDecorator.boundingBoxes)
            {
                var b = go.AddComponent<BoundingBox>();
                b.setBounds(box.bounds);
            }

        }
#endif
    }
}