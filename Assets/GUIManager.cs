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

        int time = 0;
        while (true)
        {
            time++;
            TimerLabel.text = time.ToString();
            yield return new WaitForSeconds(1);
        }
    }
}
