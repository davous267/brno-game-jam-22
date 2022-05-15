using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider bulletTrigger;

    [SerializeField]
    private float returnSpeed = 7f;

    [SerializeField]
    private Gradient returnTrailGradient;

    [SerializeField]
    private GameObject forceField;

    [SerializeField]
    private float wallLifetime = 60.0f;

    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private TrailRenderer trailRenderer;

    private bool returningToPlayer = false;

    public GameObject enemyThatFired;

    private void FixedUpdate()
    {
        if(returningToPlayer)
        {
            var dir = (GameManager.Instance.Player.BarrelPosition - transform.position).normalized;
            rb.rotation = Quaternion.LookRotation(-dir);
            rb.MovePosition(transform.position + dir * Time.deltaTime * returnSpeed);
            forceField.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {      
        if(collision.gameObject.tag == "Wall")
        {
            if (!returningToPlayer)
            {
                //rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                gameObject.layer = 6;
                enemyThatFired = null;
                bulletTrigger.enabled = true;
                // Invoke is not very good but easiest option now ...
                Invoke("DestroyBullet", wallLifetime);
            } 
            else
            {
                Destroy(gameObject);
            }
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
        CancelInvoke("DestroyBullet");
        trailRenderer.colorGradient = returnTrailGradient;
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

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
