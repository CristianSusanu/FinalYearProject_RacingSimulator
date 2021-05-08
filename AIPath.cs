using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public Color lineCol;

    private List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineCol;

        Transform[] pathTransform = GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

        for(int i = 0; i < pathTransform.Length; i++)
        {
            if(pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }

        for(int i = 0; i < nodes.Count; i++)
        {
            Vector3 previousNodePosition = Vector3.zero;
            Vector3 currentNodePosition = nodes[i].position;

            if(i > 0)
            {
                previousNodePosition = nodes[i - 1].position;
            } else if(i == 0 && nodes.Count > 1)
            {
                previousNodePosition = nodes[nodes.Count - 1].position;
            }

            Gizmos.DrawLine(previousNodePosition, currentNodePosition);
            Gizmos.DrawWireSphere(currentNodePosition, 0.7f);
        }
    }
}
