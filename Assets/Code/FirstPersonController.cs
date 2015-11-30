using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class FirstPersonController : MonoBehaviour {

    private float clock;
    public bool gunEquipped = false; //equiped gun?

    public float rotUpDown;// = 0;
    //public Vector3 speed;
    public float verticalSpeed;
    public float rotLeftRight;
    public float maxVelocityChange = 10.0f;
    public float mouseSensetivity = 1.0f;

    public float throwStrength;
    public float jumpHeight;
    public float gravity = 9.81f;
    public float upDownRange;
    private Vector3 playerPos;
    private Ray	ray;
    private RaycastHit rayHitDown;
    private bool isGrounded = true;
    public float moveSpeed;    
    public float totalJumpsAllowed;
    public float totalJumpsMade;
    private float floorInclineThreshold = 0.3f;


    public bool canCheckForJump;

    private bool isDead;
    private int messageEdited; //1 = not caring, 2 = not finished, 3 = finished

    //ACTION STRINGS
    //==================================================================
    private string Haim_str = "_Look Rotation";
    private string Vaim_str = "_Look UpDown";
    private string Strf_str = "_Strafe";
    private string FWmv_str = "_Forward";
    private string Fire_str = "_Fire";
    private string Jump_str = "_Jump";
    private string Dash_str = "_Run";
    private string Zoom_str = "_Zoom";
    private string Drop_str = "_Drop";
    private string Bury_str = "_Bury";
    //==================================================================

    //PERSONAL CHARACTER MODIFIERS
    public float runSpeedModifier;
    public float walkSpeedModifier;
    public float weightModifier;
    public float healthModifier;
    public float jumpHeightModifier;
    public float armorModifier;
    public float carryingSpeed;
    public float normalSpeed;

    public GameObject cloneToCopyUponDeath;
    public GameObject signToPlaceUponDeath;

    public bool isZoomed = false;


    //cliff control
    Ray rayOrigin;
    RaycastHit hitInfo;
    Vector3 rayOriginStart;
    bool ableToInteract;
    bool canMove;

    //For looking, we are assigning rotations, but we need original values
    //that aren't getting modified, so we can re-assign them.
    private Vector3 startingCameraRotation;
    private Vector3 newRotationAngle;

    //Anim Controller
    private Animator animController;

    //carrying object
    private bool isCarrying;
    private bool dropped;
    public Transform CarryLocation;
    private GameObject MyCarryObj;
    private Vector3 carryScaleNew;
    private Vector3 carryScaleOld;
    private GameObject burriedTV;

    private bool isBurying;
    void Awake () {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
    
    
    // Use this for initialization
    void Start () {
        isBurying = false;
        dropped = true;
        carryScaleOld = new Vector3(0.3f, 0.3f, 0.3f);
        carryScaleNew = new Vector3(0.05f, 0.05f, 0.05f);
        setControlStrings();
        //animController = gameObject.transform.GetChild (1).GetComponent<Animator> ();
        messageEdited = 1;
        canCheckForJump = true;
        newRotationAngle = new Vector3();
        startingCameraRotation = transform.GetChild(0).transform.localRotation.eulerAngles;
        totalJumpsMade = 0;
        rotLeftRight = 0.0f;
        isDead = false;
        isCarrying = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update ()
    {
        if(!isBurying && Input.GetAxis(Drop_str) == 1 && !dropped)
        {
            Drop();
        }
        if (!isBurying && Input.GetAxis(Fire_str) == 1 && !dropped)
        {
            Throw();
        }

        if (!isBurying && Input.GetAxis(Bury_str) == 1 && !dropped)
        {
            Bury();
        }

        if (!isBurying && isCarrying && !dropped && CarryLocation.childCount == 0)
        {
            //this only occurs if another squirrel steals your loot.
            MyCarryObj = null;
            isCarrying = false;
            dropped = true;
        }
    }


    void FixedUpdate () {

        if (Input.GetKeyDown("r"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        rayOrigin = new Ray(transform.position, transform.up*-1);

        //if you are able to reach something, anything important or not
        if (Physics.Raycast(rayOrigin, out hitInfo)) {
            //if (Time.time % 2f > 1.8f) { Debug.Log("Below me is: " + hitInfo.normal.y); }
            if (hitInfo.normal.y <= 0.4f)
            {
                canMove = false;
            }
            else
            {
                canMove = true;
            }
        }
        
        clock = clock + Time.deltaTime;

        if (!isBurying) {		
            //player rotation
            //left and right
            rotLeftRight = Input.GetAxis (Haim_str) * mouseSensetivity;
            transform.Rotate (0, rotLeftRight, 0);
            //up and down (with camera)
            rotUpDown -= Input.GetAxis (Vaim_str) * mouseSensetivity;
            rotUpDown = Mathf.Clamp (rotUpDown, -upDownRange, upDownRange);
            newRotationAngle.x = rotUpDown;
            newRotationAngle.y = startingCameraRotation.y;
            newRotationAngle.z = startingCameraRotation.z;
            transform.GetChild (0).transform.localRotation = Quaternion.Euler (newRotationAngle);

            //Jumping!!
            if (totalJumpsMade < totalJumpsAllowed && Input.GetButtonDown (Jump_str) && !isBurying) {
                totalJumpsMade += 1;
                isGrounded = false;
                canCheckForJump = false;

                GetComponent<Rigidbody>().velocity = new Vector3 (GetComponent<Rigidbody>().velocity.x, CalculateJumpVerticalSpeed (), GetComponent<Rigidbody>().velocity.z);

                Invoke ("AllowJumpCheck", 0.1f);
            }
            if (canMove && !isBurying) {
                Vector3 targetVelocity;
                targetVelocity = new Vector3 (Input.GetAxis (Strf_str), 0, Input.GetAxis (FWmv_str));
                
                targetVelocity = transform.TransformDirection (targetVelocity);
                targetVelocity *= moveSpeed;
                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = GetComponent<Rigidbody>().velocity;
                Vector3 velocityChange = (targetVelocity - velocity);

                velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                GetComponent<Rigidbody>().AddForce (velocityChange, ForceMode.VelocityChange);
                
                // Jump
                //Manager.say("Jumping action go. Jumps Made: " + totalJumpsMade + " Jumps Allowed: " + totalJumpsAllowed, "eliot");
            }

            GetComponent<Rigidbody>().AddForce (new Vector3 (0, -gravity * GetComponent<Rigidbody>().mass, 0));
            // We apply gravity manually for more tuning control
        }
        if (messageEdited != 1) {
            if(messageEdited == 3){
                messageEdited = 1;
            }else{
                Debug.Log(GameObject.Find ("TypeCanvas").transform.GetChild (1).GetChild(2).GetComponent<Text>().text);
                //wait till message is completely written.
                if(Input.GetKeyDown("return")){
                    GameObject sign = Instantiate(signToPlaceUponDeath, gameObject.transform.position+(Vector3.up*3.2f), gameObject.transform.rotation) as GameObject;
                    sign.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = 
                        GameObject.Find ("TypeCanvas").transform.GetChild (1).GetChild(2).GetComponent<Text>().text;
                    messageEdited = 3;
                    GameObject.Find ("TypeCanvas").transform.GetChild (1).GetChild(2).GetComponent<Text>().text = "";
                    GameObject.Find ("TypeCanvas").transform.GetChild (1).gameObject.SetActive (false);
                }
                //messageEdited = 3;
            }
        }
    }  

    private float CalculateJumpVerticalSpeed () {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * (jumpHeight+jumpHeightModifier-weightModifier) * gravity);
    }

    public bool getIsCarrying(){
        return isCarrying;
    }

    private void setControlStrings(){
        string pName = gameObject.name;

        if(pName.Contains("1")){
            Fire_str = "p1" + Fire_str;
            FWmv_str = "p1" + FWmv_str;
            Strf_str = "p1" + Strf_str;
            Haim_str = "p1" + Haim_str;
            Vaim_str = "p1" + Vaim_str;
            Jump_str = "p1" + Jump_str;
            Dash_str = "p1" + Dash_str;
            Zoom_str = "p1" + Zoom_str;
            Drop_str = "p1" + Drop_str;
            Bury_str = "p1" + Bury_str;
        }
        if (pName.Contains("4"))
        {
            Fire_str = "p4" + Fire_str;
            FWmv_str = "p4" + FWmv_str;
            Strf_str = "p4" + Strf_str;
            Haim_str = "p4" + Haim_str;
            Vaim_str = "p4" + Vaim_str;
            Jump_str = "p4" + Jump_str;
            Dash_str = "p4" + Dash_str;
            Zoom_str = "p4" + Zoom_str;
            Drop_str = "p4" + Drop_str;
            Bury_str = "p4" + Bury_str;
        }
        if (pName.Contains("2"))
        {
            Fire_str = "p2" + Fire_str;
            FWmv_str = "p2" + FWmv_str;
            Strf_str = "p2" + Strf_str;
            Haim_str = "p2" + Haim_str;
            Vaim_str = "p2" + Vaim_str;
            Jump_str = "p2" + Jump_str;
            Dash_str = "p2" + Dash_str;
            Zoom_str = "p2" + Zoom_str;
            Drop_str = "p2" + Drop_str;
            Bury_str = "p2" + Bury_str;
        }
        if (pName.Contains("3"))
        {
            Fire_str = "p3" + Fire_str;
            FWmv_str = "p3" + FWmv_str;
            Strf_str = "p3" + Strf_str;
            Haim_str = "p3" + Haim_str;
            Vaim_str = "p3" + Vaim_str;
            Jump_str = "p3" + Jump_str;
            Dash_str = "p3" + Dash_str;
            Zoom_str = "p3" + Zoom_str;
            Drop_str = "p3" + Drop_str;
            Bury_str = "p3" + Bury_str;
        }
    }

    public string GetFire_Str(){
        return Fire_str;
    }

    // piece of delays OnCollisionStay's ground check so we can't jump for 2 seconds after
    public void AllowJumpCheck()
    {
        //Manager.say("CAN CJECK JUMP", "eliot");
        canCheckForJump = true;
    }

    void OnCollisionStay(Collision floor){
        Vector3 tempVect;
        // we want to prevent isGrounded from being true and totalJumpsMade = 0 until 2 seconds later
        if(isGrounded == false && canCheckForJump){
            for(int i = 0; i < floor.contacts.Length; i++){
                tempVect = floor.contacts[i].normal;
                if( tempVect.y > floorInclineThreshold){
                    isGrounded = true;
                    totalJumpsMade = 0;
                    return;
                    //Manager.say("Collision normal is: " + tempVect);
                }
            }
        }
    }

    void OnTriggerEnter(Collider colObj)
    {
        Debug.Log("I COLLIDED WITH: " + colObj.gameObject.name);
        if (!isCarrying && colObj.gameObject.name.Contains("TV") && dropped)
            SetToCarrying(colObj.gameObject);
    }

    void OnTriggerStay(Collider colObj)
    {
        if (colObj.gameObject.name.Contains("TV") && dropped)
        {
            if(colObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled == false)
            {
                burriedTV = colObj.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider colObj)
    {
        if (colObj.gameObject.name.Contains("TV") && dropped)
        {
            if (colObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled == false)
            {
                burriedTV = null;
            }
        }
    }

    private void SetToCarrying(GameObject NewCarry)
    {
        Rigidbody colRig = NewCarry.GetComponent<Rigidbody>();
        NewCarry.GetComponent<BoxCollider>().enabled = false;
        isCarrying = true;
        dropped = false;
        colRig.isKinematic = true;
        NewCarry.transform.SetParent(CarryLocation, false);
        MyCarryObj = NewCarry;
        MyCarryObj.transform.localScale = carryScaleNew;
        MyCarryObj.transform.localPosition = Vector3.zero;
        MyCarryObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        MyCarryObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        moveSpeed = carryingSpeed;
    }

    private void Drop()
    {
        MyCarryObj.GetComponent<Rigidbody>().isKinematic = false;
        MyCarryObj.GetComponent<BoxCollider>().enabled = true;
        MyCarryObj.transform.SetParent(null);
        StartCoroutine(DelayCarrying());
        MyCarryObj.transform.localScale = carryScaleOld;
        MyCarryObj = null;
        dropped = true;
        moveSpeed = normalSpeed;
    }

    private void Throw()
    {
        MyCarryObj.GetComponent<Rigidbody>().isKinematic = false;
        MyCarryObj.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, throwStrength), ForceMode.VelocityChange);
        MyCarryObj.GetComponent<BoxCollider>().enabled = true;
        MyCarryObj.transform.SetParent(null);
        StartCoroutine(DelayCarrying());
        MyCarryObj.transform.localScale = carryScaleOld;
        MyCarryObj = null;
        dropped = true;
        moveSpeed = normalSpeed;
    }

    private void Bury()
    {
        if (!dropped)
        {
            StartCoroutine(DelayBurying());
        }
        else
        {
            StartCoroutine(TestIfBurried());
        }
    }

    IEnumerator DelayCarrying()
    {
        yield return new WaitForSeconds(1f);
        isCarrying = false;
    }

    IEnumerator TestIfBurried()
    {
        isBurying = true;
        yield return new WaitForSeconds(1.5f);
        if(burriedTV != null)
        {
            SetToCarrying(burriedTV);
            burriedTV = null;
        }
    }

    IEnumerator DelayBurying()
    {
        isBurying = true;
        yield return new WaitForSeconds(1.5f);
        MyCarryObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        MyCarryObj.transform.SetParent(null);
        StartCoroutine(DelayCarrying());
        MyCarryObj.transform.localScale = carryScaleOld;
        MyCarryObj = null;
        dropped = true;
        moveSpeed = normalSpeed;
        isBurying = false;
    }

    
}