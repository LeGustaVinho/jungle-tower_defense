using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jungle.Scripts.UI
{
    public class InGameScreen : MonoBehaviour
    {
        public event Action OnClickQuit;
        
        [SerializeField] private TextMeshProUGUI PointsText;
        [SerializeField] private TextMeshProUGUI MoneyText;
        [SerializeField] private TextMeshProUGUI LevelText;
        [SerializeField] private TextMeshProUGUI TimerText;
        [SerializeField] private Slider HealthSlider;
        [SerializeField] private TextMeshProUGUI HealthText;
        
        [SerializeField] private string PointsFormat;
        [SerializeField] private string MoneyFormat;
        [SerializeField] private string LevelFormat;
        [SerializeField] private string TimerFormat;
        [SerializeField] private string HealthFormat;
        
        [SerializeField] private Button QuitButton;
        
        public void UpdatePoints(float value)
        {
            PointsText.text = string.Format(PointsFormat, value);
        }
        
        public void UpdateMoney(float value)
        {
            MoneyText.text = string.Format(MoneyFormat, value);
        }
        
        public void UpdateLevel(float value)
        {
            LevelText.text = string.Format(LevelFormat, value);
        }
        
        public void UpdateTimer(float value)
        {
            TimerText.text = string.Format(TimerFormat, value);
        }
        
        public void UpdateHealth(float value, float maxValue)
        {
            if (value < 0)
                value = 0;
            
            HealthText.text = string.Format(HealthFormat, value, maxValue);
            HealthSlider.value = value / maxValue;
        }

        protected void Start()
        {
            QuitButton.onClick.AddListener(OnClickedQuit);
        }

        private void OnClickedQuit()
        {
            OnClickQuit?.Invoke();
        }
    }
}