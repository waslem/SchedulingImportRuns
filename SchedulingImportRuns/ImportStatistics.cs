using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    public class ImportStatistics
    {
        public int ImportedRecordCount { get; set; }
        public int TotalFileCount { get; set; }
        public int TotalRecordCount { get; set; }
        public int BailiffImportCount { get; set; }
        public int ExpressImportCount { get; set; }
        public int RunCount { get; set; }


        public void CalculateImportCounts(List<ImportedRecord> importedRecords)
        {
            int importCount = 0;
            int bailiffCount = 0;
            int expressCount = 0;

            foreach (var record in importedRecords)
            {
                if (record.OrderNum != null)
                {
                    importCount++;

                    if (record.OrderNum > 20000)
                    {
                        bailiffCount++;
                    }
                    else
                    {
                        expressCount++;
                    }
                }
            }

            this.ImportedRecordCount =  importCount;
            this.BailiffImportCount = bailiffCount;
            this.ExpressImportCount = expressCount;
        }
    }
}
