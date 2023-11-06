using System.Collections.ObjectModel;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ReferenceData
    {
        public ReferenceData()
        {
            Rubrieken = new ObservableCollection<Rubriek> {
                new Rubriek("Agrarisch", "Boerderij", "Tuinbouw", "Volkstuinen", "Zorgboerderij"),
                new Rubriek("Archeologie", "Middeleeuwen", "Prehistorie" ),
                new Rubriek("Bedrijven", "Detailhandel", "Horeca", "Industrie"),
                new Rubriek("Begraafplaats", "Algemeen Ds. Smitslaan Nieuwleusen", "Algemeen Nieuwstraat Lemelerveld", "Algemeen Ruitenborghstraat Dalfsen", "Algemeen Welsum Dalfsen", "Algemeen Westeinde Nieuwleusen", "Joods Gerner Es Dalfsen", "Rooms Katholiek Hoonhorst", "Rooms Katholiek Posthoornweg Lemelerveld", "Rooms Katholiek Wilhelminastraat Dalfsen", "Dierenbegraafplaats"),
                new Rubriek("Cartografie", "Kadasterkaarten", "Landgoedkaarten", "Topografische kaarten"),
                new Rubriek("Document", "Aktes", "Archieven van verenigingen", "Krantenartikelen"),
                new Rubriek("Dorpsgezicht", "Dalfsen", "Hoonhorst", "Lemelerveld", "Nieuwleusen", "Oudleusen"),
                new Rubriek("Evenement", "Blauwe Bogen dagen", "Braderie", "Damitobeurs", "Koninginnedag / Koningsdag", "Kunst om Dalfsen", "Proef Dalfsen", "Volksfeest"),
                new Rubriek("Folklore", "Klederdracht", "Oude Ambachten / Gebruiken"),
                new Rubriek("Foto's Algemeen", "Personen / Groepsfoto", "Families algemeen", "Onbekende foto's"),
                new Rubriek("Gebeurtenis", "Begrafenis / Crematie", "Brand", "Jubileum", "Koninklijk Bezoek", "Onderscheidingen", "Rampen"),
                new Rubriek("Gebouwen", "Aula / Mortuarium", "Gemeentehuis", "Molen", "Postkantoor", "Station", "Watertoren", "Woningen"),
                new Rubriek("Gemeente", "Brandweer", "Burgemeester / Wethouder", "Ondersteunende Diensten", "Politie", "Politieke Partijen", "Raadsleden", "Vuilstortplaats"),
                new Rubriek("Gezondheidszorg", "Apotheek", "Dierenarts", "EHBO", "Fysiotherapie", "Huisarts", "Kruisvereniging", "Pedicure", "Tandarts / Mondhygiëne", "Thuiszorg", "Woonzorgcentrum"),
                new Rubriek("Infrastructuur", "Bouwrijp maken", "Brug / Sluis / Stuw", "Elektriciteit", "Gas", "Glasvezel / Kabel", "Luchtfoto", "Riolering", "Spoorweg", "Straten", "Telefoon", "Tunnel", "Verwarming", "Waterleiding", "Waterwegen", "Wegen"),
                new Rubriek("Kastelen en Havezaten", "Havezate De Melenhorst", "Havezate De Ruitenborgh", "Havezate Den Berg", "Havezate Leemcule", "Havezate Oosterveen", "Huize Ankum", "Huize Bellingeweer", "Huize De Berkenhorst", "Huize De Bese", "Huize De Broekhuizen", "Huize De Horte", "Huize Den Aalshorst", "Huize Den Berg", "Huize Gerner", "Huize Hessum", "Huize Hofwijk", "Huize Jagtlust", "Huize Mataram / Dieze", "Huize Rollecate", "Huize Vechterweerd", "Huize Vidal (De Vechtkamp)", "Kasteel Rechteren"),
                new Rubriek("Kerken en Kloosters", "Brugkerk Lemelerveld", "De Lichtbron Lemelerveld", "Gereformeerde Kerk Dalfsen", "Gereformeerde Kerk Hersteld Dalfsen", "Gereformeerde Kerk Vrijgemaakt Dalfsen", "Gereformeerde Kerk Vrijgemaakt Nieuwleusen", "Grote Kerk Nieuwleusen", "Maranathakerk Nieuwleusen", "Nederlands Hervormde Kerk Dalfsen", "Nederlands Hervormde Kerk Oudleusen", "Nederlands Gereformeerde Kerk Dalfsen", "Ontmoetingskerk Nieuwleusen", "Rooms Katholieke Kerk H. Cyriacus Dalfsen", "Rooms Katholieke Kerk H. Hart Lemelerveld", "Rooms Katholieke Kerk St. Cyriacus Hoonhorst", "Rehoboth Nieuwleusen", "Vrije Evangelisatie Dalfsen (Vechtstroom Gemeente)", "Klooster"),
                new Rubriek("Kunst en Cultuur", "Grammofoonmuseum", "Kulturhus De Mozaïek", "Kulturhus De Trefkoele+", "Kulturhus Hoonhorst", "Museum Meesters", "Museum Palthehof", "Museum Spijkerman", "Ontmoetingscentrum De Spil", "Ontmoetingscentrum De Wiekelaar", "Takt2Aero"),
                new Rubriek("Media", "Film / Video", "Kranten", "Presentaties", "Radio / TV", "Youtube"),
                new Rubriek("Monumenten", "Grafmonumenten / Gedenktekens", "Rijks- en gemeentemonumenten", "Stoeppaal"),
                new Rubriek("Natuur", "Essen", "Flora en fauna", "Landschappen / Eikenhoutwallen", "Markante bomen", "Rivierarmen", "Ruilverkaveling"),
                new Rubriek("Oorlog", "1940-1945", "Begraafplaats / Oorlogslachtoffers", "Bevrijding", "Herdenking", "Indië", "Monument"),
                new Rubriek("Religie", "Dominee", "Godsdienstleraar", "Nonnen", "Pastoor", "Zondagsschool"),
                new Rubriek("Scholen", "Christelijke Kleuter School Dalfsen", "Christelijke Kleuter School Oudleusen", "Christelijke Lagere School Ankum", "Christelijke Lagere School Dalfsen", "Christelijke Lagere School Dalmsholte", "Christelijke Lagere School Emmen", "Christelijke Lagere School Hessum", "Christelijke Lagere School Lemelerveld", "Christelijke Lagere School Oudleusen", "Gereformeerde Lagere School Dalfsen", "Nederlands Hervormde Lagere School Dalmsholte", "Openbare Lagere School Ankum", "Openbare Lagere School Dalfsen", "Openbare Lagere School Dalmsholte", "Openbare Lagere School Emmen", "Openbare Lagere School Hessum", "Openbare Lagere School Lemelerveld", "Openbare Lagere School Lenthe", "Openbare Lagere School Oudleusen", "Rooms Katholieke Jongens School Lemelerveld", "Rooms Katholieke Lagere School Dalfsen", "Rooms Katholieke Lagere School Hoonhorst", "Rooms Katholieke Lagere School Lemelerveld", "Rooms Katholieke Lagere School", "Rooms Katholieke Meisjes School Dalfsen", "Rooms Katholieke Meisjes School Lemelerveld", "Agnieten College", "Beatrixschool (Huishoudschool)", "Landbouwschool", "ULO / MAVO", "VGLO"),
                new Rubriek("Sport en Sportpark", "Badminton", "Biljart", "Bridge", "Dammen", "Fotografie", "Gymnastiek", "Handbal", "Hockey", "Jeu de Boules", "Judo", "Marathon", "Motorsport", "Paardensport", "Schaatsen", "Schaken", "Schoolvoetbal", "Sportschool", "Tafeltennis", "Tennis", "Voetbal", "Volleybal", "Wielersport", "Zweefvliegen", "Zwemmen", "Sportpark De Potkamp", "Sportpark Gerner", "Sportpark Heidepark", "Sportpark Hulsterlanden", "Sportpark Klaverblad"),
                new Rubriek("Toerisme", "Camping", "Passanten haven", "Pontje Hessum", "Zomerhuisje"),
                new Rubriek("Verenigingen", "ABTB / CBTB / OLM", "Bejaarden", "Buurt / Wijk", "Carnaval", "Historische Kring Dalfsen", "Historische Vereniging Ni'jluusn van vrogger", "Jeugd", "Muziek / Koren", "Plaatselijk Belang", "Scouting", "Sportvereniging", "Toneel / Revue", "Vakbond", "Vrouwen"),
                new Rubriek("Verkeer en Vervoer", "Ongevallen", "Spoorweg", "Weg / Water", "Zweefvliegveld")
            };

            Categorieen = new ObservableCollection<string>
            {
                "Prentbriefkaart: orig, z/w",
                "Prentbriefkaart: orig, sepia",
                "Prentbriefkaart: orig, kleur",
                "Prentbriefkaart: kopie",
                "Foto: z/w",
                "Foto: getint",
                "Foto: sepia",
                "Foto: kleur",
                "Foto: kopie",
                "Dia",
                "Digitaal",
                "Negatief",
                "Krantenartikel",
                "Pentekening",
                "Aquarel"
            };

            Collecties = new ObservableCollection<string>
            {
                "Collectie Ab Goutbeek",
                "Collectie Dr. Plate",
                "Collectie Het Oversticht",
                "Collectie J.G. Staal",
                "Collectie Jan Berends",
                "Collectie Jo Lingeman",
                "Collectie Johan Goutbeek",
                "Collectie Jospé",
                "Collectie Miep Alberts-Reyst",
                "Collectie Schoolplaten"
            };
        }

        public ObservableCollection<Rubriek> Rubrieken { get; }
        public ObservableCollection<string> Categorieen { get; }
        public ObservableCollection<string> Collecties { get; }
    }
}
