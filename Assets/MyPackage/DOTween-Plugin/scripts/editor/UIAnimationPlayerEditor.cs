using UnityEditor;
using UnityEngine;

namespace YellowCat.TweenPlugin.Editor
{
    [CustomEditor(typeof(UIAnimationPlayer))]
    public class UIAnimationPlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

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
