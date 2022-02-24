using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGenerator : MonoBehaviour
{
    [SerializeField] private int GridSize = 5; //horizontal and vertical grid size

    [SerializeField] private GridLayoutGroup Grid;

    [SerializeField] private Panel PanelPrefab;

    private List<List<Panel>> Panels = new List<List<Panel>>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        //1100px horizontally and vertically

        //divide the available size between spaces

        int cellSize = ((1100 - 20) - 10 * GridSize) / GridSize;
        Grid.cellSize = Vector2.one * cellSize;

        for (int i = 0; i < (GridSize * GridSize); i++)
        {
            Instantiate(PanelPrefab, transform);
        }
    }
}
