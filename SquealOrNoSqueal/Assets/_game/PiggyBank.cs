using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Squeal/Assets")]
[System.Serializable]
public sealed class PiggyBank
{
    public int id = 0;
    public double contentValue = 0;
    public bool picked = false;
    public bool held = false;
    public GameObject sprite = null;

    public static PiggyBank CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PiggyBank>(jsonString);
    }
}
