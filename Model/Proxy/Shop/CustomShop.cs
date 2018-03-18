
#if (PARKITECT)
public class CustomShop : ProductShop
{
    public override void Initialize()
    {
        this.gameObject.SetActive(true);

        base.Initialize();
    }

    public override ShopSettings getSettings()
    {
        //hack to get products to be configured
        if (products == null)
            Awake();

        return base.getSettings();
    }
}
#endif
