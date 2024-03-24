using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration {
    [ExecuteInEditMode]
    [AddComponentMenu("OrbitalBlitz/GeneratedCircuit")]
    public class BasicCircuitGenerator : MonoBehaviour {
        [SerializeField] protected GameObject checkpointPrefab;
        [SerializeField] protected GameObject spawnpointPrefab;

        [HideInInspector] [SerializeField] public GridPathGenerator m_path_generator;

        [Header("Path Generation")] [SerializeField]
        protected GridPathGenerator.GenerationMode m_generationMode = GridPathGenerator.GenerationMode.Backtracking;

        [SerializeField] protected int m_seed = 157;
        [SerializeField] protected int m_circuitGridMaxSize = 5;
        [SerializeField] protected int m_gridCellSize = 25;

        [Header("Circuit Parameters")] [SerializeField]
        protected int m_circuit_laps = 2;

        const string k_spawnpoints_root = "spawnpoints";
        const string k_checkpoints_root = "checkpoints";

        [Header("Mesh Extruding")] protected const float DEFAULT_ROAD_WIDTH = 4f;
        [SerializeField] protected float m_roadWidth = DEFAULT_ROAD_WIDTH;
        [SerializeField] protected Material m_roadMaterial;
        [HideInInspector] [SerializeField] protected ExtrudeFlatRoadOnSpline m_extruder;
        [HideInInspector] [SerializeField] protected MeshFilter m_mesh_filter;
        [HideInInspector] [SerializeField] protected MeshRenderer m_mesh_renderer;
        [HideInInspector] [SerializeField] protected MeshCollider m_mesh_collider;
        [HideInInspector] [SerializeField] protected GameObject m_fall_catcher;


        [HideInInspector] [SerializeField] protected SplineContainer m_container;

        [HideInInspector] [SerializeField] protected Spline m_spline;

        [HideInInspector] [SerializeField] protected CustomSplineInstantiate m_checkpoint_instantiator;
        [HideInInspector] [SerializeField] protected int m_numberOfCheckpoints = 10;
        [HideInInspector] [SerializeField] protected CircuitData m_circuit_data;
        [SerializeField] public bool m_generated { get; protected set; }

        protected void Awake() {
            m_extruder = gameObject.GetComponent<ExtrudeFlatRoadOnSpline>();
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
            initFallCatcher();
            createCircuitData();
            m_generated = true;
        }

        protected void initFallCatcher() {
            m_fall_catcher = GameObject.CreatePrimitive(PrimitiveType.Cube);
            int plane_side = 2 * m_circuitGridMaxSize * m_gridCellSize;
            m_fall_catcher.transform.position = new Vector3(0, -2, 0);
            m_fall_catcher.transform.localScale = new Vector3(plane_side, 0.1f, plane_side); 

            Collider planeCollider = m_fall_catcher.GetComponent<Collider>();
            if (planeCollider != null) {
                planeCollider.isTrigger = true;
            }
            else {
                planeCollider = m_fall_catcher.AddComponent<MeshCollider>();
                planeCollider.isTrigger = true;
            }

            m_fall_catcher.AddComponent<FallCatcher>();
            DestroyImmediate(m_fall_catcher.GetComponent<MeshRenderer>());
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
            DestroyImmediate(m_fall_catcher);

            DestroyImmediate(GameObject.Find(k_spawnpoints_root));
            DestroyImmediate(GameObject.Find(k_checkpoints_root));

            m_generated = false;
        }

        protected void createCircuitData() {
            m_circuit_data = gameObject.AddComponent<CircuitData>();
            placeSpawnpoints();
            placeCheckpoints();

            m_circuit_data.Laps = m_circuit_laps;
        }

        protected void placeSpawnpoints() {
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
            m_circuit_data.Spawnpoints = new() { spawnpoint.GetComponent<Spawnpoint>() };
        }

        protected void placeCheckpoints() {
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
            float scale_factor = m_roadWidth / DEFAULT_ROAD_WIDTH;
            m_checkpoint_instantiator.MinScaleOffset = new Vector3(scale_factor - 1, scale_factor - 1, scale_factor - 1);
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

        protected virtual void generateSpline() {
            m_spline = m_container.Spline;
            m_path_generator = new GridPathGenerator(new int2(m_circuitGridMaxSize, m_circuitGridMaxSize), m_seed);
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

        protected void extrudeMesh() {
            if (!gameObject.TryGetComponent<MeshFilter>(out m_mesh_filter)) {
                Debug.Log("MeshFilter not found, creating it");
                m_mesh_filter = gameObject.AddComponent<MeshFilter>();
            }
            if (!gameObject.TryGetComponent<MeshCollider>(out m_mesh_collider)) {
                Debug.Log("MeshCollider not found, creating it");
                m_mesh_collider = gameObject.AddComponent<MeshCollider>();
            }
            // m_mesh_filter.sharedMesh = new();
            
            
            // transform.localScale = new Vector3(1f, 0.0001f, 1f);
            m_extruder = gameObject.AddComponent<ExtrudeFlatRoadOnSpline>();
            
            // m_mesh_collider.sharedMesh = m_mesh_filter.sharedMesh;

            m_mesh_renderer = GetComponent<MeshRenderer>();
            if (m_roadMaterial != null) {
                // m_mesh_renderer.SetMaterials(new List<Material>() {m_roadMaterial});
                m_mesh_renderer.materials = new[] { m_roadMaterial };
            }

        }

        public void ReadyForProduction() {
            var mesh = m_extruder.SaveMeshAsset();
            m_mesh_filter.sharedMesh = mesh;
            m_mesh_collider.sharedMesh = mesh;

            DestroyImmediate(m_path_generator);
            DestroyImmediate(m_extruder);
            DestroyImmediate(m_container);

            DestroyImmediate(gameObject.GetComponent<BasicCircuitGenerator>());
        }


        // void Update() { }
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(BasicCircuitGenerator))]
    public class BasicCircuitGeneratorEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            BasicCircuitGenerator generated_circuit = (BasicCircuitGenerator)target;

            if (generated_circuit.m_path_generator != null) {
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
        
            if (GUILayout.Button("Ready for Production")) {
                generated_circuit.ReadyForProduction();
            }
        }
    }
    #endif
}