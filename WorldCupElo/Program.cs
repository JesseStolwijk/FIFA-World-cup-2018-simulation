using System;
using System.IO;
using System.Collections.Generic;
using static System.Random;
using System.Linq;

namespace WorldCupElo
{
	class MainClass
	{
		static void Main(string[] args){
			Run r = new Run ();
			r.a = 0;

		}
	}
	class Run{
		public int a;
		List<Country> countries = new List<Country>();
        List<TournamentResult> results = new List<TournamentResult>();
        Random random = new Random();
		public Run()
		{



            //execTournament(@"../../data/test.csv", 10);
            //printTournament();
            simulateTournament(32);
            Console.ReadKey();

        }
        void execData()
        {
            List<Country> countries = new List<Country>();

            execTournament(@"../../data/1994Q.csv", 10);
            execTournament(@"../../data/1994T.csv", 32);

            execTournament(@"../../data/2002Q.csv", 10);
            execTournament(@"../../data/2002T.csv", 32);

            execTournament(@"../../data/2006Q.csv", 10);
            execTournament(@"../../data/2006T.csv", 32);

            execTournament(@"../../data/2010Q.csv", 10);
            execTournament(@"../../data/2010T.csv", 32);

            execTournament(@"../../data/2014Q.csv", 10);
            execTournament(@"../../data/2014T.csv", 32);
        }
		void execTournament(string loc, float k){
			bool ended = false;
			using (var fs = File.OpenRead(loc))
			using(var reader = new StreamReader(fs))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					if (!ended)
					{
						if (values[0] == "EINDE")
						{
							ended = true;
						}
						else {
							int elo;
							if (int.TryParse(values[1], out elo))
							{
								countries.Add(new Country(values[0], elo));
							}
							else {
								Country c = countries.Find(item => item.name == values[0]);
								if(c == null)
									countries.Add(new Country(values[0]));
							}
						}
					}
					else {
						Country a = (Country)countries.Find(r => r.name == values[0]);
						Country b = (Country)countries.Find(r => r.name == values[1]);
						float res = float.Parse(values[2]);
						Match m = new Match(a, b, res,k,random);
						m.playMatch();
					}

				}

			}
		}
		void printTournament(){
			countries.Sort ((a, b) =>  -1*a.elo.CompareTo(b.elo));
			int total = 0;
			int length=1;
			foreach (Country c in countries) {
				total += c.elo;
				Console.WriteLine ("{0}: {1} {2}", length, c.name, c.elo); 
				length++;
			}
			float avg = total / (length - 1);
			Console.WriteLine ("-----------------");
			Console.WriteLine ("Avg: {0}",avg.ToString());
		}
        Tournament createTourney(float k)
        {
            Tournament t = new Tournament(new List<Group>(), k, random);

            List<Country> p = new List<Country>();
            List<Group> g = new List<Group>();
            int count = 0;
            using (var fs = File.OpenRead(@"../../data/test.csv"))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(' ');
                    Country c = countries.Find(x => x.name == values[0]);
                    t.groups.Find(gr => gr.name == values[1]).participants.Add(c);
                    count++;
                }
            }
            return t;
        }
        void simulateTournament(float k)
        {

            using (var fs = File.OpenRead(@"../../data/test.csv"))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(' ');
                    Country c = new Country(values[0]);
                    results.Add(new TournamentResult(c));

                }
            }


            for (int i = 0; i < 1000; i++)
            {
                execData();
                Tournament t = createTourney(k);


                //t = new Tournament(tacc);
                //countries = new List<Country>(countryBackup);

                t.simulate();
                for (int j = 0; j < 32; j++)
                {
                    results.Find(r => r.c.name == t.ranking[j].name).addPlacement(j);
                }
 

            }
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"output.csv", true))
            {
                file.WriteLine("Name, T1, T2, T3, T4, T8, T16, T32");

                foreach (TournamentResult tt in results)
                {
                    file.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", tt.c.name, tt.first, tt.second, tt.third, tt.fourth, tt.t8, tt.t16, tt.t32);

                }
            }
           

        }

    }
	class Tournament{
		public List<Group> groups;
        public List<Country> ranking;
        public Random r;
        public float k;
        public Tournament(List<Group> groups, float k, Random r)
        {
            this.k = k;
            this.r = r;
            //this.participants = participants;
            this.groups = groups;
            this.ranking = new List<Country>();
            this.groups.Add(new Group("A", k, r));
            this.groups.Add(new Group("B", k, r));
            this.groups.Add(new Group("C", k, r));
            this.groups.Add(new Group("D", k, r));
            this.groups.Add(new Group("E", k, r));
            this.groups.Add(new Group("F", k, r));
            this.groups.Add(new Group("G", k, r));
            this.groups.Add(new Group("H", k, r));
        }
        public Tournament(Tournament cop)
        {
            this.groups = cop.groups;
            this.ranking = cop.ranking;
            this.r = cop.r;
            this.k = cop.k;
            this.groups = cop.groups;
        }
		public List<Country> simulate(){
			for(int i = 0; i < groups.Count(); i++) {
                groups[i].simulate ();
                ranking.Add(groups[i].rank[0].c);
                ranking.Add(groups[i].rank[1].c);
            }
            List<Country> init = new List<Country>();
            for (int i = 0; i < 8; i += 2)
            {
                init.Add(groups[i].rank[3].c);
                init.Add(groups[i + 1].rank[2].c);
            }
            for (int i = 0; i < 8; i += 2)
            {
                init.Add(groups[i + 1].rank[3].c);
                init.Add(groups[i].rank[2].c);
            }
            playBracket(init);

            return ranking;
        }
        public void playBracket(List<Country> input)
        {
            input = playRound(playRound(playRound(input)));

            int last = this.ranking.Count() -1;

            Match mm = new Match(this.ranking[last], this.ranking[last-1], this.k, this.r);

            mm.simulate();
            this.ranking.Remove(mm.getLoser());
            this.ranking.Remove(mm.getWinner());
            this.ranking.Add(mm.getLoser());
            this.ranking.Add(mm.getWinner());

            mm = new Match(input[0], input[1], this.k, this.r);
            mm.simulate();
            this.ranking.Add(mm.getLoser());
            this.ranking.Add(mm.getWinner());

            input.RemoveAll(x => this.ranking.Contains(x));

        }
        List<Country> playRound(List<Country> input)
        {
            for (int i = 0; i < input.Count(); i += 2)
            {
                Match m = new Match(input[i], input[i + 1], this.k, this.r);
                m.simulate();
                Country loser = m.getLoser();
                this.ranking.Add(loser);
            }
            input.RemoveAll(x => this.ranking.Contains(x));
            return input;
        }

    }
	class Group{
		public string name;
        public List<Match> matches;
        public List<Result> rank;
        public List<Country> participants;
        public Random r;
        public float k;
        public Group(string name, float k, Random r)
        {
            this.k = k;
            this.r = r;
            this.matches = new List<Match>();
            this.rank = new List<Result>();
            this.participants = new List<Country>();
            this.name = name;

        }
        public void createMatches()
        {
            this.rank = new List<Result>();

            foreach (Country c in participants)
                this.rank.Add(new Result(c));
            this.matches = new List<Match>();

            matches.Add(new Match(participants[0], participants[1], this.k, this.r));
            matches.Add(new Match(participants[2], participants[3], this.k, this.r));

            matches.Add(new Match(participants[0], participants[2], this.k, this.r));
            matches.Add(new Match(participants[3], participants[1], this.k, this.r));

            matches.Add(new Match(participants[3], participants[0], this.k, this.r));
            matches.Add(new Match(participants[1], participants[2], this.k, this.r));
        }
        public void simulate(){
            createMatches();
            for (int i = 0; i < this.matches.Count(); i++)
            {
                this.matches[i].simulate();
                this.rank.Find(x => x.c.name == this.matches[i].getWinner().name).p += 3;
            }
            this.rank.Sort((x,y)=>x.p.CompareTo(y.p));

        }
    }
	class Result
    {
        public int p;
        public Country c;
        public Result(Country c)
        {
            this.p = 0;
            this.c = c;
        }
    }
    class TournamentResult
    {
        public int first;
        public int second;
        public int third;
        public int fourth;
        public int t8;
        public int t16;
        public int t32;
        public Country c;
        public TournamentResult(Country c)
        {
            this.c = c;
            this.first = 0;
            this.second = 0;
            this.third = 0;
            this.fourth = 0;
            this.t8 = 0;
            this.t16 = 0;
            this.t32 = 0;
        }

        public void addPlacement(int i)
        {
            if (i == 31)
                this.first++;
            else if (i == 30)
                this.second++;
            else if (i == 29)
                this.third++;
            else if (i == 28)
                this.fourth++;
            else if (i > 23 && i < 28)
                this.t8++;
            else if (i > 14 && i < 23)
                this.t16++;
            else
                this.t32++;
        }
    }
	class Country{
		public string name;
		public int elo;
		public Country(string name, int elo = 1500){
			this.name = name;
			this.elo = elo;
		}
        public void adjustElo(float k, double result, double eresult)
        {
            this.elo += (int)(k * (result - eresult));

        }
    }

	class Match{
		public Country one;
		public Country two;
		public double result;
        public float k;
        public Random random;
        public Match(Country one, Country two, double result, float k, Random r)
        {
            this.one = one;
            this.two = two;
            this.result = result;
            this.k = k;
            this.random = r;
        }
        public Match(Country one, Country two, float k, Random r)
        {
            this.one = one;
            this.two = two;
            this.k = k;
            this.random = r;
        }
        public void playMatch(){

			double Ea = 1 / (1 + Math.Pow(10, (double)(this.two.elo - this.one.elo) / 400));
            this.one.adjustElo(this.k, this.result, Ea);
            this.two.adjustElo(this.k, 1-this.result, 1-Ea);			
		}
		public void simulate(){
			int winpercentage =  (int) ((1 / (1 + Math.Pow(10, (double)(this.two.elo - this.one.elo) / 400)))*1000);
			int randomNumber = this.random.Next(0, 1000);
			if (randomNumber > (int)winpercentage) {
				this.result = 0;
			} else {
				this.result = 1;
			}
			this.playMatch ();

		}
        public Country getWinner()
        {
            if ((int)result == 1)
                return one;
            else
                return two;
        }
        public Country getLoser()
        {
            if ((int)result == 1)
                return two;
            else
                return one;
        }
    }
}
