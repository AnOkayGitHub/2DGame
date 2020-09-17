using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Conversation conversation;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, 1f);
        if(col != null) {
            if(col.transform.tag == "Player") {
                int lookAngle = (player.transform.position.x > transform.position.x ?  0 : 180);
                transform.localEulerAngles = new Vector3(0, lookAngle, 0);
            }
        }
    }
}
