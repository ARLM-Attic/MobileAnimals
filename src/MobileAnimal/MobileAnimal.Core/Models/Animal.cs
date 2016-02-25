using System;
using Newtonsoft.Json;

namespace MobileAnimal.Core
{
	public class Animal
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name {
			get;
			set;
		}
	}
}

