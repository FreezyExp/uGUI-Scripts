using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( CanvasScaleBroadcast ), false )]
[CanEditMultipleObjects]
public class CanvasScaleBroadcasterInspector : Editor {
    //SerializedProperty m_Messages;

    //protected virtual void OnEnable() {
    //    m_Messages = serializedObject.FindProperty( "m_Messages" );
    //}

    //public override void OnInspectorGUI() {
    //    serializedObject.Update();
    //    DrawDefaultInspector();        
    //    EditorGUILayout.PropertyField( m_Messages, true );
    //    serializedObject.ApplyModifiedProperties();
    //}
}
