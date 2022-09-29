using System;
using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class Structure : MonoBehaviour, IDestroyable, IPlaceable
    {
        [SerializeField] private List<Vector2Int> _localPositions = new List<Vector2Int>();
        [SerializeField] private GameObject _graphics;
        public PlaceableName PlaceableName { get; set; }
        public PlaceableType Type { get; set; }
        public ResourcesContainer PlacementCost { get; set; }
        public bool BlockDeletion { get; set; }
        public float BuildProgress { get; set; }
        public bool Built { get; private set; }
        public Vector2Int pivot;
        public float Health { get; set; } = 100f;
        public Vector2 Dimensions { get; private set; }
        public List<Vector2Int> transformedLocalPositions = new List<Vector2Int>();
        public bool xEven, yEven;
        public Vector2 evenOffset;
        private List<FloorTile> _floors = new List<FloorTile>();

        private Direction _direction;
        public Direction Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                if (_graphics != null)
                    _graphics.transform.eulerAngles = _graphics.transform.eulerAngles.SetZ(_direction.DirectionToAngle());
                SetLocalPositions();
            }
        }

        public void Initialize(StructurePrefabData data)
        {
            PlaceableName = data.placeableName;
            PlacementCost = data.placementCost;
            Type = PlaceableType.Structure;
        }
        
        protected virtual void Start()
        {
            SetDimensionsAndUpdateData();
        }

        protected virtual void OnValidate()
        {
            SetDimensionsAndUpdateData();
        }

        private void SetDimensionsAndUpdateData()
        {
            SetLocalPositions();
            Dimensions = Utils.FindBoundsOfSetOfPoints(_localPositions);
            // Debug.Log(Dimensions);

            xEven = Math.Abs(Dimensions.x % 2) < .01f;
            yEven = Math.Abs(Dimensions.y % 2) < .01f;

            if (xEven && yEven)
                evenOffset = new Vector2(.5f, .5f);
            else
                evenOffset = Vector2.zero;
        }

        protected virtual void Update()
        {
           // transform.localScale = new Vector3(dimensions.x, dimensions.y, 1f);
        }

        public void SetGridPosition(Station station, Vector2Int gridPos)
        {
            transform.position = station.GridToWorldPos(gridPos) - evenOffset;
        }

        private void SetLocalPositions()
        {
            Vector2 offset = xEven && yEven ? new Vector2(.5f, .5f) : Vector2.zero;
            transformedLocalPositions =
                _localPositions.GetElementFromAll(
                    vector => 
                        (Direction.TransformVectorInDirection(vector - pivot + offset) - offset).ToInt());
        }

        public void TakeDamage()
        {
        }

        public void Die()
        {
        }

        public bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos, bool considerCost, 
            out ResourcesContainer cost, out PlacementFailCause failCause)
        {
            cost = PlacementCost;
            failCause = PlacementFailCause.None;
            if (!IPlaceable.PlayerCanAfford(considerCost, cost))
            {
                failCause = PlacementFailCause.InsufficientFunds;
                return false;
            }
            
            foreach (Vector2Int pos in transformedLocalPositions)
            {
                if (!station.DoesGridPosHaveFloorButNoPlaceable(gridPos + pos))
                {
                    failCause = PlacementFailCause.ImproperLocation;
                    return false;
                }
            }

            return true;
        }

        public void TryPlace(Station station, in Vector2Int gridPos, bool costResources, bool ignoreValidity)
        {
            if (!ignoreValidity && !IsValidPlacementAtGridPos(station, gridPos, costResources, 
                out ResourcesContainer cost, out PlacementFailCause cause)) return;

            foreach (Vector2Int localPos in transformedLocalPositions)
            {
                Vector2Int pos = localPos + gridPos;
                if (station.TryGetFloorAtGridPos(pos, out FloorTile floor))
                {
                    floor.SetPlaceable(station, this);
                    _floors.Add(floor);
                }
            }

            Built = true;
            SetGridPosition(station, gridPos);
            
            if (costResources)
                Player.Singleton.resources -= PlacementCost;
        }

        public bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool returnOnValidityAssessment, out List<Vector2Int> validPositions, bool considerCost, 
            out ResourcesContainer cost, out PlacementFailCause cause)
        {
            validPositions = new List<Vector2Int>();
            cost = default;
            cause = PlacementFailCause.None;
            return false;
        }

        public void TryFillRect(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool costResources = true, bool ignoreValidity = false)
        {
        }

        public void Delete(Station station)
        {
            if (BlockDeletion) return;
            foreach (FloorTile floor in _floors)
                floor.RemovePlaceable(station);
            Destroy(gameObject);
        }

        public static Structure CreateInstance(GameObject structure)
        {
            return Instantiate(structure).GetComponent<Structure>();
        }
    }
}