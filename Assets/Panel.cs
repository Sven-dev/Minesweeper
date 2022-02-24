using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour, I_SmartwallInteractable
{
    [HideInInspector] public Vector2 Coordinates;
    [HideInInspector] public bool Bomb = false;
    [HideInInspector] public int Value;
    [HideInInspector] public bool Revealed = false;

    [SerializeField] private Image Background;
    [SerializeField] private Text ValueLabel;
    [SerializeField] private Image BombImage;
    [Space]
    [SerializeField] private BoxCollider2D Collider;

    /// <summary>
    /// Sets the collider size to be the same as the image
    /// </summary>
    public void SetColliderSize(Vector2 size)
    {
        Collider.size = size;
    }

    public void Hit(Vector3 hitPosition)
    {
        GridManager.Instance.revealPanel(Coordinates);
    }

    public void Reveal()
    {
        Revealed = true;

        //reveal either nothing, a number or a bomb
        if (Value == -1)
        {
            //Bomb
            BombImage.enabled = true;
        }
        else if (Value != 0)
        {
            ValueLabel.enabled = true;
            ValueLabel.text = Value.ToString();
        }

        //turn the panel around (or another animation)
        //Sound effect
    }
}