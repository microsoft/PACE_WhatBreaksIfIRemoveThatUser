using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace WhatBreaksIf
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "WhatBreaksIf"),
        ExportMetadata("Description", "What breaks if I disable this user?"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAACXBIWXMAABOvAAATrwFj5o7DAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAA2ZJREFUWIXFl19oW3UUxz/nd83SVhtXmVUfmt5rGoosU5SK4ItOCpOB7kWKiMIU3yaCuAfBCaIMEfFhKgiygfim3WTgk1bp1hcfigyhiLG592aLbhU2YWATmtzL8SG3JcmSJnfesvMUfufP93Nucu75RVSVW2nmlqoDt8UJnpycHLMsax+wW0Que553QVXD/wMgg3wFuVwur6ofAM8CqRbXNVX9JAiCjyqVSm1HAHK53KyqngEy24Qtp1Kpg8Vi8WpcgG1/A1Hn/cQBHg2C4BsRkUQBosfeT3wzdr/jOM8lBjAxMXEXcChOMVU9nBiAZVkPEXNKRGQmMQBjzGjcYsCdiQGIyJWbALicGEA2m70A/BOz3k+JASwuLgYi8mmMWqEx5kRiAAD1ev1D4JdBCqnqe6VSaSVRgEqlUkulUk8D57YJC1X13XK5/H5ccRhwF4iI2LY9JyKHgUeAMeAv4EdjzImb6TwWwKCWz+fTQRC8bIyZU9W9wG5gDVhS1VO+75/bMQDbth82xpwG7t8mbH5kZOSVlZWVf3sCTE1NjavqjKre3k9UVRu+75+NxJeAOzZdwHdASVUPiMjelrSfLcvav7q6unEDgOM4z4vISaCveGSfWZZ1NAzD32jpXESOZLPZL1zXHc9kMler1eoPwBMteR97nne0DWB6enpPo9EoxxBHVR8AnhSRz1uOq77vjzqO8yXwoqoeBNIicrYlZkNVbd/317bGsF6vz8QRBy76vv+7MWau41yitfwMzQEaFZFqR0yaaNNuARhj4ogDFAFUtdBxPgx8TXMCTvq+fxp4rTNZRB5sAwD+jgmweQfsdWH5s9FovO44zjGad8lOy7QBGGOWgUuDqqvq3dHHruCq+sfQ0FAKONajxFobQDQWrwLBIAAisq9QKOwClnr4nwrDsAzs6uE/3wYA4HneAvACsDEAw2itVptV1VM9/L+KyNt0b+iiMWbhBoAIYh54DFjuR6Cqb0Wv1/ku7qKInAHWO9NE5M2uL6JWixbQAeAlEZkFxnvEHRkeHv5qfX39exF5vB8z8I7nece38gfdBbZt32dZ1h7gHlUdA64bY66pasl13ev5fD4dhuFxmiOX7lLikoi84brut20NJP3v2HGce4FDIlKgubaviMh5Y8zC5mPfUYC49h+6mF3x3vphCAAAAABJRU5ErkJggg=="),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class WhatBreaksIf : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new WhatBreaksIfControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public WhatBreaksIf()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
             AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}