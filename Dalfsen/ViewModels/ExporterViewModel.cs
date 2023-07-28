using Dalfsen.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Dalfsen.ViewModels
{
    public class ExporterViewModel : ViewModel
    {
        private readonly ISet<ExportableImageViewModel> files;
        private bool isExportWindowOpen;

        public ExporterViewModel()
        {
            files = new HashSet<ExportableImageViewModel>();
            ExportCommand = new DelegateCommand(() => IsExportWindowOpen = true);
            Images = new ObservableCollection<ExportableImageViewModel>();
            Images.CollectionChanged += Images_CollectionChanged;
        }

        public bool IsExportWindowOpen
        {
            get { return isExportWindowOpen; }
            set { SetProperty(ref isExportWindowOpen, value); }
        }

        public ICommand ExportCommand { get; }

        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExportableImageViewModel file in e.OldItems.OfType<ExportableImageViewModel>())
                {
                    files.Remove(file);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExportableImageViewModel file in e.NewItems.OfType<ExportableImageViewModel>())
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
            get { return Images.Count; }
        }

        public ObservableCollection<ExportableImageViewModel> Images { get; }

        public bool IsSelected(ExportableImageViewModel file)
        {
            return files.Contains(file);
        }

        public void Deselect(ExportableImageViewModel file)
        {
            Images.Remove(file);
        }

        public void Select(ExportableImageViewModel file)
        {
            Images.Add(file);
        }
    }
}
