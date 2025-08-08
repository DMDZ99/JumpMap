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

        controller.inventory += Toggle;                         // ��Ʈ�ѷ� �κ��丮�� ��۸޼��� �߰�
        CharacterManager.Instance.Player.addItem += AddItem;    // �÷��̾��� addItem�� AddItem �߰�

        inventoryWindow.SetActive(false);                       // ������������ �κ��丮â �Ⱥ��̰�
        slots = new ItemSlot[slotPanel.childCount];             // ���� = �����г� �ڽļ���

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();  // ����i��° = �����гο� i��° �ڽĿ��� ItemSlot ��������
            slots[i].index = i;         // ���� �ε��� ����
            slots[i].inventory = this;  // ���� �κ��丮 = �ڱ��ڽ�
            slots[i].Clear();           // ���� ����
        }

        ClearSelectedItemWindow();      // ���þ�����â �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ClearSelectedItemWindow()      // ���� ������â �ʱ�ȭ
    {
        selectedItemName.text = string.Empty;           // �ؽ�Ʈ �� ����
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);                     // ��ư �� ��Ȱ��ȭ
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }


    public void Toggle()                        // ���
    {
        if (IsOpen())                           // IsOpen�̸�
        {
            inventoryWindow.SetActive(false);   // �ݱ�
        }
        else
        {
            inventoryWindow.SetActive(true);    // ����
        }
    }

    public bool IsOpen()                        // IsOpen�޼���
    {
        return inventoryWindow.activeInHierarchy;   // �κ��丮â Ȱ��ȭ�Ǿ��ִ����� ��ȯ
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;  // �÷��̾ �ִ� ������ �����͸� �����´�

        // �������� �ߺ� �������� canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data); // ���õǴ� ������ ã��
            if (slot != null)
            {
                slot.quantity++;            // �ش� ���� ���� ����
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null; // �÷��̾� ������ ������ �ʱ�ȭ
                return;
            }
        }

        // ����ִ� ������ �����´�.
        ItemSlot emptySlot = GetEmptySlot();

        // ����ִ� ������ �ִٸ�
        if (emptySlot != null)
        {
            emptySlot.item = data;      // ����ִ� ���Կ� ������ ����
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        // ���ٸ�
        ThrowItem(data);

        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)      // ���Կ� ������ ������
            {
                slots[i].Set();             // ���Կ� �ִ� ��
            }
            else
            {
                slots[i].Clear();           // Ŭ����
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)   // ���Ծ������� �����Ϳ� ���� ���� ������ �ִ밳������ ������
            {
                return slots[i];    // ���� ��ȯ
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)  // �����۽����� null�̸�
            {
                return slots[i];        // ���� ��ȯ
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)   // �����۹�����
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));    // dropPrefab�� ����. dropPosition��, ����ȸ��������
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;      // ���Կ� �������� ���̸� ��ȯ

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";        // �� ���õȽ����̸��� �������ִ��� Ÿ�� �߰�
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";      // �� ���õȽ��Ȱ��� �Դ� �� �� �߰�
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);  // �������ִ� Ÿ�� : ����ư Ȱ��ȭ
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);   // ��������Ÿ��, �����Ȼ��°� �ƴϸ� : ������ư Ȱ��ȭ
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);  // ��������Ÿ��, ������ ���¸� : ������ư Ȱ��ȭ
        dropButton.SetActive(true); // ������ ��ư �׻� Ȱ��ȭ
    }

    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)                    // ���õ� ������ Ÿ���� �������ִ� Ÿ���̸�
        {
            for( int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:                             // ü��ȸ��Ÿ���϶� ȸ��
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);   // �Դ�Ÿ���϶� �����ȸ��
                        break;
                }
            }

            RemoveSelectedItem();   // ������ ���� (������ ����)
        }
    }

    public void OnDropButton()  // �µ�ӹ�ư ������ ������ ���õȾ���������
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()   // ���õȾ��������� �޼���
    {
        slots[selectedItemIndex].quantity--;    // ���� 1 ����

        if (slots[selectedItemIndex].quantity <= 0) // ������ 0���ϸ�
        {
            selectedItem = null;                    // ���õȾ����� �ΰ�
            slots[selectedItemIndex].item = null;   // �ε��� �ΰ�
            selectedItemIndex = -1;                 // ���� �ε��� -1
            ClearSelectedItemWindow();              // ����â �ʱ�ȭ
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
