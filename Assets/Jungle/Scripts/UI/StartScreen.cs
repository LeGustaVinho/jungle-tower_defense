using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jungle.Scripts.UI
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Leaderboard;
        [SerializeField] private Button StartButton;

        public event Action OnClickStart;

        protected void Start()
        {
            StartButton.onClick.AddListener(OnClickedStart);
        }

        private void OnClickedStart()
        {
            OnClickStart?.Invoke();
        }

        public void UpdateLeaderBoard(string text)
        {
            Leaderboard.text = text;
        }
    }
}
