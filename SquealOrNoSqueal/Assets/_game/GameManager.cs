using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    [Serializable]
    public class BriefCase
    {
        public double Value = 0;
        public bool Picked = false;
        public bool Held = false;

        public void Reset()
        {
            this.Value = 0;
            this.Picked = false;
            this.Held = false;
        }
    }

    public const int MAX_CASES = 26;
    public static GameManager instance = null;

    public double Expected = 0;
    public double Offer = 0;

    public Text UI_Expected;
    public Text UI_Offer;

    public int CurrentPick = 0;
    public GameObject BriefcaseTemplate;
    public List<BriefCase> BriefcaseList = new List<BriefCase>(MAX_CASES);

    void Awake()
    {
        kgLogger.instance.Post(kgLogger.PostChannel.INFO, "Initializing GameManager", kgLogger.PostLevel.verbose);
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        InitGame();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CycleList(Action<int> toDo)
    {
        if (toDo == null) return;
        for(int x = 0; x < MAX_CASES; x++)
        {
            toDo.Invoke(x);
        }
    }

    public void Refresh()
    {
        Expected = getOffer(false);
        Offer = getOffer(true);
        RefreshText();
    }

    private void RefreshText()
    {
        UI_Expected.text = Expected.ToString("C2");
        UI_Offer.text = Offer.ToString("C2");
    }

    public double getOffer(bool adjusted = true)
    {
        double offer = 0;
        double count = 0;

        // Calculate middle weighted avg
        CycleList(i =>
        {
            if(!BriefcaseList[i].Picked)
            {
                count++;
                offer += Math.Pow(BriefcaseList[i].Value, 2);
            }
        });

        // no divide by zero
        if(count > 0) offer = Math.Sqrt(offer / count);
        double gamePct = count / MAX_CASES;

        if (adjusted)
            // 9 increments between 15% and 85% - 100% final case
            return offer * (0.15 + (((1 - 0.15) / 9) * getOfferNumber(((int)MAX_CASES - 1) - (int)count)));
        else
            return offer;
    }

    public double getOfferNumber(int picks)
    {
        int offer = 0;
        switch(picks)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                offer = 0;
                break;
            case 7:
            case 8:
            case 9:
            case 10:
                offer = 1;
                break;
            case 11:
            case 12:
            case 13:
            case 14:
                offer = 2;
                break;
            case 15:
            case 16:
            case 17:
                offer = 3;
                break;
            case 18:
            case 19:
                offer = 4;
                break;
            case 20:
                offer = 5;
                break;
            case 21:
                offer = 6;
                break;
            case 22:
                offer = 7;
                break;
            case 23:
                offer = 8;
                break;
            case 24:
                offer = 9;
                break;
            case 25:
                offer = 10;
                break;
            case 26:
                offer = 11;
                break;
        }
        return offer;
    }

    void InitGame()
    {
        kgLogger.instance.Post(kgLogger.PostChannel.INFO, "Initializing Cases", kgLogger.PostLevel.verbose);

        CurrentPick = 0;
        Expected = 0;
        Offer = 0;
        RefreshText();

        Queue<double> amounts = new Queue<double>(new[] {
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
        });

        CycleList(i =>
        {
            BriefCase bc = new BriefCase();
            bc.Reset();
            bc.Value = amounts.Dequeue();
            BriefcaseList.Add(bc);
        });
    }
}
