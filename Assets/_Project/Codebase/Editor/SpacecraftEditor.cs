using _Project.Codebase;
using UnityEditor;
using UnityEngine;

namespace _Project.CodeBase.Editor
{
    [CustomEditor(typeof(Spacecraft), true)]
    [CanEditMultipleObjects]
    public class SpacecraftEditor : CustomEditor<Spacecraft>
    {
        protected override bool MakeInspectorDebugToggleable => false;
        //private List<

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool testbool = false;
            
           // if (EditorGUILayout.Toggle("Test", false))
             //   Debug.Log("yo mama");
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
            
            AddPositionHandle(ref CastedTarget.targetPosition);
        }
    }
    
}