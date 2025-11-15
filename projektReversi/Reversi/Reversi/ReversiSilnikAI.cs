namespace Reversi
{
    internal class ReversiSilnikAI : ReversiSilnik
    {
        public ReversiSilnikAI(int numerGraczaRozpoczynającego,
            int szerokośćPlanszy = 8, int wysokośćPlanszy = 8)
            : base(numerGraczaRozpoczynającego, szerokośćPlanszy, wysokośćPlanszy)
        {
        }

        private struct MożliwyRuch : IComparable<MożliwyRuch>
        {
            public int poziomo;
            public int pionowo;
            public int priorytet;

            public MożliwyRuch(int poziomo, int pionowo, int priorytet)
            {
                this.poziomo = poziomo;
                this.pionowo = pionowo;
                this.priorytet = priorytet;
            }
            public int CompareTo(MożliwyRuch innyRuch)
            {
                return innyRuch.priorytet - this.priorytet;
            }

        }

        public void ProponujNajlepszyRuch(out int najlepszyRuchPoziomo, out int najlepszyRuchPionowo)
        {
            List<MożliwyRuch> mozliweRuchy = new List<MożliwyRuch>();
            int skokPiorytetu = SzerokośćPlanszy * WysokośćPlanszy;

            for (int poziomo = 0; poziomo < SzerokośćPlanszy; poziomo++)
                for (int pionowo = 0; pionowo < WysokośćPlanszy; pionowo++)
                
                    {

                        int priorytet = PołóżKamień(poziomo, pionowo, true);
                        if (priorytet > 0)
                        {
                            MożliwyRuch mr = new MożliwyRuch(poziomo, pionowo, priorytet);

                            // pole w rogu +
                            if ((mr.poziomo == 0 || mr.poziomo == SzerokośćPlanszy - 1) && (mr.pionowo == 0 || mr.pionowo == WysokośćPlanszy - 1))
                                mr.priorytet += skokPiorytetu * skokPiorytetu;

                            // pole sąsiadujące z rogiem na przekątnych -
                            if ((mr.poziomo == 1 || mr.poziomo == SzerokośćPlanszy - 2) && (mr.pionowo == 1 || mr.pionowo == WysokośćPlanszy - 2))
                                mr.priorytet -= skokPiorytetu * skokPiorytetu;

                            // pole sąsiadujące z rogiem w pionie -
                            if ((mr.poziomo == 0 || mr.poziomo == SzerokośćPlanszy - 1) && (mr.pionowo == 1 || mr.pionowo == WysokośćPlanszy - 2))
                                mr.priorytet -= skokPiorytetu * skokPiorytetu;

                            // pole sąsiadujące z rogiem w poziomie -
                            if ((mr.poziomo == 1 || mr.poziomo == SzerokośćPlanszy - 2) && (mr.pionowo == 0 || mr.pionowo == WysokośćPlanszy - 1))
                                mr.priorytet -= skokPiorytetu * skokPiorytetu;

                            // pole na brzegu +
                            if (mr.poziomo == 0 || mr.poziomo == SzerokośćPlanszy - 1 || mr.pionowo == 0 || mr.pionowo == WysokośćPlanszy - 1)
                                mr.priorytet += skokPiorytetu;

                            // pole sasiadujące z brzegiem - 
                            if (mr.poziomo == 1 || mr.poziomo == SzerokośćPlanszy - 2 || mr.pionowo == 1 || mr.pionowo == WysokośćPlanszy - 2)
                                mr.priorytet -= skokPiorytetu;

                            mozliweRuchy.Add(mr);
                        }
                    }
                
            if (mozliweRuchy.Count > 0)
            {
                mozliweRuchy.Sort();
                najlepszyRuchPoziomo = mozliweRuchy[0].poziomo;
                najlepszyRuchPionowo = mozliweRuchy[0].pionowo;
            }
            else
            {
                throw new Exception("Brak możliwych ruchów");
            }
        }
    }
}
