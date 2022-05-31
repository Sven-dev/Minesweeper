using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private Text TimerLabel;
    [SerializeField] private Text BombsLeftLabel;
    [Space]
    [SerializeField] private GameObject VictoryPanel;
    [SerializeField] private GameObject GameOverPanel;

    private IEnumerator Timer;

    public void SetBombAmount(int bombAmount)
    {
        BombsLeftLabel.text = bombAmount.ToString();
    }

    public void Victory()
    {
        VictoryPanel.SetActive(true);
    }

    public void GameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void StartTimer()
    {
        Timer = _Timer();
        StartCoroutine(Timer);
    }

    public void StopTimer()
    {
        StopCoroutine(Timer);
    }

    private IEnumerator _Timer()
    {
        //To do: convert seconds to time
        double time = 0;
        while (true)
        {
            time++;
            TimerLabel.text = TimeSpan.FromMinutes(time).ToString(@"hh\:mm");
            //string test = "Time of Travel: {0:dd\\.hh\\:mm\\:ss} days", duration)
            yield return new WaitForSeconds(1);
        }
    }
}
