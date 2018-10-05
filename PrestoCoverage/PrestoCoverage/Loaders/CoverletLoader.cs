using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrestoCoverage.Loaders
{
    public static class CoverletLoader
    {
        public static List<Models.LineCoverageDetails> Load(string coverageFilePath)
        {
            return Load(new string[1] { coverageFilePath });
        }

        public static List<Models.LineCoverageDetails> Load(string[] coverageFilePaths)
        {

            var lineDetails = new List<Models.LineCoverageDetails>();

            foreach (var sourceFileName in coverageFilePaths)
            {
                if (!File.Exists(sourceFileName))
                    continue;

                var loadedClasses = new Dictionary<string, Classes>();

                //The following code is really crappy but is there a better way to resolve file read issues... if you agree then please tell me how to fix this or make a PR. Thanks!

                for (int i = 0; i < 3; i++)
                {

                    try
                    {
                        using (StreamReader r = new StreamReader(sourceFileName))
                        {
                            string json = r.ReadToEnd();
                            var tmpDocumentModules = JsonConvert.DeserializeObject<Dictionary<string, Documents>>(json);

                            foreach (var docs in tmpDocumentModules.Select(x => x.Value))
                            {
                                foreach (var doc in docs)
                                {
                                    try
                                    {
                                        loadedClasses.Add(doc.Key, doc.Value);
                                    }
                                    catch (Exception x)
                                    {
                                        Console.WriteLine($"Tried to add Document>Classes to dictionary but got exception '{x.Message}'");

                                    }
                                }
                            }
                        }

                        break;
                    }
                    catch (Exception x)//trying to catch file open exception
                    {
                        System.Threading.Thread.Sleep(50);
                    }

                }



                foreach (var cls in loadedClasses)
                {
                    var lc = new Models.LineCoverageDetails();

                    //Get the lines we gonna mark as covered
                    var lines = cls.Value
                        .Select(documents => documents.Value)
                        .SelectMany(methods => methods.Values)
                        .SelectMany(lns => lns.Lines)
                        //.Select(n => new Coverlet.Lines() {  } )
                        .ToDictionary(line => line.Key, line => line.Value);

                    lc.SourceFile = sourceFileName;
                    lc.CoveredFile = cls.Key;
                    lc.LineVisits = lines;

                    lineDetails.Add(lc);
                }
            }

            return lineDetails;
        }


        //Parts of source used from https://github.com/tonerdo/coverlet/blob/master/src/coverlet.core/CoverageResult.cs

        public class BranchInfo
        {
            public int Line { get; set; }
            public int Offset { get; set; }
            public int EndOffset { get; set; }
            public int Path { get; set; }
            public uint Ordinal { get; set; }
            public int Hits { get; set; }
        }

        public class Lines : SortedDictionary<int, int> { }

        public class Branches : List<BranchInfo> { }

        public class Method
        {
            internal Method()
            {
                Lines = new Lines();
                Branches = new Branches();
            }
            public Lines Lines;
            public Branches Branches;
        }
        public class Methods : Dictionary<string, Method> { }
        public class Classes : Dictionary<string, Methods> { }
        public class Documents : Dictionary<string, Classes> { }
        public class Modules : Dictionary<string, Documents> { }


    }
}