using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWToggle : Toggle, I_SmartwallInteractable
{
    private bool Cooldown = false;

#if !UNITY_EDITOR
    public void Hit(Vector3 hitpos)
    {
        if(!Cooldown)
        {
            isOn = !isOn;
            StartCoroutine(_Cooldown());
        }
    }
#endif

#if UNITY_EDITOR
    public void Hit(Vector3 hitpos)
    {

    }
#endif

    private IEnumerator _Cooldown()
    {
        Cooldown = true;
        yield return new WaitForSeconds(0.25f);
        Cooldown = false;
    }
}