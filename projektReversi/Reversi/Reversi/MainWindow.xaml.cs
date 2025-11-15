using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Reversi
{
    public partial class MainWindow : Window
    {
        #region Tworzenie planszy
        private ReversiSilnikAI silnik = new ReversiSilnikAI(1, 8, 8);
        private bool graPrzeciwkoKomputerowi = true;
        private DispatcherTimer timer;

        private SolidColorBrush[] kolory = { Brushes.Ivory, Brushes.Green, Brushes.Sienna };
        string[] nazwyGraczy = { "", "zielony", "brązowy" };

        private Button[,] plansza;

        private bool planszaZainicjowana
        {
            get
            {
                return plansza[silnik.SzerokośćPlanszy - 1, silnik.WysokośćPlanszy - 1] != null;
            }
        }

        private void uzgodnijZawartośćPlanszy()
        {
            if (!planszaZainicjowana) return;

            for (int i = 0; i < silnik.SzerokośćPlanszy; i++)
                for (int j = 0; j < silnik.WysokośćPlanszy; j++)
                {
                    plansza[i, j].Background = kolory[silnik.PobierzStanPola(i, j)];
                    plansza[i, j].Content = silnik.PobierzStanPola(i, j).ToString();
                }

            przyciskKolorGracza.Background = kolory[silnik.NumerGraczaWykonującegoNastępnyRuch]; ; 
            liczbaPolZielony.Text = silnik.LiczbaPólGracz1.ToString();
            LiczbaPolBrązowy.Text = silnik.LiczbaPólGracz2.ToString();
        }

        private struct WspółrzędnePola
        {
            public int Poziomo, Pionowo;
        }
        private static string symbolPola(int poziomo, int pionowo)
        {
            if (poziomo > 25 || pionowo > 8) return "(" + poziomo.ToString() + "," + pionowo.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[poziomo] + "123456789"[pionowo];
        }
        private void kliknięciePolaPlanszy(object sender, RoutedEventArgs e)
        {
            Button klikniętyPrzycisk = sender as Button;
            WspółrzędnePola współrzędne = (WspółrzędnePola)klikniętyPrzycisk.Tag;
            int klikniętePoziomo = współrzędne.Poziomo;
            int klikniętePionowo = współrzędne.Pionowo; 

            int zapamiętanyNumerGracza = silnik.NumerGraczaWykonującegoNastępnyRuch;
            if (silnik.PołóżKamień(klikniętePoziomo, klikniętePionowo))
            {
                uzgodnijZawartośćPlanszy();

                switch (zapamiętanyNumerGracza)
                {
                    case 1:
                        listaRuchówZielony.Items.Add(symbolPola(klikniętePoziomo, klikniętePionowo));
                        break;
                    case 2:
                        listaRuchówBrązowy.Items.Add(symbolPola(klikniętePoziomo, klikniętePionowo));
                        break;
                }
                listaRuchówZielony.SelectedIndex = listaRuchówZielony.Items.Count - 1;
                listaRuchówBrązowy.SelectedIndex = listaRuchówBrązowy.Items.Count - 1;

                ReversiSilnik.SytuacjaNaPlanszy sytuacjaNaPlanszy = silnik.ZbadajSytuacjęNaPlanszy();
                bool koniecGry = false;
                switch (sytuacjaNaPlanszy)
                {
                    case ReversiSilnik.SytuacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu:
                        MessageBox.Show("Gracz " + nazwyGraczy[silnik.NumerGraczaWykonującegoNastępnyRuch] + " zmuszony jest do oddania ruchu");
                        silnik.Pasuj();
                        uzgodnijZawartośćPlanszy();
                        break;
                    case ReversiSilnik.SytuacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu:
                        MessageBox.Show("Obaj gracze nie mogą wykonać ruchu");
                        koniecGry = true;
                        break;
                    case ReversiSilnik.SytuacjaNaPlanszy.WszystkiePolaPlanszySąZajęte:
                        koniecGry = true;
                        break;
                }

                if (koniecGry)
                {
                    int numerZwycięzcy = silnik.NumerGraczaMającegoPrzewagę;
                    if (numerZwycięzcy != 0) MessageBox.Show("Wygrał gracz " + nazwyGraczy[numerZwycięzcy], Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    else MessageBox.Show("Remis", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (MessageBox.Show("Czy rozpocząć grę od nowa?", "Reversi", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        przygotowaniePlanszyDoNowejGry(1, silnik.SzerokośćPlanszy, silnik.WysokośćPlanszy);
                    }
                    else
                    {
                        planszaSiatka.IsEnabled = false;
                        przyciskKolorGracza.IsEnabled = false;
                    }
                }
                else { 
                if (graPrzeciwkoKomputerowi && silnik.NumerGraczaWykonującegoNastępnyRuch == 2)
                {
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = new System.TimeSpan(0, 0, 0, 0, 300);
                        timer.Tick += (_sender, _e) => { timer.IsEnabled = false; wykonajNajlepszyRuch(); };
                    }
                    timer.Start();
                }
                }

            }
        }

        private void przygotowaniePlanszyDoNowejGry(int numerGraczaRozpoczynającego, int szerokośćPlanszy = 8, int wysokośćPlanszy = 8)
        {
            silnik = new ReversiSilnikAI(numerGraczaRozpoczynającego, szerokośćPlanszy, wysokośćPlanszy);
            listaRuchówZielony.Items.Clear();
            listaRuchówBrązowy.Items.Clear();
            uzgodnijZawartośćPlanszy();
            planszaSiatka.IsEnabled = true;
            przyciskKolorGracza.IsEnabled = true;
        }
        #endregion

        #region Ruch Gracza
        private WspółrzędnePola? ustalNajlepszyRuch()
        {
            if (!planszaSiatka.IsEnabled) return null;

            if (silnik.LiczbaPustychPól == 0)
            {
                MessageBox.Show("Nie ma już wolnych pól na planszy", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            try
            {
                int poziomo, pionowo;
                silnik.ProponujNajlepszyRuch(out poziomo, out pionowo);
                return new WspółrzędnePola() { Poziomo = poziomo, Pionowo = pionowo };
            }
            catch
            {
                MessageBox.Show("Bieżący gracz nie może wykonać ruchu", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void zaznaczNajlepszyRuch()
        {
            WspółrzędnePola? współrzędnePola = ustalNajlepszyRuch();
            if (współrzędnePola.HasValue)
            {
                SolidColorBrush kolorPodpowiedzi = kolory[silnik.NumerGraczaWykonującegoNastępnyRuch].Lerp(kolory[0], 0.5f);
                plansza[współrzędnePola.Value.Poziomo, współrzędnePola.Value.Pionowo].Background = kolorPodpowiedzi;
            }
        }

        private void wykonajNajlepszyRuch()
        {
            WspółrzędnePola? wspolrzednePola = ustalNajlepszyRuch();
            if (wspolrzednePola.HasValue)
            {
                Button przycisk = plansza[wspolrzednePola.Value.Poziomo, wspolrzednePola.Value.Pionowo];
                kliknięciePolaPlanszy(przycisk, null);
            }
        }
        #endregion

        private void przyciskKolorGracza_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) wykonajNajlepszyRuch();
            else zaznaczNajlepszyRuch();
        }

        #region Metody zdarzeniowe menu głównego
        private void MenuItem_NowaGraDla1Gracza_RozpoczynaKomputer_Click(object sender, RoutedEventArgs e)
        {
            graPrzeciwkoKomputerowi = true;
            Title = "Reversi - 1 gracz";
            przygotowaniePlanszyDoNowejGry(2);
            wykonajNajlepszyRuch(); 
        }
        private void MenuItem_NowaGraDla1Gracza_Click(object sender, RoutedEventArgs e)
        {
            graPrzeciwkoKomputerowi = true;
            Title = "Reversi - 1 gracz";
            przygotowaniePlanszyDoNowejGry(1);
        }
        private void MenuItem_NowaGraDla2Graczy_Click(object sender, RoutedEventArgs e)
        {
            Title = "Reversi - 2 graczy";
            graPrzeciwkoKomputerowi = false;
            przygotowaniePlanszyDoNowejGry(1);
        }
        private void MenuItem_Zamknij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MenuItem_PodpowiedźRuchu_Click(object sender, RoutedEventArgs e)
        {
            zaznaczNajlepszyRuch();
        }
        private void MenuItem_RuchWykonanyPrzezKomputer_Click(object sender, RoutedEventArgs e)
        {
            wykonajNajlepszyRuch();
        }
        private void MeniItem_ZasadyGry_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "W grze Reversi gracze zajmują na przemian pola planszy, przejmując przy tym wszystkie pola przeciwnika znajujące sie między nowo zajętym polem, a innymi polami gracza wykonującego ruch. Celem gry jest zdobycie większej liczby pól niż przeciwnik. \n" +
                "Gracz może zająć jedynie takie pole, które pozwoli mu przejąć przynajmniej jedno pole przeciwnika. Jeżeli takiego pola nie ma, musi oddać ruch.\n" +
                "Gra kończy się w momencie zajęcia wszystkich pól lub gdy żaden z graczy nie może wykonać ruchu. \n",
                "Reversi - Zasady gry");

        }
        private void MenuItem_StrategiaKomputera_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Komputer kieruje się nastepującymi priorytetami (od najwyższego): \n" +
                "1. Ustawić pionek w rogu.\n" +
                "2. Unikać ustawienia pionka tuż przy rogu. \n" +
                "3. Ustawić pionek przy krawędzi planszy. \n" +
                "4. Unikać ustawienia pionka w wierszu lub kolumnie oddalonej o jedno pole od krawędzi planszy. \n" +
                "5. Wybierać pole, w wyniku którego zdobyta zostanie największa liczba pól przeciwnika. \n",
                "Reversi - Strategia Komputera");
        }
        private void MenuItem_OProgramie_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("Adam Horoszkiewicz 2025");
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < silnik.SzerokośćPlanszy; i++)
                planszaSiatka.ColumnDefinitions.Add(new ColumnDefinition());
            for (int j = 0; j < silnik.WysokośćPlanszy; j++)
                planszaSiatka.RowDefinitions.Add(new RowDefinition());

            // tworzenie przycisków
            plansza = new Button[silnik.SzerokośćPlanszy, silnik.WysokośćPlanszy];
            for (int i = 0; i < silnik.SzerokośćPlanszy; i++)
                for (int j = 0; j < silnik.WysokośćPlanszy; j++)
                {
                    Button przycisk = new Button();
                    przycisk.Margin = new Thickness(0);
                    planszaSiatka.Children.Add(przycisk);
                    Grid.SetColumn(przycisk, i);
                    Grid.SetRow(przycisk, j);
                    przycisk.Tag = new WspółrzędnePola { Poziomo = i, Pionowo = j };
                    przycisk.Click += new RoutedEventHandler(kliknięciePolaPlanszy);
                    plansza[i, j] = przycisk;
                }
           

            uzgodnijZawartośćPlanszy();
    
        }


    }

}