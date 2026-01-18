using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationPathUpdater : EditorWindow
{
    private string oldPath = "";
    private string newPath = "";
    private bool dryRun = true;
    private int matchModeIndex = 0; // 0 Contains, 1 StartsWith, 2 Equals

    private readonly string[] matchModeNames = new[] { "Contains", "StartsWith", "Equals" };

    [MenuItem("Tools/Animation Path Updater")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationPathUpdater), false, "Animation Path Updater");
    }

    void OnGUI()
    {
        GUILayout.Label("Update Animation Paths", EditorStyles.boldLabel);
        oldPath = EditorGUILayout.TextField("Old Path:", oldPath);
        newPath = EditorGUILayout.TextField("New Path:", newPath);
        matchModeIndex = EditorGUILayout.Popup("Match Mode", matchModeIndex, matchModeNames);
        dryRun = EditorGUILayout.Toggle("Dry Run (no write)", dryRun);

        if (GUILayout.Button("Update Selected Animations"))
        {
            UpdateSelectedAnimations();
        }

        EditorGUILayout.HelpBox("Dry Run: выводит список binding'ів і покаже, що буде змінено. Якщо все вірно — вимкни Dry Run і повтори.", MessageType.Info);
        EditorGUILayout.HelpBox("Якщо кліп всередині FBX — можливо, потрібно екстрагувати .anim з FBX або працювати з вбудованими шляхами.", MessageType.Warning);
    }

    void UpdateSelectedAnimations()
    {
        var selected = Selection.objects;
        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("[AnimationPathUpdater] No objects selected.");
            return;
        }

        int totalUpdated = 0;
        int clipCount = 0;

        for (int i = 0; i < selected.Length; i++)
        {
            var obj = selected[i];
            AnimationClip clip = obj as AnimationClip;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (clip == null && !string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".anim"))
            {
                clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            }

            if (clip == null)
            {
                Debug.LogWarning($"[AnimationPathUpdater] Skipping selected object '{obj.name}' — not an AnimationClip.");
                continue;
            }

            clipCount++;
            EditorUtility.DisplayProgressBar("Updating animation paths", clip.name, (float)clipCount / selected.Length);

            int updatedBindings = 0;
            int listed = 0;

            // log header
            Debug.Log($"[AnimationPathUpdater] Processing clip '{clip.name}' ({AssetDatabase.GetAssetPath(clip)})");

            // Curve bindings
            var bindings = AnimationUtility.GetCurveBindings(clip);
            Debug.Log($"[AnimationPathUpdater] Curve bindings count: {bindings.Length}");
            foreach (var binding in bindings)
            {
                listed++;
                Debug.Log($"  Binding [{listed}] path:'{binding.path}' property:'{binding.propertyName}' type:{binding.type.Name}");
                if (IsMatch(binding.path, oldPath))
                {
                    Debug.Log($"    -> MATCH (will replace)");
                    if (!dryRun)
                    {
                        var curve = AnimationUtility.GetEditorCurve(clip, binding);
                        var fixedPath = binding.path.Replace(oldPath, newPath);
                        var newBinding = new EditorCurveBinding { path = fixedPath, propertyName = binding.propertyName, type = binding.type };
                        AnimationUtility.SetEditorCurve(clip, newBinding, curve);
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        updatedBindings++;
                    }
                }
            }

            // Object reference bindings (sprites, material refs etc.)
            var objBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            Debug.Log($"[AnimationPathUpdater] ObjectReference bindings count: {objBindings.Length}");
            listed = 0;
            foreach (var binding in objBindings)
            {
                listed++;
                Debug.Log($"  ObjBinding [{listed}] path:'{binding.path}' property:'{binding.propertyName}' type:{binding.type.Name}");
                if (IsMatch(binding.path, oldPath))
                {
                    Debug.Log($"    -> MATCH (will replace)");
                    if (!dryRun)
                    {
                        var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                        var fixedPath = binding.path.Replace(oldPath, newPath);
                        var newBinding = new EditorCurveBinding { path = fixedPath, propertyName = binding.propertyName, type = binding.type };
                        AnimationUtility.SetObjectReferenceCurve(clip, newBinding, keys);
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                        updatedBindings++;
                    }
                }
            }

            if (updatedBindings > 0)
            {
                string clipPath = AssetDatabase.GetAssetPath(clip);
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(clipPath, ImportAssetOptions.ForceUpdate);
                totalUpdated += updatedBindings;
                Debug.Log($"✅ Updated {updatedBindings} bindings in '{clip.name}'");
            }
            else
            {
                Debug.Log($"[AnimationPathUpdater] No bindings updated in '{clip.name}'. If you expected matches, inspect the above binding list and adjust Old Path or Match Mode.");
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"[AnimationPathUpdater] Done. Clips processed: {clipCount}, total bindings updated: {totalUpdated}");
    }

    bool IsMatch(string bindingPath, string pattern)
    {
        if (string.IsNullOrEmpty(bindingPath) || string.IsNullOrEmpty(pattern)) return false;
        switch (matchModeIndex)
        {
            case 0: return bindingPath.Contains(pattern);
            case 1: return bindingPath.StartsWith(pattern);
            case 2: return bindingPath == pattern;
            default: return bindingPath.Contains(pattern);
        }
    }
}
