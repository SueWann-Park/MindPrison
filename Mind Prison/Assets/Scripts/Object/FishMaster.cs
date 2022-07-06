using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMaster : MonoBehaviour
{
    Sprite yellow;
    Sprite green;
    List<Transform> groups;

    // Start is called before the first frame update
    void Start()
    {
        green = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        yellow = transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        groups = new List<Transform>();

        StartCoroutine(LazyMaker());
    }

    IEnumerator LazyMaker()
    {
        for(; ;)
        {
            MakeFishes();
            yield return new WaitForSeconds(5);
        }
    }

    void MakeFishes()
    {
        if (groups.Count >= 5)
            return;

        int numFishes = Random.Range(2, 11);
        int dir = Random.Range(0, 2); // 0 : right, 1: left
        int x = dir == 0 ? 20 : -20;
        int y = Random.Range(-6, 9);
        float moveCoff = Random.Range(0.025f, 0.05f);

        GameObject group = new GameObject();
        groups.Add(group.transform);
        group.transform.parent = this.transform;
        group.name = "Group" + groups.Count.ToString();
        group.transform.localScale = new Vector3(dir == 1 ? -1 : 1, 1, 1);
        group.transform.position = new Vector2(x, y);

        float groupY = group.transform.position.y;
        List<Coroutine> fishMoves = new List<Coroutine>();
        for(int i = 0; i < numFishes; i++)
        {
            GameObject fish = new GameObject();
            fish.name = "Fish" + i.ToString();
            fish.AddComponent<SpriteRenderer>().sprite = y < 0 ? green : yellow;
            fish.transform.parent = group.transform;
            fish.transform.localPosition = new Vector2(Random.Range(-2f, 2f), Random.Range(-1f, 1f));
            fish.transform.localScale = Vector3.one;

            float coff = 0.65f + groupY * 0.4f / 16;
            fish.GetComponent<SpriteRenderer>().color = new Color(coff, coff, coff);

            fishMoves.Add(StartCoroutine(TwiggleFish(fish.transform)));
        }

        StartCoroutine(MoveFishes(group.transform, moveCoff, fishMoves));
    }

    IEnumerator TwiggleFish(Transform t)
    {
        // y = c1 * sin(x * c2) + c3;
        float[] coffs = new float[6];
        for(int i =  0; i < 6; i++)
        {
            coffs[i] = Random.Range(-0.8f, 0.8f);
        }
        for(; ;)
        {
            t.localPosition = new Vector3(coffs[0] * Mathf.Sin(Time.time * coffs[1]) + coffs[2],
                0.8f * coffs[3] * Mathf.Sin(Time.time * coffs[4]) + 0.6f * coffs[5]);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator MoveFishes(Transform t, float moveCoff, List<Coroutine> fishMoves)
    {
        float accum = 0;
        for(int z = 0; ;z++)
        {
            t.position += Vector3.right * t.localScale.x * (-1) * moveCoff;
            accum += moveCoff;


            if (accum >= 40)
                break;

            yield return new WaitForFixedUpdate();
        }

        for(int i = 0; i < fishMoves.Count; i++)
        {
            StopCoroutine(fishMoves[i]);
        }

        groups.Remove(t);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
