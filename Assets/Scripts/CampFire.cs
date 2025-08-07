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
        InvokeRepeating("DealDamage", 0, damageRate);   // 0�� �ĺ��� damageRate �������� ������ �ݺ�����
    }

  
    void DealDamage()
    {
        for(int i = 0; i < things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);       // �ݺ��ؼ� TakePhysicalDamage �ҷ��´�.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))   // �ݶ��̴��� ���� ������ IDamageable�� �ִ��� Ȯ��
        {
            things.Add(damageable);                             // damageable�� ���Ѵ�.
        }
    }

    private void OnTriggerExit(Collider other)                  // �ݶ��̴����� �����
    {
        if(other.TryGetComponent(out IDamageable damageable))   // damageable ����
        {
            things.Remove(damageable);
        }
    }
}
