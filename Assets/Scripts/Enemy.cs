using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int health = 100;

    public int maxHealth = 100;

    public PlayerController playerController;

    public GameObject shoots;

    public Vector3 target;

    public Block targetBlock;

    private Rigidbody2D rb;

    private float shootTimer = 0;

    public float shootInterval = 3f;

    public Transform healthBar;

    public bool dead = false;

    public GameObject onDie;

    // Start is called before the first frame update
    void Start() {
        playerController = GameObject.FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();

        //findTarget();
    }

    public void findTarget() {
        if (dead)
            return;
        //Debug.DrawRay(transform.position, (new Vector3(0, 0, 0) - transform.position).normalized * 100, Color.yellow);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (new Vector3(0, 0, 0) - transform.position).normalized, Mathf.Infinity, 1 << 9);

        if (hit) {
            targetBlock = hit.collider.gameObject.transform.GetComponentInParent<Block>();

            if (targetBlock)
                target = targetBlock.transform.position - (new Vector3(0, 0, 0) - transform.position).normalized * 2f;
        }
    }

    // Update is called once per frame
    void Update() {
        if (health == maxHealth) {
            healthBar.localScale = new Vector3(0, healthBar.localScale.y, healthBar.localScale.z);
        }else{
            healthBar.localScale = new Vector3(healthBar.localScale.x + (((float) health / (float) maxHealth) - healthBar.localScale.x) / (700f * Time.deltaTime), healthBar.localScale.y, healthBar.localScale.z);
        }

        if (!dead && !targetBlock)
            findTarget();

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval && !dead && Vector3.Distance(transform.position, target) < 1f) {
            shootTimer = 0;

            GameObject b = Instantiate(shoots, transform.position, Quaternion.FromToRotation(transform.position, targetBlock.transform.position));
            b.GetComponent<Bullet>().fire((targetBlock.transform.position - transform.position).normalized);
        }

        if (dead) {
            transform.localScale /= Mathf.Max(1.04f, 70f * Time.deltaTime);

            if (transform.localScale.magnitude < 0.02f) {
                Destroy(gameObject);
            }
        }

        if (rb)
            rb.AddForce((target - transform.position) / (700f * Time.deltaTime));
    }

    public void damage(Bullet b) {
        if (dead)
            return;

        health -= b.damage;

        if (health <= 0) {
            die();
        }
    }

    public void die() {
        Instantiate(onDie, transform.position, Quaternion.identity);
        health = 0;
        dead = true;
        Destroy(rb);
    }
}
