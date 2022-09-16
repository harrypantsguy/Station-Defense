using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Codebase
{
    public class StructureSelectionButton : CustomUI
    {
        [SerializeField] private PlaceableName placeableName;

        [SerializeField] private Color unselectedColor;
        [SerializeField] private Color selectedColor;
        private Button _button;
        
        private void Start()
        {
            _button = GetComponent<Button>();
        }

        public void SetStructure()
        {
            Player.Singleton.SetStructure(placeableName);
        }

        private void Update()
        {
            if (Player.Singleton.PlaceableName == placeableName && _button.colors.normalColor != selectedColor)
            {
                ColorBlock buttonColors = _button.colors;
                buttonColors.normalColor = selectedColor;
                _button.colors = buttonColors;
            }
            else if (Player.Singleton.PlaceableName != placeableName && _button.colors.normalColor != unselectedColor)
            {
                ColorBlock buttonColors = _button.colors;
                buttonColors.normalColor = unselectedColor;
                _button.colors = buttonColors;
            }
        }
    }
}