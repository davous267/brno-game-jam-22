using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    private bool returningToPlayer = false;

    private void Update()
    {
        if(returningToPlayer)
        {
            
            transform.Translate((GameManager.Instance.Player.BarrelPosition - transform.position).normalized * Time.deltaTime * 10, Space.World);
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
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.collider.GetComponent<Enemy>().TakeHit();
        }
    }

    public void ReturnToPlayer()
    {
        returningToPlayer = true;
        //rb.GetComponent<Collider>().isTrigger = true;
        gameObject.layer = 0;
    }
}
