using System;

public class ParkitectObjectTag : System.Attribute
{
	public string Name { get; }

	public ParkitectObjectTag()
	{
	}
	public ParkitectObjectTag(string name)
	{
		Name = name;
	}
}


