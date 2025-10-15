using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ClickeableItem : MonoBehaviour, IPointerClickHandler
{
    private GameManager _gameManager;
    private SpriteRenderer _spriteRenderer;

    private bool isPair;
    private int assignedSlot; // 0 = left, 1 = right
    private bool isAxisRotated = false;

    void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_gameManager == null)
        {
            Debug.LogError("No GameManager found in the scene!");
        }
    }

    public void SetAssignedSlot(int slot) => assignedSlot = slot;
    public int GetAssignedSlot() => assignedSlot;
    public bool GetPairValue() => isPair;
    public void SetIsAxisRotated(bool rotated) => isAxisRotated = rotated;
    public bool GetIsAxisRotated() => isAxisRotated;

    public void SetDesign(Sprite newSprite, bool newPairValue)
    {
        _spriteRenderer.sprite = newSprite;
        isPair = newPairValue;

        SetBorder(true); // show border by default (optional)
    }

    public void SetBorder(bool showBorder)
    {
        if (showBorder)
        {
            // RELLENAR
        }
        else
        {

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameManager.OnItemClicked(this);
    }
}