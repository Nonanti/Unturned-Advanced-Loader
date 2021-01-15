using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using System.Net;
using UnityEngine;
using System.Reflection;
using Rocket.Core.Commands;
using System.IO;
using Newtonsoft.Json;
using Rocket.Unturned.Chat;
using SDG.Unturned;

namespace Nonantiy
{
    public class Main : RocketPlugin<Config>
    {
        public List<lisanslar> Plugin = new List<lisanslar>();
        public List<string> AktifLisanslar = new List<string>();
        protected override void Load()
        {
            LoadPlugin();
        }
        //Strazen https://github.com/Strazen/Unturned-RocketMod-Loader/blob/main/Main.cs
        private static string ServerIp()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/%22");
            using (WebResponse response = request.GetResponse())

            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            return address;
        }
        
        public void LoadPlugin()
        {
            try
            {
                WebClient webClient = new WebClient();
                
                if (!Configuration.Instance.Lisanslar.Contains("XXX-XXX-XXX") || !Configuration.Instance.Lisanslar.Contains("2XXX-XXX-XXX"))
                {
                    string ek = ".dll";
                    Console.WriteLine("Loader Yuklendi!", Console.ForegroundColor = ConsoleColor.Green);
                    Console.WriteLine("Suanda {Configuration.Instance.Licenses.Count} lisan kullaniliyor.", Console.ForegroundColor = ConsoleColor.Green);
                    Console.WriteLine("Made by Zeus Plugins", Console.ForegroundColor = ConsoleColor.Green);

                    for (int i = 0; i < Configuration.Instance.Lisanslar.Count; i++)
                    {
                        
                        var rawByte = webClient.DownloadData($"https://github.com/Nonantiy/deqwada/raw/main/{Configuration.Instance.Lisanslar[i]}{ek}");
                        foreach (Type type in RocketHelper.GetTypesFromInterface(Assembly.Load(rawByte), "IRocketPlugin"))
                        {
                            GameObject gameObject = new GameObject(type.Name, new Type[]
                            {
                                type
                            });
                            AktifLisanslar.Add(type.Assembly.FullName);
                            UnityEngine.Object.DontDestroyOnLoad(gameObject);
                            Console.WriteLine(type.Name + " Yuklendi.", Console.ForegroundColor = ConsoleColor.Cyan);
                            string webhook = "webhook";
                            WebRequest wr = (HttpWebRequest) WebRequest.Create(webhook);
                            
                            wr.ContentType = "application/json";
                            wr.Method = "POST";
                            var sunucuismi = Provider.serverName;
                            var Sunucuport = Provider.port;

                            using (var sw = new StreamWriter(wr.GetRequestStream()))
                            {
                                string json = JsonConvert.SerializeObject(new
                                {
                                    username = "Loader Mod",
                                    embeds = new[]
                                    {
                                        new
                                        {
                                            description = "Loader desc",
                                            title = "\nSunucun ismi: " + sunucuismi+
                                                    "\nSunucu Ip: " + ServerIp() +
                                                    "\nSunucu Portu: " + Sunucuport +
                                                    "\n Sunucu Aktif Edilen Eklenti: " + type.Assembly.FullName +
                                                    "\nAktif Edilen Lisans: " +  Configuration.Instance.Lisanslar[i],
                                        }
                                        
                                    }
                                });
                            }
                            var BlackList = webClient.DownloadString("BlackList");
                            if (BlackList.Contains(ServerIp()))
                            {
                                Plugin.Clear();
                                Unload();
                            }
                            var WhiteList = webClient.DownloadString("WhiteList");
                            if (WhiteList.Contains(ServerIp()))
                            {
                                Console.WriteLine("WhiteList algilandi!", Console.ForegroundColor = ConsoleColor.Cyan);
                            }
                            else
                            {
                                Plugin.Clear();
                                Unload();
                            }
                        }
                        
                    }
                }
                else
                {

                    Console.WriteLine(" Loader Yuklendi Ancak Icerisindeki 2 Hazir Lisansi Sil!", Console.ForegroundColor = ConsoleColor.Cyan);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Lisanslar yuklenemedi.", Console.ForegroundColor = ConsoleColor.Cyan);
                throw;
            }
        }

        public void Send(string content, string webhookUrl, string username)
        {
            WebClient wc = new WebClient();

            wc.UploadValues(webhookUrl, new NameValueCollection
            {
                {
                    "content", content
                },
                {
                    "username", username
                }
            });
        }
        
        protected override void Unload()
        {
            Console.WriteLine("Pluginler yuklenemedi.", Console.ForegroundColor = ConsoleColor.Red);
        }

        public static Main Instance;
        [RocketCommand("pluginreload", "Pluginleri tekrar yükler", "pluginreload", (AllowedCaller)1)]
        public void PluginReload(IRocketPlayer caller, string[] parametre)
        {
            if (caller.IsAdmin)
            {
                Send($"Sunucu ip adresi: {ServerIp()} \nSunucu adı: {Provider.serverName}", "", $"Eklentiler reload edildi, sahip olunan eklentiler listeleniyor.");
                foreach (var plugin in AktifLisanslar)
                {
                    Send($"LIsans: {plugin}", "Webhook", $"Lisans.");
                }
                try
                {
                    WebClient webClient = new WebClient();
                    if (!Configuration.Instance.Lisanslar.Contains("XXX-XXX-XXX") || !Configuration.Instance.Lisanslar.Contains("2XXX-XXX-XXX"))
                    {
                        string ek = ".dll";
                        Console.WriteLine("Basariyla pluginler geri yuklendi!", Console.ForegroundColor = ConsoleColor.Green);

                        for (int i = 0; i < Configuration.Instance.Lisanslar.Count; i++)
                        {
                            var rawByte = webClient.DownloadData(
                                $"https://github.com/Nonantiy/deqwada/raw/main/{Configuration.Instance.Lisanslar[i]}{ek}");
                            foreach (Type type in RocketHelper.GetTypesFromInterface(Assembly.Load(rawByte),
                                "IRocketPlugin"))
                            {
                                GameObject gameObject = new GameObject(type.Name, new Type[]
                                {
                                    type
                                });

                                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                                Console.WriteLine(type.Name + " Yuklendi.",
                                    Console.ForegroundColor = ConsoleColor.Cyan);
                                string webhook = "webhook";
                                WebRequest wr = (HttpWebRequest) WebRequest.Create(webhook);

                                wr.ContentType = "application/json";
                                wr.Method = "POST";
                                var sunucuismi = Provider.serverName;
                                var Sunucuport = Provider.port;
                                using (var sw = new StreamWriter(wr.GetRequestStream()))
                                {
                                    string json = JsonConvert.SerializeObject(new
                                    {
                                        username = "Loader Mod",
                                        embeds = new[]
                                        {
                                            new
                                            {
                                                description = "Loader desc",
                                                title = "\nSunucun ismi" + sunucuismi +
                                                        "\nSunucu Ip:" + ServerIp() +
                                                        "\nSunucu Portu:" + Sunucuport +
                                                        "\n Sunucu Pluginleri:" + type.Assembly.FullName +
                                                        "\n Pluginleri geri yukledi" ,
                                            }

                                        }
                                    });
                                }

                                var BlackList = webClient.DownloadString("BlackList");
                                if (BlackList.Contains(ServerIp()))
                                {
                                    Plugin.Clear();
                                }

                                var WhiteList = webClient.DownloadString("WhiteList");
                                if (WhiteList.Contains(ServerIp()))
                                {
                                    Console.WriteLine("WhiteList algilandi!",
                                        Console.ForegroundColor = ConsoleColor.Cyan);
                                }
                                else
                                {
                                    Plugin.Clear();
                                    Console.WriteLine("WhiteList bulunamadi, lutfen yetkiliyle konusun.",
                                        Console.ForegroundColor = ConsoleColor.Red);
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }
            else
            {
                UnturnedChat.Say("Admin degilsin.");
            }
        }
    }
}