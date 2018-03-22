public class MotorTag : System.Attribute
{
    public string Name { get; set; }

    public MotorTag()
    {
    }

    public MotorTag(string name)
    {
        Name = name;
    }
}
