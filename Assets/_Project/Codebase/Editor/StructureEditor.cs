using _Project.Codebase;
using UnityEditor;
using UnityEngine;

namespace _Project.CodeBase.Editor
{
    [CustomEditor(typeof(Structure), true)]
    [CanEditMultipleObjects]
    public class StructureEditor : CustomEditor<Structure>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isEditor)
                CastedTarget.Direction = (Direction)EditorGUILayout.EnumPopup("Direction", CastedTarget.Direction);
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