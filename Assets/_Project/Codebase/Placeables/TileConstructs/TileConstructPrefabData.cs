using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    [Serializable]
    public struct TileConstructPrefabData
    {
        public PlaceableName placeableName;
        public PlaceableType type;
        public ResourcesContainer placementCost;
        public Sprite sprite;
        public Tile tile;
    }
}