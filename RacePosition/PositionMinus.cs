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
            if (transform.root.gameObject.name == "GT86(1)")
            {
                RacePositionDisplay.GetComponent<Text>().text = "2";
            }
            else if (transform.root.gameObject.name == "GT86(2)")
            {
                RacePositionDisplay.GetComponent<Text>().text = "3";
            }
            else if (transform.root.gameObject.name == "GT86(3)")
            {
                RacePositionDisplay.GetComponent<Text>().text = "4";
            }
        }
    }
}
