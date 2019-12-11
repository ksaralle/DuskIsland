using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Ability
{
    // Start is called before the first frame update
    
    public override void Use(Vector3 spawnPos)
    {
        //RaycastHit hit;

        //if (Physics.SphereCast(spawnPos, 1f, transform.forward, out hit, m_Info.Range))
        //{

        //    if (hit.collider.CompareTag("Dragon"))
        //    {
        //        hit.collider.GetComponent<EnemyController>().DecreaseDragonHealth(m_Info.Power);
        //    }
        //    else if (hit.collider.CompareTag("BasicAttack"))
        //    {
        //        hit.collider.GetComponentInParent<EnemyController>().DecreaseDragonHealth(m_Info.Power);
        //    }
        //}
        SoundEffects.Play();
        
        RaycastHit[] hits = Physics.SphereCastAll(spawnPos, 0.5f, transform.forward, m_Info.Range);
        foreach (RaycastHit hit in hits)
        {

            if (hit.collider.CompareTag("Dragon"))
            {
                Debug.Log("hit dragon");
                hit.collider.GetComponent<EnemyController>().DecreaseDragonHealth(m_Info.Power);
            }
            else if (hit.collider.CompareTag("BasicAttack"))
            {
                Debug.Log("hit basicattack");

                hit.collider.GetComponentInParent<EnemyController>().DecreaseDragonHealth(m_Info.Power);
            }
        }



    }


}
