using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using yrwestillhere;

namespace yrwestillhere
{
    public partial class MainWindow : Window
    {
        private List<Allampolgar> lakossag = new List<Allampolgar>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeComboBox();
            LoadData();
            BindDataToGrid();
        }

        private void LoadData()
        {
            try
            {
                string filePath = @"..\..\..\src\bevölkerung.txt";
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines.Skip(1)) // Az első sor a fejlécek
                {
                    lakossag.Add(new Allampolgar(line));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatfájl beolvasása során: {ex.Message}");
            }
        }

        private void InitializeComboBox()
        {
            for (int i = 1; i <= 45; i++)
            {
                FeladatComboBox.Items.Add($"{i}.");
            }
        }

        private void BindDataToGrid()
        {
            MegoldasTeljes.ItemsSource = lakossag;
        }

        private void FeladatComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
            MegoldasLista.Items.Clear();
            MegoldasMondatos.Content = string.Empty;

            if (FeladatComboBox.SelectedItem == null) return;

            string selectedTask = FeladatComboBox.SelectedItem.ToString().TrimEnd('.');
            string methodName = $"Feladat{selectedTask}";

            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(this, null); 
        }



        private void Feladat1()
        {
            var maxIncome = lakossag.Max(a => a.NettoJovedelem);
            MegoldasMondatos.Content = $"A legmagasabb nettó éves jövedelem: {maxIncome:F2} EUR.";
        }

        private void Feladat2()
        {
            var avgIncome = lakossag.Average(a => a.NettoJovedelem);
            MegoldasMondatos.Content = $"Az átlagos nettó éves jövedelem: {avgIncome:F2} EUR.";
        }

        private void Feladat3()
        {
            var groupedByRegion = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new { Tartomany = g.Key, Count = g.Count() });

            foreach (var group in groupedByRegion)
            {
                MegoldasLista.Items.Add($"{group.Tartomany}: {group.Count} állampolgár");
            }
        }

        private void Feladat4()
        {
            var angolaiak = lakossag.Where(a => a.Nemzetiseg == "angolai");
            foreach (var angolai in angolaiak)
            {
                MegoldasLista.Items.Add(angolai.ToString(false));
            }
        }

        private void Feladat5()
        {
            var minYear = lakossag.Min(a => a.SzuletesiEv);
            var youngest = lakossag.Where(a => a.SzuletesiEv == minYear);

            foreach (var person in youngest)
            {
                MegoldasLista.Items.Add(person.ToString(false));
            }
        }

        private void Feladat6()
        {
            var nonSmokers = lakossag.Where(a => !a.Dohanyzik);

            foreach (var person in nonSmokers)
            {
                MegoldasLista.Items.Add($"ID: {person.Id}, Havi jövedelem: {person.NettoJovedelem / 12:F2} EUR");
            }
        }

        private void Feladat7()
        {
            var filtered = lakossag
                .Where(a => a.Tartomany == "Bajorország" && a.NettoJovedelem > 30000)
                .OrderBy(a => a.IskolaiVegzettseg);

            foreach (var person in filtered)
            {
                MegoldasLista.Items.Add(person.ToString(false));
            }
        }

        private void Feladat8()
        {
            var maleCitizens = lakossag.Where(a => a.Nem == "férfi");
            foreach (var person in maleCitizens)
            {
                MegoldasLista.Items.Add(person.ToString(true));
            }
        }

        private void Feladat9()
        {
            var bajorWomen = lakossag.Where(a => a.Tartomany == "Bajorország" && a.Nem == "nő");
            foreach (var woman in bajorWomen)
            {
                MegoldasLista.Items.Add(woman.ToString(false));
            }
        }

        private void Feladat10()
        {
            var top10NonSmokers = lakossag
                .Where(a => !a.Dohanyzik)
                .OrderByDescending(a => a.NettoJovedelem)
                .Take(10);

            foreach (var person in top10NonSmokers)
            {
                MegoldasLista.Items.Add(person.ToString(true));
            }
        }

        private void Feladat11()
        {
            var top5Oldest = lakossag
                .OrderBy(a => a.SzuletesiEv)
                .Take(5);

            foreach (var person in top5Oldest)
            {
                MegoldasLista.Items.Add(person.ToString(true));
            }
        }

        private void Feladat12()
        {
            var groupedByEthnicity = lakossag
                .Where(a => a.Nemzetiseg == "német")
                .GroupBy(a => a.Nepcsoport);

            foreach (var group in groupedByEthnicity)
            {
                MegoldasLista.Items.Add($"{group.Key}:");
                foreach (var citizen in group)
                {
                    string szavazo = citizen.AktivSzavazo ? "aktív szavazó" : "nem aktív szavazó";
                    MegoldasLista.Items.Add($"  {szavazo}, Politikai nézet: {citizen.PolitikaiNezet}");
                }
            }
        }

        private void Feladat13()
        {
            var avgBeerConsumption = lakossag
                .Where(a => a.Nem == "férfi")
                .Average(a => a.SorFogyasztasEvente);

            MegoldasMondatos.Content = $"Az éves átlagos sörfogyasztás a férfiak körében: {avgBeerConsumption:F2} liter.";
        }

        private void Feladat14()
        {
            var groupedByEducation = lakossag
                .OrderBy(a => a.IskolaiVegzettseg)
                .GroupBy(a => a.IskolaiVegzettseg);

            foreach (var group in groupedByEducation)
            {
                MegoldasLista.Items.Add($"{group.Key}:");
                foreach (var person in group)
                {
                    MegoldasLista.Items.Add($"  {person}");
                }
            }
        }

        private void Feladat15()
        {
            var top3HighIncome = lakossag
                .OrderByDescending(a => a.NettoJovedelem)
                .Take(3);
            var top3LowIncome = lakossag
                .OrderBy(a => a.NettoJovedelem)
                .Take(3);

            MegoldasLista.Items.Add("Legmagasabb jövedelmek:");
            foreach (var person in top3HighIncome)
            {
                MegoldasLista.Items.Add(person.ToString(false));
            }

            MegoldasLista.Items.Add("Legalacsonyabb jövedelmek:");
            foreach (var person in top3LowIncome)
            {
                MegoldasLista.Items.Add(person.ToString(false));
            }
        }
        private void Feladat16()
        {
            int totalCitizens = lakossag.Count;
            int activeVoters = lakossag.Count(a => a.AktivSzavazo);

            double percentage = (double)activeVoters / totalCitizens * 100;
            MegoldasMondatos.Content = $"Az aktív szavazók aránya: {percentage:F2}%.";
        }

        private void Feladat17()
        {
            var groupedVoters = lakossag
                .Where(a => a.AktivSzavazo)
                .OrderBy(a => a.Tartomany)
                .GroupBy(a => a.Tartomany);

            foreach (var group in groupedVoters)
            {
                MegoldasLista.Items.Add($"{group.Key}:");
                foreach (var citizen in group)
                {
                    MegoldasLista.Items.Add($"  {citizen}");
                }
            }
        }

        private void Feladat18()
        {
            int currentYear = DateTime.Now.Year;
            double averageAge = lakossag.Average(a => currentYear - a.SzuletesiEv);

            MegoldasMondatos.Content = $"Az átlagos életkor: {averageAge:F2} év.";
        }

        private void Feladat19()
        {
            var groupedByRegion = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Region = g.Key,
                    AvgIncome = g.Average(a => a.NettoJovedelem),
                    Population = g.Count()
                });

            var highestIncomeRegion = groupedByRegion
                .OrderByDescending(g => g.AvgIncome)
                .ThenByDescending(g => g.Population)
                .First();

            MegoldasMondatos.Content = $"A legmagasabb átlagjövedelem tartománya: {highestIncomeRegion.Region}, " +
                                       $"Átlagos jövedelem: {highestIncomeRegion.AvgIncome:F2} EUR, " +
                                       $"Lakosság száma: {highestIncomeRegion.Population}.";
        }

        private void Feladat20()
        {
            double averageWeight = lakossag.Average(a => a.Suly);

            var sortedWeights = lakossag
                .Select(a => a.Suly)
                .OrderBy(w => w)
                .ToList();

            double medianWeight = sortedWeights.Count % 2 == 0
                ? (sortedWeights[sortedWeights.Count / 2 - 1] + sortedWeights[sortedWeights.Count / 2]) / 2.0
                : sortedWeights[sortedWeights.Count / 2];

            MegoldasMondatos.Content = $"Az átlagos súly: {averageWeight:F2} kg, Medián súly: {medianWeight:F2} kg.";
        }

        private void Feladat21()
        {
            var activeVotersAvgBeer = lakossag
                .Where(a => a.AktivSzavazo && a.SorFogyasztasEvente != -1)
                .Average(a => a.SorFogyasztasEvente);

            var nonVotersAvgBeer = lakossag
                .Where(a => !a.AktivSzavazo && a.SorFogyasztasEvente != -1)
                .Average(a => a.SorFogyasztasEvente);

            string decision = activeVotersAvgBeer > nonVotersAvgBeer
                ? "Az aktív szavazók fogyasztanak több sört."
                : "A nem szavazók fogyasztanak több sört.";

            MegoldasLista.Items.Add($"Aktív szavazók átlagos sörfogyasztása: {activeVotersAvgBeer:F2} liter.");
            MegoldasLista.Items.Add($"Nem szavazók átlagos sörfogyasztása: {nonVotersAvgBeer:F2} liter.");
            MegoldasMondatos.Content = decision;
        }


        private void Feladat22()
        {
            var maleAvgHeight = lakossag
                .Where(a => a.Nem == "férfi")
                .Average(a => a.Magassag);

            var femaleAvgHeight = lakossag
                .Where(a => a.Nem == "nő")
                .Average(a => a.Magassag);

            MegoldasMondatos.Content = $"Átlagos magasság férfiak: {maleAvgHeight:F2} cm, nők: {femaleAvgHeight:F2} cm.";
        }

        private void Feladat23()
        {
            var groupedByEthnicity = lakossag.GroupBy(a => a.Nepcsoport)
                .Select(g => new
                {
                    Ethnicity = g.Key,
                    Count = g.Count(),
                    AvgAge = g.Average(a => DateTime.Now.Year - a.SzuletesiEv)
                })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.AvgAge)
                .First();

            MegoldasMondatos.Content = $"A legnépesebb népcsoport: {groupedByEthnicity.Ethnicity}, " +
                                       $"Létszám: {groupedByEthnicity.Count}, Átlagos életkor: {groupedByEthnicity.AvgAge:F2} év.";
        }


        private void Feladat24()
        {
            var smokerAvgIncome = lakossag
                .Where(a => a.Dohanyzik)
                .Average(a => a.NettoJovedelem);

            var nonSmokerAvgIncome = lakossag
                .Where(a => !a.Dohanyzik)
                .Average(a => a.NettoJovedelem);

            MegoldasMondatos.Content = $"Dohányzók átlagos nettó éves jövedelme: {smokerAvgIncome:F2} EUR, " +
                                       $"Nem dohányzók: {nonSmokerAvgIncome:F2} EUR.";
        }

        private void Feladat25()
        {
            var avgPotatoConsumption = lakossag
                .Where(a => a.KrumpliFogyasztasEvente != -1)
                .Average(a => a.KrumpliFogyasztasEvente);

            MegoldasLista.Items.Add($"Átlagos krumplifogyasztás: {avgPotatoConsumption:F2} kg évente.");

            var aboveAvgConsumers = lakossag
                .Where(a => a.KrumpliFogyasztasEvente > avgPotatoConsumption)
                .Take(15);

            foreach (var person in aboveAvgConsumers)
            {
                MegoldasLista.Items.Add(person.ToString(false));
            }
        }

        private void Feladat26()
        {
            var avgAgeByRegion = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Region = g.Key,
                    AvgAge = g.Average(a => DateTime.Now.Year - a.SzuletesiEv)
                });

            foreach (var region in avgAgeByRegion)
            {
                MegoldasLista.Items.Add($"{region.Region}: {region.AvgAge:F2} év átlagéletkor.");
            }
        }

        private void Feladat27()
        {
            var idosebbek = lakossag
                .Where(a => a.GetEletkor(a.SzuletesiEv) > 50)
                .Select(a => new { a.Id, a.Nem, a.SzuletesiEv, a.Suly, a.Magassag })
                .Take(5);

            foreach (var szemely in idosebbek)
            {
                MegoldasLista.Items.Add($"{szemely.Id}, {szemely.Nem}, {szemely.SzuletesiEv}, {szemely.Suly}, {szemely.Magassag}");
            }

            MegoldasMondatos.Content = $"Összesen {idosebbek.Count()} állampolgár 50 év feletti.";
        }

        private void Feladat28()
        {
            var dohanyzoNok = lakossag
                .Where(a => a.Dohanyzik && a.Nem == "nő");

            foreach (var no in dohanyzoNok)
            {
                MegoldasLista.Items.Add(no.ToString(false));
            }

            if (dohanyzoNok.Any()) 
            {
                var maxJovedelem = dohanyzoNok.Max(a => a.NettoJovedelem);
                MegoldasMondatos.Content = $"Maximális nettó éves jövedelem: {maxJovedelem} Ft";
            }
            else
            {
                MegoldasMondatos.Content = "Nincsenek dohányzó nők az adatok között.";
            }
        }



        private void Feladat29()
        {
            var legnagyobbSorivok = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Tartomany = g.Key,
                    MaxSorFogyaszto = g.OrderByDescending(a => a.SorFogyasztasEvente).First()
                });

            foreach (var tartomany in legnagyobbSorivok)
            {
                MegoldasLista.Items.Add($"{tartomany.Tartomany}, {tartomany.MaxSorFogyaszto.Id}, {tartomany.MaxSorFogyaszto.SorFogyasztasEvente} liter");
            }
        }

        private void Feladat30()
        {
            var legidosebbNo = lakossag.Where(a => a.Nem == "nő").OrderByDescending(a => a.GetEletkor(a.SzuletesiEv)).FirstOrDefault();
            var legidosebbFerfi = lakossag.Where(a => a.Nem == "férfi").OrderByDescending(a => a.GetEletkor(a.SzuletesiEv)).FirstOrDefault();

            if (legidosebbNo != null)
                MegoldasLista.Items.Add($"Legidősebb nő: {legidosebbNo.ToString(true)}");

            if (legidosebbFerfi != null)
                MegoldasLista.Items.Add($"Legidősebb férfi: {legidosebbFerfi.ToString(true)}");
        }
        private void Feladat31()
        {
            var nemzetisegek = lakossag
                .Select(a => a.Nemzetiseg)
                .Distinct()
                .OrderByDescending(n => n);

            foreach (var nemzetiseg in nemzetisegek)
            {
                MegoldasLista.Items.Add(nemzetiseg);
            }
        }

        private void Feladat32()
        {
            var tartomanyok = lakossag
                .GroupBy(a => a.Tartomany)
                .OrderBy(g => g.Count())
                .Select(g => g.Key);

            foreach (var tartomany in tartomanyok)
            {
                MegoldasLista.Items.Add(tartomany);
            }
        }

        private void Feladat33()
        {
            var topJovedelem = lakossag
                .OrderByDescending(a => a.NettoJovedelem)
                .Take(3)
                .Select(a => new { a.Id, a.NettoJovedelem });

            foreach (var szemely in topJovedelem)
            {
                MegoldasLista.Items.Add($"{szemely.Id}, {szemely.NettoJovedelem} Ft");
            }
        }

        private void Feladat34()
        {
            var atlagSulya = lakossag
                .Where(a => a.Nem == "férfi" && a.KrumpliFogyasztasEvente > 55)
                .Average(a => a.Suly);

            MegoldasMondatos.Content = $"Átlagos súly: {atlagSulya:F2} kg";
        }

        private void Feladat35()
        {
            var tartomanyEletkor = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Tartomany = g.Key,
                    MinKor = g.Min(a => a.GetEletkor(a.SzuletesiEv))
                });

            foreach (var tartomany in tartomanyEletkor)
            {
                MegoldasLista.Items.Add($"{tartomany.Tartomany}: {tartomany.MinKor} év");
            }
        }

        private void Feladat36()
        {
            var nemzetisegTartomanyParok = lakossag
                .Select(a => new { a.Nemzetiseg, a.Tartomany })
                .Distinct();

            foreach (var par in nemzetisegTartomanyParok)
            {
                MegoldasLista.Items.Add($"{par.Nemzetiseg}, {par.Tartomany}");
            }
        }


        private void Feladat37()
        {
            var atlagJovedelem = lakossag.Average(a => a.NettoJovedelem);
            var atlagFelettiek = lakossag.Where(a => a.NettoJovedelem > atlagJovedelem);

            foreach (var szemely in atlagFelettiek)
            {
                MegoldasLista.Items.Add(szemely.ToString(false));
            }

            MegoldasMondatos.Content = $"Átlagos jövedelem: {atlagJovedelem:F2} Ft, Szűrt állampolgárok: {atlagFelettiek.Count()} fő.";
        }


        private void Feladat38()
        {
            var noiSzam = lakossag.Count(a => a.Nem == "nő");
            var ferfiSzam = lakossag.Count(a => a.Nem == "férfi");

            MegoldasMondatos.Content = $"Nők száma: {noiSzam}, Férfiak száma: {ferfiSzam}.";
        }


        private void Feladat39()
        {
            var tartomanyMaxJovedelem = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Tartomany = g.Key,
                    MaxJovedelem = g.Max(a => a.NettoJovedelem)
                })
                .OrderByDescending(g => g.MaxJovedelem);

            foreach (var tartomany in tartomanyMaxJovedelem)
            {
                MegoldasLista.Items.Add($"{tartomany.Tartomany}: {tartomany.MaxJovedelem} Ft");
            }
        }

        private void Feladat40()
        {
            var nemetJovedelem = lakossag.Where(a => a.Nemzetiseg == "német").Sum(a => a.NettoJovedelem / 12);
            var nemNemetJovedelem = lakossag.Where(a => a.Nemzetiseg != "német").Sum(a => a.NettoJovedelem / 12);
            var arany = (nemetJovedelem / (nemetJovedelem + nemNemetJovedelem)) * 100;

            MegoldasMondatos.Content = $"Német jövedelem arány: {arany:F2}%";
        }

        private void Feladat41()
        {
            var osszesTorokSzavazo = lakossag
                .Where(a => a.Nemzetiseg == "török" && a.AktivSzavazo)
                .ToList();

            if (osszesTorokSzavazo.Any())
            {
                var random = new Random();
                var torokSzavazok = osszesTorokSzavazo
                    .OrderBy(_ => random.Next())
                    .Take(10);

                foreach (var szemely in torokSzavazok)
                {
                    MegoldasLista.Items.Add(szemely.ToString(true));
                }
            }
            else
            {
                MegoldasLista.Items.Add("Nincsenek aktív török szavazók az adatok között.");
            }
        }


        private void Feladat42()
        {
            var atlagSorFogyasztas = lakossag.Average(a => a.SorFogyasztasEvente);
            var atlagFelettiSorivok = lakossag
                .Where(a => a.SorFogyasztasEvente > atlagSorFogyasztas)
                .OrderBy(_ => Guid.NewGuid())
                .Take(5);

            foreach (var szemely in atlagFelettiSorivok)
            {
                MegoldasLista.Items.Add(szemely.ToString(true));
            }

            MegoldasMondatos.Content = $"Átlagos sörfogyasztás: {atlagSorFogyasztas:F2} liter.";
        }

        private void Feladat43()
        {
            var teljesAtlagJovedelem = lakossag.Average(a => a.NettoJovedelem);
            var tartomanyok = lakossag
                .GroupBy(a => a.Tartomany)
                .Select(g => new
                {
                    Tartomany = g.Key,
                    MinJovedelem = g.Min(a => a.NettoJovedelem)
                })
                .Where(g => g.MinJovedelem > teljesAtlagJovedelem)
                .OrderBy(_ => Guid.NewGuid())
                .Take(2);

            foreach (var tartomany in tartomanyok)
            {
                MegoldasLista.Items.Add($"{tartomany.Tartomany}: {tartomany.MinJovedelem} Ft");
            }

            MegoldasMondatos.Content = $"Teljes lakosság átlagjövedelme: {teljesAtlagJovedelem:F2} Ft.";
        }

        private void Feladat44()
        {
            var ismeretlenVegzettsegu = lakossag
                .Where(a => string.IsNullOrEmpty(a.IskolaiVegzettseg) ||
                            a.IskolaiVegzettseg.Trim() == "")
                .ToList();

            if (ismeretlenVegzettsegu.Any())
            {
                var random = new Random();
                var veletlenHarom = ismeretlenVegzettsegu
                    .OrderBy(_ => random.Next())
                    .Take(3);

                foreach (var szemely in veletlenHarom)
                {
                    MegoldasLista.Items.Add(szemely.ToString(true));
                }
            }
            else
            {
                MegoldasMondatos.Content=("Nincsenek olyan állampolgárok, akiknek ismeretlen az iskolai végzettsége.");
            }
        }


        private void Feladat45()
        {
            var egyetemiNok = lakossag
                .Where(a => a.Nem == "nő" && a.IskolaiVegzettseg == "Universität" && a.Nemzetiseg != "bajor")
                .Take(5)
                .ToList();

            foreach (var szemely in egyetemiNok)
            {
                MegoldasLista.Items.Add(szemely.ToString(true));
            }

            var elsoNoJovedelem = egyetemiNok.First().NettoJovedelem;

            var nemetNok = lakossag
                .Where(a => a.Nem == "nő" && a.Nemzetiseg == "német" && a.NettoJovedelem > elsoNoJovedelem)
                .OrderBy(_ => Guid.NewGuid())
                .Take(3);

            foreach (var szemely in nemetNok)
            {
                MegoldasLista.Items.Add($"{szemely.Id}, {szemely.Nem}, {szemely.SzuletesiEv}, {szemely.Suly}, {szemely.Magassag}");
            }
        }



    }
}