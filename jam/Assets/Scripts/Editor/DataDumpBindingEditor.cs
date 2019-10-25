using System.Linq;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(DataDumpBinding))]
public class DataDumpBindingEditor : Editor
{
    private int selectedPropertyIndex;
    private string selectedPropertyName;
    
    public override void OnInspectorGUI()
    {
        DataDumpBinding propertyLink = (DataDumpBinding)target;
        selectedPropertyName = propertyLink.DataDumpProperty;
        PropertyInfo[] properties = typeof(PersistantGameState).GetProperties();
        if (!properties.Select(p => p.Name).Contains(selectedPropertyName))
        {
            // This means a property once existed, but was removed from the object and recompiled.
            ((DataDumpBinding)target).Reset();
            selectedPropertyIndex = 0;
        } else
        {
            selectedPropertyIndex = properties
                .Select((p, i) => (i, p.Name))
                .First(p => p.Name.Equals(selectedPropertyName))
                .i;
        }

        string[] propertyNames = properties.Select(p => p.Name).Take(properties.Length - 2).ToArray(); // -2 to remove 'name' and 'hideFlags'
        selectedPropertyIndex = EditorGUILayout.Popup(selectedPropertyIndex, propertyNames);
        propertyLink.DataDumpProperty = properties[selectedPropertyIndex].Name;
        switch (properties[selectedPropertyIndex].PropertyType.Name)
        {
            case "String":
                propertyLink.PropertyType = DataDumpBinding.DataType.String;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStringUpdated"));
                break;
            case "Int32":
                propertyLink.PropertyType = DataDumpBinding.DataType.Int;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnIntUpdated"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnIntUpdatedAsString"));
                break;
            case "Single":
                propertyLink.PropertyType = DataDumpBinding.DataType.Int;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFloatUpdated"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFloatUpdatedAsString"));
                break;
            case "Boolean":
                propertyLink.PropertyType = DataDumpBinding.DataType.Int;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnBoolUpdated"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnBoolUpdatedAsString"));
                break;
            default:
                EditorGUILayout.HelpBox("This property is not bindable. Either make it a supported type, or add support to PropertyLink and PropertyLinkEditor.", MessageType.Error);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
