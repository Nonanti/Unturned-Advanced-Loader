using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nonantiy
{
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            Lisanslar = new List<string>
            {
                "XXX-XXX-XXX",
                "2XXX-XXX-XXX"
            };
        }
        [XmlArray("Lisanslar")]
        [XmlArrayItem("Lisans")]
        public List<string> Lisanslar = new List<string>();
    }
}