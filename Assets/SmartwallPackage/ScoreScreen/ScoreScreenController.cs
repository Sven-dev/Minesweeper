using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighscoreContainer
{
    public List<int> Highscores = new List<int>();
    public HighscoreContainer(List<int> scores)
    {
        Highscores = scores;
    }
    public HighscoreContainer() { }
}

/// <summary>
/// This Class manages the score display scene. It is Important that the scene is still called "Scores".
/// You may wish to switch the replay button arrow, it is under C_UIRoot > P_Replay > BT_Replay
/// </summary>
public class ScoreScreenController : MonoBehaviour
{
    private static List<int> Scores = new List<int>();
    /// <summary>
    /// Current highscore, publicly availible incase you want to use it for something.
    /// </summary>
    public static int GetHighscore(int levelIndex) { return _Highscore.Highscores[levelIndex]; }
    private static HighscoreContainer _Highscore = new HighscoreContainer();

    public static int IndexOfSceneToMoveTo = 1;
    private static int _LevelIndex = 0;
    private static bool _Are0ScoresIgnored = true;
    [HideInInspector]
    public float BarRiseAnimationTime = 0.7f;
    public GameObject P_Scoring;
    public GameObject ReplayButton;
    public GameObject ScoreBarBase;

    /// <summary>
    /// Moves to the scores scene to display the final scores and declare a winner and/or new highscore.
    /// Please set in the scene if you wish to use the continue or replay arrow on the button. The sceneIndex
    /// parameter is for determining what scen to move to after the scores have been shown.
    /// </summary>
    /// <param name="sceneIndex">Scene to move to from score scene, defaults to one.</param>
    public static void MoveToScores(List<int> scores,int levelIndex = 1, int sceneIndex = 1, bool ignore0Scores = true)
    {
        if (scores == null)
        {
            Debug.LogError("ScoreScreenController | MoveToScores | No scores have been stored in the scores list!");
        }
        else if (scores.Count == 0)
        {
            Debug.LogError("ScoreScreenController | MoveToScores | No scores have been stored in the scores list!");
        }
        IndexOfSceneToMoveTo = sceneIndex;
        Scores = scores;
        _LevelIndex = levelIndex;
        _Are0ScoresIgnored = ignore0Scores;
        SceneManager.LoadScene("Scores");
    }

    void Start()
    {
        //turns on input processing
        BlobInputProcessing.SetState(true);

        //load highscore from file
        if(GlobalGameSettings.GetSetting("Reset Highscore").Equals("No"))
        {
            LoadHighscore();
        }
        else if(GlobalGameSettings.GetSetting("Reset Highscore").Equals(string.Empty))
        {
            if (GlobalGameSettings.GetSetting("Reset HS").Equals("No"))
            {
                LoadHighscore();
            }
        }

        //check if we have all requirements linked
        if(ScoreBarBase == null) { Debug.LogError("ScoreScreenController | Start | Missing base object for score bars."); }
        if(P_Scoring == null) { Debug.LogError("ScoreScreenController | Start | Missing Link to perant panel."); }
        if(ReplayButton == null) { Debug.LogError("ScoreScreenController | Start | Missing Link to replay button."); }

        if (Scores == null)
        {
            Debug.LogError("ScoreScreenController | Start | No scores have been stored in the static Scores list!");
        }
        else
        {
            int numberOf0Scores = 0;
            int highestScore = 0;
            foreach (int score in Scores)
            {
                if (score == 0 && _Are0ScoresIgnored) { numberOf0Scores++; }
                if (score > highestScore) { highestScore = score; }
            }
            //safety check, if we add a level it wount be in the highscore script
            if (_Highscore.Highscores.Count - 1 < _LevelIndex)
            {
                int count = _LevelIndex + 1 - _Highscore.Highscores.Count;
                for (int index = 0; index < count; index++)
                {
                    _Highscore.Highscores.Add(0);
                }
            }
            if (Scores.Count == 0)
            {
                Debug.LogError("ScoreScreenController | Start | No scores have been stored in the static Scores list!");
                return;
            }
            else if(Scores.Count - numberOf0Scores == 1)
            {
                SetupSinglePlayer(Scores.IndexOf(highestScore));
            }
            else if (Scores.Count - numberOf0Scores > 1)
            {
                SetupMultiPlayer(highestScore);
            }
            if(highestScore > _Highscore.Highscores[_LevelIndex])
            {
                _Highscore.Highscores[_LevelIndex] = highestScore;
                SaveHighscore();
            }
        }
        Invoke("EnableReplay", BarRiseAnimationTime + 1f);
    }

    private void SetupSinglePlayer(int playerNr)
    {
        int highestScore;
        if(Scores[playerNr] > _Highscore.Highscores[_LevelIndex])
        {
            highestScore = Scores[playerNr];
        }
        else
        {
            highestScore = _Highscore.Highscores[_LevelIndex];
        }
        ScoreBar temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
        temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(playerNr+1));
        temp.Begin(Scores[playerNr], (float)Scores[playerNr] / (float)highestScore, BarRiseAnimationTime, Scores[playerNr] > _Highscore.Highscores[_LevelIndex], Scores[playerNr] > _Highscore.Highscores[_LevelIndex], 0.1f);

        temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
        temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(0));
        temp.Begin(_Highscore.Highscores[_LevelIndex], (float)_Highscore.Highscores[_LevelIndex] / (float)highestScore, BarRiseAnimationTime, false, false, 0.1f);
    }

    private void SetupMultiPlayer(int highestScore)
    {
        for(int i = 0; i < Scores.Count; i++)
        {
            if (Scores[i] == 0 && _Are0ScoresIgnored)
            { continue; }
                ScoreBar temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
                temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(i+1));
                temp.Begin(Scores[i], (float)Scores[i] / (float)highestScore, BarRiseAnimationTime, Scores[i] > _Highscore.Highscores[_LevelIndex] && Scores[i] == highestScore, Scores[i] == highestScore, 0.1f);
            
        }
    }

    private void SaveHighscore()
    {
            XML_to_Class.SaveClassToXML(_Highscore, "StreamingAssets" + Path.DirectorySeparatorChar + "HighScore");
    }

    private void LoadHighscore()
    {
        HighscoreContainer temp = XML_to_Class.LoadClassFromXML<HighscoreContainer>("StreamingAssets"+ Path.DirectorySeparatorChar +"HighScore");
        if(temp == null)
        {
            _Highscore = new HighscoreContainer(new List<int>());
        }
        else
        {
            _Highscore = temp;
        }
    }

    private void EnableReplay()
    {
        ReplayButton.SetActive(true);
    }

    public void BT_Replay_Clicked()
    {
        SceneManager.LoadScene(IndexOfSceneToMoveTo);
    }
}
