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

    private Color defaultCrosshairColor;

    public int Health
    {
        get => health;
        set => health = value; // TODO Check for game over
    }

    private void Start()
    {
        defaultCrosshairColor = crosshair.color;
    }

    private void FixedUpdate()
    {
        var crosshairRay = GetScreenRay();

        RaycastHit hit;
        if (Physics.Raycast(crosshairRay, out hit, 1000.0f, bulletWallLayer.value, QueryTriggerInteraction.Ignore))
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
}
