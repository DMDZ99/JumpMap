using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable      // 상호작용 인터페이스
{
    public string GetInteractPrompt();  // 상호작용 시 보여줄 문자열
    public void OnInteract();           // 상호작용 시 호출
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}"; // 아이템이름 -  설명
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;   // 플레이어 아이템 데이터에 데이터 저장
        CharacterManager.Instance.Player.addItem?.Invoke(); // 플레이어에게 아이템을 추가. null아니면
        Destroy(gameObject);
    }
}
