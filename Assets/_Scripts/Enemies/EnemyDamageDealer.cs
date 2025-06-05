using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{

    public bool weaponLengthGizmo;
    public LayerMask layerMask;

    bool canDealDamage;
    bool hasDealtDamage;
    

    [SerializeField] float weaponLength = 1;
    [SerializeField] int weaponDamage = 10;



    void Start()
    {

        canDealDamage = false;
        hasDealtDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (canDealDamage && !hasDealtDamage)
        {
            if (other.tag == "Player")
            {
                if (other.transform.TryGetComponent(out HealthSystem health))
                {
                    health.TakeDamage(weaponDamage);
                    //health.HitVFX(hit.point);
                    hasDealtDamage = true;
                }
            }
        }
    }


    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
    }
    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        if (weaponLengthGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
        }
    }
}
