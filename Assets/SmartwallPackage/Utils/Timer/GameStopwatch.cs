using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timer that counts up until the game is finished.
/// </summary>
public class GameStopwatch : GameTimer
{
    private bool Finished = false;

    public void Finish()
    {
        Finished = true;
    }

    protected override void Awake()
    {
        base.Awake();
        LabelOfTimer.text = "00:00";
    }

    public override void StartTimer()
    {
        _StartTime = Time.time;
        LabelOfTimer.color = _ColourStart;

        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer()
    {
        float t = 0;
        while (!Finished)
        {
            if (!Paused)
            {
                int minutes = (int)(t / 60);
                int seconds = (int)(t % 60);
                Gage.fillAmount = t / TimeLimit;
                LabelOfTimer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

                t += Time.deltaTime;
            }

            yield return null;
        }

        Color c = _FinishedFade.color;
        c.a = 0.5f;
        _FinishedFade.color = c;
        AudioManager.Instance.Play("TimeRanOut");
        yield return new WaitForSeconds(0.5f);

        //make sure the player isn't able to hit stuff anymore
        BlobInputProcessing.SetState(false);
        yield return new WaitForSeconds(1.5f);

        TimerRanOut.Invoke();
    }
}
