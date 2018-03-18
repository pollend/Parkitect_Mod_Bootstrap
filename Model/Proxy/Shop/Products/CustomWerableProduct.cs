#if (PARKITECT)
public class CustomWerableProduct : WearableProduct
{

    public CustomWerableProduct()
    {

    }

    public override void Initialize()
    {
        this.gameObject.SetActive(true);

        base.Initialize();
    }
}
#endif