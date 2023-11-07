using Bestandenselektie.HKD.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ReferenceData
    {
        public ReferenceData(Storage storage)
        {
            var settings = storage.ReadSettings();

            Rubrieken = new ObservableCollection<Rubriek>(settings.Rubrieken.OrderBy(x => x.Naam));
            Categorieen = new ObservableCollection<string>(settings.Categorieen.OrderBy(x => x));
            Collecties = new ObservableCollection<string>(settings.Collecties.OrderBy(x => x));
            Plaatsen = new ObservableCollection<string>(settings.Plaatsen.OrderBy(x => x));
        }

        public void Update(Settings settings)
        {
            settings.Rubrieken.Clear();
            foreach (Rubriek rubriek in Rubrieken)
            {
                settings.Rubrieken.Add(rubriek);
            }

            settings.Categorieen.Clear();
            foreach (string categorie in Categorieen)
            {
                settings.Categorieen.Add(categorie);
            }

            settings.Collecties.Clear();
            foreach (string collectie in Collecties)
            {
                settings.Collecties.Add(collectie);
            }

            settings.Plaatsen.Clear();
            foreach (string plaats in Plaatsen)
            {
                settings.Plaatsen.Add(plaats);
            }
        }

        public ObservableCollection<Rubriek> Rubrieken { get; }
        public ObservableCollection<string> Categorieen { get; }
        public ObservableCollection<string> Collecties { get; }
        public ObservableCollection<string> Plaatsen { get; }
    }
}
