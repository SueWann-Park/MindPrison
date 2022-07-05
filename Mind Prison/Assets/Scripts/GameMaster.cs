using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static int gameState = 0;
    public Transform playerSpriteTransform;
    public Animator doorAnimator;
    public PlayerMain pm;

    private SpriteRenderer psr;
    
    private int touchCount = 0;
    private float lastTouchTime = 0;
    public static bool isJumping;

    // Start is called before the first frame update
    void Start()
    {
        psr = playerSpriteTransform.GetComponent<SpriteRenderer>();
        boxes = new Transform[3];
        for(int i = 0; i < 3; i++)
        {
            boxes[i] = pm.transform.GetChild(i+3);
            boxes[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayerEnter()
    {
        psr.sortingLayerName = "Road";
        playerSpriteTransform.localScale = new Vector3(0.9f, 0.9f, 1);
        doorAnimator.SetBool("IsOpen", true);

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(PlayerUpDown());
        yield return StartCoroutine(PlayerScaleUp());

        psr.sortingLayerName = "Player";
        doorAnimator.SetBool("IsOpen", false);
        gameState = 1;
    }
    
    private IEnumerator PlayerUpDown()
    {
        float startX = playerSpriteTransform.position.x;
        float startY = playerSpriteTransform.position.y;
        for(int i = 0; i<100; i++)
        {
            playerSpriteTransform.position = new Vector3(startX, startY + Mathf.Sin(Time.time * 5) * 0.05f, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator PlayerScaleUp()
    {
        for(int i = 0; i < 100; i++)
        {
            playerSpriteTransform.localScale = new Vector3(0.9f + i / 1000.0f, 0.9f + i / 1000.0f, 1);
            yield return new WaitForFixedUpdate();
        }
    }

    private float xDest;
    Coroutine lastRoutine = null;
    Coroutine firstMessageRoutine = null;
    // Update is called once per frame
    void Update()
    {
        bool isTouched = Input.GetMouseButtonDown(0);
        if(isTouched == true)
        {
            touchCount++;
            lastTouchTime = Time.time;
        }
        else if(Time.time > lastTouchTime + 0.3f)
        {
            touchCount--;
            if (touchCount < 0)
                touchCount = 0;
        }

        if (gameState == 0)
        {
            if (isTouched == true && lastRoutine == null)
            {
                lastRoutine = StartCoroutine(PlayerEnter());
            }
            touchCount = 0;
        }
        else if(gameState == 1)
        {
            if(isTouched == true && firstMessageRoutine == null)
            {
                firstMessageRoutine = StartCoroutine(FirstMessages());
            }
            touchCount = 0;
        }
        else if(gameState == 2)
        {
            if (touchCount >= 2 && isTouched == true)
            {
                touchCount = 0;
                pm.ActionJump();
            }

            if(isTouched == true)
            {
                xDest = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            }
            if(pm.isJumped == false)
            {
                pm.ActionMove(xDest - pm.transform.position.x);
            }
            else
            {
                xDest = pm.transform.position.x;
            }
        }
    }

    Transform[] boxes;


    public static List<string> texts = new List<string>()
    {
        "�ܷο�", "�ɽ���", "���;�...", "���ο�...", "��... ���߾�",
        "�¾��� ���� �;�, �ϴ��� � ����ϱ�?", "���� �� ��ó�� ���ĥ �� �־�����...",
        "���� ��� �� �ٴٿ��� ������ ���Ѵٸ� �����...?", "�ٴٿ��� �¾���ϱ� �ٴٿ��� ��ٰ� ���������",
        "�ٵ� ��� �ִ� �ž�", "�ۿ� ������ �� ���÷� ������", "�� �� �̰��� ����?",
        "�� �� �̷��� �¾�ž�...", "�� ��¥ ������ ���� �;�", "���� �� ���� ��� �ִ� �ž�?",
        "�ۿ� ������ ���� �� �ؾ� �ұ�?", "���� �׸��ϰ� �;�...", "�ű�, ���� ����...?",
        "���� �ʹ� ��Ӱ�... ���� ���� �ʿ���", "���� ���⼭ ������ ����"
    };

    private IEnumerator FirstMessages()
    {
        yield return StartCoroutine(ShowText("..."));
        yield return StartCoroutine(ShowText("���õ� �״�ξ�"));
        yield return StartCoroutine(ShowText("���� ��� �� �ٴٿ��� ������ ���Ѵٸ� �����...?"));

        gameState = 2;
    }

    private IEnumerator ShowText(string s)
    {
        int count = 0;
        for(int i = 0; i < s.Length; i++)
        {
            count += s[i] == ' ' || s[i] <= 122 ? 1 : 3;
        }
        int index = 2;
        if(count < 15)
        {
            index = 0;
        }
        else if(count < 24)
        {
            index = 1;
        }

        yield return StartCoroutine(PrintText(boxes[index], s));
    }

    private IEnumerator PrintText(Transform b, string s)
    {
        b.gameObject.SetActive(true);

        SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
        TMPro.TextMeshProUGUI t = b.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        t.text = "";
        yield return StartCoroutine(FadeIn(sr));
        
        yield return PrintCharByChar(t, s);
        yield return new WaitForSeconds(3);
        t.text = "";
        yield return StartCoroutine(FadeOut(sr));

        b.gameObject.SetActive(false);
    }

    private IEnumerator PrintCharByChar(TMPro.TextMeshProUGUI t, string s)
    {
        t.text = "";
        for(int i = 0; i < s.Length; i++)
        {
            t.text += s[i];
            yield return new WaitForSeconds(0.35f);
        }
    }

    private IEnumerator FadeIn(SpriteRenderer sr)
    {
        for(int i = 0; i < 30; i++)
        {
            Color c = sr.color;
            c.a = i / 30.0f;
            sr.color = c;
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator FadeOut(SpriteRenderer sr)
    {
        for (int i = 30; i > 0; i--)
        {
            Color c = sr.color;
            c.a = i / 30.0f;
            sr.color = c;
            yield return new WaitForFixedUpdate();
        }
    }
}
