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
        weaponsList = new ReorderableList(serializedObject, weaponsProperty, true, true, true, true);
        weaponsList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Weapons");
        };
        weaponsList.elementHeightCallback = (int index) => {
            SerializedProperty element = weaponsProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };
        weaponsList.drawElementCallback = 
            (Rect rect, int index, bool isActive, bool isFocused) => {
                // EditorGUI.indentLevel = 0;
                rect.x += 8;
                rect.width -= 8;
                SerializedProperty element = weaponsProperty.GetArrayElementAtIndex(index);
                string labelName = Regex.Replace(element.managedReferenceFullTypename, ".* ", "");
                EditorGUI.PropertyField(rect, element, new GUIContent(labelName), true);
        };
        weaponsList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (System.Type type in EnemyAttackChoice.Subclasses) {
                menu.AddItem(new GUIContent(type.Name), false, clickHandler, type);
            }
            menu.ShowAsContext();
        };
    }

    private void clickHandler(object target) {
        System.Type type = (System.Type)target;
        int newIndex = 0; //weaponsProperty.arraySize-1;
        weaponsProperty.InsertArrayElementAtIndex(newIndex);
        weaponsProperty.GetArrayElementAtIndex(newIndex).managedReferenceValue = type.GetConstructor(new System.Type[0]).Invoke(new object[0]);
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "weapons");
        weaponsList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
