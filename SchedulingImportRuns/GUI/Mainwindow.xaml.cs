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
    /// Because this program is quite simple and small, we just code the back end into the mainwindow
    /// We just handle each event here also
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// list of strings containing the filenames for the files that are to be imported
        /// </summary>
        private List<String> importedFiles;

        /// <summary>
        /// list of importedRecord objects
        /// </summary>
        private List<ImportedRecord> importedRecords;

        /// <summary>
        /// collection of export data to create the export bailiff file
        /// </summary>
        private IEnumerable<ExportData> WB_Bailiff;

        /// <summary>
        /// collection of export data to create the export express file
        /// </summary>
        private IEnumerable<ExportData> WB_Express;

        /// <summary>
        /// the CsvContext object used for importing csv to lists
        /// </summary>
        private CsvContext cc;

        /// <summary>
        /// The statistics object used to track software statistics
        /// </summary>
        private ImportStatistics stats;

        /// <summary>
        /// The default constructor of the mainwindow
        /// This method initializes the GUI and also initialises the objects used by the software
        /// </summary>
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

        /// <summary>
        /// The btnImport click event method
        /// This method handles the import button click event, it:
        ///     - shows the user an openFileDialog 
        ///     - updates the GUI for the files selected
        ///     - sets the datagrid to the files selected
        ///     - Calculates the statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    listViewFiles.Items.Refresh();
                    listViewFiles.ItemsSource = importedFiles;

                    // import the data from the files into the imported records list
                    importedRecords = Helpers.Import(importedFiles, cc);

                    dataGridRecords.DataContext = importedRecords;

                    CalculateStats();
                    AssignStatisticLabels();
                }
            }
            catch (Exception ex)
            {
                // set the data source to null
                listViewFiles.ItemsSource = null;
                listViewFiles.Items.Refresh();

                MessageBox.Show("Invalid file format, please see documentation on required file specification. Error:" + ex.Message);
            }
        }

        /// <summary>
        /// The BtnExportClick method handles the btnExport click event 
        /// This method:
        ///     - sets the mouse cursor to busy
        ///     - calculates the export fields
        ///     - attempts to export the data to the required files
        ///     - set the mouse cursor back to free
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            importedRecords = Helpers.CalculateImportedRecords(importedRecords);
            WB_Bailiff = Helpers.CalculateBailiffRecords(importedRecords);
            WB_Express = Helpers.CalculateExpressRecords(importedRecords);

            //CalculateExportFields();
            ExportData();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// This private method calculates the statistics for the files imported
        /// </summary>
        private void CalculateStats()
        {
            stats.TotalFileCount = importedFiles.Count;
            stats.TotalRecordCount = importedRecords.Count;
            stats.CalculateImportCounts(importedRecords);
        }

        /// <summary>
        /// This method handles the required set up of the importDialogBox
        /// here we:
        ///     - create the new openFileDialog
        ///     - set the initial directory
        ///     - set the file type filter
        ///     - set the multiselect & checkfileexists options
        /// </summary>
        /// <returns>the importDialog that has been configured as required</returns>
        private static OpenFileDialog SetupImportDialog()
        {
            OpenFileDialog importDialog = new OpenFileDialog();

            importDialog.InitialDirectory = Properties.Settings.Default.defaultImportLocation;

            importDialog.Filter = "Text Files (*.txt) | *.txt";
            importDialog.CheckFileExists = true;
            importDialog.Multiselect = true;

            return importDialog;
        }

        /// <summary>
        /// This method handles assigning the statistic results to the labels on the GUI, calculated statistics are displayed
        /// when this method is called
        /// </summary>
        private void AssignStatisticLabels()
        {
            lblFileCountNum.Content = stats.TotalFileCount.ToString();
            lblRecordCountNum.Content = stats.TotalRecordCount.ToString();
            lblImportedCountNum.Content = stats.ImportedRecordCount.ToString();
            lblBailiffCountNum.Content = stats.BailiffImportCount.ToString();
            lblExpressCountNum.Content = stats.ExpressImportCount.ToString();
            lblRunCountNum.Content = stats.RunCount.ToString();
        }

        /// <summary>
        /// This method handles exporting data to the required data files in the required format, from the data 
        /// which was imported into the software.
        /// </summary>
        private void ExportData()
        {
            try
            {
                // we use LinQtoCSV to handle defining the file description
                CsvFileDescription outputFileDescription = new CsvFileDescription { FirstLineHasColumnNames = true };

                // use winforms folderbrowser dialog for browsing a folder, doesn't look the best but will suffice for now
                System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
                folder.SelectedPath = Properties.Settings.Default.defaultExportLocation;

                // show the dialog box
                System.Windows.Forms.DialogResult result = folder.ShowDialog();

                string bailiffLocation, expressLocation;

                // if the user accepts the location
                if (result.ToString().Equals("OK"))
                {
                    bailiffLocation = folder.SelectedPath;
                    expressLocation = folder.SelectedPath;

                    // default file name for this as this is what the Collect systems expect to import from the
                    // FileFormatSpecification
                    string bailiffFile = bailiffLocation + "\\WB Bailiff.csv";
                    string expressFile = expressLocation + "\\WB Express.csv";

                    // use the CsvContext object to write the required data to our required locations
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

        /// <summary>
        /// this method handles the change default import location event
        /// this will save the defaultImportLocation for the import setting to the selected path of the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// this method handles the change default export location event
        /// this will save the defaultImportLocation for the export setting to the selected path of the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// this method handles the exit button click event
        /// This method simply closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
