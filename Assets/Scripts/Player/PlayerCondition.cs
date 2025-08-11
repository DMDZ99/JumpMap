using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable            // IDamageable �̶�� �������̽� ����
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable   // �������̽� �������� �ܺο��� ȣ�� ����
{
    public UICondition uiCondition;
    Condition health { get { return uiCondition.health; } }     // uiCondition���ִ� health�� ����. �б����� ������Ƽ
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }


    public float noHungerHealthDecay;       // ������� ������ �پ��� ü��

    public event Action onTakeDamage;       // ������ �Ծ����� �̺�Ʈ

    //private float healthRegen = 0f; // ����ȸ����
    //private float healthRegenDuration = 0f; // ����ȸ���ð�

    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);  // ����� �پ�� �ð�����
        stamina.Add(stamina.passiveValue * Time.deltaTime);     // ���¹̳� �ð����� ����

        if(hunger.curValue <= 0f)                               // ������� 0���ϸ� ü�°���
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        //if (healthRegenDuration > 0f)       // ���� ȸ�� ����
        //{
        //    health.Add(healthRegen * Time.deltaTime);
        //    healthRegenDuration -= Time.deltaTime;
        //}


        if (health.curValue <= 0f)       // ü�� 0���ϸ� ���
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);

    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("�׾���.");
    }

    public void TakePhysicalDamage(int damage)  // ������ ����
    {
        health.Subtract(damage);                // ü�� ����
        onTakeDamage?.Invoke();                 // onTakeDamage null �˻��� ����
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }

    //public void StartHealthRegen(float regen, float duration)   // ����ȸ�� ���� ����
    //{
    //    healthRegen = regen;
    //    healthRegenDuration = duration;
    //}
}
