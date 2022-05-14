using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int health = 100;

    [SerializeField]
    private Image crosshair;

    [SerializeField]
    private Color higlightCrosshairColor = Color.red;

    [SerializeField]
    private LayerMask bulletWallLayer;

    [SerializeField]
    private Transform gunBarrel;

    private Color defaultCrosshairColor;

    public Vector3 BarrelPosition
    {
        get => gunBarrel.position;    
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if(health <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void Start()
    {
        defaultCrosshairColor = crosshair.color;
    }

    private void FixedUpdate()
    {
        var crosshairRay = GetScreenRay();

        RaycastHit hit;
        if (Physics.Raycast(crosshairRay, out hit, 1000.0f, bulletWallLayer.value, QueryTriggerInteraction.Collide))
        {
            crosshair.color = higlightCrosshairColor;
            var blt = hit.collider.gameObject.GetComponent<Bullet>();

            if(blt && Input.GetMouseButton(0))
            {
                blt.ReturnToPlayer();
            }
        }
        else
        {
            crosshair.color = defaultCrosshairColor;
        }
    }

    private Ray GetScreenRay()
    {
        return Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log(Health);
    }
}
