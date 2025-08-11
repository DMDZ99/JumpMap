using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;
    public float useStamina;

    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);

                Collider[] hits = Physics.OverlapSphere(transform.position, attackDistance);    // 공격 범위
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("NPC"))
                    {
                        NPC npc = hit.GetComponent<NPC>();
                        if (npc != null)
                        {
                            npc.TakePhysicalDamage(damage);
                        }
                    }
                }
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }
}
