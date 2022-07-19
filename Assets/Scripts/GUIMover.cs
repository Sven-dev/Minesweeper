using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class GUIMover : MonoBehaviour
{
    [SerializeField] private Vector2 Movement;

    private List<RectTransform> Contents = new List<RectTransform>();

    private void Start()
    {
        Contents = transform.GetComponentsInChildren<RectTransform>().ToList();
        Contents.Remove(GetComponent<RectTransform>());
    }

    public void MoveContentsDown()
    {
        foreach(RectTransform element in Contents)
        {
            element.anchoredPosition += Movement;
        }
    }

    public void MoveContentUp()
    {
        foreach (RectTransform element in Contents)
        {
            element.anchoredPosition -= Movement;
        }
    }
}