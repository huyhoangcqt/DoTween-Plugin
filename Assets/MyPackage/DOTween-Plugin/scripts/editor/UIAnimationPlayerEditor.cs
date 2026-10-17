using UnityEditor;
using UnityEngine;

namespace YellowCat.TweenPlugin.Editor
{
    [CustomEditor(typeof(UIAnimationPlayer))]
    public class UIAnimationPlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject, "defaultState", "states");
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
                return;

            var player = (UIAnimationPlayer)target;
            TweenConfigBase config = player.Config;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Config", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawKindMenu("Default", GenericMenuMode.Default, player);
                DrawKindMenu("New", GenericMenuMode.New, player);

                using (new EditorGUI.DisabledScope(config == null))
                {
                    if (GUILayout.Button("Ping"))
                        EditorGUIUtility.PingObject(config);

                    if (GUILayout.Button("Clear"))
                        Assign(player, null);
                }
            }

            DrawStates(player);

            if (config == null)
                return;

            if (TweenConfigLibrary.IsDefault(config))
            {
                EditorGUILayout.HelpBox(
                    "This is a shared default preset. Editing it affects every " +
                    "player using it. Make it unique to edit safely.",
                    MessageType.Warning);

                if (GUILayout.Button("Make Unique"))
                {
                    Assign(player, TweenConfigLibrary.Duplicate(
                        config, player.gameObject.name));
                }
            }
            else
            {
                if (GUILayout.Button("Duplicate"))
                {
                    Assign(player, TweenConfigLibrary.Duplicate(
                        config, player.gameObject.name));
                }
            }

            EditorGUILayout.Space();
            DrawFolderSetting();
        }

        private const string StateFolder = "Assets/_game/configs/tween_object_state";

        private void DrawStates(UIAnimationPlayer player)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object States", EditorStyles.boldLabel);

            Transform t = player.TargetTransform;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Record Default State"))
                    RecordDefault(player);

                using (new EditorGUI.DisabledScope(player.DefaultState == null))
                {
                    if (GUILayout.Button("Apply Default"))
                        ApplyState(t, player.DefaultState);
                }
            }

            for (int i = 0; i < player.States.Count; i++)
            {
                TweenObjectState state = player.States[i];

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(
                        state != null ? state.name : "(missing)");

                    using (new EditorGUI.DisabledScope(state == null))
                    {
                        if (GUILayout.Button("Apply", GUILayout.Width(55)))
                            ApplyState(t, state);

                        if (GUILayout.Button("Re-record", GUILayout.Width(75)))
                        {
                            state.Capture(t);
                            EditorUtility.SetDirty(state);
                            AssetDatabase.SaveAssetIfDirty(state);
                        }
                    }

                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        RemoveState(player, i);
                        GUIUtility.ExitGUI();
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+ Add State"))
                    AddState(player);

                using (new EditorGUI.DisabledScope(
                    player.DefaultState == null || player.States.Count == 0))
                {
                    if (GUILayout.Button("Export State to Configs"))
                        Export(player);
                }
            }

            EditorGUILayout.HelpBox(
                $"States are saved in {StateFolder}. Move the object to the " +
                "pose you want, then press + Add State.",
                MessageType.None);
        }

        private static void ApplyState(Transform t, TweenObjectState state)
        {
            if (t == null || state == null)
                return;

            Undo.RecordObject(t, "Apply Tween State");

            if (t.TryGetComponent(out CanvasGroup group))
                Undo.RecordObject(group, "Apply Tween State");

            state.Apply(t);
        }

        private static string StatePath(string fileName) =>
            $"{StateFolder}/{fileName}.asset";

        private static TweenObjectState SaveState(
            Transform t, string path, bool overwrite)
        {
            TweenConfigLibrary.EnsureFolder(StateFolder);

            var existing = overwrite
                ? AssetDatabase.LoadAssetAtPath<TweenObjectState>(path)
                : null;

            TweenObjectState state = existing;

            if (state == null)
            {
                state = CreateInstance<TweenObjectState>();
                AssetDatabase.CreateAsset(
                    state, AssetDatabase.GenerateUniqueAssetPath(path));
            }

            state.Capture(t);
            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssets();
            return state;
        }

        private static void RecordDefault(UIAnimationPlayer player)
        {
            Undo.RecordObject(player, "Record Default State");

            player.DefaultState = SaveState(
                player.TargetTransform,
                StatePath(player.gameObject.name + "_DefaultState"),
                true);

            EditorUtility.SetDirty(player);
        }

        private static void AddState(UIAnimationPlayer player)
        {
            Undo.RecordObject(player, "Add Tween State");

            // First free index, so names stay unique after removals.
            int n = 1;

            while (AssetDatabase.LoadAssetAtPath<TweenObjectState>(StatePath(
                $"{player.gameObject.name}_State_{n}")) != null)
                n++;

            player.States.Add(SaveState(
                player.TargetTransform,
                StatePath($"{player.gameObject.name}_State_{n}"),
                false));

            EditorUtility.SetDirty(player);
        }

        private static void RemoveState(UIAnimationPlayer player, int index)
        {
            Undo.RecordObject(player, "Remove Tween State");
            TweenObjectState state = player.States[index];
            player.States.RemoveAt(index);

            if (state != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(state));

            EditorUtility.SetDirty(player);
        }

        // Builds a Sequence: each state is reached from the previous one,
        // with the changed properties running in parallel (Join).
        private static void Export(UIAnimationPlayer player)
        {
            string baseName = player.gameObject.name;
            var steps = new System.Collections.Generic.List<(TweenConfigBase, bool)>();
            TweenObjectState previous = player.DefaultState;

            for (int i = 0; i < player.States.Count; i++)
            {
                TweenObjectState state = player.States[i];

                if (state == null)
                    continue;

                bool first = true;

                void Add(TweenConfigBase config)
                {
                    steps.Add((config, !first));
                    first = false;
                }

                string prefix = $"{baseName}_State_{i + 1}";

                if (previous.position != state.position)
                    Add(MakeConfig<MoveTweenConfig>(
                        prefix + "_Move", "endPosition",
                        p => p.vector2Value = state.position));

                if (previous.scale != state.scale)
                    Add(MakeConfig<ScaleTweenConfig>(
                        prefix + "_Scale", "endScale",
                        p => p.vector3Value = state.scale));

                if (previous.rotation != state.rotation)
                    Add(MakeConfig<RotateTweenConfig>(
                        prefix + "_Rotate", "endRotation",
                        p => p.vector3Value = state.rotation));

                if (!Mathf.Approximately(previous.alpha, state.alpha))
                    Add(MakeConfig<FadeTweenConfig>(
                        prefix + "_Fade", "endAlpha",
                        p => p.floatValue = state.alpha));

                previous = state;
            }

            if (steps.Count == 0)
            {
                Debug.LogWarning("Export: no differences between states.", player);
                return;
            }

            TweenConfigLibrary.EnsureFolder(TweenConfigLibrary.NewFolder);

            var sequence = CreateInstance<SequenceTweenConfig>();
            AssetDatabase.CreateAsset(
                sequence,
                AssetDatabase.GenerateUniqueAssetPath(
                    $"{TweenConfigLibrary.NewFolder}/{baseName}_StateSequence.asset"));

            var so = new SerializedObject(sequence);
            SerializedProperty list = so.FindProperty("steps");
            list.arraySize = steps.Count;

            for (int i = 0; i < steps.Count; i++)
            {
                SerializedProperty step = list.GetArrayElementAtIndex(i);
                step.FindPropertyRelative("config").objectReferenceValue = steps[i].Item1;
                step.FindPropertyRelative("mode").enumValueIndex =
                    steps[i].Item2
                        ? (int)SequenceTweenConfig.JoinMode.Join
                        : (int)SequenceTweenConfig.JoinMode.Append;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();

            Undo.RecordObject(player, "Export Tween States");
            player.Config = sequence;
            EditorUtility.SetDirty(player);
            EditorGUIUtility.PingObject(sequence);
        }

        private static T MakeConfig<T>(
            string name, string property,
            System.Action<SerializedProperty> setValue)
            where T : TweenConfigBase
        {
            var config = CreateInstance<T>();

            AssetDatabase.CreateAsset(
                config,
                AssetDatabase.GenerateUniqueAssetPath(
                    $"{TweenConfigLibrary.NewFolder}/{name}.asset"));

            var so = new SerializedObject(config);
            setValue(so.FindProperty(property));
            so.ApplyModifiedPropertiesWithoutUndo();
            return config;
        }

        private enum GenericMenuMode
        {
            Default,
            New,
        }

        private void DrawKindMenu(
            string label, GenericMenuMode mode, UIAnimationPlayer player)
        {
            Rect rect = GUILayoutUtility.GetRect(
                new GUIContent(label + " \u25BE"), EditorStyles.miniButton);

            if (!GUI.Button(rect, label + " \u25BE", EditorStyles.miniButton))
                return;

            var menu = new GenericMenu();

            foreach (var kind in TweenConfigLibrary.Kinds)
            {
                var type = kind.type;

                menu.AddItem(new GUIContent(kind.label), false, () =>
                {
                    TweenConfigBase result = mode == GenericMenuMode.Default
                        ? TweenConfigLibrary.GetDefault(type)
                        : TweenConfigLibrary.CreateNew(
                            type, player.gameObject.name);

                    Assign(player, result);
                });
            }

            menu.DropDown(rect);
        }

        private static void Assign(UIAnimationPlayer player, TweenConfigBase config)
        {
            Undo.RecordObject(player, "Assign Tween Config");
            player.Config = config;
            EditorUtility.SetDirty(player);

            if (config != null)
                EditorGUIUtility.PingObject(config);
        }

        private static void DrawFolderSetting()
        {
            EditorGUI.BeginChangeCheck();
            string folder = EditorGUILayout.DelayedTextField(
                new GUIContent("New Configs Folder",
                    "Where 'New' and 'Duplicate' save assets."),
                TweenConfigLibrary.NewFolder);

            if (EditorGUI.EndChangeCheck())
                TweenConfigLibrary.NewFolder = folder.TrimEnd('/');
        }
    }
}
