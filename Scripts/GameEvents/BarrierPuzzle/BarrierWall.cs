using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BarrierWall : MonoBehaviour
{
    [Header("Barrier Wall Settings")]
    [Tooltip("Initial charge value for the wall. Changing this updates the current charge.")]
    [SerializeField][Range(-2, 3)] public int initialCharge;

    [Tooltip("Current charge value for the wall.")]
    [SerializeField][Range(-2, 3)] public int charge;

    private int minCharge = -2;
    private int maxCharge = 3;

    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        Reset();
    }

    private void SetCharge(int newCharge)
    {
        
        charge = newCharge;

        if (charge != 0)
        {
            _collider2D.isTrigger = false;
            SetTransparent(false);
        }

        switch (charge)
        {
            case 0:
                BreakDown();
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                break;
            case -1:
                gameObject.layer = LayerMask.NameToLayer("Obstacle");
                gameObject.tag = "Obstacle";
                break;
            case -2:
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                _collider2D.isTrigger = true;
                break;
            default:
                gameObject.layer = LayerMask.NameToLayer("Relief");
                gameObject.tag = "Relief";
                break;
        }

        SetColor();
    }

    public void DecreaseCharge()
    {
        SetCharge(charge - 1);
    }

    public void BreakDown()
    {
        charge = 0;
        _collider2D.isTrigger = true;
        SetTransparent(true);
    }

    public void Reset()
    {
        SetCharge(initialCharge);
    }

    private void SetColor()
    {
        switch (charge)
        {
            case 1:
                _spriteRenderer.color = Color.cyan;
                break;
            case 2:
                _spriteRenderer.color = Color.yellow;
                break;
            case 3:
                _spriteRenderer.color = Color.red;
                break;
            case 4:
                _spriteRenderer.color = Color.magenta;
                break;
            case -1:
                _spriteRenderer.color = Color.white;
                break;
            case -2:
                _spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
                break;
            default:
                SetTransparent(true); // Default color for invalid charges
                break;
        }
    }

    private void SetTransparent(bool transparent)
    {
        Color currentColor = _spriteRenderer.color;
        currentColor.a = transparent ? 0 : 1;
        _spriteRenderer.color = currentColor;
    }

    // Called in the editor when a value is changed in the Inspector
    private void OnValidate()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // If initialCharge changes, set charge to the same value
        if (charge != initialCharge)
        {
            charge = initialCharge;
        }

        // Call SetCharge to ensure color updates in real-time
        SetCharge(charge);
    }
}
