using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starver
{
	public class StarverConfig
	{
		public string SavePath { get; set; }
		public int SaveInterval{ get; set; }

		private StarverConfig()
		{
			SavePath = Path.Combine(Environment.CurrentDirectory, "Starvers");
			SaveInterval = 60;
		}

		public static StarverConfig Read(string path)
		{
			if(!File.Exists(path))
			{
				var config = new StarverConfig();
				config.Save(path);
				return config;
			}
			return JsonConvert.DeserializeObject<StarverConfig>(File.ReadAllText(path));
		}
		public void Save(string path)
		{
			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(path, text);
		}
	}
}
