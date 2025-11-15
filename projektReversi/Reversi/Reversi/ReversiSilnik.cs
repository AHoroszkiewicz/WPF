namespace Reversi
{
    public class ReversiSilnik
    {
        public int SzerokośćPlanszy { get; private set; }
        public int WysokośćPlanszy { get; private set; }

        public int NumerGraczaWykonującegoNastępnyRuch { get; private set; } = 1;
        private int[,] plansza;


        private bool czyWspółrzędnePolaPrawidłowe(int poziom, int pionowo)
        {
            return poziom >= 0 && poziom < SzerokośćPlanszy && 
                   pionowo >= 0 && pionowo < WysokośćPlanszy;
        }

        public int PobierzStanPola(int poziomo, int pionowo)
        {
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("Nieprawidłowe wspórzędne pola");
            return plansza[poziomo, pionowo];
        }
        private static int numerPrzeciwnika(int numerGracza)
        {
            return (numerGracza == 1) ? 2 : 1;
        }
        private void czyśćPlansze()
        {
            for (int i = 0; i < SzerokośćPlanszy; i++)
                for (int j = 0; j < WysokośćPlanszy; j++)
                    plansza[i, j] = 0;

            int srodekSzer = SzerokośćPlanszy / 2;
            int srodekWys = WysokośćPlanszy / 2;
            plansza[srodekSzer - 1, srodekWys - 1] = plansza[srodekSzer, srodekWys] = 1;
            plansza[srodekSzer - 1, srodekWys] = plansza[srodekSzer, srodekWys - 1] = 2;
        }

        public ReversiSilnik(int numerGraczaRozpoczynajacego, int szerokoscPlanszy = 8, int wysokoscPlanszy = 8)
        {
            if (numerGraczaRozpoczynajacego < 1 || numerGraczaRozpoczynajacego > 2)
                throw new Exception("Nieprawidłowy numer gracza rozpoczynajacego gre");

            SzerokośćPlanszy = szerokoscPlanszy;
            WysokośćPlanszy = wysokoscPlanszy;
            plansza = new int[SzerokośćPlanszy, WysokośćPlanszy];

            czyśćPlansze();

            NumerGraczaWykonującegoNastępnyRuch = numerGraczaRozpoczynajacego;
            obliczLiczbyPól();
        }

        private void zmieńBieżącegoGracza()
        {
            NumerGraczaWykonującegoNastępnyRuch = 
                        numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch);
        }

        protected int PołóżKamień(int poziomo, int pionowo, bool tylkoTekst)
        {
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("Nieprawidłowe współrzędne pola");

            if (plansza[poziomo, pionowo] != 0) return -1;

            int ilePólPrzyjętych = 0;

            for (int kierunekPoziomo = -1; kierunekPoziomo <= 1; kierunekPoziomo++)
                for (int kierunekPionowo = -1; kierunekPionowo <= 1; kierunekPionowo++)
                {
                    if (kierunekPoziomo == 0 && kierunekPionowo == 0) continue;

                    int i = poziomo;
                    int j = pionowo;
                    bool znalezionyKamieńPrzeciwnika = false;
                    bool znalezionyKamieńGraczaWykonującegoRuch = false;
                    bool znalezionePustePole = false;
                    bool osiagniętaKrawędźPlanszy = false;
                    do
                    {
                        i += kierunekPoziomo;
                        j += kierunekPionowo;
                        if (!czyWspółrzędnePolaPrawidłowe(i, j))
                            osiagniętaKrawędźPlanszy = true;
                        if (!osiagniętaKrawędźPlanszy)
                        {
                            if (plansza[i, j] == NumerGraczaWykonującegoNastępnyRuch)
                                znalezionyKamieńGraczaWykonującegoRuch = true;
                            if (plansza[i, j] == 0) znalezionePustePole = true;
                            if (plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonującegoNastępnyRuch))
                                znalezionyKamieńPrzeciwnika = true;
                        }
                    }
                    while (!(osiagniętaKrawędźPlanszy || znalezionyKamieńGraczaWykonującegoRuch || znalezionePustePole));

                    bool położenieKamieniaJakJestMożliwe = znalezionyKamieńPrzeciwnika && znalezionyKamieńGraczaWykonującegoRuch && !znalezionePustePole;

                    if (położenieKamieniaJakJestMożliwe)
                    {
                        int maks_indeks = Math.Max(Math.Abs(i - poziomo), Math.Abs(j - pionowo));

                        if (!tylkoTekst)
                        {
                            for (int indeks = 0; indeks < maks_indeks; indeks++)
                                plansza[poziomo + indeks * kierunekPoziomo, pionowo + indeks * kierunekPionowo] = NumerGraczaWykonującegoNastępnyRuch;
                        }

                        ilePólPrzyjętych += maks_indeks - 1;
                    }
                }
            if (ilePólPrzyjętych > 0 && !tylkoTekst)
                zmieńBieżącegoGracza();
            obliczLiczbyPól();
            return ilePólPrzyjętych;
        }

        public bool PołóżKamień(int poziomo, int pionowo)
        {
            return PołóżKamień(poziomo, pionowo, false) > 0;
        }

        private int[] liczbyPól = new int[3]; 


        public int LiczbaPustychPól { get { return liczbyPól[0]; } }
        public int LiczbaPólGracz1 { get { return liczbyPól[1]; } }
        public int LiczbaPólGracz2 { get { return liczbyPól[2]; } }

        private void obliczLiczbyPól()
        {
            for (int i = 0; i < liczbyPól.Length; ++i) liczbyPól[i] = 0;

            for (int i = 0; i < SzerokośćPlanszy; ++i)
                for (int j = 0; j < WysokośćPlanszy; ++j)
                    liczbyPól[plansza[i, j]]++;
        }
        private bool czyBieżącyGraczMożeWykonaćRuch()
        {
            int liczbaPoprawnychPol = 0;
            for (int i = 0; i < SzerokośćPlanszy; ++i)
                for (int j = 0; j < WysokośćPlanszy; ++j)
                    if (plansza[i, j] == 0 && PołóżKamień(i, j, true) > 0)
                        liczbaPoprawnychPol++;
            return liczbaPoprawnychPol > 0;
        }
        public void Pasuj()
        {
            if (czyBieżącyGraczMożeWykonaćRuch())
            {
                throw new Exception("Gracz nie może oddać ruchu, jeżeli wykonanie ruchu jest możliwe");
            }
            zmieńBieżącegoGracza();
        }

        public enum SytuacjaNaPlanszy
        {
            RuchJestMożliwy,
            BieżącyGraczNieMożeWykonaćRuchu,
            ObajGraczeNieMogąWykonaćRuchu,
            WszystkiePolaPlanszySąZajęte
        }

        public SytuacjaNaPlanszy ZbadajSytuacjęNaPlanszy()
        {
            if (LiczbaPustychPól == 0) return SytuacjaNaPlanszy.WszystkiePolaPlanszySąZajęte;

            bool czyMozliwyRuch = czyBieżącyGraczMożeWykonaćRuch();

            if(czyMozliwyRuch) return SytuacjaNaPlanszy.RuchJestMożliwy; else
            {

           
            zmieńBieżącegoGracza();
            bool czyMożliwyRuchOponenta = czyBieżącyGraczMożeWykonaćRuch();
            zmieńBieżącegoGracza();
            if (czyMożliwyRuchOponenta) return SytuacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu;
            else return SytuacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu;
            }
        }

        public int NumerGraczaMającegoPrzewagę
        {
            get
            {
                if (LiczbaPólGracz1 == LiczbaPólGracz2) return 0;
                else
                 if (LiczbaPólGracz1 > LiczbaPólGracz2) return 1;
                else return 2;
            }
        }

    }
}
