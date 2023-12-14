using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts.CircuitGeneration {
    public class IntGrid {
        private int[,] m_grid;
        private int m_dim1;
        private int m_dim2;
        
        // Visualization
        private GameObject[,] m_viz_grid;
        private bool m_is_vizualization_active;
        private VisualizationOptions m_viz_options;
        public struct VisualizationOptions {
            public float cell_size;
            public Dictionary<int,Material> value_materials;
        }
        public IntGrid(int2 dimensions, VisualizationOptions? visualization_options = null) {
            m_dim1 = dimensions[0];
            m_dim2 = dimensions[1];

            if (visualization_options != null) {
                m_is_vizualization_active = true;
                m_viz_options = visualization_options.Value;
            }
            
            resetGrid();
            resetVisualizationGrid();
        }

        public void SetCellValue(int2 cell, int value) {
            m_grid[cell.x, cell.y] = value;
            if (m_is_vizualization_active 
                && m_viz_options.value_materials.TryGetValue(value, out Material material)) {
                var cube = m_viz_grid[cell.x, cell.y];
                cube.GetComponent<MeshRenderer>().SetMaterials(new () { material });
            }
        }
        public void SetCellsValue(List<int2> cells, int value) {
            foreach (var cell in cells) {
                SetCellValue(cell, value);
            }
        }

        private void resetGrid() {
            m_grid = new int[m_dim1, m_dim2];
        }
        
        private void resetVisualizationGrid() {
            if (!m_is_vizualization_active) return;
            
            m_viz_grid = new GameObject[m_dim1, m_dim2];
            var cell_size = m_viz_options.cell_size;
            
            for (int x = 0; x < m_dim1; x++) {
                for (int y = 0; y < m_dim2; y++) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(
                        x * cell_size, 
                        0, 
                        y * cell_size
                        ); 
                    cube.transform.localScale = new Vector3(cell_size, 0.01f, cell_size);
                    if (m_viz_options.value_materials.TryGetValue(m_grid[x, y], out Material material)) {
                        cube.GetComponent<MeshRenderer>().SetMaterials(new () { material });
                    }
                }
            }
        }

        public List<int2> GetAdjacentCells(int2 position, int range = 1, Func<int, bool> filter_function = null) {
            var adjacent_cells = new List<int2> { };

            // List<int> RangeAround(int center, int range) {
            //     return Enumerable.Range(-range, 2 * range + 1)
            //         .Select(k => center + k)
            //         .ToList();
            // }
            // Debug.Log($"Range to be explored around {position.ToString()} : {string.Join(',',RangeAround(position.x, range))};{string.Join(",",RangeAround(position.y, range))}");

            var delta = Enumerable.Range(-range, 2 * range + 1);
            foreach (var dx in delta) {
                foreach (var dy in delta) {
                    
                    bool is_diagonal_cell = dx == dy;
                    var cell = new int2(position.x + dx, position.y + dy);
                    if (!doesCellExist(cell) || is_diagonal_cell) {
                        continue;
                    }

                    if (filter_function == null) {
                        adjacent_cells.Add(cell);
                        continue;
                    }

                    if (filter_function(m_grid[cell.x, cell.y])) {
                        adjacent_cells.Add(cell);
                    }
                }
            }

            return adjacent_cells;
        }

        private bool doesCellExist(int2 cell) {
            if (cell.x < 0 || cell.x > m_dim1 - 1) return false;
            if (cell.y < 0 || cell.y > m_dim2 - 1) return false;
            return true;
        }
    }
}