﻿using Bestandenselektie.HKD.Commands;
using DocumentFormat.OpenXml.Packaging;
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
using Bestandenselektie.HKD.Exceptions;
using DocumentFormat.OpenXml;

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
        private bool markDirectoriesAsProcessed;
        private bool filenamesAsDescription;
        private readonly ProgressDialog dialog;
        private readonly Storage storage;
        private MetroWindow? parent;
        private CancellationTokenSource? exportcancelled;

        public ExporterViewModel(Storage storage, ReferenceData referenceData)
        {
            this.storage = storage;
            ReferenceData = referenceData;

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
            FilenamesAsDescription = settings.FilenamesAsDescription;

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
            if (e.Result == null)
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
            else if (e.Result is InvalidExcelFileFoundException exception)
            {
                var message = $"Er is iets niet goed met het Excel-bestand {exception.Filename}" +
                      $"{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"Kies een niet bestaand bestand en probeer het opnieuw." +
                      $"{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"Technische foutcode: {exception.TechnicalReason}";

                MessageBox.Show(parent, message, "Expoteren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result is Exception exception2)
            {
                MessageBox.Show(parent, exception2.Message, "Expoteren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
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
            settings.FilenamesAsDescription = FilenamesAsDescription;

            ReferenceData.Update(settings);

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

            DefaultFotograaf = "";
            defaultPlaats = "";
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

            string excelFileLocation = ExcelFileLocation!;

            CreateDirectory(Path.GetDirectoryName(excelFileLocation)!);
            if (!File.Exists(ExcelFileLocation))
            {
                CreateEmptyExcel();
            }

            string filename = Path.GetFileNameWithoutExtension(ExcelFileLocation!)!;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(excelFileLocation, true))
                {
                    WorkbookPart? workbookPart = document.WorkbookPart;

                    if (workbookPart == null)
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Geen workbook part gevonden");
                    }

                    IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>();

                    if (!sheets.Any())
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Geen werkbladen gevonden");
                    }

                    Sheet? sheet = sheets.FirstOrDefault(s => string.Equals(s.Name, "Invulvelden", StringComparison.OrdinalIgnoreCase));
                    string? relationshipId = sheet?.Id;

                    if (sheet == null || relationshipId == null)
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Werkblad 'Invulvelden' niet gevonden");
                    }

                    Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;
                    WorksheetPart? worksheetPart = worksheet.WorksheetPart;

                    if (worksheetPart == null)
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Werkblad 'Invulvelden' heeft geen worksheet part");
                    }

                    var sheetDatas = worksheetPart.Worksheet.Elements<SheetData>();

                    if (!sheetDatas.Any())
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Geen sheet datas gevonden");
                    }

                    SheetData sheetData = sheetDatas.First();

                    Row? lastRow = sheetData.Elements<Row>().FirstOrDefault(row =>
                    {
                        return row.RowIndex != null && row.RowIndex.Value > 2 && (!row.HasChildren ||
                            (row.Descendants<Cell>().Count() > 2 && row.Descendants<Cell>().Skip(2).First().CellValue == null));
                    }) ?? sheetData.Elements<Row>().LastOrDefault(row => row.RowIndex != null);

                    if (lastRow == null)
                    {
                        throw new InvalidExcelFileFoundException(excelFileLocation, "Geen lege rij gevonden");
                    }

                    var rowIndex = lastRow.RowIndex!.Value;

                    for (int i = 0; i < files.Count; i++)
                    {
                        ExportableFileViewModel file = files[i];

                        uint newRowIndex = Convert.ToUInt32(rowIndex + i + 1);
                        Row newRow = new Row { RowIndex = newRowIndex };

                        newRow.Append(GenerateEmpty(2));    // A B

                        newRow.AppendChild(new Cell         // C
                        {
                            DataType = CellValues.Number,
                            CellValue = new CellValue("" + (rowIndex - 2 + i + 1))
                        });

                        newRow.AppendChild(new Cell         // D
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(filename + (rowIndex - 2 + i + 1))
                        });

                        newRow.AppendChild(new Cell         // E
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Rubriek?.Naam ?? string.Empty)
                        });

                        newRow.AppendChild(new Cell         // F
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Subrubriek?.Naam ?? string.Empty)
                        });

                        newRow.AppendChild(new Cell         // G
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Titel ?? string.Empty)
                        });

                        if (FilenamesAsDescription)         // H
                        {
                            newRow.AppendChild(new Cell
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(file.NameWithoutExtension ?? string.Empty)
                            });
                        }
                        else
                        {
                            newRow.Append(GenerateEmpty(1));
                        }

                        newRow.AppendChild(new Cell         // I
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Fotograaf ?? string.Empty)
                        });

                        newRow.AppendChild(new Cell         // J
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Datering ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(3));    // K, L, M

                        newRow.AppendChild(new Cell         // N
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Plaats ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(4));    // O, P, Q, R

                        if (!FilenamesAsDescription)        // S
                        {
                            newRow.AppendChild(new Cell
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(file.NameWithoutExtension ?? string.Empty)
                            });
                        }
                        else
                        {
                            newRow.Append(GenerateEmpty(1));
                        }

                        newRow.AppendChild(new Cell         // T
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Archieflocatie ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(1));    // U

                        newRow.AppendChild(new Cell         // V
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Categorie ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(2));    // W, X

                        newRow.AppendChild(new Cell         // Y
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Collectie ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(3));    // Z, AA, AB

                        var path = file.Target ?? file.FullPath;
                        var segments = path.Split(Path.VolumeSeparatorChar).Last().Split(Path.DirectorySeparatorChar).TakeLast(3);
                        string newPath = Path.DirectorySeparatorChar + Path.Combine(segments.ToArray());

                        newRow.AppendChild(new Cell         // AC
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(newPath)
                        });

                        newRow.AppendChild(new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(file.Licentie ?? string.Empty)
                        });

                        newRow.Append(GenerateEmpty(6));

                        sheetData.InsertAfter(newRow, lastRow);
                        lastRow = newRow;
                    }

                    workbookPart.Workbook.Save();
                }

                return null;
            }
            catch (Exception e)
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
            if (e.PropertyName == nameof(ExcelFileLocation) || e.PropertyName == nameof(IsExportWindowOpen))
            {
                OnPropertyChanged(nameof(FileExists));
            }

            if (e.PropertyName == nameof(TargetDirectory))
            {
                Validate();

                if (IsValid)
                {
                    var target = TargetDirectory!;
                    while (Path.EndsInDirectorySeparator(target))
                    {
                        var newTarget = Path.TrimEndingDirectorySeparator(target);

                        if (newTarget == target)
                        {
                            break;
                        }

                        target = newTarget;
                    }

                    var file = Path.GetFileNameWithoutExtension(target);

                    if (string.IsNullOrWhiteSpace(file))
                    {
                        file = "export";
                    }

                    ExcelFileLocation = Path.Combine(target, file + ".xlsx");
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
                SetProperty(ref targetDirectory, value);
            }
        }

        public bool ExportAsExcel
        {
            get { return exportAsExcel; }
            set { SetProperty(ref exportAsExcel, value); }
        }

        public bool FileExists
        {
            get { return File.Exists(ExcelFileLocation); }
        }

        public bool MarkDirectoriesAsProcessed
        {
            get { return markDirectoriesAsProcessed; }
            set { SetProperty(ref markDirectoriesAsProcessed, value); }
        }

        public bool FilenamesAsDescription
        {
            get { return filenamesAsDescription; }
            set { SetProperty(ref filenamesAsDescription, value); }
        }

        private string? defaultPlaats;
        public string? DefaultPlaats
        {
            get
            {
                return defaultPlaats;
            }
            set
            {
                var oldPlaats = defaultPlaats;
                SetProperty(ref defaultPlaats, value);
           
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (!ReferenceData.Plaatsen.Contains(value))
                {
                    ReferenceData.Plaatsen.Add(value);
                }

                foreach (var file in Files)
                {
                    if (string.IsNullOrWhiteSpace(file.Plaats) || (!string.IsNullOrWhiteSpace(oldPlaats) && file.Plaats == oldPlaats))
                    {
                        file.Plaats = value;
                    }
                }
            }
        }

        private string? defaultFotograaf;
        public string? DefaultFotograaf
        {
            get
            {
                return defaultFotograaf;
            }
            set
            {
                var oldPFotograaf = defaultFotograaf;

                SetProperty(ref defaultFotograaf, value);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                foreach (var file in Files)
                {
                    if (string.IsNullOrWhiteSpace(file.Fotograaf) || (!string.IsNullOrWhiteSpace(oldPFotograaf) && file.Fotograaf == oldPFotograaf))
                    {
                        file.Fotograaf = value;
                    }
                }
            }
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

        public ReferenceData ReferenceData { get; }
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
