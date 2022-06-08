using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour, I_SmartwallInteractable
{
    [HideInInspector] public Vector2 Coordinates;
    [HideInInspector] public int Value; //-1 = bomb, 0 = no adjacent bombs, 1..9 = x ajacent bombs.
    [HideInInspector] public bool Revealed = false;
    [HideInInspector] public bool Flagged = false;

    [SerializeField] private Image Background;
    [SerializeField] private Sprite RevealedSprite;
    [SerializeField] private Color RevealedColor;  
    [Space]
    [SerializeField] private Text ValueLabel;
    [SerializeField] private Image BombImage;
    [SerializeField] private Image FlagImage;
    [SerializeField] private Image WrongFlagX;
    [SerializeField] private GameObject Explosion;
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
        print("Hit");
        if (!GridManager.Instance.GameOver && !Revealed)
        {
            if (GridManager.Instance.FlagMode)
            {
                GridManager.Instance.FlagPanel(Coordinates);
            }
            else if (!Flagged)
            {
                GridManager.Instance.revealPanel(Coordinates);
            }
        }
    }

    public void ToggleFlag()
    {
        Flagged = !Flagged;
        FlagImage.enabled = Flagged;

        if (Flagged)
        {
            AudioManager.Instance.Play("FlagMark");
        }
        else
        {
            AudioManager.Instance.Play("FlagClear");
        }
    }

    public void ToggleX()
    {
        WrongFlagX.enabled = !WrongFlagX.enabled;
    }

    public void Reveal(int value)
    {
        Revealed = true;
        Value = value;

        //turn the panel around (or another animation)
        Background.sprite = RevealedSprite;
        Background.color = RevealedColor;

        //reveal either nothing, a number or a bomb
        if (value == -1)
        {
            //Bomb
            //Move the bomb image down to make it allign with the panel
            BombImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -5);

            ShowBomb();
            return;
        }

        if (value != 0)
        {
            ValueLabel.enabled = true;
            ValueLabel.text = value.ToString();
            ValueLabel.color = ColorPicker.Instance.Colors[value - 1];
        }

        //Sound effect
        AudioManager.Instance.PlayRandom("PanelReveal");
    }

    public void ShowBomb()
    {
        BombImage.enabled = true;
        Explosion.SetActive(true);
        ScreenShaker.Instance.Shake(0.0825f, 0.1f);

        //Sound effect
        AudioManager.Instance.Play("BombExplode");
    }
}