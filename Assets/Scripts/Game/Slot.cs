using UnityEngine;

public class Slot : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Vector2 Position => transform.position;

    public Vector2 Size => _spriteRenderer.bounds.size;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
