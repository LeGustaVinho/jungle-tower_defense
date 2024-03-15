using System;
using System.Collections.Generic;
using System.Text;
using Jungle.Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jungle.Scripts.UI
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Leaderboard;
        [SerializeField] private Button StartButton;
        [SerializeField] private int LeaderboardLimit = 10;
        [SerializeField] private string LeaderboardEntryFormat = "{0}) Score: {1}, Level: {2}";

        public event Action OnClickStart;

        protected void Start()
        {
            StartButton.onClick.AddListener(OnClickedStart);
        }

        private void OnClickedStart()
        {
            OnClickStart?.Invoke();
        }

        public void UpdateLeaderBoard(List<LeaderboardEntry> leaderboardEntries)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (LeaderboardEntry entry in leaderboardEntries)
            {
                sb.AppendLine(string.Format(LeaderboardEntryFormat, i + 1, entry.Points, entry.Level));
                i++;
                if (i >= LeaderboardLimit)
                {
                    break;
                }
            }
            
            Leaderboard.text = sb.ToString();
        }
    }
}
