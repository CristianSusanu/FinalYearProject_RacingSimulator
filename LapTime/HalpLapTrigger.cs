using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalpLapTrigger : MonoBehaviour
{
    public GameObject fullLapTrigger;
    public GameObject halfLapTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            fullLapTrigger.SetActive(true);
            halfLapTrigger.SetActive(false);
        }
    }
}
