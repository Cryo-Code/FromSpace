using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool doesBeat = false;
    public Animator typeAnim;
    public float beat = 0f;
    public float beatTime = 10f;

    public bool doesSpawn = false;

    public bool doesConsume = false;

    public string consumes = "food";

    public int heals = 0;

    public GameObject waitingItem;

    public GameObject spawns;

    public string activeText = "";

    public float timer = 0;

    public float spawnTime = 5;

    public BoxCollider2D slot;

    public int amount = 0;

    public int needed = 1;

    public Block down;
    public Block right;
    public Block up;
    public Block left;

    public GameObject upCol;
    public GameObject rightCol;
    public GameObject downCol;
    public GameObject leftCol;

    public GameObject typeSprite;

    public Vector2 pos = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start() {
        timer = spawnTime;
    }

    public string getActiveText(PlayerController pc) {
        if (doesSpawn) {
            if (waitingItem) {
                return "Press spacebar to pickup";
            }else{
                return Mathf.FloorToInt(timer) + " until harvest";
            }
        }else if (doesConsume) {
            foreach (Item item in pc.inv) {
                if (item.type == consumes) {
                    return activeText;
                }
            }

            return "No items to feed";
        }else{
            return "";
        }
    }

    public void consumed(Item item) {
        Destroy(item.gameObject);
        this.amount += 1;

        GameObject.FindObjectOfType<PlayerController>().updateConsumed(this);
    }

    public void doAction(PlayerController pc) {
        if (doesSpawn) {
            if (waitingItem) {
                pc.inv.Add(waitingItem.GetComponent<Item>());
                waitingItem = null;
            }
        }else if (doesConsume) {
            foreach (Item item in pc.inv) {
                if (item.type == consumes) {
                    item.consuming = this;
                    return;
                }
            }
        }
    }

    public void damage(int amount) {
        GameObject.FindObjectOfType<PlayerController>().damage(amount);
    }

    // Update is called once per frame
    void Update() {
        if (doesConsume) {
            typeSprite.transform.localScale = new Vector3(1f + ((float) amount / (float) needed), 1f + ((float) amount / (float) needed), 1);
        }

        if (doesBeat)
            beat -= 0.05f;
        
        if (doesBeat && beat <= 0f) {
            beat = beatTime;

            typeAnim.SetTrigger("heartbeat");
        }

        if (doesSpawn) {
            timer -= 1f * Time.deltaTime;

            if (timer <= 0f) {
                timer = spawnTime;

                if (!waitingItem) {
                    waitingItem = Instantiate(spawns, slot.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
