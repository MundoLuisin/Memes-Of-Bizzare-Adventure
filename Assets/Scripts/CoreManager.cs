using UnityEngine;
using UnityEngine.UI;

public class CoreManager : MonoBehaviour
{
    public float health = 1000;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public char team;
    public GameObject coreCanvas;

    void Update()
    {
        if(health <= 0)
        {
            Destroy(coreCanvas);
            Destroy(this.gameObject);
        }

        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if(healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, 0.05f);
        }
    }
}
