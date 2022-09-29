using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Text.RegularExpressions;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{

    SerializedProperty weaponsProperty;

    private ReorderableList weaponsList;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        weaponsProperty = serializedObject.FindProperty("weapons");
        weaponsList = new SubclassableListView<EnemyAttackChoice>(serializedObject, weaponsProperty, true, true, true, true);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "weapons");
        weaponsList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
