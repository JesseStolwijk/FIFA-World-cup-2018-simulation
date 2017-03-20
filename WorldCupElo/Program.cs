using System;
using System.IO;
using System.Collections.Generic;
using static System.Random;
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
		public Run()
		{
			execTournament(@"../../data/1994Q.csv",10);
			execTournament(@"../../data/1994T.csv",32);

			execTournament(@"../../data/2002Q.csv",10);
			execTournament(@"../../data/2002T.csv",32);

			execTournament(@"../../data/2006Q.csv",10);
			execTournament(@"../../data/2006T.csv",32);

			execTournament(@"../../data/2010Q.csv",10);
			//execTournament(@"../../data/2010T.csv",32);

			execTournament(@"../../data/2014Q.csv",10);
			//execTournament(@"../../data/2014T.csv",32);

			printTournament ();
			Console.WriteLine ("------");
			Match m = new Match(new Country("A", 1600), new Country("B", 1500), 0);
			m.simulate (32);
			Console.WriteLine (m.result);
            Console.ReadKey();

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
							if (int.TryParse(values[0], out elo))
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
						Match m = new Match(a, b, res);
						m.playMatch(k);
					}

				}

			}
		}
		void printTournament(){
			countries.Sort ((a, b) =>  -1*a.elo.CompareTo(b.elo));
			//countries.Sort((a,b) => a.name.CompareTo(b.name));
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


	}/*
	class Tournament{
		public List<Country> participants;
		public List<Group> groups;
		public Tournament(){
			
		}
		public void simulate(){
			foreach (Group g in groups) {
				g.simulate ();
			}
		}
	}/*
	class Group{
		public Ranking teams;
		public string name;
		public void simulate(){
		}
	}
	*/
	class Country{
		public string name;
		public int elo;
		public Country(string name, int elo = 1500){
			this.name = name;
			this.elo = elo;
		}
	}

	class Match{
		public Country one;
		public Country two;
		public double result;

		public Match(Country one, Country two, float result){
			this.one = one;
			this.two = two;
			this.result = result;
		}
		public void playMatch(float k){
			if (this.result == 0.0) {
				double Ea = 1 / (1 + 10 ^ ((this.two.elo - this.one.elo) / 400));
				double Eb = 1 - Ea;
				this.one.elo -= (int)(k * (1.0f - Ea));
				this.two.elo -= (int)(k * (0.0f - Eb));
			} else {
				double Ea = 1 / (1 + 10 ^ ((this.two.elo - this.one.elo) / 400));
				double Eb = 1 - Ea;
				this.one.elo += (int)(k * (this.result - Ea));
				this.two.elo += (int)(k * ((1 - this.result) - Eb)); 
			}
		}
		public void simulate(float k){
			double winpercentage = 1/(1+10^((this.two.elo - this.one.elo)/400));
			Random random = new Random();
			int randomNumber = random.Next(0, 1000);
			if (randomNumber > (int)winpercentage) {
				this.result = 0;
			} else {
				this.result = 1;
			}
			this.result = 0;
			Console.WriteLine ("{0} {1} {2} {3} {4}", this.one.elo, this.two.elo, winpercentage, randomNumber, 0 - 1);
			this.playMatch (k);
			Console.WriteLine ("{0} {1} {2} {3} {4}", this.one.elo, this.two.elo, winpercentage, randomNumber, result);

		}
	}
}
