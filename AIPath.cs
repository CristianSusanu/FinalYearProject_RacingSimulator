using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public Color lineCol;

    private List<Transform> nodes = new List<Transform>();//only contains the child nodes, not itself

    //private void OnDrawGizmosSelected()//the function will be executed only when the game object is selected
    private void OnDrawGizmos()//schimb cu cea de sus cand termin, ca sa apara pathul doar cand selected
    {
        Gizmos.color = lineCol;

        Transform[] pathTransform = GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();//to make sure the list is empty at the beginning

        for(int i = 0; i < pathTransform.Length; i++)
        {
            if(pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);//if it's not our own transform, it adds it to the nodes array
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

            Gizmos.DrawLine(previousNodePosition, currentNodePosition); //to draw the line between the points
            Gizmos.DrawWireSphere(currentNodePosition, 0.7f);//to see the nodes on the circuit
        }
    }
}
