using System;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using LegendaryTools.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jungle.Scripts.UI
{
    public class UIStructure : MonoBehaviour, GameObjectListing<UIStructure, StructureConfig>.IListingItem
    {
        public TextMeshProUGUI Name;
        public Toggle Toggle;

        public EntityAttribute[] AttributesToShow;
        public TextMeshProUGUI AttributeLabelPrefab;
        public Transform AttributeListingParent;
        public string AttributeTextFormat;

        public event Action<UIStructure, bool> OnToggleChange;
        
        [ShowInInspector]
        public StructureConfig StructureConfig { private set; get; }

        private List<TextMeshProUGUI> attributeLabelListing = new List<TextMeshProUGUI>();
        
        public void Init(StructureConfig item)
        {
            StructureConfig = item;
            Name.text = item.DisplayName;
            
            Toggle.onValueChanged.AddListener(OnToggleValueChange);

            foreach (EntityAttribute attribute in AttributesToShow)
            {
                CreateAttributeLabel(attribute);
            }
        }

        private void OnToggleValueChange(bool mode)
        {
            OnToggleChange?.Invoke(this, mode);
        }
        
        private TextMeshProUGUI CreateAttributeLabel(EntityAttribute attribute)
        {
            TextMeshProUGUI newLabel = Instantiate(AttributeLabelPrefab);
            Transform newLabelTransform = newLabel.transform;
            Transform prefabTransform = AttributeLabelPrefab.transform;

            newLabelTransform.SetParent(AttributeListingParent);
            newLabelTransform.localPosition = prefabTransform.localPosition;
            newLabelTransform.localScale = prefabTransform.localScale;
            newLabelTransform.localRotation = prefabTransform.localRotation;
            
            attributeLabelListing.Add(newLabel);
            newLabel.text = string.Format(AttributeTextFormat, attribute.ToString(),
                StructureConfig.LevelAttributes[attribute].GetValueForLevel(1));

            return newLabel;
        }
    }
}