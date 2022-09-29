using TMPro;
using UnityEngine;

namespace _Project.Codebase
{
    public class ResourcesPanelUI : CustomUI
    {
        [SerializeField] private TMP_Text _creditsText;

        private void Update()
        {
            _creditsText.text = Player.Singleton.resources.credits.ToString();
        }
    }
}