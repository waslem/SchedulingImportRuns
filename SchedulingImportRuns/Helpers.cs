using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace SchedulingImportRuns
{
    /// <summary>
    /// This static class contains various helper methods which are used in the GUI to conduct various functions
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///  This static method sorts the importedRecords list to only include:
        ///     - orders which are not null
        ///     - sorted by order number
        /// </summary>
        /// <param name="importedRecords">the records to be calculated</param>
        /// <returns>the calculated records</returns>
        public static List<ImportedRecord> CalculateImportedRecords(List<ImportedRecord> importedRecords)
        {
            importedRecords = importedRecords.Where(u => u.OrderNum != null).OrderBy(u => u.OrderNum).ToList();

            return importedRecords;
        }

        /// <summary>
        ///  This static method returns only the ImportedRecords that:
        ///     - orderNumber is > 30000
        ///     - the export is in the ExportData object format
        ///     - A linq query is used to convert the data from the ImportedRecord type to the ExportData type
        /// </summary>
        /// <param name="importedRecords">the list of importedRecord objects to convert</param>
        /// <returns>the WB_Bailiff export data collection</returns>
        public static IEnumerable<ExportData> CalculateBailiffRecords(List<ImportedRecord> importedRecords)
        {
             var WB_Bailiff = importedRecords.Where(u => u.OrderNum > 30000)
                .Select(u => new ExportData
                {
                    DriverName = u.DriverName,
                    OrderNum = u.OrderNum.ToString(),
                    X = u.X,
                    Y = u.Y
                });

             return WB_Bailiff;
        }

        /// <summary>
        ///  This static method returns only the ImportedRecords that:
        ///     - orderNumber is less than 30000
        ///     - the export is in the ExportData object format
        ///     - A linq query is used to convert the data from the ImportedRecord type to the ExportData type
        /// </summary>
        /// <param name="importedRecords">the list of importedRecord objects to convert</param>
        /// <returns>the WB_Express export data collection</returns>
        public static IEnumerable<ExportData> CalculateExpressRecords(List<ImportedRecord> importedRecords)
        {
            var WB_Express = importedRecords
                .Where(u => u.OrderNum < 30000)
                .Select(u => new ExportData
                {
                    DriverName = u.DriverName,
                    OrderNum = u.OrderNum.ToString(),
                    X = u.X,
                    Y = u.Y
                });

            return WB_Express;
        }

        /// <summary>
        /// The Import static helper method imports a list of importedFile names and returns a list of imported records.
        /// </summary>
        /// <param name="importedFiles">The list of file names to import,
        /// this is populated by the user when selecting the files to import
        /// </param>
        /// <param name="cc">The csvContext file which reads the data and converts it to csv
        /// We are currently using LinqToCSV library to handle this process</param>
        /// <returns></returns>
        public static List<ImportedRecord> Import(List<string> importedFiles, CsvContext cc)
        {
            List<ImportedRecord> temp = new List<ImportedRecord>();
            List<ImportedRecord> importedRecords = new List<ImportedRecord>();

            // create a file description, we set the separator of the csv to ',' 
            //and define the first line to have headers
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
            };

            // loop through each file that was selected to contain import data
            for (int i = 0; i < importedFiles.Count; i++)
            {
                // read the data from the import file to a list of imported record objects
                temp = cc.Read<ImportedRecord>(importedFiles[i], inputFileDescription).ToList();

                // add each from the temp list to the master list
                foreach (var item in temp)
                    importedRecords.Add(item);
            }

            return importedRecords;
        }
    }
}
