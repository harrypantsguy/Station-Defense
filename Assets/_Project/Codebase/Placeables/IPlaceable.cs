using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    public interface IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public PlaceableType Type { get; set; }
        public Station Station { get; set; }
        public ResourcesContainer PlacementCost { get; set; }
        public bool BlockDeletion { get; set; }
        public float BuildProgress { get; set; }
        public bool Built { get; }
        public bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos, bool considerCost, 
            out ResourcesContainer cost, out PlacementFailCause failCause);
        public void TryPlace(Station station, in Vector2Int gridPos, bool costResources = true, bool ignoreValidity = false);
        public bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool returnOnValidityAssessment, out List<Vector2Int> validPositions, bool considerCost, 
            out ResourcesContainer cost, out PlacementFailCause failCause);
        public void TryFillRect(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly, 
            bool costResources = true, bool ignoreValidity = false);
        public void Delete();

        public static bool PlayerCanAfford(bool considerCost, ResourcesContainer cost) => !considerCost || 
                                                                                          Player.CanAfford(cost);

        public static IPlaceable MakeCopy(IPlaceable original)
        {
            switch (original.Type)
            {
                case PlaceableType.Wall:
                    return new WallTile(original as WallTile);
                case PlaceableType.Floor:
                    return new FloorTile(original as FloorTile);
                case PlaceableType.Structure:
                    return Structure.CreateInstance(((Structure) original).gameObject);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IPlaceable CreatePlaceableFromName(PlaceableName name)
        {
            switch (name)
            {
                case PlaceableName.None:
                    return null;
                default:
                    switch (References.Singleton.GetType(name))
                    {
                        case PlaceableType.Wall:
                            return new WallTile(name);
                        case PlaceableType.Floor:
                            return new FloorTile(name);
                        case PlaceableType.Structure:
                        {
                            Structure newStructure = Structure.CreateInstance(References.Singleton.GetStructure(name));
                            newStructure.Initialize(References.Singleton.GetStructurePrefabData(name));
                            return newStructure;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
        }
    }
}