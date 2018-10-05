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




    public static class Coverage
    {
        public static List<LineCoverageDetails> LineCoverages { get; set; }

        static Coverage()
        {
            if (LineCoverages == null)
                LineCoverages = new List<LineCoverageDetails>();
        }

        public static void AddUpdateCoverages(List<LineCoverageDetails> lcds)
        {
            foreach (var lcd in lcds)
                AddUpdateCoverage(lcd.SourceFile, lcd.CoveredFile, lcd.LineVisits);
        }

        public static void AddUpdateCoverage(LineCoverageDetails lcd)
        {
            AddUpdateCoverage(lcd.SourceFile, lcd.CoveredFile, lcd.LineVisits);
        }

        public static void AddUpdateCoverage(string sourceFile, string coveredFile, Dictionary<int, int> lineVisits)
        {
            var existing = LineCoverages.ToList().FirstOrDefault(f => f != null && f.SourceFile.Equals(sourceFile) && f.CoveredFile.Equals(coveredFile));

            if (existing == null)
                LineCoverages.Add(new LineCoverageDetails { SourceFile = sourceFile, CoveredFile = coveredFile, LineVisits = lineVisits });
            else
                existing.LineVisits = lineVisits;

        }

        public static void RemoveCoverage(string sourceFile) //, string coveredFile, Dictionary<int, int> lineVisits)
        {
            //premature ToList here to avoid concurrency issues
            var existing = LineCoverages.ToList().Where(f => f != null && f.SourceFile.Equals(sourceFile)).ToList();

            if (existing.Count == 0)
                return;

            foreach (var itm in existing)
            {
                try
                {
                    LineCoverages.Remove(itm);
                }
                catch (System.Exception)
                {
                }
            }
        }

        public static Dictionary<int, int> GetDocumentCoverage(string filePath)
        {
            var availability = LineCoverages.Where(f => f != null && f.CoveredFile.Equals(filePath)).Select(x => x.LineVisits);

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