using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{

    GameObject PaletteUsing;
    public GameObject PrefabPalette;

    float VelocityBall = 60;
    bool PowerBall = false;
    bool PlayerisLocking = false;

    float Velocity = 4;


    Rigidbody2D body;

    #region Movements
    //Variables for movements
    [Header("Movement")]
    [SerializeField]
    [Range(0, 50)]
    float speedMoveHorizontal = 7.5f;
    float horizontalMovement;
    bool isMovingHorizontal = false;
    public bool canMove = true;

    //Variables for jump
    bool jumpInputPressed = false;
    bool wasPressingJumpInput = false;
    [SerializeField]
    [Range(0, 50)]
    float jumpForce = 8.5f;
    bool hasDoubleJumped = false;

    //Foot
    public bool isGrounded = false;
    Vector2 topLeft = new Vector2(-0.35f, -0.45f);
    Vector2 bottomRight = new Vector2(0.35f, -0.55f);
    public LayerMask groundMask;

    //Fall
    [SerializeField]
    [Range(0, 5)]
    float fallMultiplier = 2.5f;


    //Walled
    bool isWalled = false;
    #endregion

    float Counter_Time;
    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PlayerisLocking)
        {


            //Palette.position = smoothPosition;

            if (Input.GetMouseButton(1))
            {
                InitMove();
            }

            if (Input.GetMouseButton(0))
            {
                PowerBall = true;
                Velocity = 8;
                //PartSyst.Play();
            }
            else
            {
                Velocity = 4;
                PowerBall = false;
                //PartSyst.Stop();
            }
        }
    }

    //public override void OnStartLocalPlayer()
    //{
    //    GetComponent<SpriteRenderer>().color = Color.blue;
    //}


    private void Update()
    {

        
        if (true)//isLocalPlayer)
        {
            transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            return;
        }
        else
        {
            transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            return;
        }
    }

    public bool GetPowerBall()
    {
        return PowerBall;
    }

    public void InitMove()
    {
        if (gameObject.tag == "Player1")
        {
            transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            transform.position = new Vector3(-20, 0, 0);
        }
        if (gameObject.tag == "Player2")
        {
            transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            transform.position = new Vector3(20, 0, 0);
        }
    }


    public void DelockPlayer()
    {
        PlayerisLocking = false;
    }

    public void lockPlayer()
    {
        PlayerisLocking = true;
    }

    public void SetPalette(GameObject Palette){
        PaletteUsing = Palette;
    }

    public GameObject GetPalette()
    {
        return PaletteUsing;
    }

    public void Init(GameObject Prefab)
    {
       
        if (true){//isLocalPlayer) {
            PaletteUsing = Prefab;
          //  CmdInitPalette();
        }
        else
        {
        //    PaletteUsing = Instantiate(PrefabPalette);
        }

        
    }

}

