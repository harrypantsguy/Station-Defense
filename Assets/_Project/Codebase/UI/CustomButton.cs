using TMPro;
using UnityEngine.UI;

namespace _Project.Codebase
{
    public class CustomButton : CustomUI
    {
        public Button button;
        public TMP_Text label;
        public string SetLabelText(string text) => label.text = text;
    }
}