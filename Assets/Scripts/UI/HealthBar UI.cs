using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform barPoint;
    public bool alwaysVisible = true;
    public float visibleTime = 3f;
    private Image healthSlider;
    private Transform UIbar;
    private Transform cam;
    private CharacterStats currentStats;
    private float timeLeft;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthBarPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;
        healthSlider.fillAmount = (float) currentHealth / maxHealth;
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;
        }

        if (timeLeft <= 0 && !alwaysVisible)
        {
            UIbar.gameObject.SetActive(false);
        } else {
            timeLeft -= Time.deltaTime;
        }
    }
}
