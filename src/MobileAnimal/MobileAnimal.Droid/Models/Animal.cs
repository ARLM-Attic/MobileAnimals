using System;
using Newtonsoft.Json;

namespace MobileAnimal.Droid
{
    public class Animal
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        //		[JsonProperty(PropertyName = "userid")]
        //		public string UserId {
        //			get;
        //			set;
        //		}
    }
}

