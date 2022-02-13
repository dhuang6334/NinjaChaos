using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private float moveSpeed = 8f;

    public static Vector2 mousePos;
    public static Vector2 worldMousePos;


    public GameObject weapon;
    public GameObject projectileGO;
    public int projectileSpeed;

    private Vector3 originalWeaponPos;
    private float originalWeaponAngle;


    private Rigidbody2D rb2D;
    private PlayerInputActions InputActions;
    private InputAction movementAct;
    private InputAction mouseAct;

    private InputAction mousePressAct;
    private Vector2 movement;
    private bool facingRight;
    private Animator animator;
    
    private MapGen mapgenScript;

    private void Awake() 
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        InputActions = new PlayerInputActions();
        mapgenScript = GameObject.Find("Grid").GetComponent<MapGen>();

        originalWeaponPos = (Vector3) weapon.transform.position;
        originalWeaponAngle = (float) weapon.transform.eulerAngles.z;
        transform.position = new Vector3Int(4, 4, 0);
    }
    
    private void OnEnable() 
    {
        
        movementAct = InputActions.Player.Movement;
        mouseAct = InputActions.Player.Mouse;
        mousePressAct = InputActions.Player.MousePress;
        InputActions.Enable();
        //movementAct.Enable();
        //mouseAct.Enable();
    }

    private void OnDisable() 
    {
        InputActions.Disable();
        //movementAct.Disable();
        //mouseAct.Disable();

    }

    private void FixedUpdate() // called with physics?
    {
        movement = movementAct.ReadValue<Vector2>();
        mousePos = mouseAct.ReadValue<Vector2>();
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        if (worldMousePos.x >= transform.position.x)
        {
            facingRight = true;
        } else {
            facingRight = false;
        }
        
        UpdateWeapon();
        rb2D.MovePosition(rb2D.position + movement * moveSpeed * Time.fixedDeltaTime);
        weapon.transform.position = originalWeaponPos + transform.position;
        
        if (mousePressAct.ReadValue<float>() > 0)
        {
            
            Rigidbody2D projectile = Instantiate<Rigidbody2D>(projectileGO.GetComponent<Rigidbody2D>(), weapon.transform.position, weapon.transform.rotation) as Rigidbody2D;
            projectile.velocity = weapon.transform.TransformDirection(new Vector3(0, 0, projectileSpeed));
            
            
            
            /*  DEBUGGING For Carvable Tiles
            Tilemap wallTilemap = mapgenScript.tileMaps["Walls"];
            Vector3Int mousePosInt3 = Vector3Int.FloorToInt(new Vector3(worldMousePos.x, worldMousePos.y, 0));
            Color tileColor = wallTilemap.GetColor(mousePosInt3);
            wallTilemap.SetColor(mousePosInt3 , new Color(tileColor.r, tileColor.g, tileColor.b, 0.5f));
            
            Vector3Int mousePosInt3 = Vector3Int.FloorToInt(new Vector3(worldMousePos.x, worldMousePos.y, 0));
            Tilemap wallTilemap = mapgenScript.tileMaps["Walls"];
            mapgenScript.ClearAllTiles();
            mapgenScript.SetTiles();
            Debug.Log("Set");*/
        }
        
    }


    void UpdateWeapon()
    {
        float angle = Mathf.Atan2((worldMousePos.y - transform.position.y), Mathf.Abs(worldMousePos.x - transform.position.x)) * Mathf.Rad2Deg;
        
        weapon.transform.eulerAngles = new Vector3(weapon.transform.eulerAngles.x, weapon.transform.eulerAngles.y, originalWeaponAngle + angle);
    }
    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("sqrmag", movement.sqrMagnitude);
        
        

        if (facingRight)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            weapon.transform.eulerAngles = new Vector3(weapon.transform.eulerAngles.x, 0, weapon.transform.eulerAngles.z);
        } else {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            weapon.transform.eulerAngles = new Vector3(weapon.transform.eulerAngles.x, 180, weapon.transform.eulerAngles.z);
        }
        
        
        if ((facingRight && movement.x < 0) || (!facingRight && movement.x > 0))
        {
            animator.SetFloat("Multiplier", -1);
        } else {
            animator.SetFloat("Multiplier", 1);
        }
        
        
    }
}
