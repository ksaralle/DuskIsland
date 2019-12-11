using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerHE : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("how fast the player should be moving when runnig around")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("the transform of the camera following the player")]
    private Transform m_CameraTransform;

	[SerializeField]
	[Tooltip("the offset from the dragon's origin to the player")]
	private Vector3 m_Offset;


	[SerializeField]
	[Tooltip("the dragon")]
	private Transform m_DragonTransform;

    [SerializeField]
    [Tooltip("the image used to fade")]
    public Image fadeImage;

    [SerializeField]
    [Tooltip("how fast the scene fades")]
    private float fadeSpeed;
    #endregion

    #region Cached References
    private Animator cr_Anim;
    private Renderer cr_Renderer;

    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;

    #endregion

    #region Private Variables
    //the current move directin of the player. does not include magnitude
    private Vector3 p_Velocity;

    //if the player can move
    public bool canMove = false;

    //if the player is grounded
    public bool IsGrounded;

    #endregion

    #region Warp Variables

    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;


    #endregion

    #region Initialization

    private void Awake()
    {
        AudioListener.volume = 1;

        StartCoroutine(FadeOut());

        p_Velocity = Vector3.zero;
        cc_Rb = GetComponent<Rigidbody>();
     
        cr_Anim = GetComponent<Animator>();
        cr_Renderer = GetComponentInChildren<Renderer>();

    }

    private void Start()
    {

    }
    #endregion

    #region Main Updates
    private void Update()
    {

    }

    private void FixedUpdate()
    {

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
            Debug.Log(canMove);
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
            cc_Rb.velocity = new Vector3(cc_Rb.velocity.x, 0, cc_Rb.velocity.y);
            cc_Rb.AddForce(new Vector3(0, 250, 0), ForceMode.Impulse);
            //cc_JumpSoundEffects.Play();
            //cr_Anim.SetBool("Jump", true);
        }


    }

    private void LateUpdate()
	{

		Vector3 newPos = m_DragonTransform.position + m_Offset;

		transform.position = Vector3.Slerp(transform.position, newPos, 1);



	}

    #endregion

    private IEnumerator FadeOut()
    {
        float alpha = 1;
        float fadeEndValue = 0;
        while (alpha >= fadeEndValue)
        {
            SetColorImage(ref alpha);
            yield return null;
        }
        fadeImage.enabled = false;

    }

    private void SetColorImage(ref float alpha)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        alpha -= Time.deltaTime * (1.0f / fadeSpeed);

    }
}
