using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script in charge of getting hit from attacks.
/// </summary>
public class UnitHurtbox : MonoBehaviour
{
    private Collider2D item;
    private BoxCollider2D box;

    public Color colorWire;
    public Color colorFill;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        item = null;
    }
    private void OnDrawGizmos()
    {
        if (box != null)
        {
            Gizmos.color = colorFill;
            Gizmos.DrawCube((Vector2)transform.position + box.offset, box.size);
            Gizmos.color = colorWire;
            Gizmos.DrawWireCube((Vector2)transform.position + box.offset, box.size);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 6)
        {
            if (CompareTag("Hitbox"))
            {
                if (item == null)
                {
                    Debug.LogWarning("Found hitbox!");
                    item = collision;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Avoid true collision if touching ground.
        if(collision.gameObject.layer != 6)
        {
            if (CompareTag("Hitbox"))
            {
                if (item == null)
                {
                    Debug.LogWarning("Found hitbox!");
                    item = collision;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (item != null)
        {
            if (item.CompareTag("Hitbox"))
            {
                item = null;
            }
        }
    }
}
