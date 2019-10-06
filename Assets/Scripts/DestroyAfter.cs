using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {
    public float after = 0;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        after -= Time.deltaTime;

        if (after <= 0)
            Destroy(gameObject);
    }
}
