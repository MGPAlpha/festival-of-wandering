using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using System.Linq;

public class SubclassableListView<T> : ReorderableList
{
    
    public SubclassableListView(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton) {
        this.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, serializedProperty.displayName);
        };

        this.elementHeightCallback = (int index) => {
            SerializedProperty element = elements.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };

        this.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            // EditorGUI.indentLevel = 0;
            rect.x += 8;
            rect.width -= 8;
            SerializedProperty element = elements.GetArrayElementAtIndex(index);


            System.Type elementType = typeof(T);
            string labelName;
            System.Reflection.MethodInfo subclassNameMethod = elementType.GetMethod("GetSubclassName");
            if (subclassNameMethod != null) {
                labelName = (string)subclassNameMethod.Invoke(null, null);
            } else {
                labelName = elementType.Name;
            }

            // labelName = element.managedReferenceFullTypename;

            EditorGUI.PropertyField(rect, element, new GUIContent(labelName), true);
        };

        this.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            // Get subclass types
            System.Type[] subclasses = typeof(T).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(EnemyAttackChoice))).ToArray();
            foreach (System.Type type in subclasses) {
                GUIContent itemLabel;
                System.Reflection.MethodInfo subclassNameMethod = type.GetMethod("GetSubclassName");
                if (subclassNameMethod != null) {
                    itemLabel = new GUIContent((string)subclassNameMethod.Invoke(null, null));
                } else {
                    itemLabel = new GUIContent(type.Name);
                }
                menu.AddItem(itemLabel, false, clickHandler, type);
            }
            menu.ShowAsContext();
        };
    }

    private void clickHandler(object target) {
        System.Type type = (System.Type)target;
        int newIndex = this.serializedProperty.arraySize; //weaponsProperty.arraySize-1;
        Debug.Log(newIndex);
        this.serializedProperty.InsertArrayElementAtIndex(newIndex);
        this.serializedProperty.GetArrayElementAtIndex(newIndex).managedReferenceValue = type.GetConstructor(new System.Type[0]).Invoke(new object[0]);
        this.serializedProperty.serializedObject.ApplyModifiedProperties();
    }

}
