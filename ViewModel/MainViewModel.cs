using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LeagueOfLegendsStats.Data;
using LeagueOfLegendsStats.Model;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;

namespace LeagueOfLegendsStats.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private List<Champion> _Champions;
        public List<Champion> Champions { get { return _Champions; } private set { _Champions = value; RaisePropertyChanged(nameof(Champions)); } }
        public string SummonerName { get; set; }
        internal APIClient Client { get; }

        private Summoner Summoner;

        private string _SelectedServer;
        public string SelectedServer { get { return _SelectedServer; }set{ _SelectedServer = value; Client.Region = _SelectedServer; RaisePropertyChanged(nameof(SelectedServer)); } } 

        private MatchReference _MatchReference;
        public MatchReference SelectedMatch { get { return _MatchReference; } set { _MatchReference = value; GetMatchDetails(); } }

        private bool _SummonerSelected;
        public bool SummonerSelected { get { return _SummonerSelected; } set { _SummonerSelected = value; RaisePropertyChanged(nameof(SummonerSelected));  } }

        public RelayCommand SearchForSummonerCommand { get; private set; }
        public bool CanRunCommand()
        {
            return true;
        }

        public RelayCommand<int> SearchForMatchByChampCommand { get; private set; }
        private List<MatchReference> _Matches;
        public List<MatchReference> Matches { get { return _Matches; } private set { _Matches = value; RaisePropertyChanged(nameof(Matches)); } }
        private SeriesCollection _SeriesCollection;
        public SeriesCollection SeriesCollection { get { return _SeriesCollection; } private set { _SeriesCollection = value; RaisePropertyChanged(nameof(SeriesCollection)); } }


        public MainViewModel()
        {
            this.SearchForSummonerCommand = new RelayCommand(this.SearchForSummoner, CanRunCommand);
            this.SearchForMatchByChampCommand = new RelayCommand<int>(SearchForMatchByChamp);

            SummonerSelected = false;

            SummonerName = "rideskip";

            Client = new APIClient();
            Client.APIKey = "RGAPI-4c3b0d6b-fa5a-4bc2-8394-e1f6590cda22";
            Client.Region = SelectedServer;

            var championListTask = Client.GetChampions();
            championListTask.Wait();
            Champions = championListTask.Result;
        }

        public void SearchForSummoner()
        {
            var summonerTask = Client.GetSummoner(SummonerName);
            summonerTask.Wait();

            Summoner = summonerTask.Result;

            if(Summoner != null)
                SummonerSelected = true;
        }

        public void SearchForMatchByChamp(int id)
        {
            Matches = Client.GetMatches(Summoner.id, id).Result;
        }

        public void GetMatchDetails()
        {

            if (SelectedMatch != null)
            {
                var matchDetails = Client.GetMatch(SelectedMatch.gameId).Result;

                var playerKills =0;
                var averageKills =0;
                var blueTeamKills = 0;
                var redTeamKills =0;

                foreach (var participant in matchDetails.participantIdentities)
                {
                    var CurrentParticipant = matchDetails.participants.Find(x => x.participantId == participant.participantId);

                    averageKills += CurrentParticipant.stats.kills;

                    if (participant.player.summonerId == Summoner.id)
                    {
                        playerKills += CurrentParticipant.stats.kills;
                    }

                    if(CurrentParticipant.teamId == 100)
                    {
                        blueTeamKills += CurrentParticipant.stats.kills;
                    }
                    else
                    {
                        redTeamKills += CurrentParticipant.stats.kills;
                    }

                }

                averageKills = averageKills / 10;

                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Values = new ChartValues<int>{playerKills },
                        Title = "Player Kills"
                    },
                    new ColumnSeries
                    {
                        Values = new ChartValues<int>{averageKills },
                        Title = "Average Player Kills"
                    },
                    new ColumnSeries
                    {
                        Values = new ChartValues<int>{blueTeamKills },
                        Title = "Blue team Kills"
                    },
                    new ColumnSeries
                    {
                        Values = new ChartValues<int>{redTeamKills },
                        Title = "Red Team Kills"
                    }
        
                }; 
            }
        }

    }
}