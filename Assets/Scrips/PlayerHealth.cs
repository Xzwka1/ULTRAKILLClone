using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setting")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}