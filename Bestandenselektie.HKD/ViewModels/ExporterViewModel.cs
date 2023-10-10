﻿using Bestandenselektie.HKD.Commands;
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
using Bestandenselektie.HKD.Services;
using System.Threading;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExporterViewModel : ViewModel
    {
        private readonly Settings settings;
        private readonly ISet<ExportableFileViewModel> files;
        private string? targetDirectory;
        private string? excelFileLocation;
        private bool isExportWindowOpen;
        private bool isValid;
        private bool exportAsExcel;
        private bool fileExists;
        private bool markDirectoriesAsProcessed;
        private readonly ProgressDialog dialog;
        private readonly Storage storage;
        private MetroWindow? parent;
        private CancellationTokenSource? exportcancelled;

        public ExporterViewModel(Storage storage)
        {
            this.storage = storage;
            settings = storage.ReadSettings();
            
            files = new HashSet<ExportableFileViewModel>();
            ShowExportCommand = new DelegateCommand(() => IsExportWindowOpen = true);
            ExportCommand = new DelegateCommand<MetroWindow>(parent => ExportImages(parent!), _ => IsValid);
            BrowseDirectoryCommand = new DelegateCommand<MetroWindow>(parent => BrowseDirectory(parent!));
            CancelCommand = new DelegateCommand(() => IsExportWindowOpen = false);
            BrowseForExcelFileCommand = new DelegateCommand<MetroWindow>(parent => BrowseForExcel(parent!));

            Files = new ObservableCollection<ExportableFileViewModel>();
            Files.CollectionChanged += Images_CollectionChanged;
            TargetDirectory = settings.ExportDirectory;
            ConflictResolutions = settings.ConflictResolutions.ToArray();
            ExportAsExcel = settings.ExportToExcel;
            ExcelFileLocation = settings.ExcelFilename;
            MarkDirectoriesAsProcessed = settings.MarkDirectoriesAsProcessed;

            PropertyChanged += ExporterViewModel_PropertyChanged;
            dialog = new ProgressDialog()
            {
                WindowTitle = "Exporteren",
                Text = "Bezig met exporteren...",
                ShowTimeRemaining = true,
                ShowCancelButton = false
            };

            dialog.DoWork += Dialog_DoWork;
            dialog.RunWorkerCompleted += Dialog_RunWorkerCompleted;

            Validate();
        }

        private void Dialog_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(parent, ((Exception)e.Result).Message, "Expoteren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
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
        }

        private void ExportImages(MetroWindow parent)
        {
            exportcancelled = new CancellationTokenSource();
            this.parent = parent;
            dialog.ShowDialog(parent, exportcancelled.Token);
        }

        private void Dialog_DoWork(object? sender, DoWorkEventArgs e)
        {
            var files = Files.Where(file => file.ShouldExport).ToList();

            CreateDirectory(TargetDirectory!);

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

            List<ExplorerViewModel> directories = Files.Select(x => x.ParentViewModel).Distinct().ToList();
            var settings = storage.ReadSettings();

            settings.ConflictResolutions = ConflictResolutions.ToList();
            settings.ExportDirectory = TargetDirectory;
            settings.ExportToExcel = ExportAsExcel;
            settings.ExcelFilename = ExcelFileLocation;
            settings.MarkDirectoriesAsProcessed = MarkDirectoriesAsProcessed;

            storage.Write(settings);

            if (ExportAsExcel && !string.IsNullOrEmpty(ExcelFileLocation))
            {
                dialog.ReportProgress(100, null, "Exporteren naar excel");
                e.Result = ExportToExcel(files);
            }
            else
            {
                e.Result = true;
            }

            if (MarkDirectoriesAsProcessed)
            {
                settings.MarkAsProcessed(directories.Select(directory => directory.Directory));

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var directory in directories)
                    {
                        directory.IsProcessed = true;
                    }
                });
            }

            storage.Write(settings);
        }

        private void CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            } catch (Exception e)
            {
                MessageBox.Show(parent, $"Er is een fout opgetreden bij het aanmaken van de map {path}:{Environment.NewLine}{Environment.NewLine}{e.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Exception? ExportToExcel(List<ExportableFileViewModel> files)
        {
            var cultureinfo = new CultureInfo("en-US");

            CreateDirectory(Path.GetDirectoryName(ExcelFileLocation)!);
            if (!File.Exists(ExcelFileLocation))
            {
                CreateEmptyExcel();
            }

            string filename = Path.GetFileName(TargetDirectory!)!;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(ExcelFileLocation!, true))
                {
                    WorkbookPart workbookPart = document.WorkbookPart!;
                    IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>();
                    Sheet sheet = sheets.First(s => s.Name!.Equals("Invulvelden"));

                    string? relationshipId = sheet.Id!;

                    Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;

                    WorksheetPart worksheetPart = worksheet.WorksheetPart!;
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    var lastRowIndex = sheetData.Elements<Row>().Select((row, index) => new
                    {
                        Row = row,
                        RowIndex = (uint)index
                    }).First(row =>
                    {
                        return row.RowIndex > 4 && (!row.Row.HasChildren ||
                            (row.Row.Descendants<Cell>().Count() > 2 && row.Row.Descendants<Cell>().Skip(2).First().CellValue == null));
                    }).RowIndex;

                    for (int i = 0; i < files.Count; i++)
                    {
                        ExportableFileViewModel file = files[i];

                        Row newRow = new Row { RowIndex = (uint)(lastRowIndex + i) };

                        newRow.Append(GenerateEmpty(2));

                        newRow.AppendChild(new Cell
                        {
                            DataType = CellValues.Number,
                            CellValue = new CellValue("" + (lastRowIndex - 5 + i + 1))
                        });

                        newRow.AppendChild(new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(filename + (lastRowIndex - 5 + i + 1))
                        });

                        newRow.Append(GenerateEmpty(18));

                        newRow.AppendChild(new Cell
                        {
                            DataType = CellValues.Number,
                            CellValue = new CellValue(0)
                        });

                        newRow.Append(GenerateEmpty(5));

                        newRow.AppendChild(new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.FullPath)
                        });

                        sheetData.InsertAt(newRow, (int) (newRow.RowIndex.Value));
                    }

                    workbookPart.Workbook.Save();
                }

                return null;
            }
            catch (IOException e)
            {
                return e;
            }
        }

        private IEnumerable<Cell> GenerateEmpty(int count)
        {
            return Enumerable.Range(0, count).Select(_ => new Cell());
        }

        private void CreateEmptyExcel()
        {
            using (Stream excel = GetType().Assembly.GetManifestResourceStream("Bestandenselektie.HKD.Assets.Excel-invulformulier ZCBS.xlsx")!)
            {
                using (var fileStream = File.Create(ExcelFileLocation!))
                {
                    excel.Seek(0, SeekOrigin.Begin);
                    excel.CopyTo(fileStream);
                }
            }
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

            var saveFileDialog = new SaveFileDialog()
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
            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog(parent) == true)
            {
                TargetDirectory = dialog.SelectedPath;
            }
        }

        private void ExporterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExcelFileLocation))
            {
                FileExists = File.Exists(ExcelFileLocation);
            }

            if (e.PropertyName == nameof(TargetDirectory))
            {
                Validate();
            }
        }

        private bool ShouldAutoGenerateExcelFileName
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(TargetDirectory) ||
                    string.IsNullOrEmpty(ExcelFileLocation) ||
                    Path.GetFullPath(TargetDirectory!).Equals(Path.GetFullPath(Path.GetDirectoryName(ExcelFileLocation)!), StringComparison.CurrentCultureIgnoreCase);
                } catch (Exception)
                {
                    return false;
                }
            }
        }

        private bool IsValidPath(string path)
        {
            try
            {
                new FileInfo(path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Validate()
        {
            IsValid = !string.IsNullOrWhiteSpace(TargetDirectory) && IsValidPath(TargetDirectory);
        }

        public bool[] ConflictResolutions { get; }

        public string? ExcelFileLocation
        {
            get { return excelFileLocation; }
            set { SetProperty(ref excelFileLocation, value); }
        }

        public string? TargetDirectory
        {
            get { return targetDirectory; }
            set
            {
                var shouldUpdateExcelFilename = ShouldAutoGenerateExcelFileName;

                SetProperty(ref targetDirectory, value);
                if (shouldUpdateExcelFilename)
                {
                    ExcelFileLocation = Path.Combine(TargetDirectory!, Path.GetFileNameWithoutExtension(TargetDirectory) + ".xlsx");
                }
            }
        }

        public bool ExportAsExcel
        {
            get { return exportAsExcel; }
            set { SetProperty(ref exportAsExcel, value); }
        }

        public bool FileExists
        {
            get { return fileExists; }
            set { SetProperty(ref fileExists, value); }
        }

        public bool MarkDirectoriesAsProcessed
        {
            get { return markDirectoriesAsProcessed; }
            set { SetProperty(ref markDirectoriesAsProcessed, value); }
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
