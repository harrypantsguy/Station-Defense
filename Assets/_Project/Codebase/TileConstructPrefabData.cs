using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    [Serializable]
    public class TileConstructPrefabData
    {
        public PlaceableName placeableName;
        public ConstructType type;
        public Sprite sprite;
        public Tile tile;
    }
}