using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private List<Transform> node = new List<Transform>();

    void OnDrawGizmosSelected()
    {
        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        node = new List<Transform>();

        for(int i = 0; i < pathTransform.Length; i++)
        {
            if(pathTransform[i] != transform)
            {
                node.Add(pathTransform[i]);
            }
        }
        for(int i=0; i<node.Count; i++)
        {
            Vector3 currentNode = node[i].position;
            Vector3 previousNode= Vector3.zero;
            if (i > 0)
            {
                previousNode = node[i - 1].position;
            }
            else if(i==0 && node.Count > 1)
            {
                previousNode = node[node.Count - 1].position;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.3f);
        }
    }
}
