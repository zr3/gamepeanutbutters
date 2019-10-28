using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InspectorNote))]
public class InspectorNoteEditor : Editor
{
    private bool isEditing = false;
    public override void OnInspectorGUI()
    {
        InspectorNote inspectorNote = (InspectorNote)target;
        isEditing = EditorGUILayout.Toggle("Edit", isEditing);
        if (isEditing) {
            inspectorNote.Message = GUILayout.TextArea(inspectorNote.Message);
        } else
        {
            EditorGUILayout.HelpBox(inspectorNote.Message, MessageType.Info);
        }
    }
}
