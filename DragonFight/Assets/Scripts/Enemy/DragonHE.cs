using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonHE : MonoBehaviour
{

    private Vector3 destination;
    private Quaternion _lookRot;
    private Rigidbody cc_Rb;
    //private Animator cr_Anim;


    [SerializeField]
    [Tooltip("speed")]
    private float m_Speed = 30;

    private float m_speeed;

    [SerializeField]
    [Tooltip("speed of rotation")]
    private float m_RotSpeed;

    [SerializeField]
    [Tooltip("the transform of the camera following the player")]
    private Transform m_CameraTransform;

    private Vector3 d_Velocity;
    private bool onDragon = true;

    private void Awake()
    {
        cc_Rb = GetComponent<Rigidbody>();
        //cr_Anim = GetComponent<Animator>();

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //set how hard the player is pressing movement buttons
        float forward = Input.GetAxis("Vertical");

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
    

        m_speeed = forward * m_Speed;
        //update the position of the player
        if (onDragon)
        {
            //updating the animation
            //cr_Anim.SetFloat("Velocity", d_Velocity.magnitude); // Mathf.Clamp01(Mathf.Abs(forward) + Mathf.Abs(right)));
            _lookRot = Quaternion.LookRotation(m_CameraTransform.forward);
            cc_Rb.rotation = Quaternion.Slerp(cc_Rb.rotation, _lookRot, Time.deltaTime * m_RotSpeed);


            m_CameraTransform.forward.Normalize();


            cc_Rb.MovePosition(cc_Rb.position + transform.forward * m_speeed * Time.fixedDeltaTime);
        }

    }
}
