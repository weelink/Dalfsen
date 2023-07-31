using Bestandenselektie.HKD.Commands;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExporterViewModel : ViewModel
    {
        private readonly ISet<ExportableFileViewModel> files;
        private string? targetDirectory;
        private string? excelFileLocation;
        private bool isExportWindowOpen;
        private bool isValid;
        private bool exportAsExcel;
        private readonly ProgressDialog dialog;
        private MetroWindow? parent;

        public ExporterViewModel()
        {
            files = new HashSet<ExportableFileViewModel>();
            ShowExportCommand = new DelegateCommand(() => IsExportWindowOpen = true);
            ExportCommand = new DelegateCommand<MetroWindow>(parent => ExportImages(parent!), _ => IsValid);
            BrowseDirectoryCommand = new DelegateCommand<MetroWindow>(parent => BrowseDirectory(parent!));
            CancelCommand = new DelegateCommand(() => IsExportWindowOpen = false);
            BrowseForExcelFileCommand = new DelegateCommand<MetroWindow>(parent => BrowseForExcel(parent!));

            Files = new ObservableCollection<ExportableFileViewModel>();
            Files.CollectionChanged += Images_CollectionChanged;
            ConflictSolutions = new[] { true, false, false };
            exportAsExcel = true;

            PropertyChanged += ExporterViewModel_PropertyChanged;
            dialog = new ProgressDialog()
            {
                WindowTitle = "Exporteren",
                Text = "Bezig met exporteren...",
                ShowTimeRemaining = true,
            };

            dialog.DoWork += Dialog_DoWork;
            dialog.RunWorkerCompleted += Dialog_RunWorkerCompleted;
        }

        private void Dialog_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(parent, "Exporteren voltooid", "Expoteren", MessageBoxButton.OK, MessageBoxImage.Information);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var image in Files.ToList())
                {
                    image.IsSelected = false;
                    image.ShouldExport = false;
                }
                IsExportWindowOpen = false;
                Files.Clear();
            });
        }

        private void ExportImages(MetroWindow parent)
        {
            this.parent = parent;
            dialog.ShowDialog(parent);
        }

        private void Dialog_DoWork(object? sender, DoWorkEventArgs e)
        {
            var files = Files.Where(file => file.ShouldExport).ToList();

            for (int x = 0; x < files.Count; x++)
            {
                ExportableFileViewModel file = files[x];
                dialog.ReportProgress(x, null, file.FullPath);

                file.CopyTo(TargetDirectory!);

                if (dialog.CancellationPending)
                {
                    return;
                }
            }

            if (ExportAsExcel && !string.IsNullOrEmpty(ExcelFileLocation))
            {
                dialog.ReportProgress(100, null, "Exporteren naar excel");
                ExportToExcel(files);
            }
        }

        private void ExportToExcel(List<ExportableFileViewModel> files)
        {
            var cultureinfo = new CultureInfo("en-US");

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ExcelFileLocation!, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styleSheet = new Stylesheet(GenerateCellFormats());
                stylePart.Stylesheet = styleSheet;
                stylePart.Stylesheet.Save();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                Row headerRow = new Row();

                var columns = new List<string> {
                    "Locatie",
                    "Naam",
                    "Bestandstype",
                    "Grootte",
                    "Gewijzigd d.d.",
                    "Gemaakt d.d."
                };

                foreach (string column in columns)
                {
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (ExportableFileViewModel file in files)
                {
                    Row newRow = new Row();

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(file.Directory)
                    }, 0);

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(file.Name)
                    }, 1);

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(file.Extension)
                    }, 2);

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = CellValues.Number,
                        CellValue = new CellValue((int)file.SizeInBytes),
                        StyleIndex = 1
                    }, 3);

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = new EnumValue<CellValues>(CellValues.Number),
                        CellValue = new CellValue(file.CreatedAsDate.ToOADate().ToString(cultureinfo)),
                        StyleIndex = 2
                    }, 4);

                    newRow.InsertAt<Cell>(new Cell
                    {
                        DataType = new EnumValue<CellValues>(CellValues.Number),
                        CellValue = new CellValue(file.ModifiedAsDate.ToOADate().ToString(cultureinfo)),
                        StyleIndex = 2
                    }, 5);

                    sheetData.AppendChild(newRow);
                }

                workbookPart.Workbook.Save();
            }
        }

        private static CellFormats GenerateCellFormats()
        {
            CellFormats cellFormats = new CellFormats(
                new CellFormat(new Alignment()),
                // int
                new CellFormat(new Alignment()) { NumberFormatId = 1 },
                // Date
                new CellFormat(new Alignment()) { NumberFormatId = 15, ApplyNumberFormat = true }
            );

            return cellFormats;
        }

        private void BrowseForExcel(MetroWindow parent)
        {
            string initial;
            if (!string.IsNullOrEmpty(ExcelFileLocation))
            {
                initial = Path.GetDirectoryName(ExcelFileLocation)!;
            }
            else if (!string.IsNullOrEmpty(TargetDirectory))
            {
                initial = TargetDirectory;
            }
            else
            {
                initial = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel bestand|*.xlsx",
                InitialDirectory = initial
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFileLocation = saveFileDialog.FileName;
            }
        }

        private void BrowseDirectory(MetroWindow parent)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Selecteer de doelmap";
            dialog.UseDescriptionForTitle = true;

            if (dialog.ShowDialog(parent) == true)
            {
                TargetDirectory = dialog.SelectedPath;
            }
        }

        private void ExporterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TargetDirectory))
            {
                IsValid = !string.IsNullOrWhiteSpace(TargetDirectory) && Directory.Exists(TargetDirectory);
            }
        }

        public bool[] ConflictSolutions { get; }

        public string? ExcelFileLocation
        {
            get { return excelFileLocation; }
            set { SetProperty(ref excelFileLocation, value); }
        }

        public string? TargetDirectory
        {
            get { return targetDirectory; }
            set { SetProperty(ref targetDirectory, value); }
        }

        public bool ExportAsExcel
        {
            get { return exportAsExcel; }
            set { SetProperty(ref exportAsExcel, value); }
        }

        public bool IsExportWindowOpen
        {
            get { return isExportWindowOpen; }
            set { SetProperty(ref isExportWindowOpen, value); }
        }

        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }

        public ICommand ShowExportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand BrowseDirectoryCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowseForExcelFileCommand { get; }

        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExportableFileViewModel file in e.OldItems.OfType<ExportableFileViewModel>())
                {
                    files.Remove(file);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExportableFileViewModel file in e.NewItems.OfType<ExportableFileViewModel>())
                {
                    files.Add(file);
                }
            }

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(HasSelectedItems));
        }

        public bool HasSelectedItems
        {
            get { return Count > 0; }
        }

        public int Count
        {
            get { return Files.Count; }
        }

        public ObservableCollection<ExportableFileViewModel> Files { get; }

        public bool IsSelected(ExportableFileViewModel file)
        {
            return files.Contains(file);
        }

        public void Deselect(ExportableFileViewModel file)
        {
            Files.Remove(file);
        }

        public void Select(ExportableFileViewModel file)
        {
            Files.Add(file);
        }
    }
}
