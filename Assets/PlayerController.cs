using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public GameObject player;

    public Rigidbody2D playerRigidbody;

    public SpriteRenderer playerSprite;

    GameObject inventory;

    public AnimationCurve loadCurve;

    public ParticleSystem playerParticleSystem;

    public GameObject baseBlock;

    public bool spawned = false;

    public int health = 250;

    public int maxHealth = 250;

    public float speed = 1;

    public float fireRate = 1;

    public int ammo = 0;
    public int clip = 10;

    public int maxClip = 10;

    public float loadTime = 0;

    public Text msg;

    public Color[] colors;

    public List<Item> inv = new List<Item>();

    public List<Block> blocks = new List<Block>();

    public List<GameObject> blockTemplates = new List<GameObject>();

    public Camera thisCamera;
    public Canvas canvas;

    public GameObject walkwayPrefab;

    public GameObject stars;

    public GameObject bullet;

    public AudioSource reload;

    public RectTransform ammoBar;
    public Text ammoText;
    public float ammoWidth = 220;

    public Text healthText;
    public RectTransform healthBar;

    public Text waveTimeText;
    public Text waveText;
    public RectTransform waveBar;

    public int wave = 0;

    public GameObject[] enemies;

    public GameObject explosion;

    public GameObject deadPanel;

    public GameObject tutorialPanel;

    public Dictionary<Vector2, Block> blockMap = new Dictionary<Vector2, Block>();

    // Start is called before the first frame update
    void Start() {
        blockMap[new Vector2(0, 0)] = baseBlock.GetComponent<Block>();
    }

    public void playAgain() {
        Time.timeScale = 1;
        SceneManager.LoadScene("main");
    }

    void animateIn(GameObject go, Vector3 pos) {
        Vector3 start = go.transform.position;
        LerpTo lerpTo = go.AddComponent<LerpTo>();
        lerpTo.target = start;

        int random = Random.Range(0, 4);

        /*if (random == 0) {
            go.transform.position = new Vector3(-20, 0, 0);
        } else if (random == 1) {
            go.transform.position = new Vector3(20, 0, 0);
        } else if (random == 2) {
            go.transform.position = new Vector3(0, -20, 0);
        } else if (random == 3) {
            go.transform.position = new Vector3(0, 20, 0);
        }*/

        go.transform.position = pos;
    }

    private int waveSpawns = 0;

    private int waveI = 0;

    private float waveTimer = 0;

    private float waveSpawnTimer = 0;

    public void nextWave() {
        wave++;

        int num = Mathf.CeilToInt((float) wave / 10f) * 10;

        waveSpawns = num;
        waveI = 0;
    }

    public void addBlock(GameObject spawnBlock) {
        Block block = blocks[Random.Range(0, blocks.Count)];

        Vector3 blockPos = block.transform.position;

        if (block.GetComponent<LerpTo>())
            blockPos = block.GetComponent<LerpTo>().target;

        int side = Random.Range(0, 4);

        Vector3 dirVec = new Vector3(0, 0, 0);
        int rotation = 0;
        string dir = "";

        if (side == 0) { // Up
            if (!block.up) {
                dirVec = new Vector3(0, 1, 0);
                dir = "up";
                rotation = 90;
            }else{
                addBlock(spawnBlock);
                return;
            }
        } else if (side == 1) { // Right
            if (!block.right) {
                dirVec = new Vector3(1, 0, 0);
                dir = "right";
                rotation = 0;
            }else{
                addBlock(spawnBlock);
                return;
            }
        } else if (side == 2) { // Down
            if (!block.down) {
                dirVec = new Vector3(0, -1, 0);
                dir = "down";
                rotation = 90;
            }else{
                addBlock(spawnBlock);
                return;
            }
        } else if (side == 3) { // Left
            if (!block.left) {
                dirVec = new Vector3(-1, 0, 0);
                dir = "left";
                rotation = 0;
            }else{
                addBlock(spawnBlock);
                return;
            }
        }

        GameObject inst = Instantiate(spawnBlock, blockPos + dirVec * 3f, Quaternion.identity);

        //GameObject walkway = Instantiate(walkwayPrefab, block.transform.position + dirVec * 1.5f, Quaternion.Euler(0, 0, rotation));

        Block instBlock = inst.GetComponent<Block>();
        
        if (dir == "up") {
            block.up = instBlock;
            instBlock.down = block;

            instBlock.downCol.SetActive(false);
            block.upCol.SetActive(false);
        } else if (dir == "right") {
            block.right = instBlock;
            instBlock.left = block;

            instBlock.leftCol.SetActive(false);
            block.rightCol.SetActive(false);
        } else if (dir == "down") {
            block.down = instBlock;
            instBlock.up = block;

            instBlock.upCol.SetActive(false);
            block.downCol.SetActive(false);
        } else if (dir == "left") {
            block.left = instBlock;
            instBlock.right = block;
            
            instBlock.rightCol.SetActive(false);
            block.leftCol.SetActive(false);
        }

        instBlock.pos = block.pos + new Vector2(dirVec.x, dirVec.y);

        blockMap[instBlock.pos] = instBlock;

        
        //animateIn(walkway);

        if (blockMap.ContainsKey(instBlock.pos + new Vector2(1, 0))) {
            Block r = blockMap[instBlock.pos + new Vector2(1, 0)];
            r.leftCol.SetActive(false);
            instBlock.rightCol.SetActive(false);

            r.left = instBlock;
            instBlock.right = r;

            animateIn(Instantiate(walkwayPrefab, instBlock.transform.position + new Vector3(1, 0, 0) * 1.5f, Quaternion.Euler(0, 0, 0)), block.transform.position);
        }
        
        if (blockMap.ContainsKey(instBlock.pos + new Vector2(0, -1))) {
            Block r = blockMap[instBlock.pos + new Vector2(0, -1)];
            r.upCol.SetActive(false);
            instBlock.downCol.SetActive(false);
            
            r.up = instBlock;
            instBlock.down = r;

            animateIn(Instantiate(walkwayPrefab, instBlock.transform.position + new Vector3(0, -1, 0) * 1.5f, Quaternion.Euler(0, 0, 90)) , block.transform.position);
        }

        if (blockMap.ContainsKey(instBlock.pos + new Vector2(-1, 0))) {
            Block r = blockMap[instBlock.pos + new Vector2(-1, 0)];
            r.rightCol.SetActive(false);
            instBlock.leftCol.SetActive(false);

            r.right = instBlock;
            instBlock.left = r;

            animateIn(Instantiate(walkwayPrefab, instBlock.transform.position + new Vector3(-1, 0, 0) * 1.5f, Quaternion.Euler(0, 0, 0)), block.transform.position);
        }

        if (blockMap.ContainsKey(instBlock.pos + new Vector2(0, 1))) {
            Block r = blockMap[instBlock.pos + new Vector2(0, 1)];
            r.downCol.SetActive(false);
            instBlock.upCol.SetActive(false);

            r.down = instBlock;
            instBlock.up = r;

            animateIn(Instantiate(walkwayPrefab, instBlock.transform.position + new Vector3(0, 1, 0) * 1.5f, Quaternion.Euler(0, 0, 90)), block.transform.position);
        }

        animateIn(inst, block.transform.position);

        blocks.Add(instBlock);
    }

    public void updateConsumed(Block block) {
        if (block.consumes == "food") {
            if (block.amount >= block.needed) {
                block.amount = 0;

                if (block.heals == 0) {
                    block.needed = Mathf.Min(15, block.needed + 1);
                    GameObject spawns = blockTemplates[Random.Range(0, blockTemplates.Count)];
                    if (spawns.name == "HealthBlock") {
                        for (int i = 0; i < blockTemplates.Count; i++) {
                            if (blockTemplates[i].name == "HealthBlock") {
                                blockTemplates[i] = blockTemplates[0];
                            }
                        }
                    }
                    addBlock(spawns);
                }else{
                    health += block.heals;

                    health = Mathf.Min(maxHealth, health);
                }
            }
        }
    }

    private bool playing = false;

    public int waveInterval = 60;

    // Update is called once per frame
    void Update() {
        if (spawned) {
            waveText.text = "WAVE " + wave;
            waveTimeText.text = Mathf.CeilToInt(waveInterval - waveTimer) + "s";

            float dist = 10f;
            
            float targetwb = ((1 - ((float) waveTimer / (float) waveInterval)) * ammoWidth);
            waveBar.sizeDelta = new Vector2(waveBar.sizeDelta.x + (targetwb - waveBar.sizeDelta.x) / (120f * Time.deltaTime), waveBar.sizeDelta.y);

            waveTimer += Time.deltaTime;
            waveSpawnTimer += Time.deltaTime;

            if (waveSpawnTimer > 0.4f && waveI < waveSpawns) {
                float rot = ((float) waveI / (float) waveSpawns) * Mathf.PI * 2;

                GameObject rand = enemies[Random.Range(0, enemies.Length)];

                GameObject enemy = Instantiate(rand, new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0) * dist, Quaternion.identity);

                enemy.GetComponent<Enemy>().playerController = this;
                if (wave >= 3) {
                    waveInterval = 45;
                    enemy.GetComponent<Enemy>().health = enemy.GetComponent<Enemy>().health * 2;
                }
                
                if (wave >= 6) {
                    waveInterval = 30;
                    enemy.GetComponent<Enemy>().health = Mathf.CeilToInt(enemy.GetComponent<Enemy>().health * 1.5f);
                }

                waveI++;
                waveSpawnTimer = 0;
            }

            if (waveTimer > waveInterval) {
                waveTimer = 0;
                nextWave();
            }
        }

        float target = (((float) clip / (float) maxClip) * ammoWidth);
        ammoBar.sizeDelta = new Vector2(ammoBar.sizeDelta.x + (target - ammoBar.sizeDelta.x) / (120f * Time.deltaTime), ammoBar.sizeDelta.y);
        ammoText.text = clip + " / " + ammo;

        float targeth = (((float) health / (float) maxHealth) * ammoWidth);

        healthBar.sizeDelta = new Vector2(healthBar.sizeDelta.x + (targeth - healthBar.sizeDelta.x) / (120f * Time.deltaTime), healthBar.sizeDelta.y);
        healthText.text = health + " / " + maxHealth;

        stars.transform.Rotate(new Vector3(0, 0, Time.deltaTime));
        
        if (Input.GetButtonDown("Jump") && !spawned) {
            spawned = true;

            tutorialPanel.SetActive(false);

            player.SetActive(true);

            playerParticleSystem.Play();

            playerSprite.color = colors[Random.Range(0, colors.Length)];
        }

        msg.text = "";

        float sizeZoom = 5f;

        if (dead) {
            sizeZoom = 20f;
        }

        Vector3 center = new Vector3(0, 0, 0);

        foreach (Block block in blocks) {
            center += block.transform.position;

            if (spawned) {
                if (Mathf.Abs(block.transform.position.x) > sizeZoom * 2) {
                    sizeZoom = Mathf.Abs(block.transform.position.x + Mathf.Sign(block.transform.position.x) * 2f) / 1.5f;
                }

                if (Mathf.Abs(block.transform.position.y) > sizeZoom) {
                    sizeZoom = Mathf.Abs(block.transform.position.y + Mathf.Sign(block.transform.position.y) * 2f);
                }
            }

            if (player.GetComponent<Collider2D>().IsTouching(block.slot)) {
                msg.text = block.getActiveText(this);

                RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

                Vector2 ViewportPosition = thisCamera.WorldToViewportPoint(player.transform.position + new Vector3(0, -1, 0));
                RectTransform cr = canvas.GetComponent<RectTransform>();

                msg.rectTransform.anchoredPosition = new Vector2(ViewportPosition.x * cr.rect.width, (1 - ViewportPosition.y) * -cr.rect.height);

                if (Input.GetButtonDown("Jump")) {
                    block.doAction(this);
                }
            }
        }

        thisCamera.orthographicSize += (sizeZoom - this.thisCamera.orthographicSize) / Mathf.Max(1.01f, 700f * Time.deltaTime);
        if (playing) {
            center += new Vector3(Random.value * shake, Random.value * shake);
            shake /= 700f * Time.deltaTime;
            thisCamera.transform.position += ((center / blocks.Count) - thisCamera.transform.position) / (700f * Time.deltaTime);
            thisCamera.transform.position = this.thisCamera.transform.position + new Vector3(0, 0, -10);
        }

        if (spawned && loadTime < 1f) {
            loadTime += Time.deltaTime;

            if (loadTime > 1f) {
                playerRigidbody.simulated = true;
                playing = true;

                

                addBlock(blockTemplates[0]);
            }

            baseBlock.transform.position = baseBlock.transform.position / Mathf.Max(1.01f, 70.02f * Time.deltaTime);
            player.transform.position = player.transform.position / Mathf.Max(1.01f, 70.02f * Time.deltaTime);
        }else if (spawned && !dead) {
            playerRigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * 1000 * Time.deltaTime);
        }


        Vector3 lastPos = player.transform.position + new Vector3(-1, -0.75f, 0);

        for (int i = inv.Count - 1; i >= 0; i--) {
            Vector3 myPos = inv[i].transform.position;

            if (inv[i].consuming) {
                inv[i].transform.position = myPos + ((inv[i].consuming.transform.position - myPos) / (200f * Time.deltaTime));

                if (Vector3.Distance(myPos, inv[i].consuming.transform.position) <= 0.1f) {
                    Item itm = inv[i];
                    inv.RemoveAt(i);
                    itm.doConsume();
                }
            }else{
                if (inv[i].type == "ammo") {
                    inv[i].transform.position = myPos + (((new Vector3(sizeZoom * 2, sizeZoom * -1f, 0)) - myPos) / (800f * Time.deltaTime));

                    if (Vector3.Distance(myPos, new Vector3(sizeZoom * 2, sizeZoom * -1f, 0)) <= 0.1f) {
                        Destroy(inv[i].gameObject);
                        inv.RemoveAt(i);
                        ammo += 10;

                    }
                }else{
                    inv[i].transform.position = myPos + (((new Vector3(1, 0, 0) + lastPos) - myPos) / (800f * Time.deltaTime));
                    lastPos = inv[i].transform.position;
                }
            }
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.position);

        float dx = Input.mousePosition.x - pos.x;
        float dy = Input.mousePosition.y - pos.y;

        player.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dy, dx) * Mathf.Rad2Deg - 90);

        Vector2 slope = new Vector2(dx, dy);
        slope.Normalize();

        if (clip > 0 && spawned && Input.GetButtonDown("Fire1") && !dead) {
            clip--;
            GameObject newBullet = Instantiate(bullet, player.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(dy, dx) * Mathf.Rad2Deg - 90));
            Bullet b = newBullet.GetComponent<Bullet>();
            b.fire(slope);

            if (clip <= 0) {
                int add = Mathf.Min(ammo - maxClip, 0);
                clip += maxClip + add;
                ammo -= maxClip + add;

                reload.Play();
            }
        }else{
            if (clip <= 0) {
                int add = Mathf.Min(ammo - maxClip, 0);
                clip += maxClip + add;
                ammo -= maxClip + add;

                reload.Play();
            }
        }

        if (dead) {
            Time.timeScale = Mathf.Max(Time.timeScale / 1.02f, 0.3f);
            if (Time.timeScale < 0.4f) {
                if (!deadPanel.active)
                    deadPanel.SetActive(true);
            }
        }
    }

    private float shake = 0;

    public void damage(int amount) {
        health -= amount;
        shake = 3f;

        if (health <= 0) {
            die();
        }
    }

    private bool dead = false;

    public void addForce(GameObject bb) {
        Rigidbody2D rb = bb.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.AddForce(new Vector2(Random.Range(100, 1000), Random.Range(100, 1000)));
    }

    public void die() {
        dead = true;
        Instantiate(explosion, new Vector3(0, 0, 0), Quaternion.identity);
        foreach (Block b in blocks) {
            addForce(b.gameObject);

            if (b.up) {
                addForce(b.up.gameObject);
            }

            if (b.down) {
                addForce(b.down.gameObject);
            }

            if (b.left) {
                addForce(b.left.gameObject);
            }

            if (b.right) {
                addForce(b.right.gameObject);
            }
        }
    }
}
