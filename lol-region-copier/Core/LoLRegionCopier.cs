// This file is a part of lol-region-copier, licensed under the MIT License.
//
// Copyright (c) Jakub Zagórski (jaqobb)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace lol_region_copier.Core
{
	public class LoLRegionCopier
	{
		public static readonly string SettingsFile = Path.Combine("settings.json");

		public void Start()
		{
			if (!File.Exists(SettingsFile))
			{
				Console.WriteLine("Could not find settings file.");
				Console.ReadKey();
				return;
			}
			JObject settings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(SettingsFile));
			if (!settings.ContainsKey("origin"))
			{
				Console.WriteLine("Could not find 'origin' property.");
				Console.ReadKey();
				return;
			}
			if (!settings.ContainsKey("target"))
			{
				Console.WriteLine("Could not find 'target' property.");
				Console.ReadKey();
				return;
			}
			JObject origin = settings.GetValue("origin").Value<JObject>();
			if (!origin.ContainsKey("location"))
			{
				Console.WriteLine("Could not find 'location' property inside 'origin' property.");
				Console.ReadKey();
				return;
			}
			if (!origin.ContainsKey("region"))
			{
				Console.WriteLine("Could not find 'region' property inside 'origin' property.");
				Console.ReadKey();
				return;
			}
			JObject target = settings.GetValue("target").Value<JObject>();
			if (!target.ContainsKey("location"))
			{
				Console.WriteLine("Could not find 'location' property inside 'target' property.");
				Console.ReadKey();
				return;
			}
			if (!target.ContainsKey("region"))
			{
				Console.WriteLine("Could not find 'region' property inside 'target' property.");
				Console.ReadKey();
				return;
			}
			string originLocation = origin.GetValue("location").Value<string>();
			if (!Directory.Exists(originLocation))
			{
				Console.WriteLine("'" + originLocation + "' directory does not exist.");
				Console.ReadKey();
				return;
			}
			int originVersionNumber = -1;
			string originVersionLocation = "";
			foreach (string file in Directory.GetFileSystemEntries(originLocation))
			{
				string fileName = Path.GetFileName(file);
				string[] fileNameData = fileName.Split('.');
				if (fileNameData.Length != 4)
				{
					continue;
				}
				int version = int.Parse(fileNameData.GetValue(3) as string);
				if (version > originVersionNumber)
				{
					originVersionNumber = version;
					originVersionLocation = file;
				}
			}
			string originFile = Path.Combine(originVersionLocation, "deploy", "system.yaml");
			if (!File.Exists(originFile))
			{
				Console.WriteLine("'" + originFile + "' file does not exist.");
				Console.ReadKey();
				return;
			}
			string targetLocation = target.GetValue("location").Value<string>();
			if (!Directory.Exists(targetLocation))
			{
				Console.WriteLine("'" + targetLocation + "' directory does not exist.");
				Console.ReadKey();
				return;
			}
			int targetVersionNumber = -1;
			string targetVersionLocation = "";
			foreach (string file in Directory.GetFileSystemEntries(targetLocation))
			{
				string fileName = Path.GetFileName(file);
				string[] fileNameData = fileName.Split('.');
				if (fileNameData.Length != 4)
				{
					continue;
				}
				int version = int.Parse(fileNameData.GetValue(3) as string);
				if (version > targetVersionNumber)
				{
					targetVersionNumber = version;
					targetVersionLocation = file;
				}
			}
			string targetFile = Path.Combine(targetVersionLocation, "deploy", "system.yaml");
			if (!File.Exists(targetFile))
			{
				Console.WriteLine("'" + targetFile + "' file does not exist.");
				Console.ReadKey();
				return;
			}
			string originRegion = origin.GetValue("region").Value<string>().ToUpper();
			string targetRegion = target.GetValue("region").Value<string>().ToUpper();
			if (targetRegion.Equals(originRegion))
			{
				Console.WriteLine("Target region can not be the same as the origin region.");
				Console.ReadKey();
				return;
			}
			Serializer serializer = new Serializer();
			IDictionary<object, object> originSettings = serializer.Deserialize<IDictionary<object, object>>(File.ReadAllText(originFile));
			if (!originSettings.ContainsKey("region_data"))
			{
				Console.WriteLine("Origin file does not contain 'region_data' key.");
				Console.ReadKey();
				return;
			}
			IDictionary<object, object> originRegionList = originSettings["region_data"] as IDictionary<object, object>;
			if (!originRegionList.ContainsKey(originRegion))
			{
				Console.WriteLine("Origin file does not contain '" + originRegion + "' region data.");
				Console.ReadKey();
				return;
			}
			IDictionary<object, object> targetSettings = serializer.Deserialize<IDictionary<object, object>>(File.ReadAllText(targetFile));
			if (!targetSettings.ContainsKey("region_data"))
			{
				Console.WriteLine("Target file does not contain 'region_data' key.");
				Console.ReadKey();
				return;
			}
			IDictionary<object, object> targetRegionList = targetSettings["region_data"] as IDictionary<object, object>;
			if (!targetRegionList.ContainsKey(targetRegion))
			{
				Console.WriteLine("Target file does not contain '" + targetRegion + "' region data.");
				Console.ReadKey();
				return;
			}
			IDictionary<object, object> originRegionData = originRegionList[originRegion] as IDictionary<object, object>;
			IDictionary<object, object> targetRegionData = targetRegionList[targetRegion] as IDictionary<object, object>;
			foreach (object key in originRegionData.Keys)
			{
				if (key.Equals("available_locales") || key.Equals("default_locale"))
				{
					continue;
				}
				targetRegionData[key] = originRegionData[key];
			}
			serializer.Serialize(File.Create(targetFile), targetSettings);
			Console.WriteLine("Region copied.");
			Console.ReadKey();
		}
	}
}