using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yield : MonoBehaviour
{
    public bool presence = false;
    public Color redColor;
    public Color greenColor;
    public Color invisibleColor;
    public Collider Collider;
    public Renderer Yieldrend;
    public Renderer Presencerend;

    private void OnTriggerStay(Collider other)
    {
        presence = true;
    }
    private void OnTriggerExit(Collider other)
    {
        presence = false;
    }

    private void FixedUpdate()
    {
        Presencerend = GetComponent<Renderer>();
        if (presence)
        {
            Yieldrend.material.color = redColor;
            Collider.enabled = true;
            Presencerend.material.color = greenColor;
        }
        else if(presence==false)
        {
            Collider.enabled = false;
            Yieldrend.material.color = invisibleColor;
            Presencerend.material.color = invisibleColor;
        }
    }
}