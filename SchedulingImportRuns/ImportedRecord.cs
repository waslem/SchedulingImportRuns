using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    public class ImportedRecord
    {
        [CsvColumn(FieldIndex = 1)]
        public string DriverName { get; set; }
        [CsvColumn(FieldIndex = 2)]
        public int? OrderNum { get; set; }
        [CsvColumn(FieldIndex = 3)]
        public string Address { get; set; }
        [CsvColumn(FieldIndex = 4)]
        public string City { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public string State { get; set; }
        [CsvColumn(FieldIndex = 6)]
        public string PostalCode { get; set; }
        [CsvColumn(FieldIndex = 7)]
        public string X { get; set; }
        [CsvColumn(FieldIndex = 8)]
        public string Y { get; set; }
    }
}
