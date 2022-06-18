using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeFish : MonoBehaviour
{
    public int halfX = 20;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePosition());
    }

    float moveX;
    float[] moveY;
    float startY;
    IEnumerator UpdatePosition()
    {
        moveY = new float[2];
        SetRandomValues();

        for (; ;)
        {
            transform.position = new Vector3(transform.position.x + moveX, moveY[0] * Mathf.Sin(moveY[1] * Time.time), 0);

            yield return new WaitForFixedUpdate();

            float x = Camera.main.transform.position.x;
            if (transform.position.x > x + halfX)
            {
                transform.position = new Vector3(x - halfX, Random.Range(-8, 8f), 0);
                SetRandomValues();
            }
        }
    }

    private void SetRandomValues()
    {
        moveX = Random.Range(0.005f, 0.015f);
        moveY[0] = Random.Range(0.1f, 0.5f);
        moveY[1] = Random.Range(0.5f, 2f);
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
