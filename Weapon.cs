using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    
    
    private Collider weaponCollider;
    private Rigidbody2D rb2D;
    
    private Vector3 originalPos;
    private float originalAngle;
    private int speed;

    

    protected virtual void Shoot(GameObject projectileGO)
    {
        Rigidbody2D projectile = Instantiate<Rigidbody2D>(projectileGO.GetComponent<Rigidbody2D>(), transform.position, transform.rotation) as Rigidbody2D;
        projectile.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        weaponCollider = GetComponent<Collider>();
        rb2D = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(this);
    }
    protected virtual void FixedUpdate() {
        rb2D.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
