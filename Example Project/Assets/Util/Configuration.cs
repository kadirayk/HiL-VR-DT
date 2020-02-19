using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Util
{

	class Configuration
	{
		private static Dictionary<String, String> conf;

		private static void init() {
			conf = new Dictionary<string, string>();
			string[] lines = System.IO.File.ReadAllLines("app.conf");
			foreach(string line in lines){
				string[] keyVal = line.Split(new string[] { "=" }, StringSplitOptions.None);
				conf.Add(keyVal[0].Trim(), keyVal[1].Trim());
			}
		}


		private Configuration()
		{
		}

		public static string getString(string key) {
			if (conf == null)
			{
				init();
			}
			return conf[key];
		}

		public static int getInt(string key)
		{
			Debug.Log("getInt");
			if (conf == null)
			{
				init();
			}
			return Convert.ToInt32(conf[key]);
		}
	}
}
