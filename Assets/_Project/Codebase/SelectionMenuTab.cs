using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    [Serializable]
    public class SelectionMenuTab
    {
        public string label;
        public List<GameObject> tabObjects = new List<GameObject>();

        public void Enable()
        {
            foreach (GameObject obj in tabObjects)
                obj.SetActive(true);
        }
        
        public void Disable()
        {
            foreach (GameObject obj in tabObjects)
                obj.SetActive(false);
        }
    }
}