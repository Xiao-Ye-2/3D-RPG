using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    private CharacterData_SO characterData;
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
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        UIupdates();
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - currentDamage, 0);
        defender.GetComponent<Animator>().SetTrigger("Hit");

        UIupdates();
    }

    private void UIupdates()
    {
        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
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
