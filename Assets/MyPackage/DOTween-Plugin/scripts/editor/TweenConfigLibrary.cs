using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YellowCat.TweenPlugin.Editor
{
    /// <summary>Locates, creates and duplicates tween config assets.</summary>
    public static class TweenConfigLibrary
    {
        private const string NewFolderPref = "YellowCat.Tween.NewConfigFolder";

        public static readonly (string label, Type type)[] Kinds =
        {
            ("Move", typeof(MoveTweenConfig)),
            ("Scale", typeof(ScaleTweenConfig)),
            ("Rotate", typeof(RotateTweenConfig)),
            ("Fade", typeof(FadeTweenConfig)),
            ("Sequence", typeof(SequenceTweenConfig)),
        };

        // Package root is found from this script's location, so the package
        // can be moved or renamed without breaking paths.
        public static string PackageRoot
        {
            get
            {
                foreach (string guid in AssetDatabase.FindAssets(
                    "TweenConfigLibrary t:MonoScript"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (Path.GetFileNameWithoutExtension(path) !=
                        nameof(TweenConfigLibrary))
                        continue;

                    // <root>/scripts/editor/TweenConfigLibrary.cs
                    return Path.GetDirectoryName(
                        Path.GetDirectoryName(Path.GetDirectoryName(path)))
                        .Replace('\\', '/');
                }

                return "Assets";
            }
        }

        public static string DefaultFolder =>
            PackageRoot + "/configs/tweens/default";

        public static string NewFolder
        {
            get => EditorPrefs.GetString(
                NewFolderPref, PackageRoot + "/configs/tweens/new_configs");
            set => EditorPrefs.SetString(NewFolderPref, value);
        }

        public static bool IsDefault(TweenConfigBase config)
        {
            if (config == null)
                return false;

            string path = AssetDatabase.GetAssetPath(config);
            return path.StartsWith(DefaultFolder + "/", StringComparison.Ordinal);
        }

        // Returns the shared preset, creating it with default values if missing.
        public static TweenConfigBase GetDefault(Type type)
        {
            string path = $"{DefaultFolder}/{Prefix(type)}Tween.asset";
            var existing = AssetDatabase.LoadAssetAtPath<TweenConfigBase>(path);

            if (existing != null)
                return existing;

            EnsureFolder(DefaultFolder);
            return Create(type, path);
        }

        public static TweenConfigBase CreateNew(Type type, string baseName)
        {
            EnsureFolder(NewFolder);

            string path = AssetDatabase.GenerateUniqueAssetPath(
                $"{NewFolder}/{baseName}_{Prefix(type)}.asset");

            return Create(type, path);
        }

        public static TweenConfigBase Duplicate(
            TweenConfigBase source, string baseName)
        {
            EnsureFolder(NewFolder);

            string path = AssetDatabase.GenerateUniqueAssetPath(
                $"{NewFolder}/{baseName}_{source.GetType().Name.Replace("TweenConfig", "")}.asset");

            var copy = (TweenConfigBase)UnityEngine.Object.Instantiate(source);
            copy.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(copy, path);
            AssetDatabase.SaveAssets();
            return copy;
        }

        private static TweenConfigBase Create(Type type, string path)
        {
            var asset = (TweenConfigBase)ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        private static string Prefix(Type type) =>
            type.Name.Replace("TweenConfig", "");

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
                return;

            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
        }
    }
}
