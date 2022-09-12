using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase 
{
    [Serializable]
    public abstract class TileConstruct : IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public bool BlockDeletion { get; set; }
        public float BuildProgress { get; set; }
        public bool Built => BuildProgress >= 1f;
        public ConstructType type;
        public Vector2Int gridPos;

        public TileConstruct(PlaceableName placeableName, ConstructType type, Vector2Int gridPos, 
            bool blockDeletion)
        {
            PlaceableName = placeableName;
            BlockDeletion = blockDeletion;
            this.type = type;
            this.gridPos = gridPos;
        }

        public TileConstruct(PlaceableName placeableName, bool blockDeletion = false) : this(placeableName,
            References.Singleton.GetType(placeableName), Vector2Int.zero, blockDeletion)
        {
        }

        public TileConstruct(TileConstruct construct) : this(construct.PlaceableName, construct.type, construct.gridPos, 
            construct.BlockDeletion)
        {
        }

        public virtual void Update()
        {
            if (BuildProgress < 1f)
            {
                BuildProgress += Time.deltaTime;
                BuildProgress = Mathf.Clamp01(BuildProgress);
            }
        }
        
        public abstract bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos);

        public abstract void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity = false);
        public abstract bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, 
            bool borderOnly, bool returnOnValidityAssessment, out List<Vector2Int> validPositions);

        public virtual void TryFillRect(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool ignoreValidity = false)
        {
            if (IsValidRectPlacement(station, corner1, corner2, borderOnly, false, out List<Vector2Int> validPositions)
                || ignoreValidity) 
                station.FillPositionsWithConstruct(validPositions, this);
        }

        public abstract void Delete(Station station);

        public static TileConstruct MakeCopy(TileConstruct original)
        {
            switch (original.type)
            {
                case ConstructType.Wall:
                    return new WallTile(original);
                case ConstructType.Floor:
                    return new FloorTile(original);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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