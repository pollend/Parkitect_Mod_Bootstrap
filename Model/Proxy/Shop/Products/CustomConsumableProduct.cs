#if (PARKITECT)
public class CustomConsumableProduct : ConsumableProduct
{
    public CustomConsumableProduct()
    {

    }
    public override void Initialize()
    {
        this.gameObject.SetActive(true);

        base.Initialize();
    }
}
#endif