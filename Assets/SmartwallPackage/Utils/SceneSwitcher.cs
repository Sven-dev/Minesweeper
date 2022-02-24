using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField]
    private int SceneIndex = 1;

    void Start()
    {
        StartCoroutine(switchScene(SceneIndex));
    }

    /// <summary>
    /// Switches to a new scene after 1 frame.s
    /// </summary>
    /// <param name="index">the index of the scene in build order</param>
    /// <returns></returns>
    private IEnumerator switchScene(int index)
    {
        yield return null;
        SceneManager.LoadScene(index);
    }
}