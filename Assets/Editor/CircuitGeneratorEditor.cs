#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircuitGenerator))]
public class CircuitGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        CircuitGenerator generator = (CircuitGenerator)target;

        // Add a custom button
        if (GUILayout.Button("Generate Circuit"))
        {
            generator.GenerateCircuit();
        }
    }
}
#endif
