using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle;                         // 컨트롤러 인벤토리에 토글메서드 추가
        CharacterManager.Instance.Player.addItem += AddItem;    // 플레이어의 addItem에 AddItem 추가

        inventoryWindow.SetActive(false);                       // 시작했을때는 인벤토리창 안보이게
        slots = new ItemSlot[slotPanel.childCount];             // 슬롯 = 슬롯패널 자식수로

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();  // 슬롯i번째 = 슬롯패널에 i번째 자식에서 ItemSlot 가져오기
            slots[i].index = i;         // 슬롯 인덱스 설정
            slots[i].inventory = this;  // 슬롯 인벤토리 = 자기자신
            slots[i].Clear();           // 슬롯 비우기
        }

        ClearSelectedItemWindow();      // 선택아이템창 초기화
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ClearSelectedItemWindow()      // 선택 아이템창 초기화
    {
        selectedItemName.text = string.Empty;           // 텍스트 다 비우고
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);                     // 버튼 다 비활성화
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }


    public void Toggle()                        // 토글
    {
        if (IsOpen())                           // IsOpen이면
        {
            inventoryWindow.SetActive(false);   // 닫기
        }
        else
        {
            inventoryWindow.SetActive(true);    // 열기
        }
    }

    public bool IsOpen()                        // IsOpen메서드
    {
        return inventoryWindow.activeInHierarchy;   // 인벤토리창 활성화되어있는지를 반환
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;  // 플레이어에 있는 아이템 데이터를 가져온다

        // 아이템이 중복 가능한지 canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data); // 스택되는 아이템 찾기
            if (slot != null)
            {
                slot.quantity++;            // 해당 슬롯 수량 증가
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null; // 플레이어 아이템 데이터 초기화
                return;
            }
        }

        // 비어있는 슬롯을 가져온다.
        ItemSlot emptySlot = GetEmptySlot();

        // 비어있는 슬롯이 있다면
        if (emptySlot != null)
        {
            emptySlot.item = data;      // 비어있는 슬롯에 데이터 넣음
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        // 없다면
        ThrowItem(data);

        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)      // 슬롯에 아이템 있으면
            {
                slots[i].Set();             // 슬롯에 있는 셋
            }
            else
            {
                slots[i].Clear();           // 클리어
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for( int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }

            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }
}
