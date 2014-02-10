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

        private CsvContext cc;
        private ImportStatistics stats; 

        public MainWindow()
        {
            InitializeComponent();

            importedFiles = new List<string>();
            importedRecords = new List<ImportedRecord>();
            WB_Bailiff = new List<ExportData>();
            WB_Express = new List<ExportData>();

            cc = new CsvContext();
            stats = new ImportStatistics();
        }

        private void btnImportClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog importDialog = SetupImportDialog();

            // add the file names to the importedFiles list 
            if (importDialog.ShowDialog() == true)
            {
                foreach (var file in importDialog.FileNames)
                    importedFiles.Add(file);
            }

            listViewFiles.Items.Refresh();
            listViewFiles.ItemsSource = importedFiles;

            // import the data from the files into the imported records list
            importedRecords = Importer.Import(importedFiles, cc);
           
            dataGridRecords.DataContext = importedRecords;

            CalculateStats();
            AssignStatisticLabels();
        }

        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            CalculateExportFields();
            ExportData();
        }

        private void CalculateStats()
        {
            stats.TotalFileCount = importedFiles.Count;
            stats.TotalRecordCount = importedRecords.Count;
            stats.CalculateImportCounts(importedRecords);
        }

        private static OpenFileDialog SetupImportDialog()
        {
            OpenFileDialog importDialog = new OpenFileDialog();

            importDialog.InitialDirectory = @"C:\X-Y Files";
            importDialog.Filter = "Text Files (*.txt) | *.txt";
            importDialog.CheckFileExists = true;
            importDialog.Multiselect = true;

            return importDialog;
        }

        private void AssignStatisticLabels()
        {
            lblFileCountNum.Content = stats.TotalFileCount.ToString();
            lblRecordCountNum.Content = stats.TotalRecordCount.ToString();
            lblImportedCountNum.Content = stats.ImportedRecordCount.ToString();
            lblBailiffCountNum.Content = stats.BailiffImportCount.ToString();
            lblExpressCountNum.Content = stats.ExpressImportCount.ToString();
        }
        private void ExportData()
        {

            CsvFileDescription outputFileDescription = new CsvFileDescription { FirstLineHasColumnNames = true };

            cc.Write(WB_Express, @"K:\WB Express.csv", outputFileDescription);
            cc.Write(WB_Bailiff, @"K:\WB Bailiff.csv", outputFileDescription);
        }

        private void CalculateExportFields()
        {
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
        }
    }
}
