using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsStats.Model
{
    class ChampionList
    {
        public List<Champion> Champions { get; set; }
    }

    public class Champion
    {
        public string title { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
    }
}
