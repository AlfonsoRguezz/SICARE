using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public Color greenColor;
    public Color redColor;
    public Color amberColor;
    public Renderer render;
    public float nextActionTime = 0.0f;
    public float timeGreen = 5f;
    public float timeRed = 5f;
    public float timeAmber = 1f;
    private float period;

    private void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > nextActionTime)
        {
            period = timeRed + timeGreen + timeAmber;
            Color();
            nextActionTime= Time.timeSinceLevelLoad + period;
        }
    }

    private void Color()
    {
        render = GetComponent<Renderer>();
        StartCoroutine(waiter());
    }
    IEnumerator waiter()
    {

        render.material.color = amberColor;
        yield return new WaitForSeconds(timeAmber);

        this.GetComponent<BoxCollider>().enabled = true;
        render.material.color = redColor;
        yield return new WaitForSeconds(timeRed);

        this.GetComponent<BoxCollider>().enabled = false;
        render.material.color = greenColor;
        yield return new WaitForSeconds(timeGreen);
    }
}
