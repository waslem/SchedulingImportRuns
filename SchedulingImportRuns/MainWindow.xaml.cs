using LINQtoCSV;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchedulingImportRuns
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<String> importedFiles;
        private List<ImportedRecord> importedRecords;

        private IEnumerable<ExportData> WB_Bailiff;
        private IEnumerable<ExportData> WB_Express;

        public MainWindow()
        {
            InitializeComponent();

            importedFiles = new List<string>();
            importedRecords = new List<ImportedRecord>();

            WB_Bailiff = new List<ExportData>();
            WB_Express = new List<ExportData>();
        }

        private void btnImportClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog importDialog = new OpenFileDialog();

            importDialog.Filter = "Text Files (*.txt) | *.txt";
            importDialog.CheckFileExists = true;
            importDialog.Multiselect = true;

            if (importDialog.ShowDialog() == true)
            {
                foreach (var file in importDialog.FileNames)
                    importedFiles.Add(file);
            }
        }

        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            List<ImportedRecord> temp = new List<ImportedRecord>();
            CsvContext cc = new CsvContext();

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

            importedRecords = importedRecords
                                .Where(u => u.OrderNum != null)
                                .OrderBy(u => u.OrderNum)
                                .ToList();

            WB_Bailiff = importedRecords
                .Where(u => u.OrderNum > 30000)
                .Select(u => new ExportData
                    {
                        DriverName = u.DriverName,
                        OrderNum = u.OrderNum.ToString(),
                        X = u.X,
                        Y = u.Y
                    });

            WB_Express = importedRecords
                .Where(u => u.OrderNum < 30000)
                .Select(u => new ExportData
                {
                    DriverName = u.DriverName,
                    OrderNum = u.OrderNum.ToString(),
                    X = u.X,
                    Y = u.Y
                });

            CsvFileDescription outputFileDescription = new CsvFileDescription { FirstLineHasColumnNames = true };

            cc.Write(WB_Express, @"K:\WB Express.csv", outputFileDescription);
            cc.Write(WB_Bailiff, @"K:\WB Bailiff.csv", outputFileDescription);
        }
    }
}
