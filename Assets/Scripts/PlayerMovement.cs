using System.Threading;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;

    private bool attacking, dashattacking, isDashing;

    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private float dashPower = 4f;
    private float doubleTapTime;
    public float dashSpeed = 10f;
    KeyCode lastKeyCode;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {

    /**
         *? method from unity, Horizontal Input is a value of 1 when you are pressing left or right arrow, and 0 if are not */
        float horizontalInput = Input.GetAxis("Horizontal");

        /**
         *?Player Turning left and right while keeping the player size the same */

        if (horizontalInput > 0.001f && transform.localScale.x < 0)
            transform.localScale = new Vector3((-1) * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.001f && transform.localScale.x > 0)
            transform.localScale = new Vector3((-1) * transform.localScale.x, transform.localScale.y, transform.localScale.z);

        /**
         *?is player running */
        anim.SetBool("run", horizontalInput != 0 && isGrounded());

        /**
         *?is player jumping */
        if (body.velocity.y > 0.01)
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


        if (Input.GetKeyDown(KeyCode.D))
            {
              if (doubleTapTime > Time.time && lastKeyCode == KeyCode.D)  
                StartCoroutine(Dash(1f));
                else
                doubleTapTime = Time.time + 0.5f;

                lastKeyCode = KeyCode.D;
            }   


        if (Input.GetKeyDown(KeyCode.A))
            {
              if (doubleTapTime > Time.time && lastKeyCode == KeyCode.A)  
                StartCoroutine(Dash(-1f));
                else
                doubleTapTime = Time.time + 0.5f;

                lastKeyCode = KeyCode.A;
            }   
        
            


        /**
        *?is player falling */
        if (body.velocity.y < -0.00001 && !onWall())
            anim.SetBool("fall", true);
        else
            anim.SetBool("fall", false);


        if (wallJumpCooldown > 0.2f && !isDashing){
                
                if (!attacking || !dashattacking)
                body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

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

    private void FixedUpdate() {

        float horizontalInput = Input.GetAxis("Horizontal");

        if (!isDashing)
            body.velocity = new Vector2(horizontalInput * dashSpeed, body.velocity.y);

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
        if ((isGrounded() && !onWall()))
        {anim.SetBool("dash", true);
            body.velocity = new Vector2(dashPower, body.velocity.y);}
        
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


}
