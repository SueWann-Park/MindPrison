using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public AudioSource opening;
    public AudioSource closing;

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

        texts = Shuffle<string>(texts);
    }
    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 0)
        {
            n--;
            int k = Random.Range(0,list.Count);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    private IEnumerator PlayerEnter()
    {
        psr.sortingLayerName = "Road";
        playerSpriteTransform.localScale = new Vector3(0.9f, 0.9f, 1);
        doorAnimator.SetBool("IsOpen", true);
        opening.Play();

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(PlayerUpDown());
        yield return StartCoroutine(PlayerScaleUp());

        psr.sortingLayerName = "Player";
        doorAnimator.SetBool("IsOpen", false);
        closing.Play();

        gameState = 1;
    }

    public Animator playerAnimator;
    private IEnumerator PlayerExit()
    {
        doorAnimator.SetBool("IsOpen", true);
        opening.Play();

        yield return new WaitForSeconds(0.3f);

        psr.sortingLayerName = "Road";

        playerAnimator.SetBool("IsBack", true);
        StartCoroutine(PlayerUpDown());
        yield return StartCoroutine(PlayerScaleDown());

        doorAnimator.SetBool("IsOpen", false);
        closing.Play();

        gameState = 0;

        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("IsBack", false);

        textIndex = 0;
        texts = Shuffle<string>(texts);
        goToHomeRoutine = null;
        firstMessageRoutine = null;
        lastRoutine = null;
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
        playerSpriteTransform.localScale = new Vector3(1f, 1f, 1);
    }

    private IEnumerator PlayerScaleDown()
    {
        for (int i = 0; i < 100; i++)
        {
            playerSpriteTransform.localScale = new Vector3(1f - i / 1000.0f, 1f - i / 1000.0f, 1);
            yield return new WaitForFixedUpdate();
        }
        playerSpriteTransform.localScale = new Vector3(0.9f,0.9f,1);
    }
    private IEnumerator LastMessages()
    {
        yield return StartCoroutine(ShowText("거기... 아무도 없어?"));
        yield return StartCoroutine(ShowText("..."));
        yield return StartCoroutine(ShowText("안녕..."));

        gameState = 4;
    }

    private float xDest, yDest;
    Coroutine lastRoutine = null;
    Coroutine firstMessageRoutine = null;
    Coroutine goToHomeRoutine = null;
    // Update is called once per frame
    void Update()
    {
        bool isTouched = Input.GetMouseButtonDown(0);
        if(isTouched == true)
        {
            touchCount++;
            lastTouchTime = Time.time;
        }
        else if(Time.time > lastTouchTime + 40f && goToHomeRoutine == null && gameState == 2)
        {
            goToHomeRoutine =  StartCoroutine(LastMessages());
            gameState = 3;
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
            lastTouchTime = Time.time;
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
                yDest = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

                float xDiff = Mathf.Abs(xDest - pm.transform.position.x);
                float yDiff = yDest - pm.transform.position.y;

                if(xDiff < 0.5f && yDiff < 2.2f) // player Touch
                {
                    gameState = 9;
                    StartCoroutine(PlayerTalk());
                }
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
        else if(gameState == 4)
        {
            pm.ActionMove(-pm.transform.position.x);
            if(Mathf.Abs(pm.transform.position.x) < 0.1f)
            {
                gameState = 5;
                StartCoroutine(PlayerExit());
            }
        }
    }

    int textIndex = 0;
    private IEnumerator PlayerTalk()
    {
        yield return StartCoroutine(ShowText(texts[textIndex++]));

        gameState = 2;
        if(textIndex >= texts.Count)
        {
            goToHomeRoutine = StartCoroutine(LastMessages());
            gameState = 3;
        }
    }

    Transform[] boxes;


    public static List<string> texts = new List<string>()
    {
        "외로워", "심심해", "울고싶어...", "괴로워...", "난... 망했어",
        "태양이 보고 싶어, 하늘은 어떤 모습일까?", "나도 저 고래처럼 헤엄칠 수 있었으면...",
        "내가 평생 이 바다에서 나가지 못한다면 어떡하지...?", "바다에서 태어났으니까 바다에서 살다가 사라지겠지",
        "다들 어디에 있는 거야", "밖에 나가면 꼭 도시로 가야지", "난 왜 이곳에 있지?",
        "난 왜 이렇게 태어난거야...", "내 진짜 집으로 가고 싶어", "누가 내 얘기는 듣고 있는 거야?",
        "밖에 나가면 먼저 뭘 해야 할까?", "이제 그만하고 싶어...", "거기, 누구 없어...?",
        "여긴 너무 어둡고... 나는 빛이 필요해", "제발 여기서 나가게 해줘"
    };

    private IEnumerator FirstMessages()
    {
        yield return StartCoroutine(ShowText("..."));
        yield return StartCoroutine(ShowText("오늘도 그대로야"));
        yield return StartCoroutine(ShowText("내가 평생 이 바다에서 나가지 못한다면 어떡하지...?"));

        xDest = pm.transform.position.x;
        gameState = 2;
    }

    private IEnumerator ShowText(string s)
    {
        pm.ActionMove(0);
        xDest = pm.transform.position.x;
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
        yield return new WaitForSeconds(1.5f);
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
            yield return new WaitForSeconds(0.15f);
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
