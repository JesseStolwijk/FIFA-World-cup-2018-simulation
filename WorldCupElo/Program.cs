using System;
using System.IO;
using System.Collections.Generic;
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
			//execTournament(@"../../data/1994Q.csv");
			//execTournament(@"../../data/1994T.csv",25);

			execTournament(@"../../data/2002Q.csv",10);
			execTournament(@"../../data/2002T.csv",32);

			execTournament(@"../../data/2006Q.csv",10);
			execTournament(@"../../data/2006T.csv",32);

			//execTournament(@"../../data/2010Q.csv");
			//execTournament(@"../../data/2010T.csv");

			//execTournament(@"../../data/2014Q.csv");
			//execTournament(@"../../data/2014T.csv");

			printTournament ();
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

						playMatch(new Match(a, b, res), k);
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

		void playMatch(Match match, float k){
			float Ea = 1/(1+10^((match.two.elo - match.one.elo)/400));
			float Eb = 1 - Ea;
			match.one.elo += (int)(k * (match.result - Ea));
			match.two.elo += (int)(k * ((1- match.result) - Eb)); 		
		}
	}
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
		public float result;
		public Match(Country one, Country two, float result){
			this.one = one;
			this.two = two;
			this.result = result;
		}
	}
}
