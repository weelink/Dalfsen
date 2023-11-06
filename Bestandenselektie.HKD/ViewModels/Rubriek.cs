using System.Collections.Generic;
using System.Linq;

namespace Bestandenselektie.HKD.ViewModels
{
    public class Rubriek
    {
        public Rubriek(string naam, params string[] subrubrieken)
        {
            Naam = naam;
            Subrubrieken = subrubrieken.Select(s => new Subrubriek(s)).ToList();
        }

        public override int GetHashCode()
        {
            return Naam.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is Rubriek that && Naam.Equals(that.Naam);
        }

        public override string ToString()
        {
            return Naam;
        }

        public string Naam { get; }
        public IList<Subrubriek> Subrubrieken { get; set; }
    }

    public class Subrubriek
    {
        public string Naam { get; }

        public Subrubriek(string naam)
        {
            Naam = naam;
        }

        public override int GetHashCode()
        {
            return Naam.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is Subrubriek that && Naam.Equals(that.Naam);
        }

        public override string ToString()
        {
            return Naam;
        }
    }
}
