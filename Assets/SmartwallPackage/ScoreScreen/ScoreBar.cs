using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    public Image Bar;
    public GameObject Crown;
    public GameObject NewHighscore;
    public Text T_Score;

    private int _Score;
    private float _BarFillPerc;
    private float _TimeToRaise;
    private bool _Started = false;
    private bool _IsWinner = false;
    private bool _HasHighscore = false;
    
    public void SetNewBarColour(Color colour)
    {
        Bar.color = colour;
    }

    public void Begin(int score, float barFillPerc, float timeToRaiseAllInSeconds, bool hasHighscore, bool isWinner)
    {
        Begin(score, barFillPerc, timeToRaiseAllInSeconds, hasHighscore, isWinner, 0f);
    }

    public void Begin(int score, float barFillPerc, float timeToRaiseAllInSeconds, bool hasHighscore, bool isWinner, float delay)
    {
        _IsWinner = isWinner;
        _HasHighscore = hasHighscore;
        _Score = score;
        _BarFillPerc = (barFillPerc*0.85f)+0.15f;
        _TimeToRaise = timeToRaiseAllInSeconds;
        if (delay == 0)
        {
            RaiseBar();
        }
        else
        {
            Invoke("RaiseBar", delay);
        }
    }

    private void RaiseBar()
    {
        _Started = true;
        AudioManager.Instance.Play("ScoreGrow");
    }

    // Update is called once per frame
    void Update()
    {
        if (_Started)
        {
            if(Bar.fillAmount < _BarFillPerc)
            {
                Bar.fillAmount += (Time.deltaTime / _TimeToRaise);
                if (Bar.fillAmount >= _BarFillPerc)
                {
                    Bar.fillAmount = _BarFillPerc;
                    T_Score.text = "<b>" + _Score.ToString() + "</b>";
                    Invoke("ShowTopIcons", 0.4f);
                }
            }
        }
    }

    private void ShowTopIcons()
    {
        if (_IsWinner)
        {
            Crown.SetActive(true);
            AudioManager.Instance.Play("ScoreWin");
        }
        if (_HasHighscore)
        {
            NewHighscore.SetActive(true);
        }
    }
}