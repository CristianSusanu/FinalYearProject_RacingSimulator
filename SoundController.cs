using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarControl))]
public class SoundController : MonoBehaviour
{
    public CarControl carControl;

    //private float carSlip = 0.0f;

    public AudioClip engine;
    public AudioClip engineIdle;
    public AudioClip engineReverse;
    public AudioClip skid;
    public AudioClip gearShiftUp;
    public AudioClip gearShiftDown;
    public AudioClip backFire;

    AudioSource engineSource;
    AudioSource engineIdleSource;
    AudioSource engineReverseSource;
    AudioSource skidSource;
    AudioSource gearShiftUpSource;
    AudioSource gearShiftDownSource;
    AudioSource BackFireSource;

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

    private AudioSource CreateGearShiftAudio(AudioClip audio, string name)
    {
        GameObject sound = new GameObject(name);
        sound.transform.parent = transform;
        sound.transform.localPosition = Vector3.zero;
        sound.transform.localRotation = Quaternion.identity;
        sound.AddComponent(typeof(AudioSource));
        sound.GetComponent<AudioSource>().clip = audio;
        sound.GetComponent<AudioSource>().loop = false;
        sound.GetComponent<AudioSource>().volume = 0.75f;
        sound.GetComponent<AudioSource>().spatialBlend = 1f;
        sound.GetComponent<AudioSource>().dopplerLevel = 0f;
        return sound.GetComponent<AudioSource>();
    }
/*
    public float skidSound()
    {
        WheelHit hit;

        foreach(WheelCollider wheel in carControl.wheels)
        {
            bool grounded = wheel.GetGroundHit(out hit);
            if (grounded)
            {
                carSlip += hit.sidewaysSlip / 4;
            }
        }
        return carSlip;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        engineSource = CreateAudio(engine, "Engine Audio");
        engineSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineIdleSource = CreateAudio(engineIdle, "Engine Idle Audio");
        engineIdleSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineReverseSource = CreateAudio(engineReverse, "Engine Reverse Audio");
        engineReverseSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);

        //skidSource = CreateAudio(skid, "Skidding Audio");

        gearShiftUpSource = CreateGearShiftAudio(gearShiftUp, "Gear Shift Up Audio");
        gearShiftUpSource.volume = 0.4f;
        gearShiftDownSource = CreateGearShiftAudio(gearShiftDown, "Gear Shift Down Audio");
        gearShiftDownSource.volume = 0.4f;

        BackFireSource = CreateGearShiftAudio(backFire, "Gear Change Backfire");
        BackFireSource.volume = 1.2f;
    }

    public void PlayGearShiftUp()
    {
        gearShiftUpSource.Play();
    }

    public void PlayGearShiftDown()
    {
        gearShiftDownSource.Play();
    }

    public void PlayBackFire()
    {
        BackFireSource.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(carControl.inputManager.throttle > 0.0f && carControl.engineRPM > 1050f)
        {
            engineSource.pitch = 0.2f + carControl.engineRPM / carControl.MaxEngineRPM;
            engineSource.volume = 0.3f + 0.65f * carControl.inputManager.throttle;
        }
        else if (carControl.inputManager.throttle < 0.0f)
        {
            engineReverseSource.pitch = 0.2f + carControl.engineRPM / carControl.MaxEngineRPM;
            engineReverseSource.volume = 0.3f + 0.65f * carControl.inputManager.throttle;
        }
        else
        {
            //engineIdleSource.pitch = 0.15f + carControl.engineRPM / carControl.MaxEngineRPM;
            engineIdleSource.pitch = 0.15f;
            engineIdleSource.volume = 0.3f;
        }

        if(carControl.engineRPM >= 7500f)
        {
            PlayBackFire();
        }

        //float skidVolume = Mathf.Clamp01(Mathf.Abs(skidSound()) * 0.15f - 0.95f);
        //skidSource.volume = Mathf.Clamp01(Mathf.Abs(skidSound()));
    }
}
