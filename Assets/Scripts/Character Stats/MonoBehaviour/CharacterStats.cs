using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    [HideInInspector]
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    public int maxHealth
    {
        get { return characterData != null ? characterData.maxHealth : 0; }
        set { characterData.maxHealth = value; }
    }

    public int currentHealth
    {
        get { return characterData != null ? characterData.currentHealth : 0; }
        set { characterData.currentHealth = value; }
    }

    public int baseDefence
    {
        get { return characterData != null ? characterData.baseDefence : 0; }
        set { characterData.baseDefence = value; }
    }

    public int currentDefence
    {
        get { return characterData != null ? characterData.currentDefence : 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Combat
    public void TakeDamage(CharacterStats attacker)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (attacker.isCritical)
        {
            GetComponent<Animator>().SetTrigger("Hit");
        }

        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.deathExp);
        }
    }

    public void TakeDamage(int damage)
    {
        int currentDamage = Mathf.Max(damage - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - currentDamage, 0);
        GetComponent<Animator>().SetTrigger("Hit");

        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.deathExp);
        }
    }

    private void DataUpdates()
    {
        // UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
        // if (currentHealth <= 0)
        // {
        //     attaccharacterData.UpdateExp(characterData.deathExp);
        // }
    }

    private int CurrentDamage()
    {
        int damage = (int) UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            damage = (int) (damage * attackData.criticalMultiplier);
        }
        return damage;
    }
    #endregion
}
