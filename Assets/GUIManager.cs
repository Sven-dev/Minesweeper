using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private Text TimerLabel;
    [SerializeField] private Text BombsLeftLabel;

    public void SetBombAmount(int bombAmount)
    {
        BombsLeftLabel.text = bombAmount.ToString();
    }

    public void FirstPanelReveal()
    {
        StartCoroutine(_Timer());
    }

    private IEnumerator _Timer()
    {
        int time = 0;
        while (true)
        {
            time++;
            TimerLabel.text = time.ToString();
            yield return new WaitForSeconds(1);
        }
    }
}
