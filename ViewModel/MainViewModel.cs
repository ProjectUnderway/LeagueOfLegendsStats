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
        private enum Team
        {
            blue = 100,
            red = 200
        }

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
            //Set up command bindings for the UI
            this.SearchForSummonerCommand = new RelayCommand(this.SearchForSummoner, CanRunCommand);
            this.SearchForMatchByChampCommand = new RelayCommand<int>(SearchForMatchesByChamp);

            //Initialize properties
            SummonerSelected = false;
            SummonerName = "summoner X";

            //Create API Client
            Client = new APIClient();
            //Key expires every 24 hours - Manually added each time project is worked on
            Client.APIKey = "";
            //default reqion
            Client.Region = "oce1";

            //Get list of all playable champions to popilate UI with
            var championListTask = Client.GetChampions();
            championListTask.Wait();
            Champions = championListTask.Result;
        }

        //Search for the summoner account
        public void SearchForSummoner()
        {
            var summonerTask = Client.GetSummoner(SummonerName);
            summonerTask.Wait();

            Summoner = summonerTask.Result;

            //If a summoner account was found with the entered name then this will enable the champion selection section
            if(Summoner != null)
                SummonerSelected = true;
        }

        //Search for matches where the clicked champion was used
        public void SearchForMatchesByChamp(int id)
        {
            Matches = Client.GetMatches(Summoner.id, id).Result;
        }

        //Get details on the selected match in the combo box
        public void GetMatchDetails()
        {
            //If no match was selected in the combobox do nothing
            if (SelectedMatch != null)
            {
                var matchDetails = Client.GetMatch(SelectedMatch.gameId).Result;

                var playerKills =0;
                var averageKills =0;
                var blueTeamKills = 0;
                var redTeamKills =0;

                //calculate stats for the match - in this case kills
                foreach (var participant in matchDetails.participantIdentities)
                {
                    var CurrentParticipant = matchDetails.participants.Find(x => x.participantId == participant.participantId);

                    averageKills += CurrentParticipant.stats.kills;

                    //check if the paticipant is the entered summoner
                    if (participant.player.summonerId == Summoner.id)
                    {
                        playerKills += CurrentParticipant.stats.kills;
                    }

                    //Check which team a participant was on
                    if(CurrentParticipant.teamId == (int)Team.blue)
                    {
                        blueTeamKills += CurrentParticipant.stats.kills;
                    }
                    else
                    {
                        redTeamKills += CurrentParticipant.stats.kills;
                    }

                }

                //There are always 10 players so to get the average divide by 10
                averageKills = averageKills / 10;

                //Popluate the chart
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