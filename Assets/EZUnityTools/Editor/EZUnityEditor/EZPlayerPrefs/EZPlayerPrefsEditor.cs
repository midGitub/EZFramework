/*
 * Author:      熊哲
 * CreateTime:  1/25/2018 10:15:14 AM
 * Description:
 * 
*/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EZUnityEditor
{
    public class EZPlayerPrefsEditor : EZEditorWindow
    {
        private enum ValueType { String = 0, Int = 1, Float = 2 }
        [Serializable]
        private struct KeyTypePair
        {
            public string key;
            public ValueType type;
            public KeyTypePair(string key, ValueType type)
            {
                this.key = key;
                this.type = type;
            }
        }

        private List<KeyTypePair> pairList = new List<KeyTypePair>();
        private Vector2 scrollPosition;

        private void GetPairsWin()
        {
            pairList.Clear();
            string companyName = PlayerSettings.companyName;
            string productName = PlayerSettings.productName;
            string subKey = string.Format("{0}\\{1}\\{2}", "Software\\Unity\\UnityEditor", companyName, productName);
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey != null)
            {
                string[] valueNames = registryKey.GetValueNames();
                for (int i = 0; i < valueNames.Length; i++)
                {
                    string valueName = valueNames[i];
                    if (valueName.ToLower().StartsWith("unity")) continue;
                    if (valueName.StartsWith("PackageUpdater")) continue;
                    string key = valueName.Substring(0, valueName.LastIndexOf("_"));
                    Type type = registryKey.GetValue(valueName).GetType();
                    if (type == typeof(byte[]))
                        pairList.Add(new KeyTypePair(key, ValueType.String));
                    // it doesn't work...
                    //else if (type == typeof(float)) 
                    //    pairList.Add(new KeyTypePair(key, ValueType.Float));
                    else if (type == typeof(int))
                    {
                        // int value could not be "-1" and "0" at the same time
                        // if it does, it's definitely not a int value
                        if (PlayerPrefs.GetInt(key, -1) == -1 && PlayerPrefs.GetInt(key, 0) == 0)
                            pairList.Add(new KeyTypePair(key, ValueType.Float));
                        else
                            pairList.Add(new KeyTypePair(key, ValueType.Int));
                    }
                }
            }
        }
        private void Refresh()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GetPairsWin();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
            EditorGUILayout.Space();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < pairList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string key = pairList[i].key;
                ValueType type = pairList[i].type;
                if (type == ValueType.String)
                {
                    EditorGUILayout.LabelField("string", new GUILayoutOption[] { GUILayout.Width(40), });
                    string input = EditorGUILayout.DelayedTextField(key, PlayerPrefs.GetString(key));
                    if (input != PlayerPrefs.GetString(key))
                    {
                        PlayerPrefs.SetString(key, input);
                    }
                }
                else if (type == ValueType.Int)
                {
                    EditorGUILayout.LabelField("int", new GUILayoutOption[] { GUILayout.Width(40), });
                    int input = EditorGUILayout.DelayedIntField(key, PlayerPrefs.GetInt(key));
                    if (input != PlayerPrefs.GetInt(key))
                    {
                        PlayerPrefs.SetInt(key, input);
                    }
                }
                else if (type == ValueType.Float)
                {
                    EditorGUILayout.LabelField("float", new GUILayoutOption[] { GUILayout.Width(40), });
                    float input = EditorGUILayout.DelayedFloatField(key, PlayerPrefs.GetFloat(key));
                    if (input != PlayerPrefs.GetFloat(key))
                    {
                        PlayerPrefs.SetFloat(key, input);
                    }
                }
                else
                {
                    EditorGUILayout.TextField(key, "Unknown Type");
                }
                if (GUILayout.Button("Delete", new GUILayoutOption[] { GUILayout.Width(80), }))
                {
                    PlayerPrefs.DeleteKey(key);
                    Refresh();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Delete All"))
            {
                PlayerPrefs.DeleteAll();
                Refresh();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}