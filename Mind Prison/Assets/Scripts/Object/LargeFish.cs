using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeFish : MonoBehaviour
{
    public int halfX = 28;
    public bool isLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePosition());
    }

    float coffX;
    float coffY1;
    float coffY2;
    float startY;
    IEnumerator UpdatePosition()
    {
        SetRandomValues();

        for (; ;)
        {
            transform.position = new Vector3(transform.position.x + coffX * (isLeft ? -1 : 1), startY + coffY1 * Mathf.Sin(coffY1 * Time.time), 0);

            yield return new WaitForFixedUpdate();

            float x = Camera.main.transform.position.x;
            if (Mathf.Abs(transform.position.x - x) > halfX)
            {
                transform.position = new Vector3(x - halfX * (isLeft ? -1 : 1), Random.Range(-8, 8f), 0);
                SetRandomValues();
            }
        }
    }

    private void SetRandomValues()
    {
        coffX = Random.Range(0.005f, 0.015f) * 30;
        coffY1 = Random.Range(0.1f, 0.5f);
        coffY2 = Random.Range(0.5f, 2f);
        startY = Random.Range(-5, 5f);
    }
}
