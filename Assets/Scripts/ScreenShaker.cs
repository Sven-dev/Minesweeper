using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void Shake(float amount, float duration)
    {
        StartCoroutine(_Shake(amount, duration));
    }

    private IEnumerator _Shake(float amount, float duration)
    {
        float progress = 0;
        while (progress <= 1)
        {
            Vector3 position = new Vector3(Random.Range(-amount, amount), transform.position.y, transform.position.z);
            transform.position = position;

            progress += Time.deltaTime / duration;
            yield return new WaitForSeconds(0.05f);
        }

        transform.position = new Vector3(0, 0, transform.position.z);
    }
}
