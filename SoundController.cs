using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarControl))]
public class SoundController : MonoBehaviour
{
    public CarControl carControl;

    public AudioClip engine;
    public AudioClip engineIdle;
    public AudioClip engineReverse;
    public AudioClip gearShiftUp;
    public AudioClip gearShiftDown;
    public AudioClip backFire;
    public AudioClip turboSound;
    public AudioClip SidewaysSkidSound;
    public AudioClip ForwardSkidSound;

    AudioSource engineSource;
    AudioSource engineIdleSource;
    AudioSource engineReverseSource;
    AudioSource gearShiftUpSource;
    AudioSource gearShiftDownSource;
    AudioSource BackFireSource;
    AudioSource turboSoundSource;
    AudioSource sidewaysSkidSource;
    AudioSource forwardSkidSource;

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

    private AudioSource CreateTurboWhistleAudio(AudioClip audio)
    {
        GameObject sound = new GameObject(name);
        sound.transform.parent = transform;
        sound.transform.localPosition = Vector3.zero;
        sound.transform.localRotation = Quaternion.identity;
        sound.AddComponent(typeof(AudioSource));
        sound.GetComponent<AudioSource>().clip = audio;
        sound.GetComponent<AudioSource>().playOnAwake = true;
        sound.GetComponent<AudioSource>().priority = 0;
        sound.GetComponent<AudioSource>().loop = false;
        sound.GetComponent<AudioSource>().volume = 0.8f;
        sound.GetComponent<AudioSource>().spatialBlend = 1f;
        sound.GetComponent<AudioSource>().dopplerLevel = 0f;
        return sound.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        engineIdleSource = CreateAudio(engineIdle, "Engine Idle Audio");
        engineIdleSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineSource = CreateAudio(engine, "Engine Audio");
        engineSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);
        engineReverseSource = CreateAudio(engineReverse, "Engine Reverse Audio");
        engineReverseSource.transform.localPosition = new Vector3(0f, 0.5f, 1.3f);

        sidewaysSkidSource = CreateAudio(SidewaysSkidSound, "Skidding Audio");
        sidewaysSkidSource.volume = 0.4f;
        forwardSkidSource = CreateAudio(ForwardSkidSound, "Forward Skid Audio");
        forwardSkidSource.volume = 0.4f;

        gearShiftUpSource = CreateGearShiftAudio(gearShiftUp, "Gear Shift Up Audio");
        gearShiftUpSource.volume = 0.4f;
        gearShiftDownSource = CreateGearShiftAudio(gearShiftDown, "Gear Shift Down Audio");
        gearShiftDownSource.volume = 0.4f;

        BackFireSource = CreateGearShiftAudio(backFire, "Back Fire Audio");
        BackFireSource.volume = 0.8f;

        turboSoundSource = CreateTurboWhistleAudio(turboSound);
        turboSoundSource.volume = 0.7f;
    }

    public void PlayGearShiftUp()
    {
        gearShiftUpSource.Play();
    }

    public void PlayGearShiftDown()
    {
        gearShiftDownSource.Play();
    }

    public void PlayTurboWhistle()
    {
        turboSoundSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (carControl.inputManager.throttle == 0)
        {
            engineIdleSource.volume = 0.8f;
        }
        else if (carControl.inputManager.throttle > 0.0f)
        {
            engineSource.pitch = 0.1f + carControl.engineRPM / carControl.MaxEngineRPM;
            engineSource.volume = 0.1f + 0.5f * carControl.inputManager.throttle;
        }
        else if (carControl.inputManager.throttle < 0.0f)
        {
            engineReverseSource.pitch = 0.8f + carControl.engineRPM / carControl.MaxEngineRPM;
            engineReverseSource.volume = 0.3f - 2.0f * carControl.inputManager.throttle;
            engineSource.volume = 0.0f;
        }

        if (carControl.engineRPM > 7500f)
        {
            BackFireSource.Play();
        }

        SkidSound();
    }

    private void SkidSound()
    {
        //skid sound
        float skidAt = 150f;
        float skidForwardAt = 200f;
        float sidewaysFrictionValue = 0.0f;
        float forwardFrictionValue = 0.0f;
        WheelHit hit;

        foreach (WheelCollider wheel in carControl.wheels)
        {
            wheel.GetGroundHit(out hit);
            sidewaysFrictionValue += Mathf.Abs(hit.sidewaysSlip);
            forwardFrictionValue += Mathf.Abs(hit.forwardSlip);
        }

        sidewaysFrictionValue = sidewaysFrictionValue / 4;
        forwardFrictionValue = forwardFrictionValue / 4;
        float skidVolume = Mathf.Clamp01(Mathf.Abs(sidewaysFrictionValue) * 0.15f - 0.95f);
        float skidForwardVolume = Mathf.Clamp01(Mathf.Abs(forwardFrictionValue) * 0.15f - 0.95f);

        sidewaysSkidSource.volume = sidewaysFrictionValue >= skidAt ? skidVolume : 0f;
        if (carControl.inputManager.brake)
        {
            forwardSkidSource.volume = forwardFrictionValue >= skidForwardAt ? skidForwardVolume : 0f;
        }
        else forwardSkidSource.volume = 0;
    }
}
