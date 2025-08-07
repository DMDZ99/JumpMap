using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    List<IDamageable> things = new List<IDamageable>();

    void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);   // 0초 후부터 damageRate 간격으로 데미지 반복실행
    }

  
    void DealDamage()
    {
        for(int i = 0; i < things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);       // 반복해서 TakePhysicalDamage 불러온다.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))   // 콜라이더에 뭔가 들어오면 IDamageable이 있는지 확인
        {
            things.Add(damageable);                             // damageable을 더한다.
        }
    }

    private void OnTriggerExit(Collider other)                  // 콜라이더에서 벗어나면
    {
        if(other.TryGetComponent(out IDamageable damageable))   // damageable 제거
        {
            things.Remove(damageable);
        }
    }
}
