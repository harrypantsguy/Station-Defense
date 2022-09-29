using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Codebase
{
    public class SelectionMenu : CustomUI
    {
        [SerializeField] private GameObject _tabButtonPrefab;
        [SerializeField] private Transform _tabButtonParent;
        [SerializeField] private List<SelectionMenuTab> tabs = new List<SelectionMenuTab>();

        private void Start()
        {
            for (var i = 0; i < tabs.Count; i++)
            {
                SelectionMenuTab tab = tabs[i];
                CustomButton newButton = Instantiate(_tabButtonPrefab, _tabButtonParent).GetComponent<CustomButton>();
                newButton.SetLabelText(tab.label);
                int index = i;
                newButton.button.onClick.AddListener(() => SetTab(index));
            }
            SetTab(0);
        }

        public void SetTab(int index)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                if (i == index)
                    tabs[i].Enable();
                else
                    tabs[i].Disable();
            }
        }
    }
}