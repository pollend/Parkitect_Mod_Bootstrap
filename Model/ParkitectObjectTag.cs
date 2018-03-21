using System;

public class ParkitectObjectTag : System.Attribute
{
	public string Name { get; set; }

	public ParkitectObjectTag()
	{
	}
	public ParkitectObjectTag(string name)
	{
		Name = name;
	}
}


