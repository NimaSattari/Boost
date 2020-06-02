using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float MainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineP;
    [SerializeField] ParticleSystem successP;
    [SerializeField] ParticleSystem deathP;

    Rigidbody rigidbody;
    AudioSource audiosource;

    bool isTransitioning = false;

    bool collisionAreEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            ResponToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        
    }
    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            collisionAreEnabled = !collisionAreEnabled;
        }
    }

    void OnCollisionEnter(Collision collission)
    {
        if (isTransitioning || !collisionAreEnabled) { return; }
        switch (collission.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                isTransitioning = true;
                audiosource.Stop();
                audiosource.PlayOneShot(success);
                successP.Play();
                Invoke("LoadNextLevel", levelLoadDelay);
                break;
            default:
                isTransitioning = true;
                audiosource.Stop();
                audiosource.PlayOneShot(death);
                deathP.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;
        }
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audiosource.Stop();
            mainEngineP.Stop();
        }
    }
    void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up* MainThrust * Time.deltaTime);
            if (!audiosource.isPlaying)
            {
                audiosource.PlayOneShot(mainEngine);
            }
        mainEngineP.Play();
    }

    void ResponToRotateInput()
    {
        rigidbody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}
