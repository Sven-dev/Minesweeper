using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public static ColorPicker Instance;

    public List<Color> Colors;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
}
