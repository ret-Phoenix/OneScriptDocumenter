﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneScriptDocumenter
{
    class Documenter
    {
        internal XDocument CreateDocumentation(List<string> assemblies)
        {
            XDocument result = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("oscript-docs"));

            foreach (var assembly in assemblies)
            {
                var name = Path.GetFileNameWithoutExtension(assembly);
                var xmlName = Path.Combine(Path.GetDirectoryName(assembly), name + ".xml");
                if (!File.Exists(xmlName))
                {
                    Console.WriteLine("Missing xml-doc: {0}", xmlName);
                    continue;
                }

                Console.WriteLine("Processing: {0}", name);

                var docMaker = new AssemblyDocumenter(assembly, xmlName);
                var asmDoc = docMaker.CreateDocumentation();
                result.Root.Add(new XElement("assembly", 
                    new XAttribute("name", name),
                    asmDoc.Root));
            }
            Console.WriteLine("Done");
            return result;
        }
        internal string CreateDocumentationJSON(string pathOutput, List<string> assemblies)
        {
            XDocument result = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("oscript-docs"));
            //StringBuilder sbJSON = new StringBuilder();
            using (StreamWriter sbJSON = new StreamWriter(pathOutput))
            {
                sbJSON.WriteLine("export function globalContext(): any {");
                sbJSON.WriteLine("let data = ");

                foreach (var assembly in assemblies)
                {
                    var name = Path.GetFileNameWithoutExtension(assembly);
                    var xmlName = Path.Combine(Path.GetDirectoryName(assembly), name + ".xml");
                    if (!File.Exists(xmlName))
                    {
                        Console.WriteLine("Missing xml-doc: {0}", xmlName);
                        continue;
                    }

                    Console.WriteLine("Processing: {0}", name);

                    var docMaker = new AssemblyDocumenter(assembly, xmlName);
                    docMaker.CreateDocumentationJSON(sbJSON);
                }
                Console.WriteLine("Done");
                return "";
            }
        }
    }
}
