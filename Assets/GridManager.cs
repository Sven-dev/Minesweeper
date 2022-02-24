using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int GridSize = 5; //horizontal and vertical grid size
    [SerializeField] private int Bombs = 5;
    [Space]
    [SerializeField] private GridLayoutGroup Grid;
    [SerializeField] private Panel PanelPrefab;

    private Panel[,] Panels;
    private bool FirstClick = true;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        Panels = new Panel[GridSize, GridSize];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        //Calculate the cell size
        //((total space - side padding) - padding per panel) / amount of panels;
        Vector2 cellSize = Vector2.one * ((1100 - 20) - 10 * GridSize) / GridSize;
        Grid.cellSize = cellSize;

        int x = 0;
        int y = 0;
        for (int i = 0; i < (GridSize * GridSize); i++)
        {
            //Instantiate a panel
            Panel panel = Instantiate(PanelPrefab, transform);
            panel.SetColliderSize(cellSize);

            //Deterimine the coordinates and add the panel to an array
            if (y >= GridSize)
            {
                y = 0;
                x++;
            }

            Panels[x, y] = panel;
            panel.Coordinates = new Vector2(x, y);
            y++;
        }
    }

    public void revealPanel(Vector2 coordinates)
    {
        Panel panel = GetPanel(coordinates);
        if (panel.Revealed)
        {
            //Ideally this code should never be triggered, but it will for now
            return;
        }

        if (FirstClick)
        {
            FirstClick = false;
            GenerateBombs(Bombs, coordinates);
        }

        if (panel.Bomb)
        {
            //Game over
            print("game over");
            return;
        }

        int value = GetValue(coordinates);
        panel.Value = value;
        panel.Reveal();
    }

    private Panel GetPanel(Vector2 coordinates)
    {
        return Panels[(int)coordinates.x, (int)coordinates.y];
    }

    /// <summary>
    /// Generates bombs into the array
    /// </summary>
    /// <param name="bombs">the amount of bombs</param>
    /// <param name="SkipCoordinates">the clicked coordinates that cannot have a bomb on it</param>
    private void GenerateBombs(int bombs, Vector2 SkipCoordinates)
    {
        int i = 0;
        while (i < bombs)
        {
            Vector2 bombCoordinates = new Vector2(Random.Range(0, GridSize), Random.Range(0, GridSize));

            //If the coordinates aren't the same as the skipcoordinates
            if (bombCoordinates != SkipCoordinates)
            {
                //And the coordinates don't have a bomb on them already
                Panel panel = GetPanel(bombCoordinates);
                if (!panel.Bomb)
                {
                    //place bomb
                    panel.Bomb = true;

                    i++;
                }
            }
        }
    }

    /// <summary>
    /// Checks if a set of coordinates is on the grid
    /// </summary>
    /// <returns></returns>
    private bool InGrid(Vector2 coordinates)
    {
        if (coordinates.x < 0)
        {
            return false;
        }
        else if (coordinates.x >= GridSize)
        {
            return false;
        }
        else if (coordinates.y < 0)
        {
            return false;
        }
        else if (coordinates.y >= GridSize)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines the value of the coordinate by checking itself and the adjacent spaces
    /// </summary>
    /// <param name="coordinates">The place of the panel in the array</param>
    /// <returns>an int determining its value (-1 == bomb, 0 == no adjacent bombs, 1-8 == adjacent bombs)</returns>
    public int GetValue(Vector2 coordinates)
    {
        if (GetPanel(coordinates).Bomb)
        {
            //Value is a bomb, game over
            return -1;
        }

        int value = 0;

        Vector2 tempcoordinates = coordinates + Vector2.up;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Up
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.up + Vector2.right;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Up right
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.right;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Right
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.right + Vector2.down;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Down right
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.down;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Down
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.down + Vector2.left;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Down left
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.left;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Left
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        tempcoordinates = coordinates + Vector2.left + Vector2.up;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Up left
                value++;
            }
            else
            {
                revealPanel(tempcoordinates);
            }
        }

        return value;
    }
}