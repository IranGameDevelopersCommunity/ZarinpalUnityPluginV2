using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZarinpalIAB.Editor
{
    [CustomEditor(typeof(IABConfig))]
    public class IABConfigEditor : UnityEditor.Editor
    {
        private bool m_changed = false;
        private bool _mFoldoutAndroid;
        private bool _mFoldoutIOS;
        private bool _mFoldoutZarinpal;

        private SerializedProperty CallbackurlProperty;

        public override void OnInspectorGUI()
        {
            var changed = false;
            var color = GUI.color;
            var enable = serializedObject.FindProperty("Enable");

            var merchantIDProp = serializedObject.FindProperty("MerchantID");
            var autoVerifyProp = serializedObject.FindProperty("AutoVerifyPurchase");
            var _schemeProp = serializedObject.FindProperty("Scheme");
            var _hostProp = serializedObject.FindProperty("Host");
            var _useCallbackProp = serializedObject.FindProperty("UseSchemeAndHostAsCallbackUrl");
            CallbackurlProperty = serializedObject.FindProperty("CallbackUrl");
            var _autoStartPurchaseProp = serializedObject.FindProperty("AutoStartPurchase");
            var logEnabledProp = serializedObject.FindProperty("LogEnabled");


            EditorGUILayout.LabelField("Zarinpal Setting");
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(enable, new GUIContent("Enable"));
            //Zarinpal
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(merchantIDProp);
            EditorGUILayout.PropertyField(autoVerifyProp);
            EditorGUILayout.PropertyField(_autoStartPurchaseProp);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("IOS Sdk version : " + PlayerSettings.iOS.targetOSVersionString);
            try
            {
                if (Convert.ToSingle(PlayerSettings.iOS.targetOSVersionString) < 8F)
                {
                    color = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Zarinpal need sdk version 8.0 or higher");

                    GUI.color = Color.green;
                    if (GUILayout.Button("Set IOS SDK to 8.0"))
                    {
                        PlayerSettings.iOS.targetOSVersionString = "8.0";
                    }

                    GUI.color = color;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Provide a unique scheme and host for android\nOthewise your app may conflicts with other apps.\nZarinpal use this scheme and host to identify your app\nand return the purchase result. ",
                GUILayout.Height(60));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_schemeProp);
            EditorGUILayout.PropertyField(_hostProp);
            EditorGUILayout.PropertyField(_useCallbackProp);
            if (_useCallbackProp.boolValue)
            {
                CallbackurlProperty.stringValue =
                    string.Format("{0}://{1}", _schemeProp.stringValue, _hostProp.stringValue);
            }

            EditorGUILayout.PropertyField(CallbackurlProperty);
            changed = changed | EditorGUI.EndChangeCheck();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(logEnabledProp);

            if (changed)
            {
                m_changed = true;
            }

            color = GUI.color;
            if (m_changed)
            {
                EditorGUILayout.Space();
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Hit Update to make changes affected.");
                GUI.color = color;
            }

            color = GUI.color;
            if (m_changed)
            {
                GUI.color = Color.green;
            }

            if (GUILayout.Button("Update Manifest & Files"))
            {
                applySettings(!enable.boolValue, !enable.boolValue, enable.boolValue);
                m_changed = false;
            }

            GUI.color = color;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Author : Mojtaba Pirveisi");
            EditorGUILayout.LabelField("@ 2018 KingKode Studio");

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void applySettings(bool removeAndroidLibs, bool removeIosLibs, bool addZarrinpalToManifest = true)
        {
            var pluginDirectoryAndroid = Path.Combine(Application.dataPath, "Plugins/Android");
            if (!Directory.Exists(pluginDirectoryAndroid))
            {
                Directory.CreateDirectory(pluginDirectoryAndroid);
            }

            var pluginDirectoryIOS = Path.Combine(Application.dataPath, "Plugins/IOS");
            if (!Directory.Exists(pluginDirectoryIOS))
            {
                Directory.CreateDirectory(pluginDirectoryIOS);
            }

            handleZarinpalJars(removeAndroidLibs);
            handleZarinpalIOS(removeIosLibs);

            if (addZarrinpalToManifest)
            {
                IABManifestTools.GenerateManifest();
            }
            else
            {
                IABManifestTools.RemoveZarrinpalFromManifest();
            }

            AssetDatabase.Refresh();
        }


        public static void SetCallback(bool useWebCallback, string webCallback = "")
        {
            var settings = Resources.Load<IABConfig>("ZarrinpalIABSetting");

            if (useWebCallback)
            {
                settings.CallbackUrl = webCallback;
            }
            else
            {
                settings.CallbackUrl = string.Format("{0}://{1}", settings.Scheme, settings.Host);
            }
        }

        public static void ApplySettings(bool removeAndroidLibs, bool removeIosLibs, bool addZarrinpalToManifest = true)
        {
            var pluginDirectoryAndroid = Path.Combine(Application.dataPath, "Plugins/Android");
            if (!Directory.Exists(pluginDirectoryAndroid))
            {
                Directory.CreateDirectory(pluginDirectoryAndroid);
            }

            var pluginDirectoryIOS = Path.Combine(Application.dataPath, "Plugins/IOS");
            if (!Directory.Exists(pluginDirectoryIOS))
            {
                Directory.CreateDirectory(pluginDirectoryIOS);
            }

            handleZarinpalJars(removeAndroidLibs);
            handleZarinpalIOS(removeIosLibs);

            if (addZarrinpalToManifest)
            {
                IABManifestTools.GenerateManifest();
            }
            else
            {
                IABManifestTools.RemoveZarrinpalFromManifest();
            }

            AssetDatabase.Refresh();
        }


        private void ApplyRadioButtonBehaviour(SerializedProperty checkProp, List<SerializedProperty> uncheckProps)
        {
            foreach (var property in uncheckProps)
            {
                if (property != checkProp)
                {
                    property.boolValue = false;
                }
            }
        }


        public static void handleZarinpalJars(bool remove)
        {
            try
            {
                if (remove)
                {
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/library-1.0.19.jar");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/library-1.0.19.jar.meta");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/UnityZarinpalPurchase.aar");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/UnityZarinpalPurchase.aar.meta");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/constraint-layout-1.1.2.aar");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/constraint-layout-1.1.2.aar.meta");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/constraint-layout-solver-1.1.2.jar");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/Android/constraint-layout-solver-1.1.2.jar.meta");

                }
                else
                {

                    string bpRootPath = Application.dataPath +
                                        "/Zarinpal/Templates/Android/";

                    FileUtil.CopyFileOrDirectory(bpRootPath + "UnityZarinpalPurchase.aar",
                        Application.dataPath + "/Plugins/Android/UnityZarinpalPurchase.aar");
                }
            }
            catch
            {
            }
        }



        public static void handleZarinpalIOS(bool remove)
        {
            try
            {
                if (remove)
                {
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/HttpRequest.swift");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/HttpRequest.swift.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/PaymentViewController.swift");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/PaymentViewController.swift.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/URLs.swift");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/URLs.swift.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinPalSDKPayment.h");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinPalSDKPayment.h.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnity.swift");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnity.swift.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityBridge.mm");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityBridge.mm.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityPlugin-Bridging-Header.h");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityPlugin-Bridging-Header.h.meta");

                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityWrapper.swift");
                    FileUtil.DeleteFileOrDirectory(Application.dataPath +
                                                   "/Plugins/IOS/ZarinpalUnityWrapper.swift.meta");


                }
                else
                {

                    string bpRootPath = Application.dataPath +
                                        "/Zarinpal/Templates/IOS/";

                    FileUtil.CopyFileOrDirectory(bpRootPath + "HttpRequest.swift",
                        Application.dataPath + "/Plugins/IOS/HttpRequest.swift");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "PaymentViewController.swift",
                        Application.dataPath + "/Plugins/IOS/PaymentViewController.swift");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "URLs.swift",
                        Application.dataPath + "/Plugins/IOS/URLs.swift");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "ZarinPalSDKPayment.h",
                        Application.dataPath + "/Plugins/IOS/ZarinPalSDKPayment.h");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "ZarinpalUnity.swift",
                        Application.dataPath + "/Plugins/IOS/ZarinpalUnity.swift");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "ZarinpalUnityBridge.mm",
                        Application.dataPath + "/Plugins/IOS/ZarinpalUnityBridge.mm");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "ZarinpalUnityPlugin-Bridging-Header.h",
                        Application.dataPath + "/Plugins/IOS/ZarinpalUnityPlugin-Bridging-Header.h");

                    FileUtil.CopyFileOrDirectory(bpRootPath + "ZarinpalUnityWrapper.swift",
                        Application.dataPath + "/Plugins/IOS/ZarinpalUnityWrapper.swift");
                }
            }
            catch
            {
            }
        }


        [MenuItem("Zarinpal/Setting")]
        static void ShowConfig()
        {
            string path = "Assets/Zarinpal/Resources/ZarrinpalIABSetting.asset";
            var config = AssetDatabase.LoadAssetAtPath<IABConfig>(path);
            if (config == null)
            {
                config = IABConfig.CreateInstance<IABConfig>();
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
            }

            Selection.activeObject = config;
        }


        [MenuItem("Zarinpal/Products")]
        static void ShowProducts()
        {
            string path = "Assets/Zarinpal/Resources/ZarinpalProductSetting.asset";
            var config = AssetDatabase.LoadAssetAtPath<ZarinpalProductSetting>(path);
            if (config == null)
            {
                config = IABConfig.CreateInstance<ZarinpalProductSetting>();
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
            }

            Selection.activeObject = config;
        }
    }
}
