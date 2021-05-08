using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    public List<Light> lights;
    public List<GameObject> tailLight;
    public List<GameObject> dashboard;
    public GameObject leftHeadLight;
    public GameObject rightHeadLight;

    private float headlightMinAngle = -90f;
    private float headlightMaxAngle = -158f;

    public virtual void OnOffHeadLights()
    {
        foreach (Light light in lights)
        {
            if(light.intensity == 0)
            {
                light.intensity = 4;
                light.range = 45;

                //headlight pop-up for white Trueno
                if (gameObject.name.Equals("AE86Trueno(Clone)"))
                {
                    leftHeadLight.transform.localEulerAngles = new Vector3(Mathf.Clamp(0f, headlightMinAngle, headlightMaxAngle), 0f, Time.deltaTime * 20);
                    rightHeadLight.transform.localEulerAngles = new Vector3(Mathf.Clamp(0f, headlightMinAngle, headlightMaxAngle), 0f, Time.deltaTime * 20);
                }
            } else
            {
                light.intensity = 0;

                //headlight pop-down for white Trueno
                if (gameObject.name.Equals("AE86Trueno(Clone)"))
                {
                    leftHeadLight.transform.localEulerAngles = new Vector3(Mathf.Clamp(headlightMinAngle, headlightMaxAngle, 0f), 0f, Time.deltaTime * 20);
                    rightHeadLight.transform.localEulerAngles = new Vector3(Mathf.Clamp(headlightMinAngle, headlightMaxAngle, 0f), 0f, Time.deltaTime * 20);
                }
            }
        }

        foreach(GameObject dash in dashboard)
        {
            if (dash.GetComponent<Renderer>().material.GetColor("_EmissionColor") == Color.white)
            {
                dash.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.grey);
            }
            else
            {
                dash.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
            }
        }

        foreach (GameObject tLight in tailLight)
        {
            if(tLight.GetComponent<Renderer>().material.GetColor("_EmissionColor") == Color.black)
            {
                tLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            }
            else
            {
                tLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
