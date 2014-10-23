using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    /// <summary>
    /// The ImportedRecord class is used to store import data from the Geographical Information System
    /// Currently the GIS that is used is ArcLogistics Route, this data is exported using the
    /// export routing folder feature.
    /// 
    /// Each order has an attribute FieldIndex, this is the order in which the data is presented in the csv file.
    /// </summary>
    public class ImportedRecord
    {
        /// <summary>
        /// This field contains the runid field of the order, a runid is assigned
        /// to each order that is routed, this runid is depending on the vehicle that the 
        /// order was routed to.
        /// </summary>
        [CsvColumn(FieldIndex = 1)]
        public string DriverName { get; set; }

        /// <summary>
        /// This field contains the unique identifier of the order in the Collect systems.
        /// </summary>
        [CsvColumn(FieldIndex = 2)]
        public int? OrderNum { get; set; }

        /// <summary>
        /// This field contains the address string of the order
        /// </summary>
        [CsvColumn(FieldIndex = 3)]
        public string Address { get; set; }

        /// <summary>
        /// This field contains the city string of the order
        /// </summary>
        [CsvColumn(FieldIndex = 4)]
        public string City { get; set; }

        /// <summary>
        /// This field contains the state string of the order
        /// </summary>
        [CsvColumn(FieldIndex = 5)]
        public string State { get; set; }

        /// <summary>
        /// This field contains the postal code of the order
        /// </summary>
        [CsvColumn(FieldIndex = 6)]
        public string PostalCode { get; set; }

        /// <summary>
        /// This field contains the X coordinate in Decimal Degrees format of the order
        /// </summary>
        [CsvColumn(FieldIndex = 7)]
        public string X { get; set; }

        /// <summary>
        ///  This field contains the Y coordinate in Decimal Degrees format of the order
        /// </summary>
        [CsvColumn(FieldIndex = 8)]
        public string Y { get; set; }
    }
}
