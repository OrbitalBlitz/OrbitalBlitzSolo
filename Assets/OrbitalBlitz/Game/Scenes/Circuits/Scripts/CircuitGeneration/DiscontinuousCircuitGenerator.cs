using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor.Splines;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration {
    [ExecuteInEditMode]
    [AddComponentMenu("OrbitalBlitz/DiscontinuousCircuitGenerator")]
    public class DiscontinuousCircuitGenerator : BasicCircuitGenerator {

        [Header("Discontinuity")]
        [SerializeField] private int min_knot_per_segment = 3;
        [SerializeField] private int max_knot_per_segment = 7;
        [SerializeField] private float jump_height = 3;
        protected override void generateSpline() {
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
            knots = m_spline.Knots.ToArray();
            
            // Split the knots array into sublists of size in [3, 10]
            var sublists = SplitIntoSublists(knots, min_knot_per_segment, max_knot_per_segment);

            // For each sublist, create a new spline and add it to m_container
            foreach (var sublist in sublists) {
                var new_spline = new Spline(); 
                new_spline.Knots = sublist.ToArray();
                // m_spline.SetTangentMode(TangentMode.Mirrored);
                m_container.AddSpline(new_spline); 
            }
            m_container.RemoveSpline(m_spline);
            
        }

        // Utility method to split the array into sublists
        private List<List<BezierKnot>> SplitIntoSublists(BezierKnot[] knots, int min_size, int max_size) {
            var random = new System.Random(m_seed);
            var sublists = new List<List<BezierKnot>>();
            int start_index = 0;

            // split sublists
            while (start_index < knots.Length) {
                int size = Math.Min(knots.Length - start_index, random.Next(min_size, max_size + 1));
                
                var sublist = new List<BezierKnot>();
                for (int i = start_index; i < start_index + size; i++) {
                    sublist.Add(knots[i]);
                }
                
                if (size < min_size) {
                    sublists[^1].AddRange(sublist);
                    break;
                }
                sublists.Add(sublist);
                start_index += size;
            }
            
            // handle junctions
            for (int i = 0; i < sublists.Count; i++) {
                // Merge the last and first section
                if (i == sublists.Count - 1) {
                    sublists[^1].Add(knots[0]);
                    break;
                }

                sublists[i].Add(copyKnot(sublists[i + 1][0], new float3(0f,jump_height,0f)));
            }

            foreach (var list in sublists) {
                Debug.Log($"Generated sublist {string.Join("->", list.Select(i => $"({i.Position.x},{i.Position.y})"))}");
            }
            return sublists;
        }

        protected BezierKnot copyKnot(BezierKnot copied_knot, float3? position_offset) {
            return new BezierKnot(
                position: copied_knot.Position + (position_offset ?? new float3(0f,0f,0f)), 
                tangentIn: copied_knot.TangentIn, 
                tangentOut: copied_knot.TangentOut,
                rotation: copied_knot.Rotation
                );
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(DiscontinuousCircuitGenerator))]
    public class DiscontinuousCircuitGeneratorEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            DiscontinuousCircuitGenerator generated_circuit = (DiscontinuousCircuitGenerator)target;

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