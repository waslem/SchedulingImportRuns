using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    /// <summary>
    /// This class handles the statistics which are generated for each import process
    /// The statistics that are currently gathered are:
    ///     - ImportedRecordCount
    ///     - TotalFileCount
    ///     - TotalRecordCount
    ///     - BailiffImportCount
    ///     - ExpressImportCount
    ///     - RunCount
    /// </summary>
    public class ImportStatistics
    {
        /// <summary>
        /// field containing the total records imported
        /// </summary>
        public int ImportedRecordCount { get; set; }

        /// <summary>
        /// field containing the total Total files selected by user
        /// </summary>
        public int TotalFileCount { get; set; }

        /// <summary>
        /// field containing the Total records
        /// </summary>
        public int TotalRecordCount { get; set; }

        /// <summary>
        /// field containing the Total bailiff records
        /// </summary>
        public int BailiffImportCount { get; set; }

        /// <summary>
        /// field containing the total Total Express records
        /// </summary>
        public int ExpressImportCount { get; set; }

        /// <summary>
        /// field containing the Total routes created (unique count of runid's)
        /// </summary>
        public int RunCount { get; set; }

        /// <summary>
        /// This method calculates the following statistics:
        ///     - total run count
        ///     - total imported record count
        ///     - total bailiff import count
        ///     - total express import count
        /// </summary>
        /// <param name="importedRecords">the list of records that have been imported</param>
        public void CalculateImportCounts(List<ImportedRecord> importedRecords)
        {
            int importCount = 0;
            int bailiffCount = 0;
            int expressCount = 0;

            // loop through each record
            foreach (var record in importedRecords)
            {
                // only count if an order number exists
                if (record.OrderNum != null)
                {
                    importCount++;

                    // hard coded cut off for express reference #'s
                    if (record.OrderNum > 20000)
                        bailiffCount++;
                    else
                        expressCount++;
                }
            }

            // linq query to get the total run count
            // a unique run is identified by the driver name, here we group them and find the count of unique groups
            this.RunCount = importedRecords.GroupBy(u => u.DriverName)
                                        .ToDictionary(grp => grp.Key, grp => grp.ToList()).Count;

            this.ImportedRecordCount =  importCount;
            this.BailiffImportCount = bailiffCount;
            this.ExpressImportCount = expressCount;
        }
    }
}
