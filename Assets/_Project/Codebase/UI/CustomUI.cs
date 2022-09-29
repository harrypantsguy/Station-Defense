using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Codebase
{
    public class CustomUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static List<CustomUI> elementsWithMouseOver = new List<CustomUI>();
        public static bool MouseOverUI => elementsWithMouseOver.Count > 0;
        public virtual bool FlagWhenMouseOver => true;
        protected RectTransform rectTransform;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();        
        }

        protected virtual void Start() { }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (FlagWhenMouseOver)
                elementsWithMouseOver.Add(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (FlagWhenMouseOver)
                elementsWithMouseOver.Remove(this);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad()
        {
            elementsWithMouseOver = new List<CustomUI>();
        }
    }
}