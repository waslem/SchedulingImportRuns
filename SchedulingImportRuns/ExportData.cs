using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingImportRuns
{
    public class ExportData
    {
        [CsvColumn(Name = "OrderNum", FieldIndex = 1)]
        public string OrderNum { get; set; }
        [CsvColumn(Name = "X", FieldIndex = 2)]
        public string X { get; set; }
        [CsvColumn(Name = "Y", FieldIndex = 3)]
        public string Y { get; set; }
        [CsvColumn(Name = "DriverName", FieldIndex = 4)]
        public string DriverName { get; set; }
    }
}
