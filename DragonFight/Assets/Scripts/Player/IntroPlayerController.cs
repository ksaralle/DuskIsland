using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroPlayerController : MonoBehaviour
{

    // player's starting position:
    // pos: -31, 24.8, -40.8
    // rot: 0, 0, 0

    private int[] states;

    private GameObject Question;
    private GameObject Shock;
    private GameObject SwordLight;
    private GameObject Camera;

    private bool AudioFadeOutCalled;
    private AudioSource BGM;
    private AudioSource Breath;
    private bool breathHasPlayed;
    private AudioSource Thinking;
    private bool thinkingHasPlayed;
    private AudioSource Scream;
    private bool screamHasPlayed;

    private ParticleSystem Fire;

    // not sword's position, but the position where player picks up the sword
    private Vector3 Sword;
    

    private Vector3 stopPos;

    private bool isCameraUpdated;

    private float elapsedTime;
    private float timer;
    private float delay;
    private float tillLightOn;
    private float pickingUpSwordDuration;
    private float fireDuration;


    private Animator cr_Animator;

    

    void Awake()
    {
        cr_Animator = GetComponent<Animator>();

        states = new int[7];
        for (int i = 0; i < states.Length; i++)
        {
            if (i == 0)
            {
                states[i] = 1;
            }
            else
            {
                states[i] = 0;
            }

        }

        Question = GameObject.Find("Question");
        Shock = GameObject.Find("Shock");
        SwordLight = GameObject.Find("SwordLight");
        Camera = GameObject.Find("Main Camera");
        Fire = GameObject.Find("FireAttackVisual").GetComponent<ParticleSystem>();
        BGM = GameObject.Find("BGM").GetComponent<AudioSource>();
        Breath = GameObject.Find("OutofBreadth").GetComponent<AudioSource>();
        Thinking = GameObject.Find("Thinking").GetComponent<AudioSource>();
        //Scream = GameObject.Find("Scream").GetComponent<AudioSource>();
        
        breathHasPlayed = false;
        thinkingHasPlayed = false;
        screamHasPlayed = false;
        AudioFadeOutCalled = false;

        Sword = new Vector3(-30.85f, 25.3f, -35.5f);

        //Sword = new Vector3(-30.8f, 25.3f, -35.3f);


        stopPos = new Vector3(-30.56f, 24.6f, -37);

        isCameraUpdated = false;


        elapsedTime = 0;
        timer = 2.3f;
        delay = 1.3f;
        tillLightOn = 0.8f;
        pickingUpSwordDuration = 2f;
        fireDuration = 1.7f;
    }

    void Start()
    {
        Question.SetActive(false);
        Shock.SetActive(false);
        SwordLight.SetActive(false);
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (states[0] == 1)
            // in state 0 - frozen
        {
            return;
        }

        else if (states[1] == 1)
            // in state 1 - climbing
        {
            
           
            // move towards
            if (!breathHasPlayed)
            {
                Breath.Play();
                breathHasPlayed = true;
            }
            //Debug.Log("thinking..");
            transform.position = Vector3.MoveTowards(transform.position, stopPos, 0.7f * Time.deltaTime);
           if (transform.position == stopPos)
            {
                cr_Animator.SetTrigger("Stop");
                // transit to state 2
                Debug.Log("stop climbing");
                states[1] = 0;
                states[2] = 1;
            }

        } else if (states[2] == 1)
        {
            // in state 2 - in shock
            if (!AudioFadeOutCalled)
            {
                StartCoroutine(AudioFadeOut(BGM, 4));
                AudioFadeOutCalled = true;
            }

            // set UI element active
            if (elapsedTime <= timer)
            {
                if (elapsedTime <= delay)
                {
                    
                    if (elapsedTime <= tillLightOn)
                    {
                        // do nothing
                    } else
                    {
                        
                        SwordLight.SetActive(true);
                    }

                } else
                {
                    Shock.SetActive(true);
                }
                elapsedTime += Time.deltaTime;
            } else
            {
                // transit to state 3
                states[2] = 0;
                states[3] = 1;

                // turn off UI element
                Shock.SetActive(false);
            }
        }
            


        else if (states[3] == 1)
            // in state 3 - waiting for player to choose yes "Y"
        {
            
            Question.SetActive(true);
            
            if (!thinkingHasPlayed)
            {
                Thinking.Play();
                thinkingHasPlayed = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Y))
                // transit to state 4
            {
                Debug.Log("y pressed.");
                states[3] = 0;
                states[4] = 1;
               
                Question.SetActive(false);
                cr_Animator.SetTrigger("Start");
            }

        } else if (states[4] == 1)
            // in state 4 - player walking towards sword
        {
            
            transform.position = Vector3.MoveTowards(transform.position, Sword, 2f * Time.deltaTime);

            if (transform.position == Sword)
                // transit to state 5
            {
                states[4] = 0;
                states[5] = 1;
                elapsedTime = 0;
                if (!isCameraUpdated)
                {
                    Camera.GetComponent<IntroCamera>().swordIsPickedUp();
                    isCameraUpdated = true;
                }
                
            }

        } else if (states[5] == 1)
        {
            // in state 5 - animation for picking up the sword 
            
            cr_Animator.SetTrigger("Pick");
            if (elapsedTime <= pickingUpSwordDuration)
            {
                elapsedTime += Time.deltaTime;
            } else
            {
                states[5] = 0;
                states[6] = 1;
                elapsedTime = 0;
            }
            

        } else if (states[6] == 1)
        {
            // in state 6 - animation for after picking up the sword: fire consumes player
            Fire.Play();
            //if (!screamHasPlayed)
            //{
            //    Scream.Play();
            //    screamHasPlayed = true;
            //}
            if (elapsedTime <= fireDuration)
            {
                elapsedTime += Time.deltaTime;
            } else
            {
                SceneManager.LoadScene("BattleScene");
                Debug.Log("load scene: battlescene. ");
            }
        }
    }




    public void introIsOver()
    {
        states[0] = 0;
        states[1] = 1;
    }




    public IEnumerator AudioFadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }




}

