﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RiotSharp;
using src;

namespace src.views {
    public partial class SummonerView : View, SummonerListener {

        private Summoner summoner, previous;
        private List<LeagueItem> leagues; 
        private Region region;
        private String summonerName;
        private RiotApi riotApi;

        public SummonerView() {
            InitializeComponent();
            SummonerHandler.getInstance().register(this);
            riotApi = Core.getInstance().getRiotApi();
            if (SummonerHandler.getInstance().getSummoner() != null && summoner == null) {
                summonerUpdated(SummonerHandler.getInstance().getSummoner());
            }

            Console.WriteLine(this.Content);
        }

        private delegate void ObjectDelegate(object obj);

        private async Task loadSummoner() {
            leagues = await summoner.GetLeaguesAsync();
        }

        private void init(Task task) {
            if(summoner == null || summoner.Id == 0) return;
            resetControls();
            if (leagues == null) return;
            lblSummonerName.Content = summoner.Name;
            lblLevel.Content = "level " + summoner.Level;
            if (leagues == null) return;
            for (int i = 0; i < leagues.Count; i++) {
                LeagueItem league = leagues[i];
                if (league.PlayerOrTeamName != summoner.Name) return;
                Log.info("League Type: " + league.QueueType);
                lblLeagueName.Content = league.LeagueName;
                lblDivision.Content = league.Tier + " " + league.Rank;
                lblWins.Content = league.Wins;
                lblLeaguePoints.Content = league.LeaguePoints;
                int medals = 0;
                if (league.IsHotStreak) {
                    Image imgHot = new Image();
                    imgHot.Source = Util.CreateImage(Core.getInstance().getAssetsPath() + @"profile\league_hot.png");
                    Grid.SetColumn(imgHot, medals);
                    Grid.SetRow(imgHot, 0);
                    grdMedals.Children.Add(imgHot);
                    medals++;
                }
                if (league.IsFreshBlood) {
                    Image imgNew = new Image();
                    imgNew.Source = Util.CreateImage(Core.getInstance().getAssetsPath() + @"profile\league_new.png");
                    Grid.SetColumn(imgNew, medals);
                    Grid.SetRow(imgNew, 0);
                    grdMedals.Children.Add(imgNew);
                    medals++;
                }
                MiniSeries series = league.MiniSeries;
                if (series != null) {
                    if (!(series.Losses == 0 && series.Wins == 0)) {
                        lblSeriesTag.Visibility = Visibility.Visible;
                        grdSeries.Visibility = Visibility.Visible;
                        grdSeries.Children.Clear();
                        for (int j = 0; j < series.Progress.Length; j++) {
                            char c = series.Progress[j];
                            Log.info("" + c);
                            switch (c) {
                                case 'W':
                                    Image img1 = new Image();
                                    img1.Source = Util.CreateImage(Core.getInstance().getCurrentPath() + @"\img\profile\series_win.png");
                                    Grid.SetColumn(img1, j);
                                    Grid.SetRow(img1, 0);
                                    grdSeries.Children.Add(img1);
                                    break;
                                case 'L':
                                    Image img2 = new Image(); 
                                    img2.Source = Util.CreateImage(Core.getInstance().getCurrentPath() + @"\img\profile\series_lose.png");
                                    Grid.SetColumn(img2, j);
                                    Grid.SetRow(img2, 0);
                                    grdSeries.Children.Add(img2);
                                    break;
                                case 'N':
                                    Image img3 = new Image();
                                    img3.Source = Util.CreateImage(Core.getInstance().getCurrentPath() + @"\img\profile\series_none.png");
                                    Grid.SetColumn(img3, j);
                                    Grid.SetRow(img3, 0);
                                    grdSeries.Children.Add(img3);
                                    break;
                            }
                        }
                    } else {
                        lblSeriesTag.Visibility = Visibility.Hidden;
                        grdSeries.Visibility = Visibility.Hidden;
                    }
                } else {
                    lblSeriesTag.Visibility = Visibility.Hidden;
                    grdSeries.Visibility = Visibility.Hidden;
                }
            }
        }

        private void resetControls() {
            lblSummonerName.Content = "";
            lblLevel.Content = "";
            lblLeagueName.Content = "";
            lblDivision.Content = "";
            lblLeaguePoints.Content = "";
            lblWins.Content = "0";
            lblSeriesTag.Visibility = Visibility.Hidden;
            grdSeries.Children.Clear();
            grdMedals.Children.Clear();
        }

        public void summonerUpdated(Summoner summoner) {
            previous = this.summoner;
            this.summoner = summoner;
            loadSummoner().ContinueWith(init, TaskContinuationOptions.ExecuteSynchronously);
            Log.info("Summoner Updated");
        }

    }
}