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

    [SerializeField]
    private GameObject[] gunScreens;

    [SerializeField]
    private AudioClip bodyHit;

    [SerializeField]
    private TMPro.TMP_Text enemyCountText;

    [SerializeField]
    private GameObject bloodyVignette;

    private AudioSource audioSource;
    private Color defaultCrosshairColor;
    private bool waitForButtonUp;
    private Bullet lastBullet;
    private int startHealth;
    private int currentActiveGunScreen = 0;

    
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
        GameManager.Instance.EnemySpawner.EnemyCountChanged.AddListener(UpdateEnemyCountScreen);
        SetActiveGunScreen(0);
        startHealth = Health;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            waitForButtonUp = false;
        }

        if(Input.GetMouseButtonDown(1))
        {
            SetActiveGunScreen((currentActiveGunScreen + 1) % gunScreens.Length);
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

    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(ShowDamageVignette());
    }

    private Ray GetScreenRay()
    {
        return Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    private void UpdateEnemyCountScreen()
    {
        enemyCountText.text = GameManager.Instance.EnemySpawner.EnemyCount.ToString();
    }

    private void SetActiveGunScreen(int idx)
    {
        for(int i = 0; i < gunScreens.Length; ++i)
        {
            gunScreens[i].SetActive(idx == i);
        }

        currentActiveGunScreen = idx;
    }

    private IEnumerator ShowDamageVignette()
    {
        if(!bloodyVignette.activeInHierarchy)
        {
            bloodyVignette.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            bloodyVignette.SetActive(false);
        }
    
    }
}
