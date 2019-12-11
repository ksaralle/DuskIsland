using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipAttack : Ability
{
    // Start is called before the first frame update
    public override void Use(Vector3 spawnPos)
    {
        //RaycastHit hit;
        ParticleSystem cc_PS;
        cc_PS = GetComponent<ParticleSystem>();
        RaycastHit[] hits = Physics.SphereCastAll(spawnPos, 0.5f, transform.forward, m_Info.Range);
        foreach (RaycastHit hit in hits) { 

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

        //var emitterShape = cc_PS.shape;
        //emitterShape.length = m_Info.Range;
        //cc_PS.Play();
    }
}
