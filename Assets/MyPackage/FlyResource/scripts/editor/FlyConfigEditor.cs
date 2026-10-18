using UnityEditor;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    [CustomEditor(typeof(FlyConfig), true)]
    public class FlyConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = (FlyConfig)target;
            bool hasBurst = config.Style == FlyStyle.FlyBlooming
                || config.Style == FlyStyle.FlyFirework;
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(property);
                }
                else if (property.propertyPath == "burstRadius")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.EnumPopup("FlyStyle", config.Style);

                    if (hasBurst)
                        EditorGUILayout.PropertyField(property, true);
                }
                else if (property.propertyPath == "burstDuration"
                    || property.propertyPath == "holdDuration")
                {
                    if (hasBurst)
                        EditorGUILayout.PropertyField(property, true);
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
