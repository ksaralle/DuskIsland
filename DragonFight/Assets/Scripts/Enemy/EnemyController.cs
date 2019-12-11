using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemyController : MonoBehaviour
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("Enemy's maximum health.")]
    private int m_Maxhealth;

    [SerializeField]
    [Tooltip("Health threshold for desperation attack.")]
    private int m_threshold;

    [SerializeField]
    [Tooltip("Health thresholds for fire attack")]
    private float[] m_FireThresHolds;

    [SerializeField]
    [Tooltip("Stomp Attack Cooldown time.")]
    private float m_StompCDInSec;

    [SerializeField]
    [Tooltip("How long should Stomp Attack last in Seconds.")]
    private float m_StompAttackLasting;

    [SerializeField]
    [Tooltip("Stomp Attack Damage per 0.5 Second.")]
    private float m_StompAttackDamage;

    [SerializeField]
    [Tooltip("Basic Attack Damage per strike.")]
    private float m_BasicAttackDamage;

    [SerializeField]
    [Tooltip("Desperation Attack Damage. ")]
    private float m_DesperationAttackDamage;

    [SerializeField]
    [Tooltip("Desperation Attack lasting in seconds. ")]
    private float m_DesperationLifeSpan;

    [SerializeField]
    [Tooltip("Fire breath attack damage")]
    private float m_FireAttackDamage;

    [SerializeField]
    [Tooltip("Fire attack lasting in seconds")]
    private float m_FireAttackDuration;

    [SerializeField]
    [Tooltip("The speed at which Dragon chases the player.")]
    private float m_DragonChasingSpeed;

    [SerializeField]
    [Tooltip("the HUD script")]
    private HUDController m_HUD;

    [SerializeField]
    [Tooltip("Health percentage of the dragon")]
    private Text HealthPercent;

    #endregion

    #region Private Variables
    // the current health of the game object
    private float CurHealth;

    // the max health of the dragon
    private float MaxHealth;

    // the player
    private GameObject PlayerObject;

    // if the dragon spots the player or not
    private bool Hostile;

    // health threshold for desperation attack
    private float HealthThreshold;

    // Health Thresholds for fire attack
    private float[] FireHealthThresholds;

    // Counter for each fire attack health threshholds
    private int FireCounter;

    // stomp attack cool down time
    private float StompAttackRemainingCDTime;

    // basic attack
    private GameObject BasicAttackObject;

    // stomp attack
    private GameObject StompAttackObject;

    // range in which dragon can spot the player
    private GameObject HostileRange;

    // if an attack is already activated;
    private bool AttackActivated;

    // if stomp attack is on
    private bool StompAttackOn;

    // if basic attack is on
    private bool BasicAttackOn;

    // if desperation attack is on
    private bool DesperationAttackOn;

    // if fire attack is on
    private bool FireAttackOn;

    // if within Stomp attack's range
    private bool StompAttackInRange;

    // if within basic attack's range
    public bool BasicAttackInRange;

    // if within desperation attack's range
    private bool DesperationAttackInRange;

    //if within Fire attack's range
    private bool FireAttackInRange;

    // stomp attack damage
    private float StompAttackDamage;

    // basic attack damage
    private float BasicAttackDamage;

    // desperation attack damage
    private float DesperationAttackDamage;

    // desperation attack lasting time
    private float DesperationAttackLifeSpan;

    // Fire attack damage
    private float FireAttackDamage;

    //Fire attack duration
    private float FireAttackDuration;

    // stomp attack windup time
    private float StompAttackWindUpTime = 1;

    // stomp attack frozen time
    private float StompAttackFrozenTime = 1;

    // stomp attack lasting seconds
    private float StompAttackLasting;

    // dragon chasing player speed
    private float ChaseSpeed;

    // basic attack coroutine
    private Coroutine BasicAttackCoroutine;

    // stomp attack coroutine
    private Coroutine StompAttackCoroutine;

    // desperation attack coroutine
    private Coroutine DesperationAttackCoroutine;

    // fire attack coroutine
    private Coroutine FireAttackCoroutine;
    #endregion

    #region Cached Component
    // the particle system of the stomp attack visual effect
    private ParticleSystem cc_ParticleSystem;

    // desperation attack visual
    private ParticleSystem cc_DesperationAttackVisual;

    //Fire attack visual
    private ParticleSystem cc_FireAttackVisual;

    // the body of the dragon
    private MeshCollider cc_MeshCollider;

    // the radius in which dragon can sense the presence of player
    private SphereCollider cc_SphereCollider;

    // rigidbody
    private Rigidbody cc_Rigidbody;

    // dragon get hit sound effects
    private AudioSource cc_DragonGetHitSound;

    // dragon basic attack sound effects
    private AudioSource cc_DragonBasicAttackSound;

    // dragon stomp attack sound effects
    private AudioSource cc_DragonStompAttackSound;

    // dragon desperation attack sound effects
    private AudioSource cc_DragonDesperationAttackSound;

    // dragon desperation attack warning sound
    private AudioSource cc_DragonDesperationAttackWarningSound;

    // whether desperation attack is used
    private bool isDesperationAttackUsed;


    // dragon animator controller 
    private Animator cr_Anim;
    #endregion


    #region Initialization
    private void Awake()
    {
        // get sound effects audio
        cc_DragonGetHitSound = GameObject.Find("DragonGetHit").GetComponent<AudioSource>();

        // get sound effects audio
        cc_DragonBasicAttackSound = GameObject.Find("DragonBasicAttackSoundEffects").GetComponent<AudioSource>();

        // get sound effects audio
        cc_DragonStompAttackSound = GameObject.Find("DragonStompAttackSoundEffects").GetComponent<AudioSource>();

        // get sound effects audio
        cc_DragonDesperationAttackSound = GameObject.Find("DragonDesperationAttackSoundEffects").GetComponent<AudioSource>();

        // get sound effects audio
        cc_DragonDesperationAttackWarningSound = GameObject.Find("DragonDesperationAttackWarningSoundEffects").GetComponent<AudioSource>();

        // initially no attack
        BasicAttackCoroutine = null;

        // initially no attack
        StompAttackCoroutine = null;

        // initially no attack
        DesperationAttackCoroutine = null;

        // initially no attack
        FireAttackCoroutine = null;

        // initially dragon roams around randomly; not hostile
        Hostile = false;

        // initialize status
        BasicAttackOn = false;

        // initialize status
        StompAttackOn = false;

        // initialize status
        DesperationAttackOn = false;

        FireAttackOn = false;

        // initialize status
        AttackActivated = false;

        // initialize current health to max health at the start of the game
        CurHealth = m_Maxhealth;

        // Get the max health;
        MaxHealth = m_Maxhealth;

        // Initialize fire counter
        FireCounter = 0;

        // get desperation attack's particle system
        cc_DesperationAttackVisual = GameObject.Find("DragonDesperationAttackVisual").GetComponent<ParticleSystem>();

        // get fire attack's particle system
        cc_FireAttackVisual = GameObject.Find("FireAttackVisual").GetComponent<ParticleSystem>();
        //cc_FireAttackVisual.Stop();

        // get particle system component
        // cc_ParticleSystem = GetComponent<ParticleSystem>();

        // get mesh collider component
        cc_MeshCollider = GetComponent<MeshCollider>();

        // get sphere collider
        cc_SphereCollider = GetComponent<SphereCollider>();

        // get rigidbody
        cc_Rigidbody = GetComponent<Rigidbody>();

        // player (as a game object)
        PlayerObject = GameObject.Find("Player");

        // setting health threshold for desperation attack
        HealthThreshold = m_threshold;

        // setting stomp attack's remaining cooldown time
        StompAttackRemainingCDTime = 0f;

        // find the basic attack object / collider
        BasicAttackObject = GameObject.Find("BasicAttack");

        // find the stomp attack object / collider
        StompAttackObject = GameObject.Find("StompAttack");

        // find the hostile range object / collider
        HostileRange = GameObject.Find("Hostile");

        // initially attack is off
        AttackActivated = false;

        // initiallly not in stomp attack's range
        StompAttackInRange = false;

        // initiallly not in stomp attack's range
        BasicAttackInRange = false;

        // initiallly not in desperation attack's range
        DesperationAttackInRange = false;

        // initially no in fire attack's range
        FireAttackInRange = false;

        // initialize stomp attack's damage
        StompAttackDamage = m_StompAttackDamage;

        // initialize basic attack's damage
        BasicAttackDamage = m_BasicAttackDamage;

        // initialize desperation attack's damage
        DesperationAttackDamage = m_DesperationAttackDamage;

        // initialize fire attack's damage
        FireAttackDamage = m_FireAttackDamage;

        // desperation attack's lifespan
        DesperationAttackLifeSpan = m_DesperationLifeSpan;

        // stomp attack lasting in seconds
        StompAttackLasting = m_StompAttackLasting;

        // Fire attack lasting in seconds
        FireAttackDuration = m_FireAttackDuration;

        // dragon chasing player speed
        ChaseSpeed = m_DragonChasingSpeed;

        // initialize bool
        isDesperationAttackUsed = false;


        // initialize fire health thresholds
        FireHealthThresholds = new float[9];
        for (int i = 0; i < 9; i++)
        {
            FireHealthThresholds[i] = 1.0f - 0.1f * (i + 1);
        }

        // init animator controller 
        cr_Anim = GetComponent<Animator>();

        cr_Anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    

    #endregion
    // Update is called once per frame
    #region Update Timer

    // Update Stomp Attack Remaining CD
    void UpdateStompAttackRemainingCD()
    {
        // cooling down
        if (StompAttackRemainingCDTime > 0)
        {
            StompAttackRemainingCDTime -= Time.deltaTime;
        }
        else
        {
            StompAttackRemainingCDTime = 0;
        }
    }
    #endregion

    #region Main Updates

    void Update()
    {
        // update attack method's cooldown time
        UpdateStompAttackRemainingCD();
        updateHealthPercentage();

        //Debug.Log("current health of dragon: " + CurHealth);

        // if the dragon spots the player in vicinity
        if (Hostile)
        {
            //Debug.Log("why isnt dragon triggered");
            //disable wander when player in hostile range
            if (GetComponent<Wander>().enabled)
            {
                //cr_Anim.SetBool("wander", false);
                GetComponent<Wander>().enabled = false;
            }
            // dragon's rotation
            if (PlayerObject.GetComponent<PlayerController>().IsGrounded && !AttackActivated)
            {
                //cr_Anim.SetBool("walking", true);
                // update dragon's rotation
                Vector3 newRot = new Vector3(PlayerObject.transform.position.x - transform.position.x, 0, PlayerObject.transform.position.z - transform.position.z);
                Quaternion LookRotation = Quaternion.LookRotation(newRot);
                transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, 0.2f);
            }
            // priority : desperation > fire > stomp > basic
            if (AttackActivated)
            {
                if (BasicAttackOn)
                {
                    // ??
                }
            }
            else
            // -> !attackactivated
            {
                if (ShouldActivateDesperationAttack())
                {
                    StartDesperationAttack();
                }
                else if (ShouldActivateFireAttack())
                {
                    StartFireAttack();
                }
                else if (ShouldActivateStompAttack())
                {
                    StartStompAttack();
                }
                else
                // not yet desperation / fire attack
                // not within range of stomp attack
                {
                    if (BasicAttackInRange)
                    {
                        if (!BasicAttackOn)
                        {
                            StartBasicAttack();
                        }
                        
                    }
                    else
                    {
                        if (PlayerObject.GetComponent<PlayerController>().IsGrounded)
                        {
                            Vector3 destination = new Vector3(PlayerObject.transform.position.x, (float)(PlayerObject.transform.position.y + 1), PlayerObject.transform.position.z);

                            transform.position = Vector3.MoveTowards(transform.position, destination, ChaseSpeed * Time.deltaTime);
                        }
                    }
                }
            }
        }
        else
        // when player is not in hostile range / left the hostile range
        {
            // desperation attack on: do nothing
            // desperation attack off: ->
            if (!AttackActivated)
            {
                //enable wander
                if (!GetComponent<Wander>().enabled)
                {
                    GetComponent<Wander>().enabled = true;

                }
            }
        }
    }
    #endregion

    #region Mesh Collider Methods
    // player having physical contact with dragon's mesh
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerObject)
        {
            if (PlayerObject.GetComponent<PlayerController>().IsGrounded)
            {
                
                if (!Hostile)
                {
                    ChangeHostilityStatus(true);
                }

            }
        }
    }
    #endregion

    #region Dragon's Attack Methods
    // check if stomp attack ability is still cooling down
    bool isStompAttackReady()
    {
        return StompAttackRemainingCDTime <= 0;
    }

    // start stomp attack
    void StartStompAttack()
    {

        // start coroutine
        Debug.Log("do attack");

        //cr_Anim.SetBool("walking", false);
        cr_Anim.SetTrigger("stompatk");
        StompAttackCoroutine = StartCoroutine(StompAttack());

        // set bool vars
        AttackActivated = true;
        StompAttackOn = true;
    }

    // abrupt stop of stomp attack
    void AbortStompAttack()
    {

        // stop coroutine
        StopCoroutine(StompAttackCoroutine);
        // set bool vars
        AttackActivated = false;
        StompAttackOn = false;


    }

    // start basic attack
    void StartBasicAttack()
    {
        // set bool vars
        cc_DragonBasicAttackSound.Play();

        //cr_Anim.SetBool("walking", false);
        cr_Anim.SetTrigger("basicatk");

        AttackActivated = true;
        BasicAttackOn = true;

        // start coroutine
        BasicAttackCoroutine = StartCoroutine(BasicAttack());

        // display visual effect
        // ???

    }
    // abrupt stop of basic attack
    void AbortBasicAttack()
    {
        // set bool vars
        AttackActivated = false;
        BasicAttackOn = false;
        // stop coroutine
        StopCoroutine(BasicAttackCoroutine);
    }



    void StartDesperationAttack()
    {
        // stop all other attacks
        if (AttackActivated)
        {
            if (BasicAttackOn)
            {
                AbortBasicAttack();
            }
            if (StompAttackOn)
            {
                AbortStompAttack();
            }


        }
        if (!DesperationAttackOn)
        {

            //cr_Anim.SetBool("walking", false);
            cr_Anim.SetTrigger("desperationatk");

            DesperationAttackCoroutine = StartCoroutine(DesperationAttack());
        }

    }

    void StartFireAttack()
    {
        //cr_Anim.SetBool("walking", false);
        cr_Anim.SetTrigger("fireatk");

        AttackActivated = true;
        FireAttackOn = true;
        FireCounter += 1;
        FireAttackCoroutine = StartCoroutine(FireAttack());
    }

    //// stomp attack frozen time; not ready for mvp
    //IEnumerator StompAttackFrozenTimeEffect()
    //{
    //    yield return new WaitForSeconds(StompAttackFrozenTime);

    //}

    // stomp attack
    IEnumerator StompAttack()
    {
        // windup time
        gameObject.GetComponentInChildren<StompAttackController>().showWarning();

        cc_DragonStompAttackSound.Play();
        yield return new WaitForSeconds(StompAttackWindUpTime);

        // display visual effect
        //cc_ParticleSystem.Play(true);
        // reset CD

        StompAttackRemainingCDTime = m_StompCDInSec;

        // setting the timer for how long stomp attack should last
        //float timer = StompAttackLasting;
        //float elapsedTime = 0;
        // stomp attack loop
        //while(elapsedTime <= timer)
        //{
        // damage player's health frame of a proportional amount


        if (StompAttackInRange)
        {
            Debug.Log("do stomp damage");
            // kinda stun
            PlayerObject.GetComponent<PlayerController>().playerAddForce(Vector3.up * 500);
            //PlayerObject.GetComponent<PlayerController>().changeCanMoveStatus(false);

            // do damage
            DecreasePlayerHealth(StompAttackDamage);
        }
        //Debug.Log("stomp attack: "+PlayerObject.GetComponent<PlayerController>().getHealth());
        //elapsedTime += Time.deltaTime;
        //yield return null;
        // }

        // frozen time

        yield return new WaitForSeconds(StompAttackFrozenTime);
        gameObject.GetComponentInChildren<StompAttackController>().hideWarning();
        // attack is over, set the bool vars to false
        StompAttackOn = false;
        AttackActivated = false;
        // stop the visual effect
        //cc_ParticleSystem.Stop();

    }

    //Fire Attack
    IEnumerator FireAttack()
    {
        yield return new WaitForSeconds(1.0f);
        float timer = FireAttackDuration;
        float elapsedTime = 0;
        cc_FireAttackVisual.Play();

        //player pushed back

        Vector3 movement = PlayerObject.transform.forward * (-500);

        if (PlayerObject.GetComponent<PlayerController>().IsGrounded)
        {
            PlayerObject.GetComponent<PlayerController>().playerAddForce(movement);
        }


        //Fire Attack loop
        while (elapsedTime <= timer)
        {
            RaycastHit[] objects = Physics.SphereCastAll(transform.position, 15.0f, transform.forward, 0.0f);
            foreach (RaycastHit hit in objects)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Vector3 vectorToCollider = Vector3.Normalize(hit.transform.position - transform.position);
                    if (Vector3.Dot(vectorToCollider, transform.forward) > 0.5f)
                    {
                        DecreasePlayerHealth(FireAttackDamage);
                    }
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cc_FireAttackVisual.Stop();
        yield return new WaitForSeconds(1.5f);
        FireAttackOn = false;
        AttackActivated = false;
    }

    // basic attack
    IEnumerator BasicAttack()
    {
        // windup time


        yield return new WaitForSeconds(0.5f);
        // damage done on player
        DecreasePlayerHealth(BasicAttackDamage);
        // setting bool vars
        yield return new WaitForSeconds(1.0f);

        BasicAttackOn = false;
        AttackActivated = false;
        

        //Debug.Log("basic attack: "+PlayerObject.GetComponent<PlayerController>().getHealth());
    }

    // desperation attack
    IEnumerator DesperationAttack()
    {
        // set bool vars
        AttackActivated = true;
        DesperationAttackOn = true;
        isDesperationAttackUsed = true;

        // desperation attack will only damage player once per attack
        bool damaged = false;

        // warning sound played in windup time
        // this is temporary!!! i will come up with better ways to do the warning
        cc_DragonDesperationAttackWarningSound.Play();

        // windup time
        yield return new WaitForSeconds(4);

        // the sound contained in this audio file is too long i have to manually stop it
        cc_DragonDesperationAttackWarningSound.Stop();

        cc_DesperationAttackVisual.Play();
        cc_DragonDesperationAttackSound.Play();
        float elapsedTime = 0;
        float timer = DesperationAttackLifeSpan;
        while (elapsedTime <= timer)
        {
            if (DesperationAttackInRange && !damaged)
            {
                DecreasePlayerHealth(DesperationAttackDamage);
                damaged = true;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cc_DesperationAttackVisual.Stop();
        BasicAttackOn = false;
        AttackActivated = false;
    }

    bool ShouldActivateDesperationAttack()
    {
        return (CurHealth <= HealthThreshold) && (!isDesperationAttackUsed);
    }

    bool ShouldActivateFireAttack()
    {
        return (CurHealth / MaxHealth) < FireHealthThresholds[FireCounter];
    }

    bool ShouldActivateStompAttack()
    {

        return isStompAttackReady() && BasicAttackInRange;
    }



    #endregion

    #region Decrease Health Methods
    // decrease the player's health
    public void DecreasePlayerHealth(float amount)
    {
        PlayerObject.GetComponent<PlayerController>().DecreaseHealth(amount);
    }

    // decrease the dragon's health
    public void DecreaseDragonHealth(float amount)
    {
        // under attack -> turn hostile
        ChangeHostilityStatus(true);

        // decrease heath
        CurHealth = CurHealth - amount;

        // sound effects
        cc_DragonGetHitSound.Play();

        // update health bar
        m_HUD.UpdateHealth(1.0f * CurHealth / m_Maxhealth);


        if (CurHealth <= 0)
        {
            // dragon is dead
            SceneManager.LoadScene("VictoryScene");

        }

    }

    #endregion

    #region Children Colliders Accessible Methods
    // return dragon's transform info
    public Transform GetDragonTransform()
    {
        return transform;
    }

    // return hostility status
    public bool GetHostilityStatus()
    {
        return Hostile;
    }

    public void ChangeHostilityStatus(bool status)
    // for hostilityrange collider that detects if player is in hostility range
    {
        Hostile = status;
    }
    public void ChangeStompAttackRangeStatus(bool status)
    // for stomp attack range collider that detects if player is in range for stomp attack
    {
        StompAttackInRange = status;
    }
    public void ChangeBasicAttackRangeStatus(bool status)
    // for basic attack range collider that detects if player is in range for basic attack
    {
        BasicAttackInRange = status;
    }
    public void ChangeDesperationAttackRangeStatus(bool status)
    // for basic attack range collider that detects if player is in range for basic attack
    {
        DesperationAttackInRange = status;
    }

    // get stomp attack wind up time
    public float getStompAttackWindupTime()
    {
        return StompAttackWindUpTime;
    }
    #endregion

    #region UI Related
    void updateHealthPercentage()
    {
        float curPercent = (1.0f * CurHealth / m_Maxhealth) * 100.0f;
        HealthPercent.text = curPercent.ToString() + "%";
    }


    #endregion
}




#region OLD VERSION

//// if desperation attack should be activated
//if (CurHealth <= HealthThreshold)
//{
//    if (!isDesperationAttackUsed)
//    {
//        StartDesperationAttack();
//        isDesperationAttackUsed = true;
//    }
//}
//if (!AttackActivated)
//// no attack launched yet
//{
//    // If fire attack should be activated
//    if ((CurHealth / MaxHealth) < FireHealthThresholds[FireCounter])
//    {
//        StartFireAttack();
//        // FireCounter += 1; -> moved into startfireattack() function
//    }
//    if (BasicAttackInRange)
//        // close enough to the player, can launch attack now
//    {
//        if (isStompAttackReady())
//        {
//            // launch stomp attack
//            Debug.Log("start stomp attack");

//            StartStompAttack();

//        }
//        else
//        {
//            // launch basic attack
//            StartBasicAttack();
//            //Debug.Log("start basic attack");
//        }
//    } else
//        // not close enough to the player
//    {
//        if (PlayerObject.GetComponent<PlayerController>().IsGrounded)
//        {
//            Vector3 destination = new Vector3(PlayerObject.transform.position.x, (float)(PlayerObject.transform.position.y + 3.8), PlayerObject.transform.position.z);
//            transform.position = Vector3.MoveTowards(transform.position, destination, ChaseSpeed * Time.deltaTime);
//        } 
//    }
//}
//else
//// still within hostile range but already in attack mode
//{
//    if (BasicAttackOn && !BasicAttackInRange)
//        // player leaves the basic attack's range
//    {
//        AbortBasicAttack();
//        //Debug.Log("abort basic attack");
//    }
//    if (StompAttackOn && !StompAttackInRange)
//        // player leaves the stomps attack's range
//    {
//        //AbortStompAttack();
//        //Debug.Log("abort stomp attack");
//    }
//    if (BasicAttackOn && isStompAttackReady())
//        // stomp attack becomes ready, we need to abort basic attack and activate stomp attack
//    {
//        //Debug.Log("switch from basic to stomp attack");
//        AbortBasicAttack();
//        StartStompAttack();
//        //Debug.Log("attackactivated should be true: " + AttackActivated);
//    }
//}






//if (!DesperationAttackOn)
//{
//    //enable wander
//    if (!GetComponent<Wander>().enabled)
//    {
//        GetComponent<Wander>().enabled = true;
//    }
//    if (AttackActivated)
//    // if dragon is in attack mode
//    // which means player left the hostile range, we need to end the attack
//    {
//        // end the attack that is on
//        if (BasicAttackOn)
//        {
//            // stopcoroutine
//            AbortBasicAttack();
//            // Debug.Log("abort basic attack");
//        }
//        if (StompAttackOn)
//        {
//            // stopcoroutine
//            AbortStompAttack();
//            //Debug.Log("abort stomp attack");
//        }
//    }
//}

//// check if attack should switch
//if (ShouldActivateDesperationAttack())
//{
//    AbortBasicAttack();
//    StartDesperationAttack();
//}
//else if (ShouldActivateFireAttack())
//{
//    AbortBasicAttack();
//    StartFireAttack();
//}
//else if (isStompAttackReady())
//{
//    AbortBasicAttack();
//    StartStompAttack();
//}

//// if player leaves basic attack range, dragon should chase immediately
//if (!BasicAttackInRange)
//{
//    AbortBasicAttack();
//}
#endregion