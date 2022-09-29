using _Project.Codebase;
using UnityEditor;
using UnityEngine;

namespace _Project.CodeBase.Editor
{
    [CustomEditor(typeof(Structure), true)]
    [CanEditMultipleObjects]
    public class StructureEditor : CustomEditor<Structure>
    {
        private PlaceableScriptable _scriptable;
        private PlaceableName _name;

        public override void OnInspectorGUI()
        {
            /*
            if (Application.isEditor)
            {
                EditorGUI.BeginChangeCheck();
                AddEnumField(ref _name, "Structure Name");
                AddObjectField(ref _scriptable, "Scriptable");
                if (EditorGUI.EndChangeCheck())
                {
                    if (_scriptable != null)
                    {
                        StructurePrefabData structureData = _scriptable.structureData.Find(data => data.placeableName == _name);
                        CastedTarget.PlaceableName = structureData.placeableName;
                        CastedTarget.Type = PlaceableType.Structure;
                        CastedTarget.PlacementCost = structureData.placementCost;
                        Debug.Log("Assigned structure data");
                    }
                }
            }
            */
            
            base.OnInspectorGUI();
            
            if (Application.isEditor)
                CastedTarget.Direction = (Direction) EditorGUILayout.EnumPopup("Direction", CastedTarget.Direction);
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (!debug) return;

            //Vector2 pivot = CastedTarget.GetLocalPivot();
            Handles.color = Color.green;
            foreach (Vector2Int localPos in CastedTarget.transformedLocalPositions)
            {
                Handles.DrawWireCube((Vector2)CastedTarget.transform.position + localPos + 
                                     CastedTarget.evenOffset, Vector3.one);
            }
            
            //Handles.DrawWireDisc(CastedTarget.GetLocalCenter()
            //    + (Vector2)CastedTarget.transform.position,
            //    Vector3.forward, .125f);
            
            Handles.color = Color.yellow;
            Handles.DrawWireDisc( (Vector2)CastedTarget.transform.position, 
                Vector3.forward, .25f);
        }
    }
}