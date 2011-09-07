using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Acuerdo.Plugin;
using Inscribe.Common;
using Inscribe.Storage;

namespace SearchAndPostPlugin
{
    [Export(typeof(IPlugin))]
    public class EntryPoint : IPlugin
    {
        public string Name
        {
            get { return "Search and Post plugin"; }
        }

        public double Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return double.Parse(version.Major + "." + version.Minor);
            }
        }

        public static Dictionary<string, string> ServiceDictionary = new Dictionary<string, string>()
        {
            { "Google", "http://www.google.com/search?hl=ja&q={0}" },
            { "Wikipedia", "http://ja.wikipedia.org/w/index.php?search={0}" },
            { "MSDN", "http://social.msdn.microsoft.com/Search/ja-JP?query={0}" }
        };

        public void Loaded()
        {
            NotifyStorage.NotifyTextChanged += (sender, e) =>
            {
                var match = Regex.Match(e.NotifyString,
                    @"^ツイートしました:@[a-zA-Z0-9_]+:\s*(?<service>.+?)\s*:\s*(?<query>.+)$");

                if (!match.Success) return;

                KeyValuePair<string, string> service;

                try
                {
                    service = ServiceDictionary.First(kvp =>
                        kvp.Key.Equals(match.Groups["service"].ToString(), StringComparison.InvariantCultureIgnoreCase));
                }
                catch (InvalidOperationException)
                {
                    return;
                }

                Browser.Start(string.Format(service.Value, Uri.EscapeDataString(match.Groups["query"].ToString())));
            };
        }

        public IConfigurator ConfigurationInterface
        {
            get { return null; }
        }
    }
}
