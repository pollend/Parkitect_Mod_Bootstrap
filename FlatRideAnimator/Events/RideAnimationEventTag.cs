public class RideAnimationEventTag : System.Attribute
{
    public string Name { get; set; }

    public RideAnimationEventTag()
    {
    }

    public RideAnimationEventTag(string name)
    {
        Name = name;
    }
}
