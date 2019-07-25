using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 1f;
    [SerializeField] float mainThrust = 1f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelCompleteSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;
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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;

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
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("Restart", 1.8f);
    }

    private void StartFinishSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleteSound);
        levelCompleteParticles.Play();
        Invoke("LoadNextScene", 1.8f);
    }

    private void Restart()
    {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        state = State.Alive;
        SceneManager.LoadScene(1);
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
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);

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
