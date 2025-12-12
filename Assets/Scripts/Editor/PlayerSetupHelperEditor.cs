using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor cho PlayerSetupHelper - thêm button để setup dễ dàng
/// </summary>
[CustomEditor(typeof(PlayerSetupHelper))]
[CanEditMultipleObjects]
public class PlayerSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Vẽ default inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
        
        PlayerSetupHelper helper = (PlayerSetupHelper)target;
        
        // Button để setup Player
        if (GUILayout.Button("Setup Player", GUILayout.Height(30)))
        {
            helper.SetupPlayer();
            EditorUtility.DisplayDialog(
                "Setup Complete", 
                "Player đã được setup thành công!\n\n" +
                "Bây giờ bạn có thể Play scene để test.",
                "OK"
            );
        }
        
        EditorGUILayout.Space();
        
        // Button để reset position
        if (GUILayout.Button("Reset Player Position", GUILayout.Height(25)))
        {
            helper.ResetPlayerPosition();
        }
    }
}

