using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown, dashCooldown = 2.5f, dashTimeLeft, lastImageXpos, lastPlayerXpos, lastDash = -100;

    private bool attacking, dashattacking, isDashing;

    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float dashPower = 2f;
    private float dashTime = 0.2f;
    private float doubleTapTime;
    public float distanceBetweenImages = 0.1f;
    KeyCode lastKeyCode;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask stairsLayer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void AttemptToDash(){

        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;

    }

    private void Update()
    {

    /**
         *? method from unity, Horizontal Input is a value of 1 when you are pressing left or right arrow, and 0 if are not */
        float horizontalInput = Input.GetAxis("Horizontal");

        if(isOnStairs() && (horizontalInput == 0))
        {body.velocity = Vector2.zero;
         body.gravityScale = 0;}
        else if(!isOnStairs())
         body.gravityScale = 3;

        CheckInput();

        if(Input.GetButtonDown("Dash"))
        {

            if(Time.time >= (lastDash + dashCooldown))
            AttemptToDash();
        }

        


        /**
         *?Player Turning left and right while keeping the player size the same */

        if (horizontalInput > 0.001f && transform.localScale.x < 0)
            {
            transform.localScale = new Vector3((-1) * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            lastPlayerXpos = transform.position.x + (horizontalInput * 1);}
        else if (horizontalInput < -0.001f && transform.localScale.x > 0)
            transform.localScale = new Vector3((-1) * transform.localScale.x, transform.localScale.y, transform.localScale.z);

        /**
         *?is player running */
        anim.SetBool("run", horizontalInput != 0 && (isGrounded() || isOnStairs()));

        /**
         *?is player jumping */
        if (body.velocity.y > 0.01 && !isOnStairs())
            anim.SetBool("jump", true);
        else if (body.velocity.y > 0.01 && onWall())
            anim.SetBool("jump", true);
        else
            anim.SetBool("jump", false);

        /**
         *? Crouch animation no function yet*/
        if (body.velocity.x == 0 && (Input.GetKey(KeyCode.LeftControl)))
            anim.SetBool("crouch", true);
        else
            anim.SetBool("crouch", false);


        /**
         *? Normal 2 hit attack*/
        if (body.velocity.x == 0 && (Input.GetKey(KeyCode.K)))
            {doubleattack();}
        else
            {anim.SetBool("attacking", false);
            attacking = false;}

        /**
         *? Dash attack animation*/
        if (body.velocity.x == 0 && (Input.GetKey(KeyCode.J)))
            {heavyattack();}
        else
            {anim.SetBool("dashattacking", false);
            dashattacking = false;}


        if (Input.GetKey(KeyCode.L) && (horizontalInput == 1))
            {
              if (doubleTapTime > Time.time && lastKeyCode == KeyCode.L)  
                StartCoroutine(Dash(1f));
                else
                doubleTapTime = Time.time + 0.1f;

                lastKeyCode = KeyCode.L;

                
            }   


        if (Input.GetKey(KeyCode.H) && (horizontalInput == -1))
            {
              if (doubleTapTime > Time.time && lastKeyCode == KeyCode.H)  
                StartCoroutine(Dash(-1f));
                else
                doubleTapTime = Time.time + 0.1f;

                lastKeyCode = KeyCode.H;
               
            }   
        
            


        /**
        *?is player falling */
        if (body.velocity.y < -0.00001 && !onWall() && !isOnStairs() && !isGrounded())
            anim.SetBool("fall", true);
        else
            anim.SetBool("fall", false);
        
        
        if ((isGrounded() || isOnStairs()) && !(horizontalInput == 0))
                body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        if (wallJumpCooldown > 0.2f && !isDashing){
                
                

                if(onWall() && !isGrounded() && (body.velocity.y < 0.01) && (  body.velocity.y > -0.01)){

                    body.gravityScale = 0;
                    body.velocity = Vector2.zero;
                    anim.SetBool("onWall", true);
                }
                else{
                    body.gravityScale = 3;
                    anim.SetBool("onWall", false);
                }

                if (Input.GetKey(KeyCode.Space))
                    Jump();

        }
        else
            wallJumpCooldown += Time.deltaTime;
            


    }

    private void CheckInput(){


    }

    private void FixedUpdate() {

        

    }

    private void Jump()
    {
        if ((isGrounded() || onWall()))
        {body.velocity = new Vector2(body.velocity.x, jumpPower);}
        else if ((!isGrounded() && onWall())){
                wallJumpCooldown = 0f;
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5, 6);
                
        }


    }

    private void Dash()
    {
        float horiInput = Input.GetAxis("Horizontal");

        if ((isGrounded() && !onWall()))
        {anim.SetBool("dash", true);
            body.velocity = new Vector2(dashPower * horiInput, body.velocity.y);}
        
    }

    private void doubleattack(){
        anim.SetBool("attacking", true);
            attacking = true;
            //yield return new WaitForSeconds(0.2f);

    }
     private void heavyattack(){
         anim.SetBool("dashattacking", true);
            dashattacking = true;
            

    }

    private bool isGrounded()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        return raycastHit.collider != null;

    }
    private bool isOnStairs()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, stairsLayer);

        return raycastHit.collider != null;

    }

    private bool onWall()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

        return raycastHit.collider != null;

    }
    
    IEnumerator Dash (float direction){
        isDashing = true;
        anim.SetBool("dash", true);
        body.velocity = new Vector2(body.velocity.x, 0f);
        body.AddForce(new Vector2(dashPower * direction, 0f), ForceMode2D.Impulse);
        float gravity = body.gravityScale;
        body.gravityScale = 0;
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("dash", false);
        isDashing = false;
        body.gravityScale = gravity;


    }
    private void CheckDash()
    {

        if (isDashing){


            if(dashTimeLeft > 0)
            {dashTimeLeft -= Time.deltaTime;

            if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;

            }
            
        }

        if(dashTimeLeft <= 0 || onWall())
        {
            isDashing = false;

        }

        }

        
    }


}
