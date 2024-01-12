using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration {
    public class GridSplineGenerator : MonoBehaviour, IPathGenerator {
        [SerializeField] private GenerationMode _mode;
        [SerializeField] private int2 m_dims;
        [SerializeField] private Random m_rd;

        public enum GenerationMode {
            Backtracking,
            BetterBacktracking,
        }

        enum Mark {
            Empty = 0,
            Filled = 1,
            Locked = -1
        }

        public GridSplineGenerator SetMode(GenerationMode mode) {
            _mode = mode;
            return this;
        }

        public GridSplineGenerator(int2 dimensions, int seed = 123, GenerationMode mode = GenerationMode.Backtracking) {
            m_dims = dimensions;
            m_rd = new(seed);
        }

        public int2[] Generate() {
            return _mode switch {
                GenerationMode.Backtracking => generateBacktracking(),
                GenerationMode.BetterBacktracking => generateBetterBacktracking(),
                _ => throw new Exception("Generation mode not implemented.")
            };
        }

        private int2[] generateBacktracking() {
            var path_ll = new LinkedList<int2>();
            var grid = new IntGrid(m_dims);

            int2? GetRandomAdjacentCell(int2 cell) {
                var adj_cells = grid.GetAdjacentCells(
                    cell,
                    filter_function: i => (Mark)i == Mark.Empty
                );
                if (adj_cells.Count == 0) {
                    return null;
                }

                return adj_cells[m_rd.Next(0, adj_cells.Count - 1)];
            }

            ;

            int2 current_cell = new int2(
                m_rd.Next(0, m_dims[0]),
                m_rd.Next(0, m_dims[1])
            );
            path_ll.AddLast(current_cell);
            current_cell = GetRandomAdjacentCell(current_cell)
                           ?? throw new Exception($"No cell adjacent to starting cell {current_cell.ToString()}");

            while (true) {
                var loop_complete = current_cell.Equals(path_ll.First.Value);
                if (loop_complete) {
                    var backtracked_to_start = path_ll.Count == 1;
                    if (backtracked_to_start) {
                        return generateBacktracking();
                    }

                    path_ll.RemoveLast();
                    break;
                }

                var next_cell = GetRandomAdjacentCell(current_cell);
                if (next_cell == null) {
                    grid.SetCellValue(current_cell, (int)Mark.Locked);
                    path_ll.RemoveLast();
                    current_cell = path_ll.Last.Value;
                    continue;
                }

                grid.SetCellValue(current_cell, (int)Mark.Filled);
                current_cell = next_cell.Value;
                path_ll.AddLast(current_cell);
            }

            return path_ll.ToArray();
        }

        private int2[] generateBetterBacktracking() {
            var path_ll = new LinkedList<int2>();
            var grid = new IntGrid(m_dims);

            List<int2> GetAdjacentCells(int2 cell, int range) {
                return grid.GetAdjacentCells(
                    cell,
                    range: 2,
                    filter_function: i => (Mark)i == Mark.Empty
                );
            }

            int2 ChooseRandomCell(List<int2> cells) {
                return cells[m_rd.Next(0, cells.Count - 1)];
            }

            ;

            int2 current_cell = new int2(
                m_rd.Next(0, m_dims[0]),
                m_rd.Next(0, m_dims[1])
            );
            path_ll.AddLast(current_cell);
        
            var direct_adj_cells = GetAdjacentCells(current_cell, 1);
            if (direct_adj_cells.Count == 0) {
                throw new Exception($"No cell is adjacent to the starting cell {current_cell.ToString()}.");
            }
            current_cell = ChooseRandomCell(
                GetAdjacentCells(current_cell, 2)
            );
            grid.SetCellsValue(direct_adj_cells, (int)Mark.Filled);

            while (true) {
                var loop_complete = current_cell.Equals(path_ll.First.Value);
                if (loop_complete) {
                    var backtracked_to_start = path_ll.Count == 1;
                    if (backtracked_to_start) {
                        return generateBetterBacktracking();
                    }
                    path_ll.RemoveLast();
                    break;
                }
            
                direct_adj_cells = GetAdjacentCells(current_cell, 1);
                if (direct_adj_cells.Count == 0) {                
                    grid.SetCellValue(current_cell, (int)Mark.Locked);
                    path_ll.RemoveLast();
                    current_cell = path_ll.Last.Value;
                    continue;
                }
            
                var next_cell = ChooseRandomCell(
                    GetAdjacentCells(current_cell, 2)
                );
                grid.SetCellsValue(direct_adj_cells, (int)Mark.Filled);
            
                grid.SetCellValue(current_cell, (int)Mark.Filled);
                current_cell = next_cell;
                path_ll.AddLast(current_cell);
            }

            return path_ll.ToArray();
        }
    }
    
    [CustomEditor(typeof(GridSplineGenerator))]
    public class GridSplineGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // GridSplineGenerator generator = (GridSplineGenerator)target;
            // generator.generator.someField = EditorGUILayout.FloatField("Some Field", generator.generator.someField);
        }
    }
}