using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private Outline outline;
    
    public UIInventory inventory;

    public int index;
    public bool equipped;
    public int quantity;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;     // 테두리 활성화 = 장착했을때
    }

    public void Set()                   // 슬롯에 아이템 UI 세팅
    {
        icon.gameObject.SetActive(true);// 아이콘 활성화
        icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;  // 개수가 1보다 크면 숫자표시 : 작으면 빈문자열;

        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()     // 빈칸
    {
        item = null;        // 아이템이 null
        icon.gameObject.SetActive(false);   // 아이콘 비활성화
        quantityText.text = string.Empty;   // 개수 : 빈문자열
    }

    public void OnClickButton()         // 클릭 시
    {
        inventory.SelectItem(index);    // 인벤토리의 SelectItem 각 인덱스마다 호출
    }
}
