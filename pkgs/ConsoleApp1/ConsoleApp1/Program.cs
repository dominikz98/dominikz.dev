using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string resName = @"D:\Games\steamapps\common\RUSSIAPHOBIA\RUSSIAPHOBIA_Data\resources.resource";
            var file = GetResourceStream(resName);
            string all = "";

            using (var reader = new StreamReader(file))
            {
                all = reader.ReadToEnd();
            }
            ;
        }

        static UnmanagedMemoryStream GetResourceStream(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var strResources = assembly.GetName().Name + ".g.resources";
            var rStream = assembly.GetManifestResourceStream(strResources);
            var resourceReader = new System.Resources.ResourceReader(rStream);
            var items = resourceReader.OfType<System.Collections.DictionaryEntry>();
            var stream = items.First(x => (x.Key as string) == resName.ToLower()).Value;
            return (UnmanagedMemoryStream)stream;
        }
    }
}
