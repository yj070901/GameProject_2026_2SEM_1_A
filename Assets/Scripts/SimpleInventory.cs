using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject inventoryPanel;             //인벤토리 UI 패널
    [SerializeField] private TextMeshProUGUI[] slotText;             //인벤토리 슬롯에 표시할 텍스트
    [SerializeField] private TextMeshProUGUI selectedItemText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private List<InventoryItem> items = new List<InventoryItem>(); //Items를 관리하는 List 자료 구조선언

    private PlayerController playerController;
    private PlayerPickup playerPickup;
    private ThirdPersonCamera thirdPersonCamera;                     //인벤토리가 오픈되었을때 임의로 카메라가 움직이지 않게 하기 위해
    private bool isOpen;
    [SerializeField] private int selectedIndex = -1;
    //-1 : 선택없음 , 0 : 첫번째 슬롯, 1 : 두번째 슬

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }

    void Start()
    {
        UpdateUI();
    }



    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slotText.Length; i++)
        {
            if (i < items.Count)
            {
                slotText[i].text = items[i].itemName + " x " + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        }
        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            selectedItemText.text = "선택 : " + items[selectedIndex].itemName;
            descriptionText.text = items[selectedIndex].description;
        }
        else
        {
            selectedItemText.text = "아이템을 선택하세요";
            descriptionText.text = "슬롯을 클릭하면 설명이 표시 됩니다.";
        }
    }

    private void ToggleInventory()
    {
        if (isOpen == false && playerController.IsNormalState() == false)
        {
            return;
        }

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerController.ChangeState(PlayerState.Inventory);
            playerPickup.enabled = false;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerController.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public bool AddItem(string itemName, int amount,string description)
    {
        // Slot을 순환 하면서 아이템 이름이 같은 것이 있을 경우 수량을 추가 해준다.
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }

        if (items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        InventoryItem newItem = new InventoryItem();
        newItem.itemName = itemName;
        newItem.amount = amount;
        newItem.description = description;

        items.Add(newItem);
        UpdateUI();

        return true;
    }
    public void SelectSlot(int index)                    //슬롯 선택함수
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;

        UpdateUI();
    }

    public void SelectSlot0() { SelectSlot(0); }
    public void SelectSlot1() { SelectSlot(1); }
    public void SelectSlot2() { SelectSlot(2); }
    public void SelectSlot3() { SelectSlot(3); }
}
