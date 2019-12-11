using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("how fast the player should be moving when runnig around")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("the transform of the camera following the player")]
    private Transform m_CameraTransform;

    [SerializeField]
    [Tooltip("a list of all attacks and info about them")]
    private PlayerAttackInfo[] m_Attacks;


    [SerializeField]
    [Tooltip("amount of health that the player starts with")]
    private int m_MaxHealth;

    [SerializeField]
    [Tooltip("the HUD script")]
    private HUDController m_HUD;

    [SerializeField]
    [Tooltip("attack cool down txt")]
    private Text ACD;

    [SerializeField]
    [Tooltip("Whip attack cool down txt")]
    private Text WCD;

    [SerializeField]
    [Tooltip("number of targets left")]
    private Text targetNumbertxt;

    [SerializeField]
    [Tooltip("the dragon")]
    private GameObject m_Dragon;



    #endregion

    #region Cached References
    private Animator cr_Anim;
    private Renderer cr_Renderer;

    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;

    // sound effects
    private AudioSource cc_WarpSoundEffects;
    private AudioSource cc_GetHitSoundEffects;
    private AudioSource cc_JumpSoundEffects;
    private AudioSource cc_WarpLockAimSoundEffects;

    private ParticleSystem SwordFire;

    #endregion

    #region Private Variables
    //the current move directin of the player. does not include magnitude
    private Vector3 p_Velocity;

    //in order to do anything, we cannot be frozen (timer must be 0)
    private float p_FrozenTimer;

    //current amount of health that the player has
    private float p_CurHealth;

    //if the player is within the distance of short range attacks
    private bool shortRange;

    //if the player is on lava
    private bool onLava;

    //if the restore coroutine has started
    private bool isStarted;

    //if the player can move
    private bool canMove = true;

    //if the player is grounded
    public bool IsGrounded;


    public float fireElapsedTime;
    public float fireTimer;

    private float targetnumber = 5;
    private bool befriend;

    #endregion

    #region Warp Variables
    public bool isLocked;
    public bool canWarp = true;

    //public CinemachineFreeLook cameraFreeLook;
    //private CinemachineImpulseSource impulse;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    public List<Transform> screenTargets = new List<Transform>();
    public Transform target;
    public float warpDuration = .5f;
    private Transform curTarget;

    public Transform sword;
    //public Transform swordHand;
    private Vector3 swordOrigRot;
    private Vector3 swordOrigPos;
    //private MeshRenderer swordMesh;

    //public Material glowMaterial;


    //public ParticleSystem blueTrail;
    //public ParticleSystem whiteTrail;
    //public ParticleSystem swordParticle;


    public GameObject hitParticle;

    public Image aim;
    public Image lockAim;
    public Vector2 uiOffset;


    //attack cooldown ui
    public Image acdFill;
    public Image wcdFill;
    public Image befriendFill;


    #endregion

    #region Initialization

    private void Awake()
    {
        p_Velocity = Vector3.zero;
        cc_Rb = GetComponent<Rigidbody>();
        cc_WarpSoundEffects = GetComponent<AudioSource>();
        cc_GetHitSoundEffects = GameObject.Find("PlayerGetHitSoundEffects").GetComponent<AudioSource>();
        cc_JumpSoundEffects = GameObject.Find("PlayerJumpSoundEffects").GetComponent<AudioSource>();
        cc_WarpLockAimSoundEffects = GameObject.Find("WarpLockAimSoundEffects").GetComponent<AudioSource>();
        cr_Anim = GetComponent<Animator>();
        cr_Renderer = GetComponentInChildren<Renderer>();

        //SwordFire = GameObject.Find("SwordFire").GetComponent<ParticleSystem>();
        p_FrozenTimer = 0;
        p_CurHealth = m_MaxHealth;

        onLava = false;

        fireElapsedTime = 0;
        fireTimer = 0.6f;


        for (int i = 0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];
            attack.Cooldown = 0;

            if (attack.WindUpTime > attack.FrozenTime)
            {
                //Debug.LogError(attack.AttackName + "has a wind up time that is larger than the amount of time that the player is frozen for");
            }
        }
    }

    private void Start()
    {

        Cursor.visible = false;

        //impulse = cameraFreeLook.GetComponent<CinemachineImpulseSource>();
        //postVolume = Camera.main.GetComponent<PostProcessVolume>();
        //postProfile = postVolume.profile;
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        //swordMesh = sword.GetComponentInChildren<MeshRenderer>();
        //swordMesh.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;

        //SwordFire.Play();
    }
    #endregion

    #region Main Updates
    private void Update()
    {

        if (fireElapsedTime < fireTimer)
        {
            fireElapsedTime += Time.deltaTime;
        } else
        {
            //SwordFire.Stop();
        }




        UserInterface();
        //Debug.Log(screenTargets.Count);
        if (screenTargets.Count < 1)
            return;

        //If no warp target is locked, keep updating the nearest target 
        if (!isLocked)
        {
            target = screenTargets[targetIndex()];
        }

        //Lock warp target when L is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // sound effects
            cc_WarpLockAimSoundEffects.Play();

            cr_Anim.SetBool("WarpLock", true);
            transform.LookAt(target);

            LockInterface(true);
            isLocked = true;
            m_Speed = m_Speed / 3;

        }

        //unlock target when L is released
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            cr_Anim.SetBool("WarpLock", false);

            LockInterface(false);
            isLocked = false;
            m_Speed = m_Speed * 3;

        }


        if (!isLocked)
            return;

        //warp
        if (Input.GetKeyDown(KeyCode.F) && canWarp)
        {
            canMove = false;
            p_Velocity = Vector3.zero;
            cc_Rb.useGravity = false;
            cc_WarpSoundEffects.Play();
            curTarget = target;
            Warp();

        }




    }
    private void FixedUpdate()
    {
        //Debug.Log(p_CurHealth);
        //Debug.Log(onLava);
        //Debug.Log(IsGrounded);

        //if (IsGrounded)
        //{
        //    canMove = true;
        //}



        //for lava

        if (onLava && !isStarted)
        {
            StartCoroutine(Burning());
            isStarted = true;
        }

        //
        if (p_FrozenTimer > 0)
        {
            p_Velocity = Vector3.zero;
            p_FrozenTimer -= Time.deltaTime;
            return;
        }
        else
        {
            p_FrozenTimer = 0;
        }

        //ability use
        for (int i = 0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];

            if (attack.IsReady())
            {
                //For the cooldown timers
                if (attack.AttackName == "WhipAttack")
                {
                    //GameObject.Find("WhipIcon").transform.GetChild(0).gameObject.SetActive(false);
                    WCD.text = "0.0";
                }
                else if (attack.AttackName == "BasicAttack")
                {
                    //GameObject.Find("AttackIcon").transform.GetChild(0).gameObject.SetActive(false);
                    ACD.text = "0.0";
                }


                if (Input.GetButtonDown(attack.Button))
                {
                    p_FrozenTimer = attack.FrozenTime;
                    StartCoroutine(UseAttack(attack));

                    if (attack.AttackName == "WhipAttack")
                    {
                        GameObject.Find("WhipIcon").transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else if (attack.AttackName == "BasicAttack")
                    {
                        GameObject.Find("AttackIcon").transform.GetChild(0).gameObject.SetActive(true);
                    }

                    break;
                }
            }
            else if (attack.Cooldown > 0)
            {
                attack.Cooldown -= Time.deltaTime;
                if (attack.AttackName == "WhipAttack")
                {
                    wcdFill.fillAmount = (float)(attack.Cooldown / 2.2);

                    WCD.text = attack.Cooldown.ToString();
                }
                else if (attack.AttackName == "BasicAttack")
                {
                    acdFill.fillAmount = (float)(attack.Cooldown / 1.5);
                    ACD.text = attack.Cooldown.ToString();
                }
            }
        }


        //set how hard the player is pressing movement buttons
        float forward = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");





        //updating velocity
        float moveThreshold = 0.3f;

        if (forward > 0 && forward < moveThreshold)
        {
            forward = 0;
        }
        else if (forward < 0 && forward > -moveThreshold)

        {
            forward = 0;
        }
        if (right > 0 && right < moveThreshold)
        {
            right = 0;
        }
        else if (right < 0 && right > -moveThreshold)

        {
            right = 0;
        }
        p_Velocity.Set(right, forward, 0);
        //update the position of the player
        if (canMove)
        {
            //updating the animation
            cr_Anim.SetFloat("Velocity", p_Velocity.magnitude); // Mathf.Clamp01(Mathf.Abs(forward) + Mathf.Abs(right))); 


            cc_Rb.MovePosition(cc_Rb.position + m_Speed * Time.fixedDeltaTime * transform.forward * p_Velocity.magnitude);
        }

        //update the rotation of the player
        cc_Rb.angularVelocity = Vector3.zero;
        if (p_Velocity.sqrMagnitude > 0)
        {
            float angleToRotCam = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.up, p_Velocity);
            Vector3 camForward = m_CameraTransform.forward;
            Vector3 newRot = new Vector3(Mathf.Cos(angleToRotCam) * camForward.x - Mathf.Sin(angleToRotCam) * camForward.z, 0,
                Mathf.Cos(angleToRotCam) * camForward.z + Mathf.Sin(angleToRotCam) * camForward.x);
            float theta = Vector3.SignedAngle(transform.forward, newRot, Vector3.up);
            cc_Rb.rotation = Quaternion.Slerp(cc_Rb.rotation, cc_Rb.rotation * Quaternion.Euler(0, theta, 0), 0.2f);
        }

        //player jump
        if (Input.GetKeyDown(KeyCode.Space) & IsGrounded)
        {
            //cc_Rb.velocity = new Vector3(cc_Rb.velocity.x, 0, cc_Rb.velocity.y);
            cc_Rb.AddForce(new Vector3(0, 100, 0), ForceMode.Impulse);
            //cc_JumpSoundEffects.Play();
            cr_Anim.SetTrigger("Jump");
        }

        //getdown
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetDown();
        }

        //destroy warp targets
        if (Input.GetKeyDown(KeyCode.H) && curTarget != null)
        {
            Destroy(curTarget.gameObject);
            targetnumber -= 1;
            targetNumbertxt.text = "Targets Left: " + targetnumber.ToString() + " / 5"; 
            if (targetnumber == 0)
            {
                befriend = true;
                befriendFill.fillAmount = 0;
            }
            GetDown();
        }


        if (befriend && Input.GetKeyDown(KeyCode.L) && m_Dragon.GetComponent<EnemyController>().BasicAttackInRange)
        {
            StartCoroutine(FadeAudio(2));
            StartCoroutine(FindObjectOfType<FadingEffects>().FadeAndLoadScene(FadingEffects.FadeDirection.In, "HappyEnding"));
        }

    }

    #endregion



    #region Health/Dying Methods
    public void DecreaseHealth(float amount)
    {
        if (amount > 3f)
        {
            // initiate sound effects
            cc_GetHitSoundEffects.Play();

        }
        p_CurHealth -= amount;
        
        m_HUD.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
        if (p_CurHealth <= 0)
        {
            //Debug.Log("player dies!!!");

            SceneManager.LoadScene("LoseScene");
        }
    }

    //public void IncreaseHealth(int amount)
    //{
    //    p_CurHealth += amount;
    //    if (p_CurHealth > m_MaxHealth)
    //    {
    //        p_CurHealth = m_MaxHealth;
    //    }
    //    m_HUD.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
    //}

    #endregion


    #region Attack Methods
    private IEnumerator UseAttack(PlayerAttackInfo attack)
    {
        cc_Rb.rotation = Quaternion.Euler(0, m_CameraTransform.eulerAngles.y, 0);
        cr_Anim.SetTrigger(attack.TriggerName);

        yield return new WaitForSeconds(attack.WindUpTime);

        Vector3 offset = transform.forward * attack.Offset.z + transform.right * attack.Offset.x + transform.up * attack.Offset.y;
        GameObject go = Instantiate(attack.AbilityGO, transform.position + offset, cc_Rb.rotation);
        go.GetComponent<Ability>().Use(transform.position + offset);

        yield return new WaitForSeconds(attack.Cooldown);

        attack.ResetCooldown();
    }

    #endregion

    #region Misc Methods
    private void GetDown()
    {
        cc_Rb.useGravity = true;
        canMove = true;
    }
    #endregion

    #region Lava

    private IEnumerator Burning()
    {
        while (onLava)
        {
            DecreaseHealth(8);
            yield return new WaitForSeconds(2);
        }
    }

    public void setLava()
    {
        onLava = true;
    }

    public void fsetLava()
    {
        onLava = false;
    }

    public void setGrounded()
    {
        IsGrounded = true;
    }

    public void unGrounded()
    {
        IsGrounded = false;
    }

    #endregion


    #region Warp

    public void Warp()
    {

        //GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
        //      Destroy(clone.GetComponent<PlayerController>().sword.gameObject);
        //Destroy(clone.GetComponent<Animator>());
        // Destroy(clone.GetComponent<PlayerController>());
        //Destroy(clone.GetComponent<MovementInput>());
        // Destroy(clone.GetComponent<CharacterController>());

        //SkinnedMeshRenderer[] skinMeshList = clone.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer smr in skinMeshList)
        //{
        //    smr.material = glowMaterial;
        //    smr.material.DOFloat(2, "_AlphaThreshold", 5f).OnComplete(() => Destroy(clone));
        //}

        //ShowBody(false);
        //anim.speed = 0;

        // aims at target
        transform.LookAt(curTarget);


        cr_Anim.SetTrigger("Warp");

        StartCoroutine(WarpCoroutine());


        //Particles
        //blueTrail.Play();
        //whiteTrail.Play();

        //Lens Distortion
        // DOVirtual.Float(0, -80, .2f, DistortionAmount);
        //DOVirtual.Float(1, 2f, .2f, ScaleAmount);
    }


    IEnumerator WarpCoroutine()
    {
        // wait one second for animation windup
        yield return new WaitForSeconds(.8f);


        transform.DOMove(target.position, warpDuration).SetEase(Ease.InExpo).OnComplete(() => FinishWarp());

        sword.parent = null;
        sword.DOMove(target.position, warpDuration / 1.2f);
        sword.DOLookAt(target.position, .2f, AxisConstraint.None);

        yield break;
    }

    void FinishWarp()
    {
        //ShowBody(true);

        //sword.parent = swordHand;
        sword.parent = GetComponentInParent<PlayerController>().transform;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;

        // reset player rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);

        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            //   GlowAmount(30);
            //   DOVirtual.Float(30, 0, .5f, GlowAmount);
        }

        //Instantiate(hitParticle, sword.position, Quaternion.identity);

        //target.GetComponentInParent<Animator>().SetTrigger("hit");
        //target.parent.DOMove(target.position + transform.forward, .5f);
        //Debug.Log("target's parent is : " + target.parent.name);

        //StartCoroutine(HideSword());
        //StartCoroutine(PlayAnimation());
        //StartCoroutine(StopParticles());

        isLocked = false;
        LockInterface(false);
        //aim.color = Color.clear;

        //Shake
        //impulse.GenerateImpulse(Vector3.right);

        //Lens Distortion
        //DOVirtual.Float(-80, 0, .2f, DistortionAmount);
        //DOVirtual.Float(2f, 1, .1f, ScaleAmount);
    }

    public int targetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }

        return index;

    }

    private void UserInterface()
    {
        if (target != null)
        {
            aim.transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3)uiOffset);
        }

        //if (!input.canMove)
        //	return;

        Color c = screenTargets.Count < 1 ? Color.clear : Color.white;
        aim.color = c;
    }

    private void LockInterface(bool state)
    {
        float size = state ? 1 : 2;
        float fade = state ? 1 : 0;
        lockAim.DOFade(fade, .15f);
        lockAim.transform.DOScale(size, .15f).SetEase(Ease.OutBack);
        lockAim.transform.DORotate(Vector3.forward * 180, .15f, RotateMode.FastBeyond360).From();
        aim.transform.DORotate(Vector3.forward * 90, .15f, RotateMode.LocalAxisAdd);
        if (state)
        {
            sword.DOLookAt(target.position, .2f, AxisConstraint.None);
        }
        else
        {
            sword.transform.DORotate(swordOrigRot, .15f, RotateMode.Fast);
        }

    }
    #endregion


    #region enemycontroller helper 
    public float getHealth()
    {
        return p_CurHealth;
    }

    public void playerAddForce(Vector3 movement)
    {
        cc_Rb.AddForce(movement, ForceMode.Impulse);
    }
    #endregion

    private IEnumerator FadeAudio(float time)
    {
        float start = 1;
        float end = 0;
        float i = 0;
        float speed = 1 / time;
        while (i <= 1)
        {
            i += speed * Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(start, end, i);
            yield return null;
        }
    }
}
