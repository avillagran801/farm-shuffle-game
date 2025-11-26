using UnityEngine;
using UnityEngine.EventSystems;

public class ClickeableItem : MonoBehaviour, IPointerClickHandler
{
    public Material normalMaterial;
    public Material outlineMaterial;
    private GameManager _gameManager;
    private SpriteRenderer _spriteRenderer;

    private bool isPair;
    private int assignedSlot; // 0 = left, 1 = right
    private bool isAxisRotated = false;

    void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("No GameManager found in the scene!");
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.material = normalMaterial;
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
    }

    public void SetBorder(bool showBorder)
    {
        if (showBorder)
        {
            _spriteRenderer.material = outlineMaterial;
        }
        else
        {
            _spriteRenderer.material = normalMaterial;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameManager.OnItemClicked(this);
    }
}