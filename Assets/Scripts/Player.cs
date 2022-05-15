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
    private Image healthBar;

    [SerializeField]
    private Color higlightCrosshairColor = Color.red;

    [SerializeField]
    private LayerMask raycastLayers;

    [SerializeField]
    private LayerMask bulletWallLayer;

    [SerializeField]
    private Transform gunBarrel;

    [SerializeField]
    private Transform hitPoint;

    private Color defaultCrosshairColor;

    private bool waitForButtonUp;

    private Bullet lastBullet;

    private int startHealth;

    public Vector3 BarrelPosition
    {
        get => gunBarrel.position;    
    }

    public Vector3 HitPoint
    {
        get => hitPoint.position;
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.fillAmount = Mathf.Max((float)health / startHealth, 0.0f);
            if(health <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void Start()
    {
        defaultCrosshairColor = crosshair.color;
        startHealth = Health;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            waitForButtonUp = false;
        }
    }

    private void FixedUpdate()
    {
        var crosshairRay = GetScreenRay();

        RaycastHit hit;
        if (Physics.Raycast(crosshairRay, out hit, 1000.0f, raycastLayers.value, QueryTriggerInteraction.Collide) &&
            ((1 << hit.collider.gameObject.layer) & bulletWallLayer) != 0)
        {
            crosshair.color = higlightCrosshairColor;;
            var blt = hit.collider.gameObject.GetComponent<Bullet>();
            blt.ShowForceField();
            lastBullet = blt;

            if(blt && Input.GetMouseButton(0) && !waitForButtonUp)
            {
                blt.ReturnToPlayer();
                waitForButtonUp = true;
            }
        }
        else
        {
            crosshair.color = defaultCrosshairColor;
            if(lastBullet != null)
            {
                lastBullet.HideForceField();
                lastBullet = null;
            }
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
