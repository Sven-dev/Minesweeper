using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWTempButton : Button, I_SmartwallInteractable
{
    public void Hit(Vector3 Hitpos)
    {
        onClick.Invoke();
    }
}
