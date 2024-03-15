using System;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using LegendaryTools;
using LegendaryTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jungle.Scripts.UI
{
    [Serializable]
    public class StructureListing : GameObjectListing<UIStructure, StructureConfig>
    {
    }
    
    public class InGameScreen : MonoBehaviour
    {
        public event Action OnClickQuit;
        public event Action<StructureConfig> OnStructureSelect;
        
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
        [SerializeField] private string TimerTimeSpanFormat;
        [SerializeField] private string HealthFormat;
        
        [SerializeField] private Button QuitButton;

        [SerializeField] private ToggleGroup structureToggleGroup;
        [SerializeField] private StructureListing StructureListing;

        private Func<float> timerGetter;

        public void GenerateStructureUI(List<StructureConfig> structureConfigs)
        {
            foreach (UIStructure uiStructure in StructureListing.Listing)
            {
                uiStructure.OnToggleChange -= OnStructureToggleChange;
            }
            
            StructureListing.GenerateList(structureConfigs.ToArray());

            foreach (UIStructure uiStructure in StructureListing.Listing)
            {
                uiStructure.Toggle.group = structureToggleGroup;
                uiStructure.OnToggleChange += OnStructureToggleChange;
            }
        }

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

        public void SetTimeGetter(Func<float> timerGetter)
        {
            this.timerGetter = timerGetter;
        }
        
        public void UpdateTimer(string time)
        {
            TimerText.text = string.Format(TimerFormat, time);
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

        protected void Update()
        {
            if (timerGetter != null)
            {
                TimeSpan timerTimeSpan = TimeSpan.FromSeconds(timerGetter.Invoke());
                UpdateTimer(timerTimeSpan.Beautify(TimerTimeSpanFormat));
            }
        }
        
        private void OnStructureToggleChange(UIStructure uiStructure, bool mode)
        {
            if (mode)
            {
                OnStructureSelect?.Invoke(uiStructure.StructureConfig);
            }
        }

        private void OnClickedQuit()
        {
            OnClickQuit?.Invoke();
        }
    }
}