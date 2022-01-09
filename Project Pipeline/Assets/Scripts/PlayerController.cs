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
    private float shotCounter;
    private float muzzleFlashDisplayTime = (1/60);
    private float muzzleFlashCounter;

    public float maxHeat = 10f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    public Guns[] allGuns;
    private int selectedGun;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        UIController.instance.weaponTempSlider.maxValue = maxHeat;

        SwitchGun();

        Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        transform.position = newTrans.position;
        transform.rotation = newTrans.rotation;
    }

    private void Update() {

        //Move Char and Camera
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

        //Select Gun
        if(allGuns[selectedGun].muzzleFlash.activeInHierarchy){
            muzzleFlashCounter -= Time.deltaTime;

            if(muzzleFlashCounter <= 0f){
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }

        //Fire
        if(!overHeated){    
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }

            if(Input.GetButton("Fire1") && allGuns[selectedGun].isAutomatic){
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

        //Switching Guns
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f){
            selectedGun++;

            if(selectedGun >= allGuns.Length){
                selectedGun = 0;
            }
            SwitchGun();

        }else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f){
            selectedGun--;
            if(selectedGun < 0){
                selectedGun = allGuns.Length -1; //0
            }
            SwitchGun();
        }

        //Selecting Weapon With Number Keys
        for(int i = 0; i < allGuns.Length; i++){
            if(Input.GetKeyDown((i + 1).ToString())){
                selectedGun =i;
                SwitchGun();
            }
        }

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

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if(heatCounter >= maxHeat){ //UI Controller (Heat)
            heatCounter = maxHeat;

            overHeated = true;

            UIController.instance.overheatedMsg.gameObject.SetActive(true);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleFlashCounter = muzzleFlashDisplayTime;
    }

    private void LateUpdate() {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
        

    }

    void SwitchGun(){
        foreach (Guns gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }
}
