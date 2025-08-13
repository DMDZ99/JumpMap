using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipable,
    Consumable
}


public enum ConsumableType
{
    Health,
    Hunger,
    Stamina
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;

    [Header("RegenHeal")]       // 지속회복
    public bool isRegen;        // 여부
    public float duration;      // 지속시간
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}
