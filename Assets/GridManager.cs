﻿using System.Collections;
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

    private int PanelsLeft;

    private Panel[,] Panels;
    private bool FirstClick = true;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        PanelsLeft = GridSize * GridSize - Bombs;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Panels = new Panel[GridSize, GridSize];

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

        int value = GetValue(coordinates);
        panel.Reveal(value);
        
        if (panel.Bomb)
        {
            //Game over
            print("game over");
            return;
        }

        PanelsLeft--;
        if (PanelsLeft == 0)
        {
            print("win");
            return;
        }

        //reveal the surrounding panels
        StartCoroutine(_AutoReveal(coordinates));       
    }

    private IEnumerator _AutoReveal(Vector2 panelcoords)
    {
        List<Vector2> coordinates = new List<Vector2>();

        //Add the surrounding panel coords to the list
        Vector2 temp = panelcoords + Vector2.up;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.down;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.left;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.right;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        for (int i = 0; i < coordinates.Count; i++)
        {
            Panel panel = GetPanel(coordinates[i]);
            if (!panel.Revealed)
            {
                int value = GetValue(coordinates[i]);

                //Don't reveal the panel if the panel is a bomb
                if (value != -1)
                {
                    panel.Reveal(value);
                    PanelsLeft--;
                    if (PanelsLeft == 0)
                    {
                        print("win");
                        break;
                    }

                    //Add the surrounding coordinates to the reveal list, but only if they aren't in there already
                    Vector2 newcoordinates = coordinates[i] + Vector2.up;
                    if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                    {
                        coordinates.Add(newcoordinates);
                    }

                    newcoordinates = coordinates[i] + Vector2.left;
                    if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                    {
                        coordinates.Add(newcoordinates);
                    }

                    newcoordinates = coordinates[i] + Vector2.right;
                    if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                    {
                        coordinates.Add(newcoordinates);
                    }

                    newcoordinates = coordinates[i] + Vector2.down;
                    if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                    {
                        coordinates.Add(newcoordinates);
                    }
                }
            }

            yield return null;
        }
    }

    private Panel GetPanel(Vector2 coordinates)
    {
        return Panels[(int)coordinates.x, (int)coordinates.y];
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
        }

        tempcoordinates = coordinates + Vector2.up + Vector2.right;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Up
                value++;
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
        }

        tempcoordinates = coordinates + Vector2.right + Vector2.down;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Right
                value++;
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
        }

        tempcoordinates = coordinates + Vector2.down + Vector2.left;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Right
                value++;
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
        }

        tempcoordinates = coordinates + Vector2.left + Vector2.up;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Bomb)
            {
                //Right
                value++;
            }
        }

        return value;
    }
}