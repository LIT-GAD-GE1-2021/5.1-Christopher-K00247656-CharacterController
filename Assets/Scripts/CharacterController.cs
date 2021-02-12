using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    public float jumpSpeed;

    public float fallMultiplier = 5.0f;

    public float lowJumpMultiplier = 3.0f;

    public float horizontalSpeed = 10;

    public float dashMultiplier = 1.0f;

    public bool dashEnabled;

    public float dashDuration = 0.5f;

    public LayerMask whatIsGround;

    public Transform groundcheck;

    public Transform headcheck;

    private float groundRadius = 0.5f;

    private float headRadius = 0.3f;

    private bool grounded;

    public bool platformOverHead;

    private bool jump;

    private bool duck;

    private bool doDash;

    private bool attackEnabled;

    private bool dashing;

    bool facingRight = true;

    private float hAxis;

    private Rigidbody2D theRigidBody;

    private Animator theAnimator;


    void Start()
    {
        // Set variables to a default state
        jump = false;
        grounded = false;
        duck = false;
        attackEnabled = false;
        doDash = false;
        dashing = false;
        dashEnabled = true;

        // Get the components we need
        theRigidBody = GetComponent<Rigidbody2D>();
        theAnimator = GetComponent<Animator>();

    }
    
    // Update is called once per frame
    void Update()
    {
        jump = Input.GetKeyDown(KeyCode.Space);

        hAxis = Input.GetAxis("Horizontal");

        doDash = Input.GetKeyDown(KeyCode.LeftShift);

        theAnimator.SetFloat("Speed", Mathf.Abs(hAxis));

        attackEnabled = Input.GetKeyDown(KeyCode.L);


        Collider2D colliderWeCollidedWith = Physics2D.OverlapCircle(groundcheck.position, groundRadius, whatIsGround);

        grounded = (bool)colliderWeCollidedWith;



        theAnimator.SetBool("Ground", grounded);

        float yVelocity = theRigidBody.velocity.y;

        theAnimator.SetFloat("vspeed", yVelocity);

        duck = Input.GetButton("Fire1");

        if (duck)
        {
            theAnimator.SetBool("Duck", true);
        }
        
        else
        {

            platformOverHead = false;

            if (grounded)
            {
                platformOverHead = Physics2D.OverlapCircle(headcheck.position, headRadius, whatIsGround);
            }
            theAnimator.SetBool("Duck", platformOverHead);
        }

        if (doDash == true && dashing == false && dashEnabled)
        {


             
            StartCoroutine("Dash");
        }

        if (grounded)
        {
            if ((hAxis > 0) && (facingRight == false))
            {
                Flip();
            }
            else if ((hAxis < 0) && (facingRight == true))
            {
                Flip();
            }
        }

        if (grounded && !jump) { 

            if (Input.GetKeyDown(KeyCode.L))
        {
            theAnimator.SetBool("attack", true);
        }
        else if (Input.GetKeyUp(KeyCode.L))
                {
                theAnimator.SetBool("attack", false);
            }
        }
    }

    void FixedUpdate()
    {
        
        if (grounded && !jump)
        {

            if (dashing == true)
            {
                theRigidBody.velocity = new Vector2(horizontalSpeed * hAxis * dashMultiplier, theRigidBody.velocity.y);
            }
            else
            {
                theRigidBody.velocity = new Vector2(horizontalSpeed * hAxis, theRigidBody.velocity.y);
            }
        }
        else if (grounded && jump)
        {
            // Set the velocity, this time we keep the horizontal velocity the same but change the vertical (y)
            // velocity to jumpSpeed
            theRigidBody.velocity = new Vector2(theRigidBody.velocity.x, jumpSpeed);
        }

        
        if (theRigidBody.velocity.y < 0)
        {

            theRigidBody.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;

        }
        else if ((theRigidBody.velocity.y > 0) && (!Input.GetKey(KeyCode.Space)))
        {

            theRigidBody.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
        }
        
    }

    private void Flip()
    {

        facingRight = !facingRight;
        
        Vector3 theScale = transform.localScale;

        //flip the x axis
        theScale.x *= -1;

        //apply it back to the local scale
        transform.localScale = theScale;  
    }

    private IEnumerator Dash()
    {
        dashing = true;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
    }
}
