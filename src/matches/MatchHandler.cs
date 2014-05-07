﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RiotSharp;
using src.stats;
using src.summoner;
using src.util;

namespace src.matches {
    class MatchHandler {
        private static MatchHandler instance;

        private Core core;
        private RiotApi api;
        private SummonerHandler summonerHandler;

        private string HOME_PATH;

        private MatchHandler() {
            core = Core.getInstance();
            api = core.getRiotApi();
            summonerHandler = SummonerHandler.getInstance();

            HOME_PATH = core.getHomePath() + @"matches\";

            //Check for folders
            if (!Directory.Exists(HOME_PATH)) {
                Directory.CreateDirectory(HOME_PATH);
            }

            foreach (TrackedSummoner summoner in summonerHandler.trackedSummoners) {
                if (!Directory.Exists(HOME_PATH + summoner.Name + @"\")) {
                    Directory.CreateDirectory(HOME_PATH + summoner.Name + @"\");
                }

                updateMatches(summoner);
                updateAverageStats(summoner);
            }
        }

        public static MatchHandler getInstance() {
            if (instance == null) {
                instance = new MatchHandler();
            }
            return instance;
        }

        private async Task<Summoner> processSummoner(Region region, long id) {
            return await api.GetSummonerAsync(region, (int)id);
        }

        private async Task<List<Game>> processMatch(Task<Summoner> task) {
            return await task.Result.GetRecentGamesAsync();
        }

        private async void updateMatches(TrackedSummoner tsummoner) {
            try {
                Task<Summoner> summonerTask = api.GetSummonerAsync(tsummoner.Region, (int)tsummoner.Id);
                Summoner summoner = await summonerTask;
                Task<List<Game>> matchTask = summoner.GetRecentGamesAsync();

                try {
                    List<Game> games = await matchTask;
                    foreach (Game game in games) {
                        if (!Directory.EnumerateFiles(HOME_PATH + tsummoner.Name).Any(x => x == game.GameId.ToString())) {
                            File.WriteAllText(HOME_PATH + tsummoner.Name + @"\" + game.GameId + ".json",
                                JsonConvert.SerializeObject(game));
                        }
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);

                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void updateAverageStats(TrackedSummoner tsummoner) {
            List<Game> games = loadTrackedMatches(tsummoner.Name);
            AverageStats stats = new AverageStats();

            //list of champions played
            //first array element contains the champion id, second the times champion was played
            List<ChampionPlayed> championsPlayed = new List<ChampionPlayed>();

            Console.WriteLine(tsummoner.Name);
            foreach (Game game in games) {
                //champions stats
                if (championsPlayed.Exists(x => x.championId == game.ChampionId)) {
                    championsPlayed.Find(x => x.championId == game.ChampionId).timesPlayed++;
                } else {
                    championsPlayed.Add(new ChampionPlayed() {
                        championId = game.ChampionId, timesPlayed = 1
                    });
                }
            }

            stats.champions = championsPlayed.OrderByDescending(x => x.timesPlayed).Take(5).ToList();

            stats.averageKills = games.Average(x => x.Statistics.ChampionsKilled);
            stats.averageDeaths = games.Average(x => x.Statistics.Assists);
            stats.averageAssists = games.Average(x => x.Statistics.NumDeaths);

            stats.totalKills = games.Sum(x => x.Statistics.ChampionsKilled);
            stats.totalDeaths = games.Sum(x => x.Statistics.NumDeaths);
            stats.totalAssists = games.Sum(x => x.Statistics.Assists);

            stats.totalDoubleKills = games.Sum(x => x.Statistics.DoubleKills);
            stats.totalTripleKills = games.Sum(x => x.Statistics.TripleKills);
            stats.totalQuadraKills = games.Sum(x => x.Statistics.QuadraKills);
            stats.totalPentaKills = games.Sum(x => x.Statistics.PentaKills);

            stats.totalMagicDamageDealt = games.Sum(x => x.Statistics.MagicDamageDealtToChampions);
            stats.totalPhysicalDamageDealt = games.Sum(x => x.Statistics.PhysicalDamageDealtToChampions);
            stats.totalTrueDamageDealt = games.Sum(x => x.Statistics.TrueDamageDealtToChampions);
            stats.totalDamageDealt = games.Sum(x => x.Statistics.TotalDamageDealtToChampions);

            stats.totalMagicDamageTaken = games.Sum(x => x.Statistics.MagicDamageTaken);
            stats.totalPhysicalDamageTaken = games.Sum(x => x.Statistics.PhysicalDamageTaken);
            stats.totalTrueDamageTaken = games.Sum(x => x.Statistics.TrueDamageTaken);
            stats.totalDamageTaken = games.Sum(x => x.Statistics.TotalDamageTaken);
        }

        public List<Game> loadTrackedMatches(String summonerName) {
            List<Game> games = new List<Game>();
            String path = core.getHomePath() + @"matches\" + summonerName + @"\";
            foreach (String filePath in Directory.EnumerateFiles(path)) {
                games.Add(JsonConvert.DeserializeObject<Game>(File.ReadAllText(filePath)));
            }

            return games;
        }
    }
}
