using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider bulletCollider;

    [SerializeField]
    private Collider bulletTrigger;

    [SerializeField]
    private float returnSpeed = 7f;

    [SerializeField]
    private GameObject forceField;

    private bool returningToPlayer = false;

    private int damage = 35;

    public GameObject enemyThatFired;

    private void Update()
    {
        if(returningToPlayer)
        {        
            transform.Translate((GameManager.Instance.Player.BarrelPosition - transform.position).normalized * Time.deltaTime * returnSpeed, Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {      
        if(collision.gameObject.tag == "Wall" && !returningToPlayer)
        {
            //rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            gameObject.layer = 6;
            enemyThatFired = null;
            bulletTrigger.enabled = true;
        }

        if (collision.gameObject.tag == "Enemy" && collision.gameObject != enemyThatFired)
        {          
                collision.collider.GetComponent<Enemy>().TakeHit();
                Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            if (returningToPlayer)
            {
                Destroy(gameObject);
            }
            else
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(returningToPlayer)
        {
            if (collider.gameObject.tag == "Gun Barrel")
            {
                Destroy(gameObject);
            }
        }
    }

    public void ReturnToPlayer()
    {
        bulletTrigger.enabled = false;
        returningToPlayer = true;

        //rb.GetComponent<Collider>().isTrigger = true;
        gameObject.layer = 0;
    }

    public void ShowForceField()
    {
        forceField.SetActive(true);
    }

    public void HideForceField()
    {
        forceField.SetActive(false);
    }
}
