using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    /// <summary>
    /// This export class contains data for each entity that has been scheduled
    /// in the Geographical Information System, this class contains the fields
    /// which are imported into the Collect database systems.
    /// </summary>
    public class ExportData
    {
        /// <summary>
        /// The Reference Number of the order, named OrderNum in Collect
        /// </summary>
        [CsvColumn(Name = "OrderNum", FieldIndex = 1)]
        public string OrderNum { get; set; }

        /// <summary>
        /// The X coordinate of the address in the Decimal Degrees format
        /// </summary>
        [CsvColumn(Name = "X", FieldIndex = 2)]
        public string X { get; set; }

        /// <summary>
        /// The Y coordinate of the address in the Decimal Degrees format
        /// </summary>
        [CsvColumn(Name = "Y", FieldIndex = 3)]
        public string Y { get; set; }

        /// <summary>
        /// The DriverName field, this is the RunID field as defined in Collect
        /// </summary>
        [CsvColumn(Name = "DriverName", FieldIndex = 4)]
        public string DriverName { get; set; }
    }
}
