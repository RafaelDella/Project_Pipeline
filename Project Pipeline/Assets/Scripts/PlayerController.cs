using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    public float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;

    public float jumpForce = 7.5f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask whatIsGround;

    public GameObject bulletImpact;
    public float timeBetweenShots = .1f;
    private float shotCounter;

    public float maxHeat = 10f, heatPerShot =1f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        UIController.instance.weaponTempSlider.maxValue = maxHeat;
    }

    private void Update() {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))  * mouseSensivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if(!invertLook){
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }else{
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if(Input.GetButton("Run")){
            activeMoveSpeed = runSpeed;
        }else{
            activeMoveSpeed = moveSpeed;
        }

        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if(charCon.isGrounded){
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, whatIsGround);

        if(Input.GetButtonDown("Jump") && isGrounded){
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

        charCon.Move(movement * Time.deltaTime);

        if(!overHeated){    
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }

            if(Input.GetButton("Fire1")){
                shotCounter -= Time.deltaTime;

                if(shotCounter <= 0){
                    Shoot();
                }
            }

            heatCounter -= coolRate * Time.deltaTime;
        }else{
            heatCounter -= overheatCoolRate * Time.deltaTime;
            if(heatCounter <= 0f){
                overHeated = false;

                UIController.instance.overheatedMsg.gameObject.SetActive(false);
            }
        }

        if(heatCounter < 0){
            heatCounter = 0f;
        }

        UIController.instance.weaponTempSlider.value = heatCounter;



        if(Input.GetButton("Pause")){
            Cursor.lockState = CursorLockMode.None;
        }else if(Cursor.lockState == CursorLockMode.None){
            if(Input.GetButtonDown("Fire1")){
                Cursor.lockState = CursorLockMode.Locked;
            }
        }


    }

    private void Shoot(){
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit)){
            Debug.Log(hit.collider.gameObject.name);

            GameObject obj_bulletImpact = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(obj_bulletImpact, 10f);
        }

        shotCounter = timeBetweenShots;

        heatCounter += heatPerShot;
        if(heatCounter >= maxHeat){
            heatCounter = maxHeat;

            overHeated = true;

            UIController.instance.overheatedMsg.gameObject.SetActive(true);
        }

    }

    private void LateUpdate() {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
        

    }
}
