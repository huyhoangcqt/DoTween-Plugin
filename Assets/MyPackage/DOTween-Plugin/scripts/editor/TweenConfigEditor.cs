using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YellowCat.TweenPlugin.Editor
{
    [CustomEditor(typeof(TweenConfigBase), true)]
    public class TweenConfigEditor : UnityEditor.Editor
    {
        private static readonly Dictionary<int, string> Snapshots =
            new Dictionary<int, string>();

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                    TakeSnapshots();
                else if (state == PlayModeStateChange.EnteredEditMode)
                    Snapshots.Clear();
            };
        }

        // Remember every config's values at play start so they can be reverted.
        private static void TakeSnapshots()
        {
            Snapshots.Clear();

            foreach (string guid in AssetDatabase.FindAssets("t:TweenConfigBase"))
            {
                var config = AssetDatabase.LoadAssetAtPath<TweenConfigBase>(
                    AssetDatabase.GUIDToAssetPath(guid));

                if (config != null)
                    Snapshots[config.GetInstanceID()] =
                        EditorJsonUtility.ToJson(config);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawProperties((TweenConfigBase)target);

            if (EditorGUI.EndChangeCheck())
            {
                // Values are applied now; OnValidate raises the live-reload event.
                EditorUtility.SetDirty(target);
            }

            if (!Application.isPlaying)
                return;

            var config = (TweenConfigBase)target;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Play mode: edits apply live. Save writes them to the asset.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Reset GameObject State"))
                    ResetState(config);

                if (GUILayout.Button("Replay"))
                    Replay(config);

                using (new EditorGUI.DisabledScope(
                    !Snapshots.ContainsKey(config.GetInstanceID())))
                {
                    if (GUILayout.Button("Revert"))
                        Revert(config);
                }

                if (GUILayout.Button("Save"))
                    Save(config);
            }
        }

        // Sequence duration is computed from its steps, so it is read-only.
        // Total Duration is shown right below Duration.
        private void DrawProperties(TweenConfigBase config)
        {
            serializedObject.Update();

            bool isSequence = config is SequenceTweenConfig
                || config is TweenConfigPreset;
            bool isPreset = config is TweenConfigPreset;
            SerializedProperty useCurve =
                serializedObject.FindProperty("useCurve");
            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (prop.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(prop);
                }
                else if (prop.propertyPath == "duration")
                {
                    if (isSequence)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.FloatField(
                                "Duration (auto)", config.Duration);
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }

                    using (new EditorGUI.DisabledScope(true))
                    {
                        if (config.Loops < 0)
                            EditorGUILayout.TextField(
                                "Total Duration", "Infinite");
                        else
                            EditorGUILayout.FloatField(
                                "Total Duration", config.TotalDuration);
                    }
                }
                else if (isPreset && (prop.propertyPath == "ease"
                    || prop.propertyPath == "useCurve"
                    || prop.propertyPath == "curve"))
                {
                    // Presets choose their own eases.
                }
                else if (prop.propertyPath == "ease")
                {
                    using (new EditorGUI.DisabledScope(useCurve.boolValue))
                        EditorGUILayout.PropertyField(prop, true);
                }
                else if (prop.propertyPath == "curve")
                {
                    if (useCurve.boolValue)
                        EditorGUILayout.PropertyField(prop, true);
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void Save(TweenConfigBase config)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssetIfDirty(config);
            Snapshots[config.GetInstanceID()] = EditorJsonUtility.ToJson(config);
            Debug.Log($"Saved tween config: {config.name}", config);
        }

        private static void Revert(TweenConfigBase config)
        {
            if (!Snapshots.TryGetValue(config.GetInstanceID(), out string json))
                return;

            EditorJsonUtility.FromJsonOverwrite(json, config);
            EditorUtility.SetDirty(config);
            config.NotifyChanged();
        }

        private static void ResetState(TweenConfigBase config)
        {
            foreach (UIAnimationPlayer player in FindPlayers())
            {
                if (player.Config != null && player.Config.Uses(config))
                    player.ResetState();
            }
        }

        private static UIAnimationPlayer[] FindPlayers()
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindObjectsByType<UIAnimationPlayer>(
                FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<UIAnimationPlayer>();
#endif
        }

        private static void Replay(TweenConfigBase config)
        {
            foreach (UIAnimationPlayer player in FindPlayers())
            {
                if (player.Config != null && player.Config.Uses(config))
                    player.Play();
            }
        }
    }
}
