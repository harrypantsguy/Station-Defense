using System;
using UnityEngine;

namespace _Project.Codebase
{
    [Serializable]
    public struct StructurePrefabData
    {
        public PlaceableName placeableName;
        public GameObject structurePrefab;
        public Sprite sprite;
    }
}