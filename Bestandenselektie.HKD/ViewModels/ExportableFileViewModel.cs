using Bestandenselektie.HKD.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bestandenselektie.HKD.ViewModels
{
    public abstract class ExportableFileViewModel : ViewModel
    {
        private static readonly CultureInfo DutchCulture = new CultureInfo("nl-NL");

        private readonly FileInfo file;
        private readonly ExporterViewModel exporter;
        private bool isSelected;
        private bool shouldExport;
        private Rubriek? rubriek;
        private Subrubriek? subrubriek;
        private string? categorie;
        private string? collectie;
        private string? titel;
        private string? fotograaf;
        private string? datering;
        private string? plaats;
        private string? archieflocatie;

        protected ExportableFileViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter, ReferenceData referenceData)
        {
            this.file = file;
            this.exporter = exporter;
            ReferenceData = referenceData;
            ParentViewModel = parentViewModel;
            Name = file.Name;
            Extension = file.Extension;
            FullPath = file.FullName!;
            Directory = file.Directory!;
            isSelected = exporter.IsSelected(this);
            Created = string.Empty;
            Modified = string.Empty;
            CreatedAsDate = DateTime.Today;
            ModifiedAsDate = DateTime.Today;

            PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(IsSelected))
                {
                    ShouldExport = IsSelected;

                    if (IsSelected)
                    {
                        exporter.Select(this);
                    }
                    else
                    {
                        exporter.Deselect(this);
                    }
                }

                if (args.PropertyName == nameof(Rubriek))
                {
                    OnPropertyChanged(nameof(HasRubriek));
                }
            };

            try
            {
                CreatedAsDate = file.CreationTime;
                ModifiedAsDate = file.LastWriteTime;
                Created = CreatedAsDate.ToString(DutchCulture.DateTimeFormat);
                Modified = ModifiedAsDate.ToString(DutchCulture.DateTimeFormat);
            }
            catch (Exception)
            {
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }

        public bool ShouldExport
        {
            get { return shouldExport; }
            set { SetProperty(ref shouldExport, value); }
        }

        public DirectoryInfo Directory { get; }
        public string FullPath { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Modified { get; }
        public string Created { get; }
        public DateTime ModifiedAsDate { get; }
        public DateTime CreatedAsDate { get; }
        public virtual string? Dimensions { get; }
        public string? Target { get; private set; }

        public Rubriek? Rubriek
        {
            get { return rubriek; }
            set
            {
                if (value != null)
                {
                    Subrubriek = value.Subrubrieken.SingleOrDefault(s => s.Naam.Equals(Subrubriek?.Naam));
                }

                SetProperty(ref rubriek, value);
            }
        }

        public bool HasRubriek
        {
            get
            {
                return Rubriek != null && !string.IsNullOrWhiteSpace(Rubriek.Naam);
            }
        }

        private string? newRubriek;
        public string? NewRubriek
        {
            get
            {
                return newRubriek;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Rubriek = null;
                    SetProperty(ref newRubriek, null);
                    return;
                }

                if (!ReferenceData.Rubrieken.Any(s => s.Naam.Equals(value, StringComparison.OrdinalIgnoreCase)))
                {
                    ReferenceData.Rubrieken.Add(new Rubriek(value));
                }

                Rubriek = ReferenceData.Rubrieken.LastOrDefault(s => s.Naam.Equals(value, StringComparison.OrdinalIgnoreCase));
                SetProperty(ref newRubriek, value);
            }
        }

        public Subrubriek? Subrubriek
        {
            get { return subrubriek; }
            set { SetProperty(ref subrubriek, value); }
        }

        private string? newSubrubriek;
        public string? NewSubrubriek
        {
            get
            {
                return newSubrubriek;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Subrubriek = null;
                    SetProperty(ref newSubrubriek, value);
                    return;
                }

                if (Rubriek != null)
                {
                    if (!Rubriek.Subrubrieken.Any(s => s.Naam.Equals(value, StringComparison.OrdinalIgnoreCase)))
                    {
                        Rubriek.Subrubrieken.Add(new Subrubriek(value));
                    }

                    Subrubriek = Rubriek.Subrubrieken.LastOrDefault(s => s.Naam.Equals(value, StringComparison.OrdinalIgnoreCase));
                }

                SetProperty(ref newSubrubriek, value);
            }
        }

        public string? Categorie
        {
            get { return categorie; }
            set { SetProperty(ref categorie, value); }
        }

        private string? licentie;
        public string? Licentie
        {
            get { return licentie; }
            set { SetProperty(ref licentie, value); }
        }

        private string? newCategorie;
        public string? NewCategorie
        {
            get
            {
                return newCategorie;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Categorie = null;
                    SetProperty(ref newCategorie, null);
                    return;
                }

                if (!ReferenceData.Categorieen.Contains(value))
                {
                    ReferenceData.Categorieen.Add(value);
                }

                Categorie = value;
                SetProperty(ref newCategorie, value);
            }
        }

        public string? Collectie
        {
            get { return collectie; }
            set { SetProperty(ref collectie, value); }
        }

        private string? newCollectie;
        public string? NewCollectie
        {
            get
            {
                return newCollectie;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Collectie = null;
                    SetProperty(ref newCollectie, value);
                    return;
                }

                if (!ReferenceData.Collecties.Contains(value))
                {
                    ReferenceData.Collecties.Add(value);
                }

                Collectie = value;
                SetProperty(ref newCollectie, value);
            }
        }

        public string? Titel
        {
            get { return titel; }
            set { SetProperty(ref titel, value); }
        }

        public string? Fotograaf
        {
            get { return fotograaf; }
            set { SetProperty(ref fotograaf, value); }
        }

        public string? Datering
        {
            get { return datering; }
            set { SetProperty(ref datering, value); }
        }

        public string? Plaats
        {
            get { return plaats; }
            set { SetProperty(ref plaats, value); }
        }


        private string? newPlaats;
        public string? NewPlaats
        {
            get
            {
                return newPlaats;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Plaats = null;
                    SetProperty(ref newPlaats, value);
                    return;
                }

                if (!ReferenceData.Plaatsen.Contains(value))
                {
                    ReferenceData.Plaatsen.Add(value);
                }

                Plaats = value;
                SetProperty(ref newPlaats, value);
            }
        }

        public string? Archieflocatie
        {
            get { return archieflocatie; }
            set { SetProperty(ref archieflocatie, value); }
        }

        public string Size
        {
            get
            {
                return file.Length.ToSize();
            }
        }

        public long SizeInBytes
        {
            get { return file.Length; }
        }

        public ExplorerViewModel ParentViewModel { get; }
        public ReferenceData ReferenceData { get; }

        public void CopyTo(string targetDirectory)
        {
            Target = DetermineTarget(targetDirectory);
            if (Target == null)
            {
                return;
            }

            try
            {
                File.Copy(FullPath, Target, true);
            }
            catch (Exception)
            {
            }
        }

        private string? DetermineTarget(string targetDirectory)
        {
            string target = Path.Combine(targetDirectory, file.Name);

            if (File.Exists(target))
            {
                if (exporter.ConflictResolutions[0])
                {
                    return EnsureUniqueFileName(targetDirectory);
                }
                if (exporter.ConflictResolutions[1])
                {
                    return target;
                }
                if (exporter.ConflictResolutions[2])
                {
                    return null;
                }
            }

            return target;
        }

        private string EnsureUniqueFileName(string targetDirectory)
        {
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            var extension = Path.GetExtension(file.Name);

            int i = 1;
            while (File.Exists(Path.Combine(targetDirectory, $"{filename}_{i}{extension}")))
            {
                i++;
            }

            return Path.Combine(targetDirectory, $"{filename}_{i}{extension}");
        }

        public override int GetHashCode()
        {
            return file.FullName.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ExportableFileViewModel that)
            {
                return file.FullName.Equals(that.file.FullName, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}
