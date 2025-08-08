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

    int curEquipIndex;

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
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)   // 슬롯아이템이 데이터와 같고 슬롯 개수가 최대개수보다 많으면
            {
                return slots[i];    // 슬롯 반환
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)  // 아이템슬롯이 null이면
            {
                return slots[i];        // 슬롯 반환
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)   // 아이템버리기
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));    // dropPrefab을 생성. dropPosition에, 랜덤회전값으로
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;      // 슬롯에 아이템이 널이면 반환

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";        // 각 선택된스탯이름에 먹을수있는지 타입 추가
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";      // 각 선택된스탯값에 먹는 양 값 추가
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);  // 먹을수있는 타입 : 사용버튼 활성화
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);   // 장착가능타입, 장착된상태가 아니면 : 장착버튼 활성화
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);  // 장착가능타입, 장착된 상태면 : 해제버튼 활성화
        dropButton.SetActive(true); // 버리기 버튼 항상 활성화
    }

    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)                    // 선택된 아이템 타입이 먹을수있는 타입이면
        {
            for( int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:                             // 체력회복타입일때 회복
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);   // 먹는타입일때 배고픔회복
                        break;
                }
            }

            RemoveSelectedItem();   // 아이템 제거 (먹으면 제거)
        }
    }

    public void OnDropButton()  // 온드롭버튼 누르면 버리고 선택된아이템제거
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()   // 선택된아이템제거 메서드
    {
        slots[selectedItemIndex].quantity--;    // 개수 1 차감

        if (slots[selectedItemIndex].quantity <= 0) // 수량이 0이하면
        {
            selectedItem = null;                    // 선택된아이템 널값
            slots[selectedItemIndex].item = null;   // 인덱스 널값
            selectedItemIndex = -1;                 // 선택 인덱스 -1
            ClearSelectedItemWindow();              // 선택창 초기화
        }

        UpdateUI();
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
