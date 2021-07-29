using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using WaterKat;

namespace WaterKat.CustomVariables
{
    public abstract class CustomReferencePropertyDrawer<TReference, TVariable> : PropertyDrawer
    {
        private GUIAssistant WKGui;
        private const int lines = 1;

        private string[] _optionsA = new string[] { "Use Constant Value", "Use Variable Reference" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            WKGui = new GUIAssistant(position, lines);

            SerializedProperty useConstantProperty = property.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProperty = property.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProperty = property.FindPropertyRelative("Variable");

            if (position.width > 500)
            {
                EditorGUI.LabelField(WKGui.GetRect(0f, 0.2f), new GUIContent(label.text));
                useConstantProperty.boolValue = EditorGUI.Popup(WKGui.GetRect(0.2f, 0.4f), useConstantProperty.boolValue ? 0 : 1, _optionsA) == 0;
            }
            else
            {
                Rect labelRect = WKGui.GetRect(0f, 0.4f);
                labelRect.size = new Vector2(labelRect.size.x - 20, WKGui.lineHeight);
                EditorGUI.LabelField(labelRect, new GUIContent(label.text));

                Rect buttonRect = WKGui.GetRect(0.4f, 0.4f);
                buttonRect.size = new Vector2(20, WKGui.lineHeight);
                buttonRect.position = buttonRect.position - new Vector2(buttonRect.size.x, 0);

                useConstantProperty.boolValue = EditorGUI.Popup(buttonRect, useConstantProperty.boolValue ? 0 : 1, _optionsA) == 0;
            }

            if (useConstantProperty.boolValue)
            {
                EditorGUI.PropertyField(WKGui.GetRect(0.4f, 1f), constantValueProperty, GUIContent.none);
            }
            else
            {

                UnityEngine.Object obj = EditorGUI.ObjectField(WKGui.GetRect(0.4f, 0.7f), GUIContent.none, variableProperty.objectReferenceValue, typeof(TVariable), false);
                variableProperty.objectReferenceValue = obj;
                variableProperty.serializedObject.ApplyModifiedProperties();


                EditorGUI.BeginDisabledGroup(true);
                if (variableProperty.objectReferenceValue != null)
                {
                    SerializedObject serializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                    SerializedProperty valueProperty = serializedObject.FindProperty("value");

                    EditorGUI.PropertyField(WKGui.GetRect(0.71f, 1f), valueProperty, GUIContent.none);
                }
                else
                {
                    EditorGUI.LabelField(WKGui.GetRect(0.71f, 1f), new GUIContent("Variable not assigned!"));
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (WKGui != null)
            {
                return WKGui.GetPropertyHeight();
            }
            else
            {
                return 1;
            }
        }
    }
}