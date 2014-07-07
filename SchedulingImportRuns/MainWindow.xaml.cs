using LINQtoCSV;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        // TODO: refactor these lists and ienumerables to potentially encapsulated in 1 object
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

            try
            {
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
            catch (Exception)
            {
                listViewFiles.ItemsSource = null;
                listViewFiles.Items.Refresh();

                MessageBox.Show("Invalid file format, please see documentation on required file specification");
            }
        }

        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            CalculateExportFields();
            ExportData();

            Mouse.OverrideCursor = null;
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

            importDialog.InitialDirectory = Properties.Settings.Default.defaultImportLocation;

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
            lblRunCountNum.Content = stats.RunCount.ToString();
        }
        private void ExportData()
        {
            try
            {
                CsvFileDescription outputFileDescription = new CsvFileDescription { FirstLineHasColumnNames = true };

                // use winforms folderbrowser dialog for browsing a folder
                System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
                folder.SelectedPath = Properties.Settings.Default.defaultExportLocation;

                System.Windows.Forms.DialogResult result = folder.ShowDialog();

                string bailiffLocation, expressLocation;

                if (result.ToString() == "OK")
                {
                    bailiffLocation = folder.SelectedPath;
                    expressLocation = folder.SelectedPath;

                    string bailiffFile = bailiffLocation + "\\WB Bailiff.csv";
                    string expressFile = expressLocation + "\\WB Express.csv";

                    cc.Write(WB_Bailiff, bailiffFile, outputFileDescription);
                    cc.Write(WB_Express, expressFile, outputFileDescription);

                    MessageBox.Show("Export completed successfully.\n Created " + bailiffFile + " \n Created " + expressFile);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error exporting files" + Environment.NewLine + Ex.Message);
            }
        }

        // todo potentially factor this out to another class?
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

        private void MenuChangeDefaultImport_Click(object sender, RoutedEventArgs e)
        {
            var importDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = importDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.defaultImportLocation = importDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void MenuChangeDefaultExport_Click(object sender, RoutedEventArgs e)
        {
            var exportDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = exportDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.defaultExportLocation = exportDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
