using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable            // IDamageable 이라는 인터페이스 정의
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable   // 인터페이스 구현으로 외부에서 호출 가능
{
    public UICondition uiCondition;
    Condition health { get { return uiCondition.health; } }     // uiCondition에있는 health에 접근. 읽기전용 프로퍼티
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }


    public float noHungerHealthDecay;       // 배고픔이 없을때 줄어드는 체력

    public event Action onTakeDamage;       // 데미지 입었을때 이벤트

    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);  // 배고픔 줄어듬 시간마다
        stamina.Add(stamina.passiveValue * Time.deltaTime);     // 스태미나 시간마다 증가

        if(hunger.curValue <= 0f)                               // 배고픔이 0이하면 체력감소
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if(health.curValue <= 0f)       // 체력 0이하면 사망
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
        Debug.Log("죽었다.");
    }

    public void TakePhysicalDamage(int damage)  // 데미지 구현
    {
        health.Subtract(damage);                // 체력 감소
        onTakeDamage?.Invoke();                 // onTakeDamage null 검사후 실행
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
}
