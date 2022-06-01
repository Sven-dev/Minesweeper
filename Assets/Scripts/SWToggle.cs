using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWToggle : Toggle, I_SmartwallInteractable
{
#if !UNITY_EDITOR
    public void Hit(Vector3 hitpos)
    {
        isOn = !isOn;
    }
#endif

#if UNITY_EDITOR
    public void Hit(Vector3 hitpos)
    {

    }
#endif
}