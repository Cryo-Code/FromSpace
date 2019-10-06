using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public bool moving = false;
    public float speed = 1f;

    private float life = 10f;

    public bool enemyBullet = false;

    public int damage = 25;

    public Rigidbody2D r2;

    public Vector2 slope;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (moving) {
            life -= Time.deltaTime;

            if (life <= 0f) {
                Destroy(gameObject);
            }
        }
    }

    public void fire(Vector2 _slope) {
        slope = _slope;
        
        moving = true;
        r2.AddForce(slope * speed * 1000f);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (enemyBullet) {
            if (other.transform.GetComponentInParent<Block>()) {
                transform.localScale = new Vector3(0, 0, 0);
                
                Destroy(this.GetComponent<Collider2D>());

                other.transform.GetComponentInParent<Block>().damage(damage);
            }
        }else{
            if (other.gameObject.GetComponent<Enemy>()) {
                other.gameObject.GetComponent<Enemy>().damage(this);
                transform.localScale = new Vector3(0, 0, 0);
                Destroy(this.GetComponent<Collider2D>());
            }
        }
    }
}
