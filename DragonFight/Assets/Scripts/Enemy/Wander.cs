using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{


    private Vector3 dir;
    private Rigidbody cc_Rb;


    [SerializeField]
    [Tooltip("speed")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("directionchangeinterval")]
    private float m_dirChangeInterval;

    [SerializeField]
    [Tooltip("speed of rotation")]
    private float m_RotSpeed;

    [SerializeField]
    [Tooltip("the trigger string to use to activate this movement in the animator")]
    private string m_BoolName;
    public string BoolName
    {
        get
        {
            return m_BoolName;
        }
    }


    private Quaternion _lookRot;
    private Vector3 _direction;

    private Animator cr_Anim;



    private void Awake()
    {
        cc_Rb = GetComponent<Rigidbody>();
        cr_Anim = GetComponent<Animator>();

        StartCoroutine(RandomMove());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        //_direction = (dir - cc_Rb.position).normalized;
        cr_Anim.SetBool(m_BoolName, true);
        _lookRot = Quaternion.LookRotation(dir);
        cc_Rb.rotation = Quaternion.Slerp(cc_Rb.rotation, _lookRot, Time.deltaTime * m_RotSpeed);


        dir.Normalize();


        cc_Rb.MovePosition(cc_Rb.position + transform.forward * m_Speed * Time.fixedDeltaTime);

    }

    private IEnumerator RandomMove()
    {
        while (true)
        {
            RandomVector(-40, 40);
            yield return new WaitForSecondsRealtime(m_dirChangeInterval);
        }
    }



    private void RandomVector(float min, float max)
    {
        var x = Random.Range(min, max);
        var z = Random.Range(min, max);
        dir = new Vector3(x, 0, z);
        //transform.LookAt(transform.position + dir);
    }
}
