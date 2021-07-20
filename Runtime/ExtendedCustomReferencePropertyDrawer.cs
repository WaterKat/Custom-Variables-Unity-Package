using UnityEditor;
using UnityEngine;
using WaterKat;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WaterKat.CustomVariables
{
    public abstract class ExtendedCustomReferencePropertyDrawer<TReference, TVariable> : PropertyDrawer
    {
        private GUIAssistant WKGui;
        private int lines = 1;

        private string[] _options = new string[] { "Use Constant", "Use Variable Reference" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            WKGui = new GUIAssistant(position, lines);

            SerializedProperty useConstantProperty = property.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProperty = property.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProperty = property.FindPropertyRelative("Variable");

            EditorGUI.LabelField(WKGui.GetRect(0f, 0.6f), new GUIContent(label.text));


            useConstantProperty.boolValue = EditorGUI.Popup(WKGui.GetRect(0.6f, 1f), useConstantProperty.boolValue ? 0 : 1, _options) == 0;

            WKGui.NextLine();

            if (useConstantProperty.boolValue)
            {
                EditorGUI.PropertyField(WKGui.GetRect(), constantValueProperty, true);
            }
            else
            {
                EditorGUI.LabelField(WKGui.GetRect(0f, 0.4f), new GUIContent("Referenced Variable"));
                UnityEngine.Object obj = EditorGUI.ObjectField(WKGui.GetRect(0.4f, 1f), variableProperty.objectReferenceValue, typeof(TVariable), false);
                variableProperty.objectReferenceValue = obj;
                variableProperty.serializedObject.ApplyModifiedProperties();

                WKGui.NextLine();

                if (variableProperty.objectReferenceValue != null)
                {
                    SerializedObject serializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                    SerializedProperty valueProperty = serializedObject.FindProperty("Value");

                    EditorGUI.PropertyField(WKGui.GetRect(), valueProperty, true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty useConstantProperty = property.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProperty = property.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProperty = property.FindPropertyRelative("Variable");

            float size = 16f;
            if (useConstantProperty.boolValue)
            {
                size += EditorGUI.GetPropertyHeight(constantValueProperty, true);
            }
            else
            {
                size += 16f;
                if (variableProperty.objectReferenceValue != null)
                {
                    SerializedObject serializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                    SerializedProperty valueProperty = serializedObject.FindProperty("Value");

                    size += EditorGUI.GetPropertyHeight(valueProperty, true);
                }
            }

            return size;
        }
    }
}