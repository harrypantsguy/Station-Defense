﻿using System;
 using UnityEngine;
 using UnityEngine.Tilemaps;

 namespace _Project.Codebase
{
    [Serializable]
    public struct StructurePrefabData
    {
        public StructureName structureName;
        public StructureType type;
        public Sprite sprite;
        public Tile tile;
    }
}