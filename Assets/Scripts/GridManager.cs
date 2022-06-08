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
    [SerializeField] private int Padding = 10;
    [SerializeField] private GridLayoutGroup Grid;
    [SerializeField] private Panel PanelPrefab;
    [Space]
    [SerializeField] private GUIManager GUIManager;

    [HideInInspector] public bool GameOver = false;
    [HideInInspector] public bool FlagMode = false;
    private int BombsLeft; 

    private Panel[,] Panels;
    private bool FirstClick = true;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        GenerateGrid();

        switch (GlobalGameSettings.GetSetting("Difficulty"))
        {
            case "Easy":
                Bombs = 8;
                break;

            case "Medium":
                Bombs = 10;
                break;
                
            case "Hard":
                Bombs = 12;
                break;

            default:
                throw new System.Exception("Unknown difficulty");
        }
        BombsLeft = Bombs;
        GUIManager.SetBombAmount(BombsLeft);
    }

    public void revealPanel(Vector2 coordinates)
    {
        Panel panel = GetPanel(coordinates);
        if (panel.Revealed)
        {
            //Ideally this code should never be triggered
            Debug.LogError("reveal duplicate");         
            return;
        }

        if (FirstClick)
        {
            FirstClick = false;
            GenerateBombs(Bombs, coordinates);

            GUIManager.StartTimer();
        }

        int value = GetValue(coordinates);
        panel.Reveal(value);
        
        //If the panel is a bomb, game over
        if (panel.Value == -1)
        {
            StartCoroutine(_GameOver());
            return;
        }

        //reveal the surrounding panels if there isn't a bomb next to it
        if (panel.Value == 0)
        {
            StartCoroutine(_AutoReveal(coordinates));
        }
    }

    /// <summary>
    /// Flag a panel as having a bomb
    /// </summary>
    public void FlagPanel(Vector2 coordinates)
    {
        Panel panel = GetPanel(coordinates);
        panel.ToggleFlag();

        int bombChange = 1;
        if (panel.Flagged)
        {
            bombChange = -1;
        }

        BombsLeft += bombChange;
        GUIManager.SetBombAmount(BombsLeft);

        if (BombsLeft == 0)
        {
            StartCoroutine(CheckVictory());
        }
    }

    /// <summary>
    /// Enables or disables the ability to place flags on panels players suspect to have bombs
    /// </summary>
    public void ToggleFlagMode(bool value)
    {
        FlagMode = value;
    }

    private void GenerateGrid()
    {
        Panels = new Panel[GridSize, GridSize];

        //Calculate the cell size
        //((total space - side padding) - (padding per panel (amount of panels)) / amount of panels;
        Vector2 cellSize = Vector2.one * ((1100 - Padding * 2) - Padding * (GridSize - 1)) / GridSize;

        Grid.padding = new RectOffset(Padding, Padding, Padding, Padding);
        Grid.spacing = new Vector2(Padding, Padding);
        Grid.cellSize = cellSize;

        int x = 0;
        int y = 0;
        for (int i = 0; i < (GridSize * GridSize); i++)
        {
            //Instantiate a panel
            Panel panel = Instantiate(PanelPrefab, transform);
            panel.SetColliderSize(cellSize + Vector2.one * 10);

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
    /// <param name="clickedCoordinates">the clicked coordinates that cannot have a bomb on it</param>
    private void GenerateBombs(int bombs, Vector2 clickedCoordinates)
    {
        int i = 0;
        while (i < bombs)
        {
            //Generate a random position
            Vector2 bombCoordinates = new Vector2(Random.Range(0, GridSize), Random.Range(0, GridSize));

            //Make sure the bombs get spawned at least 2 panels away from the clicked position
            if (bombCoordinates.x > clickedCoordinates.x + 1 || bombCoordinates.x < clickedCoordinates.x - 1 && bombCoordinates.y > clickedCoordinates.y + 1 || bombCoordinates.y < clickedCoordinates.y - 1)
            {
                //Check if the coordinates don't have a bomb on them already
                Panel panel = GetPanel(bombCoordinates);
                if (panel.Value != -1)
                {
                    //place bomb
                    panel.Value = -1;
                    i++;
                }
            }         
        }
    }

    private IEnumerator _AutoReveal(Vector2 panelcoords)
    {
        yield return new WaitForSeconds(0.25f);

        List<Vector2> coordinates = new List<Vector2>();

        #region Adding surrounding coordinates
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

        temp = panelcoords + Vector2.up + Vector2.left;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.down + Vector2.left;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.up + Vector2.right;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }

        temp = panelcoords + Vector2.down + Vector2.right;
        if (InGrid(temp))
        {
            coordinates.Add(temp);
        }
        #endregion

        for (int i = 0; i < coordinates.Count; i++)
        {
            Panel panel = GetPanel(coordinates[i]);
            if (!panel.Revealed && !panel.Flagged)
            {
                int value = GetValue(coordinates[i]);

                //Don't reveal the panel if the panel is a bomb
                if (value != -1)
                {
                    panel.Reveal(value);
                    if (value == 0)
                    {
                        //Add the surrounding coordinates to the reveal list, but only if they aren't in there already
                        #region Adding surrounding coordinates
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

                        newcoordinates = coordinates[i] + Vector2.up + Vector2.left;
                        if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                        {
                            coordinates.Add(newcoordinates);
                        }

                        newcoordinates = coordinates[i] + Vector2.down + Vector2.left;
                        if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                        {
                            coordinates.Add(newcoordinates);
                        }

                        newcoordinates = coordinates[i] + Vector2.up + Vector2.right;
                        if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                        {
                            coordinates.Add(newcoordinates);
                        }

                        newcoordinates = coordinates[i] + Vector2.down + Vector2.right;
                        if (InGrid(newcoordinates) && !coordinates.Contains(newcoordinates))
                        {
                            coordinates.Add(newcoordinates);
                        }
                        #endregion
                    }
                }
            }

            yield return new WaitForSeconds(0.075f);
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
    private int GetValue(Vector2 coordinates)
    {
        if (GetPanel(coordinates).Value == -1)
        {
            //Value is a bomb, game over
            return -1;
        }

        int value = 0;

        Vector2 tempcoordinates = coordinates + Vector2.up;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Up
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.up + Vector2.right;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Up
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.right;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Right
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.right + Vector2.down;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Right
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.down;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Down
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.down + Vector2.left;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Right
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.left;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Left
                value++;
            }
        }

        tempcoordinates = coordinates + Vector2.left + Vector2.up;
        if (InGrid(tempcoordinates))
        {
            if (GetPanel(tempcoordinates).Value == -1)
            {
                //Right
                value++;
            }
        }

        return value;
    }

    private IEnumerator CheckVictory()
    {
        AudioManager.Instance.Play("DrumRoll");

        int bombCounter = 0;
        foreach (Panel panel in Grid.transform.GetComponentsInChildren<Panel>())
        {
            if (panel.Flagged && panel.Value == -1)
            {
                bombCounter++;
            }
        }

        //Victory
        if (bombCounter == Bombs)
        {
            GameOver = true;

            GUIManager.StopTimer();
            yield return new WaitForSeconds(3);
            GUIManager.Victory();

            AudioManager.Instance.Play("VictoryFeedback");
        }
    }

    private IEnumerator _GameOver()
    {
        GameOver = true;
        GUIManager.StopTimer();
        yield return new WaitForSeconds(2f);

        Panel[] panels = Grid.transform.GetComponentsInChildren<Panel>();
        foreach (Panel panel in panels)
        {
            if (panel.Value == -1 && !panel.Flagged)
            {
                panel.ShowBomb();
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
            else if (panel.Value != -1 && panel.Flagged)
            {
                panel.ToggleX();
            }
        }

        yield return new WaitForSeconds(2f);
        GUIManager.GameOver();

        AudioManager.Instance.Play("GameOverFeedback");
    }
}