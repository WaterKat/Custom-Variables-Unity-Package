using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace WaterKat.CustomVariables
{
    [System.Serializable]
    class CustomVariableFactory : EditorWindow
    {
        [SerializeField]
        public List<string> typeList = new List<string>()
        {
            "System.String",
            "System.Char",
            "System.Boolean",
            "System.Double",
            "System.Single",
            "System.Int64",
            "System.Int32",
        };

        [SerializeField]
         public List<string> nameList = new List<string>()
        {
            "String",
            "Char",
            "Bool",
            "Double",
            "Float",
            "Long",
            "Int",
        };

        [SerializeField]
        public List<string> assetMenuList = new List<string>()
        {
            "",
            "Extra/",
            "",
            "Extra/",
            "",
            "Extra/",
            "",
        };

        [SerializeField]
        public List<bool> useExtendedList = new List<bool>()
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
        };

        [SerializeField]
        private string savePath = "Assets/WaterKat/CustomVariables/";

        private const string defaultStateJSON = "{\"typeList\":[\"System.String\",\"System.Char\",\"System.Boolean\",\"System.Double\",\"System.Single\",\"System.Int64\",\"System.Int32\"],\"nameList\":[\"String\",\"Char\",\"Bool\",\"Double\",\"Float\",\"Long\",\"Int\"],\"assetMenuList\":[\"\",\"Extra/\",\"\",\"Extra/\",\"\",\"Extra/\",\"\"],\"useExtendedList\":[false,false,false,false,false,false,false],\"_savePath\":\"Assets/WaterKat/CustomVariables/\",\"ResetSafeguard\":false}";
        private const string CustomVariableTemplatePath = "Packages/com.waterkat.customvariables/Templates/CustomVariableReferenceTemplate.txt";
        private const string CustomReferencePropertyDrawerPath = "Packages/com.waterkat.customvariables/Templates/CustomReferencePropertyDrawerTemplate.txt";
        private const string ExtendedCustomReferencePropertyDrawerPath = "Packages/com.waterkat.customvariables/Templates/ExtendedCustomReferencePropertyDrawerTemplate.txt";

        [SerializeField]
        private bool ResetSafeguard = false;
        [SerializeField]

        protected void OnEnable()
        {
            string data = EditorPrefs.GetString("CustomVariableFactory", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
            JsonUtility.FromJsonOverwrite("{\"ResetSafeguard\":false}", this);
        }
        protected void OnDisable()
        {
            SaveJSONData();
        }

        private void SaveJSONData()
        {
            string data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("CustomVariableFactory", data);
        }

        [MenuItem("Window/WaterKat/Custom Variable Factory")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CustomVariableFactory), true, "Custom Variable Factory Window", true);
        }

        void OnGUI()
        {
            SerializedObject thisObject = new SerializedObject(this);
            thisObject.Update();

            SerializedProperty typeListProperty = thisObject.FindProperty("typeList");
            SerializedProperty nameListProperty = thisObject.FindProperty("nameList");
            SerializedProperty assetMenuListProperty = thisObject.FindProperty("assetMenuList");
            SerializedProperty useExtendedListProperty = thisObject.FindProperty("useExtendedList");

            SerializedProperty savePathProperty = thisObject.FindProperty("savePath");
            SerializedProperty resetSafeguardProperty = thisObject.FindProperty("ResetSafeguard");

            EditorGUILayout.LabelField("Custom Variable and Reference Factory", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please input a type field, and a desired name for the variable/references, on generate");
            EditorGUILayout.LabelField("Use typeof(*CustomClass*).AssemblyQualifiedName to get the string name of the class you'd like to use");

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Reset Variable Names?");
            EditorGUILayout.PropertyField(resetSafeguardProperty, GUIContent.none);

            if (!ResetSafeguard)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
            if (GUILayout.Button("Reset to Default"))
            {
                ResetVariableNames(thisObject);
                resetSafeguardProperty.boolValue = false;
                return;
            };
            if (!ResetSafeguard)
            {
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            for (int i = typeList.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                {
                    typeListProperty.DeleteArrayElementAtIndex(i);
                    nameListProperty.DeleteArrayElementAtIndex(i);
                    assetMenuListProperty.DeleteArrayElementAtIndex(i);
                    useExtendedListProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.LabelField("Type: ", GUILayout.MaxWidth(50));
                EditorGUILayout.PropertyField(typeListProperty.GetArrayElementAtIndex(i), GUIContent.none);

                EditorGUI.BeginDisabledGroup(true);
                if (Type.GetType(typeListProperty.GetArrayElementAtIndex(i).stringValue) != null)
                {
                    EditorGUILayout.LabelField("Valid Type", GUILayout.MaxWidth(75));
                }
                else
                {
                    EditorGUILayout.LabelField("Invalid Type", GUILayout.MaxWidth(75));
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField("Name: ", GUILayout.MaxWidth(50));
                EditorGUILayout.PropertyField(nameListProperty.GetArrayElementAtIndex(i), GUIContent.none);

                EditorGUILayout.LabelField("Asset Directory: ", GUILayout.MaxWidth(100));
                EditorGUILayout.PropertyField(assetMenuListProperty.GetArrayElementAtIndex(i), GUIContent.none);

                EditorGUILayout.PrefixLabel("Extended Prop Drawer?");
                EditorGUILayout.PropertyField(useExtendedListProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MaxWidth(15));

                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Field"))
            {
                typeListProperty.InsertArrayElementAtIndex(0);
                nameListProperty.InsertArrayElementAtIndex(0);
                assetMenuListProperty.InsertArrayElementAtIndex(0);
                assetMenuListProperty.GetArrayElementAtIndex(0).stringValue = "Custom/";
                useExtendedListProperty.InsertArrayElementAtIndex(0);
                useExtendedListProperty.GetArrayElementAtIndex(0).boolValue = false;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Generation Path", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(savePathProperty);

            EditorGUILayout.BeginHorizontal();

            if (savePath[savePath.Length - 1] != '/') { savePath += "/"; }

            if (AssetDatabase.IsValidFolder(savePath))
            {
                EditorGUILayout.LabelField("Valid Directory");
            }
            else
            {
                EditorGUILayout.LabelField("Directory is invalid");
                if (GUILayout.Button("Create Directory"))
                {
                    CreateDirectory(savePath);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (AssetDatabase.IsValidFolder(savePath))
            {
                if (GUILayout.Button("Create Classes"))
                {
                    for (int i = 0; i < typeList.Count; i++)
                    {
                        if (Type.GetType(typeList[i]) == null)
                        {
                            Debug.Log("Type not Valid! : " + typeList[i]);
                            continue;
                        }
                        if ((nameList[i].Length < 1) || (typeList[i].Length < 1))
                        {
                            Debug.Log("Empty Type, Skipping...");
                            continue;
                        }

                        int commaIndex = typeList[i].IndexOf(',');
                        string typeNameSub;
                        if (commaIndex > 0)
                        {
                            typeNameSub = typeList[i].Substring(0, commaIndex);
                        }
                        else
                        {
                            typeNameSub = typeList[i];
                        }
                        string localDirectory = savePath + nameList[i] + "/";
                        CreateDirectory(localDirectory);
                        CreateVariableScript(localDirectory, nameList[i], typeNameSub, assetMenuList[i]);
                        CreatePropertyDrawerScript(localDirectory, nameList[i], typeNameSub, assetMenuList[i], useExtendedList[i] ? ExtendedCustomReferencePropertyDrawerPath : CustomReferencePropertyDrawerPath);
                    }
                    AssetDatabase.Refresh();
                }
            }

            thisObject.ApplyModifiedProperties();
        }

        private void ResetVariableNames(SerializedObject serializedObject)
        {
            EditorPrefs.DeleteKey("CustomVariableFactory");
            JsonUtility.FromJsonOverwrite(defaultStateJSON, this);
            SaveJSONData();
        }

        private void CreateVariableScript(string _savePath,string variableName, string typeName, string assetMenu)
        {
            FileStream newScriptFilestream;
            if (!File.Exists(_savePath + variableName + "Variable.cs"))
            {
                newScriptFilestream = File.Create(_savePath + variableName + "Variable.cs");
            }
            else
            {
                File.WriteAllText(_savePath + variableName + "Variable.cs", "");

                newScriptFilestream = File.Open(_savePath + variableName + "Variable.cs", FileMode.Open);
            }

            FileStream variableTemplateFileStream = File.OpenRead(CustomVariableTemplatePath);
            string templateString;
            using (StreamReader reader = new StreamReader(variableTemplateFileStream))
            {
                templateString = reader.ReadToEnd();
            }
            variableTemplateFileStream.Close();

            templateString = templateString.Replace("#VARIABLENAME#", variableName);
            templateString = templateString.Replace("#VARIABLETYPE#", typeName);
            templateString = templateString.Replace("#ASSETSUBPATH#", assetMenu);
            templateString = templateString.Replace("#ASSETORDER#", (-1000 + assetMenu.Length).ToString());


            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(templateString);
            newScriptFilestream.Write(bytes, 0, bytes.Length);

            newScriptFilestream.Close();
        }

        private void CreatePropertyDrawerScript(string _savePath, string variableName, string typeName, string assetMenu, string templatePath)
        {
            CreateDirectory(_savePath + "Editor/");
            FileStream newScriptFilestream;
            if (!File.Exists(_savePath + "Editor/" + variableName + "ReferencePropertyDrawer.cs"))
            {
                newScriptFilestream = File.Create(_savePath + "Editor/" + variableName + "ReferencePropertyDrawer.cs");
            }
            else
            {
                newScriptFilestream = File.Open(_savePath + "Editor/" + variableName + "ReferencePropertyDrawer.cs", FileMode.Open);
            }

            FileStream variableTemplateFileStream = File.OpenRead(templatePath);

            string templateString;
            using (StreamReader reader = new StreamReader(variableTemplateFileStream))
            {
                templateString = reader.ReadToEnd();
            }
            variableTemplateFileStream.Close();

            templateString = templateString.Replace("#VARIABLENAME#", variableName);
            templateString = templateString.Replace("#VARIABLETYPE#", typeName);
            templateString = templateString.Replace("#ASSETSUBPATH#", assetMenu);
            templateString = templateString.Replace("#ASSETORDER#", (-1000 + assetMenu.Length).ToString());


            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(templateString);
            newScriptFilestream.Write(bytes, 0, bytes.Length);

            newScriptFilestream.Close();
        }

        private void CreateDirectory(string path)
        {
            string saveDir = path;
            string pathSoFar = "";

            for (int i = 0; i < path.Length; i++)
            {
                int nextSlash = saveDir.IndexOf('/');

                string currentFolder = saveDir.Substring(0, nextSlash + 1);
                saveDir = saveDir.Substring(nextSlash + 1);

                if (!AssetDatabase.IsValidFolder(pathSoFar + currentFolder))
                {
                    Debug.Log("Creating " + currentFolder + " in " + pathSoFar);
                    AssetDatabase.CreateFolder(pathSoFar.Substring(0, pathSoFar.Length - 1), currentFolder.Substring(0, currentFolder.Length - 1));
                }
                else
                {
                    //Debug.Log(pathSoFar + currentFolder+" already exists");
                }

                pathSoFar += currentFolder;

                if (nextSlash == -1)
                {
                    //Debug.Log("Directory is now Valid");
                    break;
                }
            }
        }
    }
}