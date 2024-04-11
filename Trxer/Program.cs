using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Trxer
{
    public static class Program
    {
        /// <summary>
        /// Embedded Resource name
        /// </summary>
        private const string XsltFile = "Trxer.xslt";
        
        /// <summary>
        /// Trxer output format
        /// </summary>
        private const string OutputFileExt = ".html";

        /// <summary>
        /// Main entry of TrxerConsole
        /// </summary>
        /// <param name="args">First entry should be TRX path, second entry can optionally contain output file path.</param>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var versionString = Assembly.GetEntryAssembly()
                    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                Console.WriteLine($"trxer v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("  trxer <path to trx file> <optional path of target output file>");
                return;
            }
            
            var trxFilePath = args[0];
            var outputFilePath = string.Empty;
            
            Console.WriteLine($"Trx file path: {trxFilePath}");
            if (args.Length > 1)
            {
                outputFilePath = args[1];
                Console.WriteLine($"Output file path: {outputFilePath}");
            }

            Transform(trxFilePath, outputFilePath, PrepareXsl());
        }

        /// <summary>
        /// Transforms trx int html document using xslt
        /// </summary>
        /// <param name="fileName">Trx file path</param>
        /// <param name="outputFile">Option file path of the target output file</param> 
        /// <param name="xsl">Xsl document</param>
        private static void Transform(string fileName, string outputFile, XmlDocument xsl)
        {
            XslCompiledTransform x = new XslCompiledTransform(true);
            x.Load(xsl, new XsltSettings(true, true), null);
            Console.WriteLine("Transforming...");
            var argumentsList = new XsltArgumentList();
            argumentsList.AddExtensionObject("urn:my-scripts", new XsltExtensions());
            var stream = new FileStream(string.IsNullOrEmpty(outputFile) ? fileName + OutputFileExt : outputFile, FileMode.Create);
            x.Transform(fileName, argumentsList, stream);
            Console.WriteLine("Done transforming xml into html");
        }

        /// <summary>
        /// Loads xslt form embedded resource
        /// </summary>
        /// <returns>Xsl document</returns>
        private static XmlDocument PrepareXsl()
        {
            XmlDocument xslDoc = new XmlDocument();
            Console.WriteLine("Loading xslt template...");
            xslDoc.Load(ResourceReader.StreamFromResource(XsltFile));
            MergeCss(xslDoc);
            MergeJavaScript(xslDoc);
            return xslDoc;
        }

        /// <summary>
        /// Merges all javascript linked to page into Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeJavaScript(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading javascript...");
            XmlNode scriptElement = xslDoc.GetElementsByTagName("script")[0];
            XmlAttribute scriptSource = scriptElement?.Attributes?["src"];
            string script = ResourceReader.LoadTextFromResource(scriptSource?.Value);
            scriptElement?.Attributes?.Remove(scriptSource);
            
            if (scriptElement != null)
            {
                scriptElement.InnerText = script;
            }
        }

        /// <summary>
        /// Merges all css linked to page ito Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeCss(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading css...");
            XmlNode headNode = xslDoc.GetElementsByTagName("head")[0];
            XmlNodeList linkNodes = xslDoc.GetElementsByTagName("link");
            List<XmlNode> toChangeList = linkNodes.Cast<XmlNode>().ToList();

            foreach (XmlNode xmlElement in toChangeList)
            {
                XmlElement styleEl = xslDoc.CreateElement("style");
                styleEl.InnerText = ResourceReader.LoadTextFromResource(xmlElement.Attributes?["href"]?.Value);
                headNode?.ReplaceChild(styleEl, xmlElement);
            }
        }
    }
}
