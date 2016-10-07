/*****************************
Project:    CODEiverse - Open Source Tools (OST)
Created By: EJ Alexandra - 2016
            An Abstract Level, llc
License:    Mozilla Public License 2.0
*****************************/
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace CODEiverse.OST.Lib
{
    public static class CDVExtensions
    {
        public static string SafeToString(this object objectToMakeText)
        {
            if (ReferenceEquals(objectToMakeText, null)) return String.Empty;
            return objectToMakeText.ToString();
        }

        public static String HtmlToXml(this String htmlText)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            MemoryStream ms = new MemoryStream();
            doc.OptionOutputAsXml = true;
            var anchors = doc.DocumentNode
                            .SelectNodes("//a");

            if (!ReferenceEquals(anchors, null))
            {
                anchors.Where(whereNode => !ReferenceEquals(whereNode, null))
                        .ToList()
                        .ForEach(feAnchor => feAnchor.CleanSPECDOCLink());
            }

            doc.Save(ms);
            ms.Position = 0;
            var sr = (new StreamReader(ms));
            return sr.ReadToEnd();
        }

        public static String XmlToJson(this XmlDocument doc)
        {
            return JsonConvert.SerializeXmlNode(doc.DocumentElement, Newtonsoft.Json.Formatting.Indented);

        }

        private static void CleanSPECDOCLink(this HtmlNode node)
        {
            if (!ReferenceEquals(node.Attributes, null))
            {
                var href = node.Attributes["href"];
                if (!ReferenceEquals(href, null))
                {
                    if (!String.IsNullOrEmpty(href.Value))
                    {
                        if (href.Value.Contains("://specdocs/"))
                        {
                            href.Value = href.Value.Substring(href.Value.IndexOf("://specdocs/") + "://specdocs/".Length);
                            var entityIndex = href.Value.ToLower().IndexOf("/entity/");
                            var viewResourceIndex = href.Value.ToLower().IndexOf("/view-resource/");
                            var simpleLinkIndex = href.Value.ToLower().IndexOf("/link/");
                            if (entityIndex >= 0)
                            {
                                var entityName = href.Value.Substring(entityIndex + "/entity/".Length);
                                var candidates = entityName.Split("/&?".ToCharArray());
                                var finalName = candidates.FirstOrDefault(fodCandidate => !String.IsNullOrEmpty(fodCandidate.SafeToString().Trim()));

                                href.Value = String.Format("$SDK_ROOT_URI$Docs/DataModel/Entity_{0}.html", finalName);
                            }
                            else if (viewResourceIndex >= 0)
                            {
                                var resourceName = href.Value.Substring(viewResourceIndex + "/view-resource/".Length);
                                var candidates = resourceName.Split("/&?".ToCharArray());
                                var finalName = candidates.FirstOrDefault(fodCandidate => !String.IsNullOrEmpty(fodCandidate.SafeToString().Trim()));

                                href.Value = String.Format("$SDK_ROOT_URI$Docs/AdditionalResources/Resource_{0}.html", finalName);
                            }
                            else if (simpleLinkIndex >= 0)
                            {
                                href.Value = HttpUtility.UrlDecode(href.Value.Substring(viewResourceIndex + "/link/".Length));
                            }
                            else
                            {
                                href.Value = "ERROR - COULD NOT FIND A MATCHING SPECDOCS// syntax for: '" + href.Value + "'";
                            }

                            if (href.Value.Contains(".html&")) href.Value = href.Value.Substring(0, href.Value.IndexOf(".html&") + ".html".Length);
                        }
                        else if (href.Value.StartsWith("https://www.google.com/url?q="))
                        {
                            var decodedUrl = HttpUtility.UrlDecode(href.Value.Substring("https://www.google.com/url?q=".Length)) + "&amp;";
                            href.Value = decodedUrl.Substring(0, decodedUrl.IndexOf("&amp;"));
                            node.SetAttributeValue("target", "_blank");
                        } else
                        {
                            node.SetAttributeValue("target", "_blank");
                            object o = 1;
                        }
                    }
                }
            }
        }

        public static T Get<T>(this string[] args)
            where T : class
        {
            if (typeof(T) == typeof(Uri))
            {
                var uriString = args.GetFirst<String>();
                if (!String.IsNullOrEmpty(uriString))
                {
                    return new Uri(uriString) as T;
                }
            }

            // If we get here - return nothing
            return default(T);
        }

        public static T GetFirst<T>(this String[] args)
            where T : class
        {
            if (typeof(T) == typeof(FileInfo))
            {
                String fileNameString = args.GetFirst<String>();
                return new FileInfo(fileNameString) as T;
            }
            else if (typeof(T) == typeof(String))
            {
                if (ReferenceEquals(args, null)) return String.Empty as T;
                else if (!args.Any()) return String.Empty as T;
                else return args[0] as T;
            }
            else
            {
                var msg = String.Format("Handler not writtent o handle finding first argument of type '{0}'", typeof(T).Name);
                throw new NotImplementedException(msg);
            }
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(this byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

    }
}
