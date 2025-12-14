using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper script để tự động setup StreetLamp cho đèn đường
/// </summary>
public class StreetLampSetupHelper : EditorWindow
{
    [MenuItem("Tools/Setup Street Lamp Helper")]
    public static void ShowWindow()
    {
        GetWindow<StreetLampSetupHelper>("Street Lamp Setup");
    }
    
    private GameObject selectedObject;
    private bool autoFindLight = true;
    private bool autoCreateCollider = true;
    
    void OnGUI()
    {
        GUILayout.Label("Street Lamp Setup Helper", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Chọn object
        selectedObject = (GameObject)EditorGUILayout.ObjectField(
            "Select Lamp Object", 
            selectedObject, 
            typeof(GameObject), 
            true
        );
        
        GUILayout.Space(10);
        autoFindLight = EditorGUILayout.Toggle("Auto Find Light", autoFindLight);
        autoCreateCollider = EditorGUILayout.Toggle("Auto Create Collider", autoCreateCollider);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Setup Street Lamp", GUILayout.Height(30)))
        {
            if (selectedObject != null)
            {
                SetupStreetLamp(selectedObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select an object first!", "OK");
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Find All Objects Named 'Japan_Thing'", GUILayout.Height(30)))
        {
            FindAndSetupJapanThing();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Instructions:\n" +
            "1. Select the lamp object (or 'g Japan_Thing')\n" +
            "2. Click 'Setup Street Lamp'\n" +
            "3. The script will:\n" +
            "   - Add StreetLamp component\n" +
            "   - Find or create Point Light\n" +
            "   - Create Collider if needed",
            MessageType.Info
        );
    }
    
    void SetupStreetLamp(GameObject obj)
    {
        // 1. Add StreetLamp component
        StreetLamp streetLamp = obj.GetComponent<StreetLamp>();
        if (streetLamp == null)
        {
            streetLamp = obj.AddComponent<StreetLamp>();
            Debug.Log($"Added StreetLamp component to {obj.name}");
        }
        else
        {
            Debug.Log($"StreetLamp component already exists on {obj.name}");
        }
        
        // 2. Find or create Light
        if (autoFindLight)
        {
            Light light = obj.GetComponentInChildren<Light>();
            if (light == null)
            {
                // Create new Point Light
                GameObject lightObj = new GameObject("Point Light");
                lightObj.transform.SetParent(obj.transform);
                lightObj.transform.localPosition = Vector3.zero;
                light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 8f;
                light.intensity = 1.5f;
                light.color = new Color(1f, 0.78f, 0.39f); // Warm yellow
                light.enabled = false;
                Debug.Log($"Created Point Light for {obj.name}");
            }
            else
            {
                // Ensure it's a Point Light
                if (light.type != LightType.Point)
                {
                    light.type = LightType.Point;
                    Debug.Log($"Changed Light type to Point for {obj.name}");
                }
            }
            
            // Assign to StreetLamp
            SerializedObject so = new SerializedObject(streetLamp);
            SerializedProperty lightProp = so.FindProperty("lampLight");
            if (lightProp != null)
            {
                lightProp.objectReferenceValue = light;
                so.ApplyModifiedProperties();
                Debug.Log($"Assigned Light to StreetLamp on {obj.name}");
            }
        }
        
        // 3. Create Collider if needed
        if (autoCreateCollider)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider == null)
            {
                // Try to find in children
                collider = obj.GetComponentInChildren<Collider>();
            }
            
            if (collider == null)
            {
                // Create Box Collider
                BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(2f, 2f, 2f); // Large enough to hit
                boxCollider.isTrigger = false; // IMPORTANT: Must be false for raycast!
                Debug.Log($"Created Box Collider for {obj.name}");
            }
            else
            {
                // Ensure Is Trigger = false
                if (collider.isTrigger)
                {
                    collider.isTrigger = false;
                    Debug.Log($"Set Is Trigger = false for {obj.name}'s Collider");
                }
            }
        }
        
        EditorUtility.DisplayDialog("Success", 
            $"Street Lamp setup complete for {obj.name}!\n\n" +
            "Next steps:\n" +
            "1. Check that Light is assigned in StreetLamp component\n" +
            "2. Adjust Collider size if needed\n" +
            "3. Test in Play mode", 
            "OK");
    }
    
    void FindAndSetupJapanThing()
    {
        // Find all objects with "Japan_Thing" in name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Japan_Thing"))
            {
                SetupStreetLamp(obj);
                count++;
            }
        }
        
        if (count == 0)
        {
            EditorUtility.DisplayDialog("Not Found", 
                "No objects with 'Japan_Thing' in name found in scene.", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Success", 
                $"Found and setup {count} object(s) with 'Japan_Thing' in name!", 
                "OK");
        }
    }
}


