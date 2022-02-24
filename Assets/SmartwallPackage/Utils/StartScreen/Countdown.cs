using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Countdown : MonoBehaviour, I_SmartwallInteractable
{
    public List<GameObject> Numbers = new List<GameObject>();
    public Image I_HitMe;
    public UnityEvent CountdownFinished = new UnityEvent();

    private bool started = false;

    IEnumerator CountDown()
    {
        foreach (GameObject go in Numbers)
        {
            go.SetActive(true);
            go.GetComponent<Animation>().Play();
            AudioManager.Instance.Play("CountdownBeep");
            yield return new WaitForSeconds(1);
            go.SetActive(false);
        }
      
        CountdownFinished.Invoke();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        gameObject.GetComponent<Image>().enabled = false;
        AudioManager.Instance.Play("CountdownGo");
    }

    public void Hit(Vector3 hitPosition)
    {
        if (!started)
        {
            I_HitMe.enabled = false;
            StartCoroutine(CountDown());
            started = true;
        }
    }
}
