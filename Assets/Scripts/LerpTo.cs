using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTo : MonoBehaviour {
    public Vector3 target;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        transform.position += (target - transform.position) / Mathf.Max(1.05f, 700f * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.02f)
            Destroy(this);
    }
}
