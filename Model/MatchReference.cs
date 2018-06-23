using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsStats.Model
{
    public class MatchReference 
    {
        public string lane { get; set; }
        public int gameId { get; set; }
        public int champion { get; set; }
        public string platformId { get; set; }
        public long timestamp { get; set; }
        public int queue { get; set; }
        public string role { get; set; }
        public int season { get; set; }
    }

    public class MatchList
    {
        public List<MatchReference> matches { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public int totalGames { get; set; }
    }

    //public class Matches : IEnumerator, IEnumerable
    //{
    //    int position = -1;
    //    public List<MatchReference> MatchesReference { get; set; }

    //    //IEnumerator and IEnumerable require these methods.
    //    public IEnumerator GetEnumerator()
    //    {
    //        return (IEnumerator)this;
    //    }

    //    //IEnumerator
    //    public bool MoveNext()
    //    {
    //        position++;
    //        return (position < Matches.Count);
    //    }

    //    //IEnumerable
    //    public void Reset()
    //    { position = 0; }

    //    //IEnumerable
    //    public object Current
    //    {
    //        get { return Matches[position]; }
    //    }
    //}
}
