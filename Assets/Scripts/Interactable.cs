using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum Type {
        Chest,
        LockedChest
    };

    public Type type; 
    public bool isLocked;
    public bool isInteractedWith = false;
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize() {
        switch(type) {

            case Type.Chest:
                spriteRenderer.sprite = sprites[0];
                break;

            case Type.LockedChest:
                spriteRenderer.sprite = sprites[1];
                isLocked = true;
                break;
            
            default:
                spriteRenderer.sprite = sprites[0];
                break;
            

        }
    }
}
