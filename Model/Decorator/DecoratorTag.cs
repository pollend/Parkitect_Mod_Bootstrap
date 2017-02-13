using System;
namespace Spark
{
	[Serializable]
	public class DecoratorTag : System.Attribute
	{
		public readonly string name;
		public string Name
		{
			get { return name; }
		}
		public DecoratorTag()
		{
		}
		public DecoratorTag(string name)
		{
			this.name = name;
		}
	}
}


