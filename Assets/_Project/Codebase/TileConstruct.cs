using System;
using UnityEngine;

namespace _Project.Codebase 
{
    [Serializable]
    public abstract class TileConstruct : IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public ConstructType type;
        public Vector2Int gridPos;
        public bool blockPlayerReplacement;

        public TileConstruct(PlaceableName placeableName, ConstructType type, Vector2Int gridPos, bool blockPlayerReplacement)
        {
            PlaceableName = placeableName;
            this.type = type;
            this.gridPos = gridPos;
            this.blockPlayerReplacement = blockPlayerReplacement;
        }

        public TileConstruct(PlaceableName placeableName, bool blockPlayerReplacement = false) : this(placeableName,
            References.Singleton.GetType(placeableName), Vector2Int.zero, blockPlayerReplacement)
        {
        }

        public TileConstruct(TileConstruct construct) : this(construct.PlaceableName, construct.type, construct.gridPos,
            construct.blockPlayerReplacement)
        {
        }

        public abstract bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos);

        public abstract void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity = false);
        public abstract void Delete(Station station);

        public static TileConstruct GetTileConstructFromName(PlaceableName name)
        {
            switch (name)
            {
                case PlaceableName.None:
                    return null;
                default:
                    switch (References.Singleton.GetType(name))
                    {
                        case ConstructType.Wall:
                            return new WallTile(name);
                        case ConstructType.Floor:
                            return new FloorTile(name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
        }

    }
}