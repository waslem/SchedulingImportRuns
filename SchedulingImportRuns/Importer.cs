using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    public static class Importer
    {
        internal static List<ImportedRecord> Import(List<string> importedFiles, CsvContext cc)
        {
            List<ImportedRecord> temp = new List<ImportedRecord>();
            List<ImportedRecord> importedRecords = new List<ImportedRecord>();

            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
            };

            for (int i = 0; i < importedFiles.Count; i++)
            {
                temp = cc.Read<ImportedRecord>(importedFiles[i], inputFileDescription).ToList();
                foreach (var item in temp)
                    importedRecords.Add(item);
            }

            return importedRecords;
        }
    }
}
