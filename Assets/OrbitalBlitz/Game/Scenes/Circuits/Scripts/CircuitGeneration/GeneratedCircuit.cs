using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

[ExecuteInEditMode]
[AddComponentMenu("OrbitalBlitz/GeneratedCircuit")]
public class GeneratedCircuit : MonoBehaviour {
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private GameObject spawnpointPrefab;

    
    [HideInInspector][SerializeField]
    public GridSplineGenerator m_path_generator;
    [Header("Path Generation")] 
    [SerializeField]
    private GridSplineGenerator.GenerationMode m_generationMode = GridSplineGenerator.GenerationMode.Backtracking;
    [SerializeField] private int m_seed = 40;
    [SerializeField] private int m_circuitGridMaxSize = 40;
    [SerializeField] private int m_gridCellSize = 10;

    [Header("Circuit Parameters")]
    [SerializeField] private int m_circuit_laps = 2;
    const string k_spawnpoints_root = "spawnpoints";
    const string k_checkpoints_root = "checkpoints";
    
    [Header("Mesh Extruding")] 
    [SerializeField] private float m_roadWidth = 4f;
    [SerializeField] private Material m_roadMaterial;
    [HideInInspector][SerializeField] private SplineExtrude m_extruder;
    [HideInInspector][SerializeField] private MeshFilter m_mesh_filter;
    [HideInInspector][SerializeField] private MeshRenderer m_mesh_renderer;
    [HideInInspector][SerializeField] private MeshCollider m_mesh_collider;


    [HideInInspector][SerializeField] private SplineContainer m_container;

    [HideInInspector][SerializeField] private Spline m_spline;

    [HideInInspector][SerializeField] private CustomSplineInstantiate m_checkpoint_instantiator;
    [HideInInspector][SerializeField] private int m_numberOfCheckpoints = 10;
    [HideInInspector][SerializeField] private CircuitData m_circuit_data;
    [SerializeField] public bool m_generated { get; private set; }

    private void Awake() {
        m_extruder = gameObject.GetComponent<SplineExtrude>();
        m_mesh_filter = gameObject.GetComponent<MeshFilter>();
        m_mesh_renderer = gameObject.GetComponent<MeshRenderer>();
        m_mesh_collider = gameObject.GetComponent<MeshCollider>();
        m_container = gameObject.GetComponent<SplineContainer>();
        m_circuit_data = gameObject.GetComponent<CircuitData>();
    }

    public void Generate() {
        // Create a new Spline on the SplineContainer.
        Clear();
        m_container = gameObject.AddComponent<SplineContainer>();
        generateSpline();
        extrudeMesh();
        createCircuitData();
        m_generated = true;
    }

    public void Clear() {
        Debug.Log("Clearing");
        DestroyImmediate(m_path_generator);
        DestroyImmediate(m_extruder);
        DestroyImmediate(m_mesh_renderer);
        DestroyImmediate(m_mesh_filter);
        DestroyImmediate(m_mesh_collider);
        DestroyImmediate(m_container);
        DestroyImmediate(m_circuit_data);

        DestroyImmediate(GameObject.Find(k_spawnpoints_root));
        DestroyImmediate(GameObject.Find(k_checkpoints_root));
        
        m_generated = false;
    }
    private void createCircuitData() {
        m_circuit_data = gameObject.AddComponent<CircuitData>();
        placeSpawnpoints();
        placeCheckpoints();

        m_circuit_data.Laps = m_circuit_laps;
    }

    private void placeSpawnpoints() {
        var spawnpointsContainer = new GameObject(k_spawnpoints_root);
        spawnpointsContainer.transform.SetParent(transform, false);

        SplineUtility.Evaluate(m_spline, 0.98f, out float3 position, out float3 direction, out float3 up);
        var rotation = Quaternion.LookRotation(direction, up);
        var spawnpoint = Instantiate(
            spawnpointPrefab,
            position + up,
            rotation,
            spawnpointsContainer.transform
        );
        m_circuit_data.Spawnpoints = new () { spawnpoint.GetComponent<Spawnpoint>() };
    }

    private void placeCheckpoints() {
        m_checkpoint_instantiator = gameObject.AddComponent<CustomSplineInstantiate>();
        m_checkpoint_instantiator.setRootName(k_checkpoints_root);
        m_checkpoint_instantiator.itemsToInstantiate = new[] {
            new CustomSplineInstantiate.InstantiableItem() {
                Prefab = checkpointPrefab,
                Probability = 0f
            }
        };
        m_checkpoint_instantiator.InstantiateMethod = CustomSplineInstantiate.Method.InstanceCount;

        m_checkpoint_instantiator.MinSpacing = (int)(m_spline.Knots.Count() / 2);
        m_checkpoint_instantiator.MinPositionOffset = new Vector3(0f, 1f, 0f);
        m_checkpoint_instantiator.MinScaleOffset = new Vector3(5f, 2f, 2f);
        m_checkpoint_instantiator.UpdateInstances();

        List<Checkpoint> checkpoints = FindObjectsOfType<Checkpoint>().ToList();
        checkpoints.Reverse(); // Checkpoints are instantiated from last to first, so we need to reverse them

        // last cp collides with first, so we destroy it
        DestroyImmediate(checkpoints.Last().gameObject);
        checkpoints.RemoveAt(checkpoints.Count - 1);

        m_circuit_data.Checkpoints = checkpoints;
        DestroyImmediate(m_checkpoint_instantiator);
    }

    public void CountCheckpoints() {
        List<Checkpoint> components = FindObjectsOfType<Checkpoint>().ToList();
        Debug.Log($"Found {components.Count} checkpoints !");
    }

    private void generateSpline() {
        m_spline = m_container.Spline;
        m_path_generator = new GridSplineGenerator(new int2(m_circuitGridMaxSize, m_circuitGridMaxSize), m_seed);
        var path = m_path_generator.Generate();

        Debug.Log($"Generated path {string.Join("->", path.Select(i => $"({i.x},{i.y})"))}");
        var knots = new BezierKnot[path.Length];
        int i = 0;
        foreach (var point in path) {
            knots[i] = new BezierKnot(new float3(
                point.x * m_gridCellSize,
                0f,
                point.y * m_gridCellSize
            ));
            i++;
        }

        m_spline.Knots = knots;
        m_spline.Closed = true;
        m_spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    private void extrudeMesh() {
        transform.localScale = new Vector3(1f, 0.0001f, 1f);
        m_extruder = gameObject.AddComponent<SplineExtrude>();
        m_mesh_collider = gameObject.AddComponent<MeshCollider>();
        m_extruder.Radius = m_roadWidth;
        m_extruder.Rebuild();
        
        m_mesh_renderer = GetComponent<MeshRenderer>();
        if (m_roadMaterial != null) {
            m_mesh_renderer.SetMaterials(new List<Material>() {m_roadMaterial});
        }
            
        m_mesh_filter = GetComponent<MeshFilter>();
    }


    // void Update() { }
}

[CustomEditor(typeof(GeneratedCircuit))]
public class GeneratedCircuitEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GeneratedCircuit generated_circuit = (GeneratedCircuit)target;

        if (generated_circuit.m_path_generator != null)
        {
            // Create a foldout or some kind of label to separate properties
            EditorGUILayout.LabelField("GridPathGenerator Properties", EditorStyles.boldLabel);

            // Use Editor.CreateEditor to create an editor for the GridPathGenerator
            Editor pathGeneratorEditor = CreateEditor(generated_circuit.m_path_generator);

            // Draw the GridPathGenerator editor
            pathGeneratorEditor.OnInspectorGUI();
        }
        
        if (GUILayout.Button("Generate")) {
            generated_circuit.Generate();
        }
        if (GUILayout.Button("Clear")) {
            generated_circuit.Clear();
        }


        // if (GUILayout.Button("Count Checkpoints")) {
        //     generated_circuit.CountCheckpoints();
        // }
    }
}