using System;
using System.Collections.Generic;
using LegendaryTools;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    [Serializable]
    public class LeaderboardEntry
    {
        public int Points;
        public int Level;
        public DateTime DateTime;

        public LeaderboardEntry(int points, int level)
        {
            Points = points;
            Level = level;
            DateTime = DateTime.Now;
        }
    }
    
    public class Leaderboard
    {
        public List<LeaderboardEntry> LeaderboardEntries = new List<LeaderboardEntry>();

        [ShowInInspector]
        private UnityFilePath leaderBoardFilePath;
        private Player player;
        private LevelController levelController;

        public event Action<List<LeaderboardEntry>> OnLeaderboardUpdate;

        public Leaderboard(Player player, LevelController levelController)
        {
            this.player = player;
            this.levelController = levelController;

            leaderBoardFilePath.RootPathType = UnityFilePathType.Data;
            leaderBoardFilePath.FileName = "Leaderboard";
            leaderBoardFilePath.Extension = "json";

            player.OnLoseGame += OnLoseGame;

            Load();
        }

        private void OnLoseGame()
        {
            LeaderboardEntries.Add(new LeaderboardEntry(player.Points, levelController.Level));
            LeaderboardEntries.Sort((x, y) => x.Points.CompareTo(y.Points));
            LeaderboardEntries.Reverse();
            Save();
            OnLeaderboardUpdate?.Invoke(LeaderboardEntries);
        }

        [Button]
        public void Save()
        {
            string json = JsonConvert.SerializeObject(LeaderboardEntries, Formatting.Indented);
            System.IO.File.WriteAllText(leaderBoardFilePath.Path, json);
        }

        [Button]
        public void Load()
        {
            if (System.IO.File.Exists(leaderBoardFilePath.Path))
            {
                string json = System.IO.File.ReadAllText(leaderBoardFilePath.Path);
                LeaderboardEntries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(json);
            }
        }
    }
}
