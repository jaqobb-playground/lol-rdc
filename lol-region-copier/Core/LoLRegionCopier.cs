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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lol_region_copier.Core
{
	public class LoLRegionCopier
	{
		public static readonly string SettingsFile = Path.Combine("settings.json");

		public void Start()
		{
			Console.WriteLine("Copying region...");
			if (!File.Exists(SettingsFile))
			{
				Console.WriteLine("Could not find settings file.");
				Console.ReadKey();
				return;
			}
			JObject json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(SettingsFile));
			if (!json.ContainsKey("origin"))
			{
				Console.WriteLine("Could not find 'origin' property.");
				Console.ReadKey();
				return;
			}
			if (!json.ContainsKey("target"))
			{
				Console.WriteLine("Could not find 'target' property.");
				Console.ReadKey();
				return;
			}
			string origin = json.GetValue("origin").Value<string>();
			string target = json.GetValue("target").Value<string>();
			Console.WriteLine("Region copied.");
			Console.ReadKey();
		}
	}
}