#if UNITY_EDITOR
using UnityEditor.Splines;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using Interpolators = UnityEngine.Splines.Interpolators;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration {
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SplineContainer), typeof(MeshRenderer), typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class ExtrudeFlatRoadOnSpline : MonoBehaviour {
        [SerializeField] List<SplineData<float>> m_Widths = new();
        [SerializeField] float m_default_road_width = 5f;

        public List<SplineData<float>> Widths {
            get {
                foreach (var width in m_Widths) {
                    width.DefaultValue = m_default_road_width;

                    // if (width.DefaultValue == 0)
                    //     width.DefaultValue = m_default_road_width;
                }

                return m_Widths;
            }
        }

        [SerializeField] SplineContainer m_Spline;

        public SplineContainer Container {
            get {
                if (m_Spline == null)
                    m_Spline = gameObject.GetComponent<SplineContainer>();

                return m_Spline;
            }
            set => m_Spline = value;
        }

        [SerializeField] int m_SegmentsPerMeter = 1;

        [SerializeField] Mesh m_Mesh;

        [SerializeField] float m_TextureScale = 1f;

        public IReadOnlyList<Spline> splines => LoftSplines;

        public IReadOnlyList<Spline> LoftSplines {
            get {
                if (m_Spline == null)
                    m_Spline = gameObject.GetComponent<SplineContainer>();

                if (m_Spline == null) {
                    Debug.LogError("Cannot loft road mesh because Spline reference is null");
                    return null;
                }

                return m_Spline.Splines;
            }
        }

        [Obsolete("Use LoftMesh instead.", false)]
        public Mesh mesh => LoftMesh;

        public Mesh LoftMesh {
            get {
                if (m_Mesh != null)
                    return m_Mesh;

                var meshFilter = gameObject.GetComponent<MeshFilter>();
                if (!meshFilter) {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                }

                if (meshFilter.sharedMesh == null) {
                    meshFilter.sharedMesh = new Mesh();
                }

                return m_Mesh = meshFilter.sharedMesh;
            }
        }

        [Obsolete("Use SegmentsPerMeter instead.", false)]
        public int segmentsPerMeter => SegmentsPerMeter;

        public int SegmentsPerMeter => Mathf.Min(10, Mathf.Max(1, m_SegmentsPerMeter));

        List<Vector3> m_Positions = new List<Vector3>();
        List<Vector3> m_Normals = new List<Vector3>();
        List<Vector2> m_Textures = new List<Vector2>();
        List<int> m_Indices = new List<int>();

        public void OnEnable() {
            // Avoid to point to an existing instance when duplicating the GameObject
            if (m_Mesh != null)
                m_Mesh = null;

            if (m_Spline == null)
                m_Spline = gameObject.GetComponent<SplineContainer>();

            LoftAllRoads();

            #if UNITY_EDITOR
            EditorSplineUtility.AfterSplineWasModified += OnAfterSplineWasModified;
            EditorSplineUtility.RegisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Undo.undoRedoPerformed += LoftAllRoads;
            #endif

            SplineContainer.SplineAdded += OnSplineContainerAdded;
            SplineContainer.SplineRemoved += OnSplineContainerRemoved;
            SplineContainer.SplineReordered += OnSplineContainerReordered;
            Spline.Changed += OnSplineChanged;
        }

        public void OnDisable() {
            #if UNITY_EDITOR
            EditorSplineUtility.AfterSplineWasModified -= OnAfterSplineWasModified;
            EditorSplineUtility.UnregisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Undo.undoRedoPerformed -= LoftAllRoads;
            #endif

            if (m_Mesh != null)
                #if UNITY_EDITOR
                DestroyImmediate(m_Mesh);
            #else
                Destroy(m_Mesh);
            #endif

            SplineContainer.SplineAdded -= OnSplineContainerAdded;
            SplineContainer.SplineRemoved -= OnSplineContainerRemoved;
            SplineContainer.SplineReordered -= OnSplineContainerReordered;
            Spline.Changed -= OnSplineChanged;
        }

        void OnSplineContainerAdded(SplineContainer container, int index) {
            if (container != m_Spline)
                return;

            if (m_Widths.Count < LoftSplines.Count) {
                var delta = LoftSplines.Count - m_Widths.Count;
                for (var i = 0; i < delta; i++) {
                    #if UNITY_EDITOR
                    Undo.RecordObject(this, "Modifying Widths SplineData");
                    #endif
                    m_Widths.Add(new SplineData<float>() { DefaultValue = 1f });
                }
            }

            LoftAllRoads();
        }

        void OnSplineContainerRemoved(SplineContainer container, int index) {
            if (container != m_Spline)
                return;

            if (index < m_Widths.Count) {
                #if UNITY_EDITOR
                Undo.RecordObject(this, "Modifying Widths SplineData");
                #endif
                m_Widths.RemoveAt(index);
            }

            LoftAllRoads();
        }

        void OnSplineContainerReordered(SplineContainer container, int previousIndex, int newIndex) {
            if (container != m_Spline)
                return;

            LoftAllRoads();
        }

        void OnAfterSplineWasModified(Spline s) {
            if (LoftSplines == null)
                return;

            foreach (var spline in LoftSplines) {
                if (s == spline) {
                    LoftAllRoads();
                    break;
                }
            }
        }

        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification) {
            OnAfterSplineWasModified(spline);
        }

        void OnAfterSplineDataWasModified(SplineData<float> splineData) {
            Debug.Log("OnAfterSplineDataWasModified");
            foreach (var width in m_Widths) {
                if (splineData == width) {
                    LoftAllRoads();
                    break;
                }
            }
        }

        public void LoftAllRoads() {
            LoftMesh.Clear();
            m_Positions.Clear();
            m_Normals.Clear();
            m_Textures.Clear();
            m_Indices.Clear();

            for (var i = 0; i < LoftSplines.Count; i++)
                Loft(LoftSplines[i], i);

            LoftMesh.SetVertices(m_Positions);
            LoftMesh.SetNormals(m_Normals);
            LoftMesh.SetUVs(0, m_Textures);
            LoftMesh.subMeshCount = 1;
            LoftMesh.SetIndices(m_Indices, MeshTopology.Triangles, 0);
            LoftMesh.UploadMeshData(false);

            gameObject.GetComponent<MeshFilter>().sharedMesh = LoftMesh;
            gameObject.GetComponent<MeshCollider>().sharedMesh = LoftMesh;
        }

        public void Loft(Spline spline, int widthDataIndex) {
            if (spline == null || spline.Count < 2)
                return;

            LoftMesh.Clear();

            float length = spline.GetLength();

            if (length <= 0.001f)
                return;

            var segmentsPerLength = SegmentsPerMeter * length;
            var segments = Mathf.CeilToInt(segmentsPerLength);
            var segmentStepT = (1f / SegmentsPerMeter) / length;
            var steps = segments + 1;
            var vertexCount = steps * 2;
            var triangleCount = segments * 6;
            var prevVertexCount = m_Positions.Count;

            m_Positions.Capacity += vertexCount;
            m_Normals.Capacity += vertexCount;
            m_Textures.Capacity += vertexCount;
            m_Indices.Capacity += triangleCount;

            var t = 0f;
            for (int i = 0; i < steps; i++) {
                SplineUtility.Evaluate(spline, t, out var pos, out var dir, out var up);

                // If dir evaluates to zero (linear or broken zero length tangents?)
                // then attempt to advance forward by a small amount and build direction to that point
                if (math.length(dir) == 0) {
                    var nextPos = spline.GetPointAtLinearDistance(t, 0.01f, out _);
                    dir = math.normalizesafe(nextPos - pos);

                    if (math.length(dir) == 0) {
                        nextPos = spline.GetPointAtLinearDistance(t, -0.01f, out _);
                        dir = -math.normalizesafe(nextPos - pos);
                    }

                    if (math.length(dir) == 0)
                        dir = new float3(0, 0, 1);
                }

                var scale = transform.lossyScale;
                var tangent = math.normalizesafe(math.cross(up, dir)) *
                              new float3(1f / scale.x, 1f / scale.y, 1f / scale.z);

                var w = m_default_road_width;
                // if (widthDataIndex < m_Widths.Count) {
                //     w = m_Widths[widthDataIndex].DefaultValue;
                //     if (m_Widths[widthDataIndex] != null && m_Widths[widthDataIndex].Count > 0) {
                //         w = m_Widths[widthDataIndex].Evaluate(spline, t, PathIndexUnit.Normalized,
                //             new Interpolators.LerpFloat());
                //         w = math.clamp(w, .001f, 10000f);
                //     }
                // }

                m_Positions.Add(pos - (tangent * w));
                m_Positions.Add(pos + (tangent * w));
                m_Normals.Add(up);
                m_Normals.Add(up);
                m_Textures.Add(new Vector2(0f, t * m_TextureScale));
                m_Textures.Add(new Vector2(1f, t * m_TextureScale));

                t = math.min(1f, t + segmentStepT);
            }

            for (int i = 0, n = prevVertexCount; i < triangleCount; i += 6, n += 2) {
                m_Indices.Add((n + 2) % (prevVertexCount + vertexCount));
                m_Indices.Add((n + 1) % (prevVertexCount + vertexCount));
                m_Indices.Add((n + 0) % (prevVertexCount + vertexCount));
                m_Indices.Add((n + 2) % (prevVertexCount + vertexCount));
                m_Indices.Add((n + 3) % (prevVertexCount + vertexCount));
                m_Indices.Add((n + 1) % (prevVertexCount + vertexCount));
            }
        }

        public Mesh SaveMeshAsset() {
            // var mesh = new Mesh();
            // mesh.name = name;

            #if UNITY_EDITOR
            var scene = SceneManager.GetActiveScene();
            var sceneDataDir = "Assets";

            if (!string.IsNullOrEmpty(scene.path)) {
                var dir = Path.GetDirectoryName(scene.path);
                sceneDataDir = $"{dir}/{Path.GetFileNameWithoutExtension(scene.path)}";
                if (!Directory.Exists(sceneDataDir))
                    Directory.CreateDirectory(sceneDataDir);
            }

            var path = AssetDatabase.GenerateUniqueAssetPath($"{sceneDataDir}/SplineExtrude_{LoftMesh.name}.asset");

            AssetDatabase.CreateAsset(LoftMesh, path);
            EditorGUIUtility.PingObject(LoftMesh);
            #endif

            return LoftMesh;
        }
    }
}