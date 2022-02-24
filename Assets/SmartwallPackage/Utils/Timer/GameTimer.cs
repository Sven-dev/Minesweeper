using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GameTimer : MonoBehaviour
{
    [Tooltip("Time limit can be overwritten by the setting file if it contains a setting from Time.")]
    public float TimeLimit;
    public float TimeRemaining;

    [Space]
    public Text LabelOfTimer;
    public Image Gage;

    [Space][Tooltip("The amount of seconds when the timer needs to execute certain behaviours.")]
    public int AlmostFinishedTime = 5;
    public Color AlmostFinishedColor;

    [Space]
    public Image _FinishedFade;
    public UnityEvent TimerRanOut = new UnityEvent();

    protected float _StartTime;
    protected Color _ColourStart;
    protected bool Paused = false;

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
    }

    /// <summary>
    /// Start running the set timer.
    /// </summary>
    public virtual void StartTimer()
    {
        StopCoroutine("RunTimer");

        _StartTime = Time.time;
        LabelOfTimer.color = _ColourStart;
        StartCoroutine("RunTimer");
    }

    /// <summary>
    /// Starts the timer with a custom time.
    /// </summary>
    public void StartTimer(float time)
    {
        StopCoroutine("RunTimer");

        TimeLimit = time;
        _StartTime = Time.time;
        LabelOfTimer.color = _ColourStart;
        StartCoroutine("RunTimer");
    }

    /// <summary>
    /// Pause or unpause the timer.
    /// </summary>
    public void PauseTimer(bool pause)
    {
        Paused = pause;
    }

    protected virtual void Awake()
    {
        //Check if a Text class has been linked
        if (LabelOfTimer == null)
        {
            LabelOfTimer = gameObject.GetComponent<Text>(); //Try to find a Text class
            if (LabelOfTimer == null)
            {
                Debug.LogWarning("L_Text | Start | Text changer has no label to change and can't find one on its gameobject: " + gameObject.name);
                return;
            }
            else
            {
                Debug.LogWarning("L_Text | Start | Text changer has no label to change but it has found a Text class on its gameobject: " + gameObject.name);
            }
        }

        _ColourStart = LabelOfTimer.color;

        //load time setting from settings file; if there is no time setting in the file, the inspector value is used instead.
        string[] setting = GlobalGameSettings.GetSetting("Playtime").Split(' ');
        if (setting.Length > 0 && setting[0] != string.Empty)
        {
            TimeLimit = int.Parse(setting[0]);
        }

        int minutes = (int)(TimeLimit / 60);
        int seconds = (int)(TimeLimit % 60);
        LabelOfTimer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    IEnumerator RunTimer()
    {
        TimeRemaining = TimeLimit;
        float redFade = 0;
        bool finale = false;

        while (TimeRemaining > 0)
        {
            if (!Paused)
            {
                int minutes = (int)(TimeRemaining / 60);
                int seconds = (int)(TimeRemaining % 60);
                Gage.fillAmount = TimeRemaining / TimeLimit;
                LabelOfTimer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

                if (TimeRemaining < AlmostFinishedTime)
                {
                    redFade += Time.deltaTime / AlmostFinishedTime;
                    LabelOfTimer.color = Color.Lerp(_ColourStart, AlmostFinishedColor, redFade);

                    if (!finale)
                    {
                        AudioManager.Instance.Play("TimeRunningOut");
                        finale = true;
                    }
                }

                TimeRemaining -= Time.deltaTime;
            }

            yield return null;          
        }

        TimeRemaining = 0;
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