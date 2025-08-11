using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable      // ��ȣ�ۿ� �������̽�
{
    public string GetInteractPrompt();  // ��ȣ�ۿ� �� ������ ���ڿ�
    public void OnInteract();           // ��ȣ�ۿ� �� ȣ��
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}"; // �������̸� -  ����
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;   // �÷��̾� ������ �����Ϳ� ������ ����
        CharacterManager.Instance.Player.addItem?.Invoke(); // �÷��̾�� �������� �߰�. null�ƴϸ�
        Destroy(gameObject);
    }
}
