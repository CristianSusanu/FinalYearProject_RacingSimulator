using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISound : MonoBehaviour
{
    public Rigidbody rigidB;
    public AIController aIController;

    public AudioClip engine;
    public AudioClip engineIdle;
    public AudioClip engineReverse;

    AudioSource engineSource;
    AudioSource engineIdleSource;
    AudioSource engineReverseSource;

    private AudioSource CreateAudio(AudioClip audio, string name)
    {
        GameObject sound = new GameObject(name);
        sound.transform.parent = transform;
        sound.transform.localPosition = Vector3.zero;
        sound.transform.localRotation = Quaternion.identity;
        sound.AddComponent(typeof(AudioSource));
        sound.GetComponent<AudioSource>().clip = audio;
        sound.GetComponent<AudioSource>().loop = true;
        sound.GetComponent<AudioSource>().volume = 0.2f;
        sound.GetComponent<AudioSource>().spatialBlend = 1f;
        sound.GetComponent<AudioSource>().dopplerLevel = 0f;
        sound.GetComponent<AudioSource>().Play();
        return sound.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        
        engineIdleSource = CreateAudio(engineIdle, "Engine Idle Audio");
        engineIdleSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineSource = CreateAudio(engine, "Engine Audio");
        engineSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineReverseSource = CreateAudio(engineReverse, "Engine Reverse Audio");
        engineReverseSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            engineIdleSource.volume = 0.8f;
        }
        else if (Input.GetAxis("Vertical") > 0.0f)
        {
            engineSource.pitch = 0.07f + aIController.engineRPM / aIController.maxEngineRPM;
            engineSource.volume = 0.07f + 0.5f * Input.GetAxis("Vertical");
        }
        else if (Input.GetAxis("Vertical") < 0.0f)
        {
            engineReverseSource.pitch = 0.6f + aIController.engineRPM / aIController.maxEngineRPM;
            engineReverseSource.volume = 0.3f - 2.0f * Input.GetAxis("Vertical");
            engineSource.volume = 0.0f;
        }
    }
}