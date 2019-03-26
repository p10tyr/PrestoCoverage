using System.Collections.Generic;
using System.Linq;

namespace PrestoCoverage.Models
{

    public class LineCoverageDetails
    {
        public string SourceFile { get; set; }
        public string CoveredFile { get; set; }
        public Dictionary<int, int> LineVisits { get; set; }
    }

    public class CoverageRepository
    {
        public List<LineCoverageDetails> LineCoverages { get; set; } = new List<LineCoverageDetails>();

        public void AddUpdateCoverage(string sourceFile, string coveredFile, Dictionary<int, int> lineVisits)
        {
            lock (LineCoverages)
            {
                var existing = LineCoverages.FirstOrDefault(f => f.SourceFile.Equals(sourceFile) && f.CoveredFile.Equals(coveredFile));

                if (existing == null)
                    LineCoverages.Add(new LineCoverageDetails { SourceFile = sourceFile, CoveredFile = coveredFile, LineVisits = lineVisits });
                else
                    existing.LineVisits = lineVisits;
            }
        }

        public void ClearAll()
        {
            lock (LineCoverages)
            {
                LineCoverages = new List<LineCoverageDetails>();
            }
        }


        public void RemoveCoverage(string sourceFile)
        {
            lock (LineCoverages)
            {
                var existing = LineCoverages.Where(f => f.SourceFile.Equals(sourceFile)).ToList();

                if (existing.Count == 0)
                    return;

                foreach (var itm in existing)
                {
                    LineCoverages.Remove(itm);
                }
            }
        }

        public Dictionary<int, int> GetDocumentCoverage(string filePath)
        {
            lock (LineCoverages)
            {
                var availability = LineCoverages.Where(f => f.CoveredFile.Equals(filePath)).Select(x => x.LineVisits);

                var result = availability
                        .SelectMany(d => d)
                        .GroupBy(
                          kvp => kvp.Key,
                          (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }
                        )
                        .ToDictionary(x => x.Key, x => x.Value);

                return result;
            }
        }
    }
}