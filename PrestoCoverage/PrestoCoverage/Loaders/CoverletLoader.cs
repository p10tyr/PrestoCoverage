using Coverlet.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrestoCoverage.Loaders
{
    public static class CoverletLoader
    {

        public static string CoverageDirectory { get; set; }


        private static Dictionary<string, Classes> _loadedClasses = null;
        private static Dictionary<string, Dictionary<int, int>> _documentLineNumbers = new Dictionary<string, Dictionary<int, int>>();
        private static readonly Dictionary<string, FileInformation> _fileEntries = new Dictionary<string, FileInformation>();


        static CoverletLoader()
        {
            CoverageDirectory = Settings.WatchFolder;
        }


        public struct FileInformation
        {
            public DateTime LastAccessTime { get; set; }
            //public bool Exists { get; set; }

            public FileInformation(DateTime lastAccessTime) //, bool exists)
            {
                LastAccessTime = lastAccessTime;
                //Exists = exists;
            }
        }

        internal static Dictionary<string, Classes> GetCoverletModules()
        {
            string[] coverageFilesOnDisk = Directory.GetFiles(CoverageDirectory, "*coverage.json");

            //Check we know about all files on disk
            foreach (var filePath in coverageFilesOnDisk)
                if (!_fileEntries.ContainsKey(filePath))
                    _fileEntries.Add(filePath, new FileInformation(new DateTime(0)));

            foreach (var fileEntryKey in _fileEntries.Keys.ToList())
            {
                if (!File.Exists(fileEntryKey))
                {
                    _fileEntries.Remove(fileEntryKey);

                    _loadedClasses = null;
                    _documentLineNumbers = new Dictionary<string, Dictionary<int, int>>();
                    continue;
                }

                var currentLastWriteTime = File.GetLastWriteTime(fileEntryKey);
                if (currentLastWriteTime != _fileEntries[fileEntryKey].LastAccessTime)
                {
                    _loadedClasses = null;
                    _documentLineNumbers = new Dictionary<string, Dictionary<int, int>>();
                    _fileEntries[fileEntryKey] = new FileInformation(currentLastWriteTime);
                }
            }



            if (_loadedClasses == null)
            {
                _loadedClasses = new Dictionary<string, Classes>();

                foreach (var fileName in _fileEntries)
                {
                    using (StreamReader r = new StreamReader(fileName.Key))
                    {
                        string json = r.ReadToEnd();
                        var tmpDocumentModules = JsonConvert.DeserializeObject<Dictionary<string, Documents>>(json);

                        foreach (var docs in tmpDocumentModules.Select(x => x.Value))
                        {
                            foreach (var doc in docs)
                            {
                                try
                                {
                                    _loadedClasses.Add(doc.Key, doc.Value);
                                }
                                catch (Exception x)
                                {
                                    Console.WriteLine($"Tried to add Document>Classes to dictionary but got exception '{x.Message}'");
                                }
                            }
                        }

                    }
                }
            }

            return _loadedClasses;
        }

        public static Dictionary<int, int> GetLinesForDocument(string coverletFilePath)
        {
            //First check if anything has changed with files and invalidate all cache
            var currentSpanDoc = GetCoverletModules().FirstOrDefault(x => x.Key == coverletFilePath);

            //If nothing has changed we can just return the previously calculated line values
            if (_documentLineNumbers.TryGetValue(coverletFilePath, out var LineResults))
                return LineResults;

            if (currentSpanDoc.Value != null) //.Any())
            {
                //Get the lines we gonna mark as covered
                var lines = currentSpanDoc.Value
                    .Select(documents => documents.Value)
                    .SelectMany(methods => methods.Values)
                    .SelectMany(lns => lns.Lines)
                    //.Select(n => new Coverlet.Lines() {  } )
                    .ToDictionary(line => line.Key, line => line.Value);

                _documentLineNumbers.Add(coverletFilePath, lines);
            }
            else
                _documentLineNumbers.Add(coverletFilePath, new Dictionary<int, int>());


            return _documentLineNumbers[coverletFilePath];
        }

    }
}
