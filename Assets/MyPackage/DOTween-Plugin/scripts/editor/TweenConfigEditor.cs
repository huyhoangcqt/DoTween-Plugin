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
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                // Values are applied now; OnValidate raises the live-reload event.
                serializedObject.ApplyModifiedProperties();
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
