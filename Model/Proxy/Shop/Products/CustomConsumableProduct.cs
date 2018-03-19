#if (PARKITECT)
public class CustomConsumableProduct : ConsumableProduct
{
    public override void Initialize()
    {
        gameObject.SetActive(true);

        base.Initialize();
    }
}
#endif