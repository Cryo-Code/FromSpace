using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public string type = "";

    public Block consuming;

    // Start is called before the first frame update
    void Start() {
        
    }

    public void doConsume() {
        if (consuming) {
            consuming.consumed(this);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
