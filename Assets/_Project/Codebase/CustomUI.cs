using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Codebase
{
    public class CustomUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static List<CustomUI> elementsWithMouseOver = new List<CustomUI>();
        public static bool MouseOverUI => elementsWithMouseOver.Count > 0;
        public void OnPointerEnter(PointerEventData eventData)
        {
            elementsWithMouseOver.Add(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            elementsWithMouseOver.Remove(this);
        }
    }
}