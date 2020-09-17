using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public float maxHp = 12f;
    public float hp;
    public float speed;
    public bool isMoving = false;
    public bool isDead = false;
    public bool canMove = true;
    public bool canReload = true;
    public int lookDirection = 1;
    public int moveDirection = 1;
    public float fadeSpeed = 1f;
    public float dashTime;
    public float startDashTime;
    public float dashSpeed;
    public bool isDashing = false;
    public bool canShoot = true;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 velocity;

    public Sprite deadSprite;
    public GameObject dashPS;
    public Weapon currentWeapon;
    public int[] equippedIDs = new int[2];
    public int scrollIndex = 0;

    public Image healthbar;
    public Color[] healthbarColors;
    public CameraShake shake;
    public FlashSprite flash;

    public bool inEvent = false;
    public bool inRangeOfNPC = false;
    public Collider2D NPCInRange;
    public DialogueManager dm;
    public Collider2D interactableInRange;
    public bool inRangeOfInteractable = false;
    public Event currentEvent;
    public GameObject[] lockpickingObjects;

    // Different kinds of events that the player can be apart of
    public enum Event {
        None,
        Dialogue,
        Lockpick
    };

    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = maxHp;
        dashTime = startDashTime;

        // Updates sprite, attributes, ammo, etc
        currentWeapon.InitializeWeapon();
    }

    void Update() {

        // Not in an event, perform normal actions
        if(!inEvent) {
            // NPC dialogue handling
            if(inRangeOfNPC && Input.GetKeyDown(KeyCode.E)) {
                if(NPCInRange != null) {
                    currentEvent = Event.Dialogue;
                    DialogueManager.StartConversation(NPCInRange.GetComponent<NPC>().conversation);
                    dm.npc = NPCInRange.gameObject;
                }
            }

            // Interactable handling (Chests, etc)
            if(inRangeOfInteractable && Input.GetKeyDown(KeyCode.E)) {
                if(interactableInRange != null) {
                    if(interactableInRange.GetComponent<Interactable>().isLocked) {
                        currentEvent = Event.Lockpick;
                    }
                }
            }

            DeathHandler();
            HealthBarHandler();
            MovementHandler();
            WeaponHandler();
            
        } else {
            // In an event, lock up the player
            velocity = Vector2.zero;
            isMoving = false;
            animator.SetBool("isMoving", isMoving);

            // Back out of event
            if(Input.GetKeyDown(KeyCode.Escape)) {
                inEvent = false;

                if(currentEvent == Event.Dialogue) {
                    dm.isStopped = true;
                    dm.ReadNext();
                }

                if(currentEvent == Event.Lockpick) {
                    foreach(GameObject obj in lockpickingObjects) {
                        Animator anim = obj.GetComponent<Animator>();

                        if(anim != null) {
                            anim.SetBool("inEvent", false);
                        }

                        StartCoroutine(TurnOffLockpickHUD());
                    }
                        
                } 

                currentEvent = Event.None;
            }
            
            // Handle skipping dialogue
            if(currentEvent == Event.Dialogue) {
                if(Input.GetKeyDown(KeyCode.E)) {
                    dm.ReadNext();
                }
            }
        }

        // Lockpicking event handling
        if(currentEvent == Event.Lockpick) {
            if(Input.GetKeyDown(KeyCode.E)) {
                inEvent = true;
                lockpickingObjects[0].SetActive(true);
            }
        }

        // Update speech if in dialogue
        if(DialogueManager.GetSpeechStatus()) {
            inEvent = true;
        } else {
            if(currentEvent == Event.Dialogue) {
                inEvent = false;
            }
        }
    }

    // Lockpick HUD disable
    public IEnumerator TurnOffLockpickHUD() {
        yield return new WaitForSeconds(1f);
        inEvent = false;
        lockpickingObjects[0].SetActive(false);
    }
   
    // Handles shooting, reloading, etc
    private void WeaponHandler() {

        // Shoot
        if(Input.GetMouseButton(0) && canShoot && currentWeapon.GetAmmo() > 0 && !currentWeapon.isReloading) {
            canShoot = false;
            canReload = false;
            currentWeapon.Shoot();
            
        }

        // Reload
        if ((Input.GetKeyDown(KeyCode.R) || currentWeapon.GetAmmo() == 0) && (currentWeapon.GetReserveAmmo() > 0 && currentWeapon.GetAmmo() != currentWeapon.GetMaxAmmo()) && (canReload && canShoot) && !currentWeapon.isShooting)
        {
            canReload = false;
            canShoot = false;
            currentWeapon.Reload();
        }

        // Switch
        if(Input.mouseScrollDelta.y != 0f && canReload && canShoot)
        {
            scrollIndex = (scrollIndex == 1 ? 0 : 1);
            currentWeapon.ID = equippedIDs[scrollIndex];
            currentWeapon.InitializeWeapon();
        }
    }

    // Handles Moving
    private void MovementHandler() {
        if(canMove) {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            velocity = input.normalized * speed;

            if(input.x > 0) {
                moveDirection = 1;
            } else if(input.x < 0) {
                moveDirection = 3;
            } else if(input.y < 0) {
                moveDirection = 4;
            } else if(input.y > 0) {
                moveDirection = 2;
            }

            // Mouse location -> direction
            if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x) {
                lookDirection = 1;
            } else {
                lookDirection = -1;
            }
            
            isMoving = (input.x  != 0 || input.y != 0);
            animator.SetBool("isMoving", isMoving);
            int targetRotation = (lookDirection == 1 ? 0 : 180);
            transform.localEulerAngles = new Vector3(0, targetRotation, 0);

            // Start dashing
            if(Input.GetMouseButtonDown(1) && !isDashing) {
                isDashing = true;
                canMove = false;
            }
        } else {
            // Handle dash 
            if(isDashing) {
                if(!dashPS.gameObject.activeSelf) {
                    dashPS.gameObject.SetActive(true);
                }
                
                if(dashTime <= 0 && isDashing) {
                    animator.SetBool("isMoving", false);
                    dashTime = startDashTime;
                    velocity = Vector2.zero;
                    isDashing = false;
                    canMove = true;
                    dashPS.gameObject.SetActive(false);
                } else if(isDashing && dashTime > 0) {
                    animator.SetBool("isMoving", true);
                    dashTime -= Time.deltaTime;
                    dashPS.transform.localScale = transform.localScale;

                    if(moveDirection == 1) {
                        velocity = Vector2.right * dashSpeed;
                    }
                    else if(moveDirection == 2) {
                        velocity = Vector2.up * dashSpeed;
                    }
                    else if (moveDirection == 3) {
                        velocity = Vector2.left * dashSpeed;
                    }
                    else if (moveDirection == 4) {
                        velocity = Vector2.down * dashSpeed;
                    }
                } 
            }  
        }
    }

    // Handles healthbar fill amount and color
    private void HealthBarHandler() {
        healthbar.fillAmount = 1 - (maxHp - hp) / maxHp;

        if(hp <= maxHp - (maxHp * 0.7)) {
            healthbar.color = healthbarColors[2];
        } else if(hp <= maxHp - (maxHp * 0.3)) {
            healthbar.color = healthbarColors[1];
        } else if(hp > maxHp - (maxHp * 0.3)) {
            healthbar.color = healthbarColors[0];
        }

        // TEST
        if(Input.GetKeyDown(KeyCode.Space)) {
            float d = 0.2f;
            StartCoroutine(TakeDamage(2f, d, false));
        }
    }

    // Take damage
    public IEnumerator TakeDamage(float damage, float decay, bool onceDone) {
        if(!onceDone) {
            shake.shakeDuration = 0.2f;
            flash.t = 0;
        }

        hp = (float) Math.Round((double) hp - decay, 2);
        damage = (float) Math.Round((double) damage - decay, 2);

        yield return new WaitForSeconds(0.05f);

        if(damage > 0) {
            StartCoroutine(TakeDamage(damage, decay, true));
        }
        
    }

    // Dying
    private void DeathHandler() {

        if(hp <= 0) {
            hp = 0;
            isDead = true;
            velocity = Vector2.zero;
            rb.velocity = velocity;
            spriteRenderer.sprite = deadSprite;
        }

        if(isDead) {
            animator.enabled = false;
            healthbar.transform.parent.gameObject.SetActive(false);
            canMove = false;
        }
    }

    // Update velocity
    void FixedUpdate() {
        rb.velocity = velocity * Time.deltaTime;
    }

    // Collisions -> Enter
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "NPC" && !inEvent && interactableInRange == null) {
            inRangeOfNPC = true;
            NPCInRange = other;
        }

        if(other.gameObject.tag == "Interactable" && !inEvent && NPCInRange == null) {
            inRangeOfInteractable = true;
            interactableInRange = other;
        }
    }

    // Collisions -> Exit
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "NPC" && !inEvent) {
            inRangeOfNPC = false;
            NPCInRange = null;
        }

        if(other.gameObject.tag == "Interactable" && !inEvent) {
            inRangeOfInteractable = false;
            interactableInRange = null;
        }
    }
}
