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
		public float k = 25;
		List<Country> countries = new List<Country>();
		public Run()
		{
			bool ended = false;
            using (var fs = File.OpenRead(@"datahier.csv"))
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
                                countries.Add(new Country(values[0]));
                            }
                        }
                    }
                    else {
                        Country a = (Country)countries.Find(r => r.name == values[0]);
                        Country b = (Country)countries.Find(r => r.name == values[1]);
                        float res = float.Parse(values[2]);

                        playMatch(new Match(a, b, res));
                    }

                }
                
			}
			countries.Sort ((a, b) =>  -1*a.elo.CompareTo(b.elo));
			int total = 0;
			int length=0;
			foreach (Country c in countries) {
				total += c.elo;
				Console.WriteLine ("{0} {1}", c.name, c.elo); 
				length++;
			}
			float avg = total / length;
			Console.WriteLine ("-----------------");
			Console.WriteLine ("Avg: {0}",avg.ToString());
            Console.ReadKey();

        }

        void playMatch(Match match){
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
