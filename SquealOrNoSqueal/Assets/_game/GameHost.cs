using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class GameHost : StateMachine
{
    #region Vars
    public const int MAX_CASES = 26;
    public const int CASES_PER_ROW = 5;

    [Header("Game")]
    public int Picked = 0;
    public int Round = 0;
    public int PickedThisRound = 0;
    public int[] Picks = new int[] { 1, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1 };
    public List<PiggyBank> BankList = new List<PiggyBank>(MAX_CASES);
    Dictionary<double, GameObject> ValueDisplays = new Dictionary<double, GameObject>();
    bool IsPicking = false;

    AudioSource Audio;
    [Header("Audio")]
    public AudioClip SndPick;
    public AudioClip SndStop;
    public AudioClip SndNewRound;

    [Header("Objects")]
    public Text InstructionsText;
    public GameObject PigPrefab;
    public Transform PlayAreaTransform;
    public Transform PickedAreaTransform;
    public GameObject PickedValuePrefab;

    [Space(10)]
    [Header("Settings")]
    [Range(1, 100)]
    public float CurveHandleDistance = 10f;
    [Range(0, 3)]
    public float EntryAnimTime = 1.5f;
    [Range(0.1f, 3)]
    public float EntryAnimGap = 0.5f;

    public enum GameState
    {
        Setup, FirstCase, PickCase, BankOffer, Reveal
    }
    #endregion

    #region Monobehavior
    // Use this for initialization
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        InitializeStateMachine<GameState>(GameState.Setup, true);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    #endregion

    #region Setup
    void EnterSetup(Enum previous)
    {
        kgLogger.instance.Post(kgLogger.PostChannel.INFO, "Initializing...");
        InstructionsText.text = "Get ready.";
        StartCoroutine(Wobble());

        List<double> amtList = new List<double>() {
            0.01d,
            1d,
            5d,
            10d,
            25d,
            50d,
            75d,
            100d,
            200d,
            300d,
            400d,
            500d,
            750d,

            1000d,
            5000d,
            10000d,
            25000d,
            50000d,
            75000d,
            100000d,
            200000d,
            300000d,
            400000d,
            500000d,
            750000d,
            1000000d
        };

        for(int i = 0; i < amtList.Count; i++)
        {
            // Value Display
            GameObject newButton = Instantiate(PickedValuePrefab) as GameObject;
            Text text = newButton.transform.GetComponentInChildren<Text>();
            text.text = amtList[i].ToString("C2");
            newButton.transform.SetParent(PickedAreaTransform);
            ValueDisplays.Add(amtList[i], newButton);
        }

        Vector3 center = Camera.main.ScreenToWorldPoint(PlayAreaTransform.transform.position);
        SpriteRenderer sr = PigPrefab.GetComponent<SpriteRenderer>();
        int grid = 5;
        float texWidth = sr.size.x;
        float texHeight = sr.size.y;
        float halfWidth = texWidth * grid * 0.5f;
        float halfHeight = texHeight * grid * 0.5f;
        Vector3 dest = Vector3.zero;

        // Lazy Randomize
        amtList = amtList.OrderBy(o => Guid.NewGuid()).ToList();
        // Iterate
        foreach (var details in amtList.Select((value, idx) => new { Index = idx, Value = value }))
        {
            PiggyBank bank = new PiggyBank();
            bank.contentValue = details.Value;
            bank.sprite = Instantiate(PigPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            bank.sprite.transform.SetParent(this.transform);
            bank.sprite.transform.position = new Vector3(center.x, center.y - 10, 0f);

            dest.x = center.x + halfWidth - (details.Index % grid) * texWidth;
            dest.y = center.y + halfHeight - (details.Index / grid) * texHeight;

            //bank.id = count++;
            bank.id = details.Index;

            if (bank.id == 25) dest.x -= texWidth * 2;
            StartCoroutine(MoveToPositionCurved(bank.sprite.transform, dest, EntryAnimTime, EntryAnimGap * details.Index, CurveHandleType.END));

            BankList.Add(bank);
        }




        StartCoroutine(BlockWait(EntryAnimGap * MAX_CASES + (EntryAnimTime*2), ()=> {
            ChangeCurrentState(GameState.FirstCase);
        }));
    }
    #endregion

    #region FirstCase
    void EnterFirstCase(Enum previous)
    {
        // Update instructions
        InstructionsText.text = "Select your first piggy bank.";
        Audio.PlayOneShot(SndNewRound, 1);
        StartCoroutine(WaitForInstruction());
    }

    void UpdateFirstCase()
    {
        if(!IsPicking)
        {
            ChangeCurrentState(GameState.PickCase);
        }
    }
    #endregion

    #region PickCase
    int newTarget = 1;
    void EnterPickCase(Enum previous)
    {
        InstructionsText.text = "Select 6 more piggy banks.";
        Audio.PlayOneShot(SndNewRound, 1);
        StartCoroutine(WaitForInstruction());
        newTarget = 7;
    }
    void UpdatePickCase()
    {
        if (!IsPicking)
        {
            if(Picked < newTarget)
            {
                StartCoroutine(WaitForInstruction());
            } else
            {
                ChangeCurrentState(GameState.BankOffer);
            }            
        }
    }
    #endregion

    #region BankOffer
    #endregion

    #region Reveal
    #endregion


    #region Utils
    void CycleList(Action<PiggyBank> toDo)
    {
        if (toDo == null) return;
        for (int i = 0; i < this.BankList.Count; i++)
            toDo.Invoke(BankList[i]);
    }

    enum CurveHandleType
    {
        START, MID, END
    }

    public Vector3 Bezier(float t, Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = Vector3.Lerp(a, b, t);
        var bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    IEnumerator MoveToPositionCurved(Transform item, Vector3 destination, float animTime = 0.5f, float animationDelay = 0f, CurveHandleType curveType = CurveHandleType.MID, bool randomized = true)
    {
        float elapsedTime = 0;
        Vector3 startingPos = item.transform.position;
        Vector3 norm = Vector3.zero;
        Vector3 handle = Vector3.zero;

        if (randomized && Random.value > 0.5f)
            norm = (destination - startingPos).PerpendicularClockwise().normalized;
        else
            norm = (destination - startingPos).PerpendicularCounterClockwise().normalized;

        if (curveType == CurveHandleType.MID)
            handle = (destination + startingPos) * 0.5f + (norm * CurveHandleDistance);
        else if (curveType == CurveHandleType.START)
            handle = startingPos + (norm * CurveHandleDistance);
        else if (curveType == CurveHandleType.END)
            handle = destination + (norm * CurveHandleDistance);

        while (elapsedTime < animationDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;

        while (elapsedTime < animTime)
        {
            item.transform.position = Bezier((elapsedTime / animTime), startingPos, handle, destination);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.position = destination;
        Audio.PlayOneShot(SndStop, 1);
    }

    IEnumerator MoveToPosition(Transform item, Vector3 destination, float animTime = 0.5f)
    {
        float elapsedTime = 0;
        Vector3 startingPos = item.transform.position;
        while (elapsedTime < animTime)
        {
            item.transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / animTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        item.transform.position = destination;
        Audio.PlayOneShot(SndStop, 1);
    }

    IEnumerator Wobble()
    {
        float speed = 0.5f;
        float variance = 0.05f;
        Vector3 axis = new Vector3(0, 0, 1);
        float rot = 0;

        while (true)
        {
            rot = variance * (Mathf.Cos(Mathf.PI * Time.fixedTime * speed));
            InstructionsText.transform.Rotate(axis, rot);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator WaitForInstruction()
    {
        IsPicking = true;
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider && hit.transform.gameObject.tag == "PiggyBank")
                {
                    this.PickedThisRound++;

                    CycleList((i) =>
                    {
                        if (i.sprite == hit.transform.gameObject && !i.picked && !i.held)
                        {
                            // Don't show what is in the first case picked.
                            if(Picked > 0)
                            {
                                GameObject display = ValueDisplays[i.contentValue];
                                Image img = display.GetComponent<Image>();
                                img.color = new Color(209, 211, 212);
                                Text txt = display.GetComponentInChildren<Text>();
                                txt.color = Color.red;
                                txt.fontStyle = FontStyle.Italic;
                            }
                            else
                            {
                                i.held = true;
                            }

                            i.picked = true;
                            Picked++;
                            IsPicking = false;
                            Audio.PlayOneShot(SndPick, 1);

                        }
                    });
                    
                    // Ignore if it wasn't valid
                    if(!IsPicking) yield break;
                }
            }
            yield return null;
        }
    }

    public double getOffer(bool adjusted = true)
    {
        double offer = 0;
        double count = 0;

        // Calculate middle weighted avg
        CycleList(i =>
        {
            if(i.picked)
            {
                count++;
                offer += Math.Pow(i.contentValue, 2);
            }
        });

        // no divide by zero err
        if (count <= 0) return 0;

        offer = Math.Sqrt(offer / count);
        double gamePct = count / MAX_CASES;

        if (adjusted)
            // 9 increments between 15% and 85% - 100% final case
            return offer * (0.15 + (((1 - 0.15) / 9) * (Picked / MAX_CASES)));
        else
            return offer;
    }

    IEnumerator BlockWait(float delayInSeconds, Action action)
    {
        yield return new WaitForSeconds(delayInSeconds);
        if (action != null) action.Invoke();

        print("wait completed");
    }
    #endregion
}
