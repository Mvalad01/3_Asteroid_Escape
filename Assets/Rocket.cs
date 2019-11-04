using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody body;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float thrustPower = 20f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem succesParticles;
    [SerializeField] ParticleSystem DeadParticles;
    [SerializeField] float levelLoadDelay = 2f;
    enum State { Alive, Dying, Trasending }
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }
    private void ProcessInput()
    {
        if (state == State.Alive)
        {
            RespondToThrust();
            RespondToControl();
        }
    }
    private void RespondToControl()
    {

        body.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        body.freezeRotation = false;
    }
    private void RespondToThrust()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            if (state != State.Trasending)
                audioSource.Stop();
                mainEngineParticles.Stop();
            
        }
    }

    private void ApplyThrust()
    {
        body.AddRelativeForce(Vector3.up * thrustPower*Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                FinishSequence();
                break;
            default:
                FailSequence();
                break;
        }
    }

    private void FailSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deadSound);
        DeadParticles.Play();
        state = State.Dying;
        Invoke("LoadFirstScene", levelLoadDelay);
    }

    private void FinishSequence()
    {

        audioSource.PlayOneShot(winSound, 1);
        succesParticles.Play();
        state = State.Trasending;
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }
}
