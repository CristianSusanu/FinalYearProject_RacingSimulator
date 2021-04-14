using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionMinus : MonoBehaviour
{
    public GameObject RacePositionDisplay;

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "CarPositionInRace")
        {
            RacePositionDisplay.GetComponent<Text>().text = "2";
        }
    }
}
