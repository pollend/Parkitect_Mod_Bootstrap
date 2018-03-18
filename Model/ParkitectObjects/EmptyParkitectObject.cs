using System;

[ParkitectObjectTag("Empty")]
[Serializable]
public class EmptyParkitectObject  : ParkitectObj
{
    public EmptyParkitectObject()
    {
    }

    public override Type[] SupportedDecorators()
    {
        return new Type[]
        {
        };
    }
}
