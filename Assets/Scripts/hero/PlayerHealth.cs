using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public Image healthBar;
    float currentHealth;

    bool dead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar)
        {
            healthBar.transform.localScale = new Vector3(Mathf.Max(currentHealth/maxHealth,0), 1, 1);
        }
        
        if (currentHealth <= 0 && !dead)
        {
            dead = true;
            PlayerDeath playerDeath = GetComponent<PlayerDeath>();
            if (playerDeath == null)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                GetComponent<PlayerDeath>().OnDeath();
            }
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        if (healthBar)
        {
            healthBar.transform.localScale = new Vector3(Mathf.Max(currentHealth / maxHealth, 0), 1, 1);
        }
    }
}
