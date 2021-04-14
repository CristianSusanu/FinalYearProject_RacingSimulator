using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private LineRenderer line;
    private GameObject track;
    public GameObject car;
    public GameObject aiCar;
    public Transform carTr;
    public GameObject map;
    public GameObject pointerSphere;
    public GameObject oponentCarPointerSphere;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        track = this.gameObject;

        int nodeNumber = track.transform.childCount;
        line.positionCount = nodeNumber + 1;

        for(int i = 0; i < nodeNumber; i++)
        {
            line.SetPosition(i, new Vector3(track.transform.GetChild(i).transform.position.x, 7, track.transform.GetChild(i).transform.position.z));
        }

        line.SetPosition(nodeNumber, line.GetPosition(0));
        line.startWidth = 13f;
        line.endWidth = 13f;
    }

    // Update is called once per frame
    void Update()
    {
        map.transform.position = (new Vector3(car.transform.position.x, map.transform.position.y, car.transform.position.z));
        map.transform.rotation = Quaternion.Euler(90f, carTr.eulerAngles.y, 0f);

        pointerSphere.transform.position = (new Vector3(car.transform.position.x, pointerSphere.transform.position.y, car.transform.position.z));
        oponentCarPointerSphere.transform.position = (new Vector3(aiCar.transform.position.x, oponentCarPointerSphere.transform.position.y, aiCar.transform.position.z));
    }
}
