using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Acuerdo.Plugin;
using Inscribe.Common;
using Inscribe.Communication.Posting;

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
            PostOffice.UpdateInjection.Injection((arg, next, last) =>
            {
                if (!arg.Item3.HasValue)
                {
                    try
                    {
                        var service = ServiceDictionary.First(kvp =>
                            arg.Item2.StartsWith(kvp.Key + ":", StringComparison.InvariantCultureIgnoreCase));

                        var address = string.Format(service.Value, Uri.EscapeDataString(arg.Item2.Substring(service.Key.Length + 1)));

                        Browser.Start(address);
                    }
                    catch { }
                }

                next.Invoke(arg);
            });
        }

        public IConfigurator ConfigurationInterface
        {
            get { return null; }
        }
    }
}
