using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 100;

    public float currentHealthPercentage
    {
        get
        {
            return (float)currentHealth / (float)maxHealth;
        }
    }
    Character character;

    private void Awake()
    {
        currentHealth = maxHealth;

        character = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + "took damage:" + damage);
        Debug.Log(gameObject.name + "currentHealth:" + currentHealth);

        CheckHealth();
    }

    public void CheckHealth()
    {
        if(currentHealth <= 0)
        {
            character.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int value)
    {
        currentHealth += value;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
