using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingLane : MonoBehaviour
{
    public Transform CL;
    private int Size;
    public Vector3 Length;
    private string ChargingLaneSizeSaved = "ChargingLaneSize";

    void Start()
    {
        LoadMenudata();
        ChangeLength();
    }

    public void LoadMenudata()
    {
        Size = PlayerPrefs.GetInt(ChargingLaneSizeSaved, 5);
    }

    public void ChangeLength()
    {
        Length = CL.transform.localScale;
        Length.x = Length.x * Size;
        transform.localScale = Length;
    }
}
