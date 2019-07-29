using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 1.8f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelCompleteSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;

    BoxCollider hitBox;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    bool isKillable = true;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Dying || state == State.Transcending) { return; }

        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();     
        }
        else
        {
            audioSource.Stop();
        }

        if(Debug.isDebugBuild)
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LoadNextScene();

        else if (Input.GetKeyDown(KeyCode.C))
            isKillable = !isKillable;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !isKillable) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        if(isKillable == true)
        {
            state = State.Dying;
            audioSource.Stop();
            audioSource.PlayOneShot(deathSound);
            deathParticles.Play();
            Invoke("Restart", levelLoadDelay);
        }      
    }

    private void StartFinishSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleteSound);
        levelCompleteParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void Restart()
    {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)        
            nextSceneIndex = 0;
        
            state = State.Alive;
            SceneManager.LoadScene(nextSceneIndex);             
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) ApplyThrust();
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        } 
    }

    private void ApplyThrust()
    {      
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSound);
            mainEngineParticles.Play();
        }       
    }

    private void RespondToRotateInput()
    {
            rigidBody.freezeRotation = true;
            float rotationThisFrame = rcsThrust * Time.deltaTime;

            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * rotationThisFrame);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(-Vector3.forward * rotationThisFrame);
            }

            rigidBody.freezeRotation = false;      
    }

}
