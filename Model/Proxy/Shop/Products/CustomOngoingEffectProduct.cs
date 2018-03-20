#if (PARKITECT)
public class CustomOngoingEffectProduct : OngoingEffectProduct
{
    public CustomOngoingEffectProduct()
    {
    }

    public override void Initialize()
    {
        this.gameObject.SetActive(true);

        base.Initialize();
    }

}
#endif
