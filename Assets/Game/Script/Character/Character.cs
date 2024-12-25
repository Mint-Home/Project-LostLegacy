using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float gravity = 10f;
    public float attackSlideSpeed = 0.06f;
    public float attackSlideDuration = 0.4f;
    public int coin = 0;
    public float rollSpeed = 9f;
    public float spawnDuration = 2f;
    public float currentSpawnTime = 0;
    public bool isInvincible = false;

    [HideInInspector]
    public Vector3 horizontalVelocity;
    [HideInInspector]
    public Vector3 verticalVelocity;
    [HideInInspector]
    public Vector3 velocity;
    
    CharacterController controller;
    PlayerInput input;
    Animator animator;
    Health health;
    DamageCaster damageCaster;
    

    //ENEMY //Maybe method of parent class could also work here.
    public bool isPlayer = true;

    
    NavMeshAgent agent;
    Transform targetPlayer;

    //STATE MACHINE
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Rolling, Spawning
    }
    public CharacterState currentState;

    private float attackStartTime;
    private float invincibleDuration = 2;
    private float attackAnimationDuration;

    //MATERIAL ANIMATION
    private MaterialPropertyBlock materialPropertyBlock;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    //ITEM DROP
    public GameObject itemToDrop;

    private Vector3 impactOnCharacter;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        damageCaster = GetComponentInChildren<DamageCaster>();

        skinnedMeshRenderer= GetComponentInChildren<SkinnedMeshRenderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(materialPropertyBlock);

        if (isPlayer)
        {
            input = GetComponent<PlayerInput>();
        }
        else
        {
            agent = GetComponent<NavMeshAgent>();
            targetPlayer = GameObject.FindWithTag("Player").transform;
            agent.speed = moveSpeed;
            SwitchStateTo(CharacterState.Spawning);
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                //PLAYER
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                }
                //ENEMY
                else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if(isPlayer)
                {
                    //Attack Slide
                    horizontalVelocity = Vector3.zero;

                    if(Time.time < attackStartTime + attackSlideDuration)
                    {
                        float lerpTime = (Time.time - attackStartTime) / attackSlideDuration;
                        horizontalVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if(input.AttackInput && controller.isGrounded)
                    {
                        string currentClipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if(currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f)
                        {
                            input.AttackInput = false;
                            SwitchStateTo(CharacterState.Attacking);
                            CalculatePlayerMovement();
                        }
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                if(impactOnCharacter.magnitude > 0.2f)
                {
                    horizontalVelocity = impactOnCharacter * Time.deltaTime;
                }
                
                impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);
                break;
            case CharacterState.Rolling:
                horizontalVelocity = transform.forward * rollSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawning:
                currentSpawnTime -= Time.deltaTime;
                if(currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        //CONCLUSION OF MOVEMENT
        if (isPlayer)
        {
            verticalVelocity = new Vector3(0f, -gravity, 0f);
            velocity = horizontalVelocity + verticalVelocity * Time.deltaTime;

            controller.Move(velocity);
            horizontalVelocity = Vector3.zero;
        }
    }

    private void CalculatePlayerMovement()
    {
        //STATE
        if (input.AttackInput && controller.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            attackStartTime = Time.time;
            return;
        }
        else if(input.RollInput && controller.isGrounded)
        {
            SwitchStateTo(CharacterState.Rolling);
            return;
        }

        //HORIZONTAL MOVE && ROTATION
        horizontalVelocity = Quaternion.Euler(0, -45, 0) * new Vector3(input.HorizontalInput, 0f, input.VerticalInput).normalized * moveSpeed * Time.deltaTime;


        if (horizontalVelocity.sqrMagnitude > 0)
        {
            //transform.rotation = Quaternion.LookRotation(horizontalVelocity);

            //smooth rotate
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(horizontalVelocity), 720 * Time.deltaTime);
        }


        //ANIMATION
        animator.SetFloat("speed", horizontalVelocity.magnitude);
        animator.SetBool("airborne", !controller.isGrounded);   //How to accomplish the transition between fall and move blend tree accroding to the speed      A:Event System
    }

    private void CalculateEnemyMovement()
    {
        //ENEMY SETTING && TRACING
        if (Vector3.Distance(targetPlayer.transform.position, transform.position) >= agent.stoppingDistance)
        {
            agent.SetDestination(targetPlayer.position);
            animator.SetFloat("speed", 1f);
        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetFloat("speed", 0f);
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    //Single state at a moment
    public void SwitchStateTo(CharacterState newState)
    {
        if(isPlayer)
        {
            input.ClearCache();
        }
        //EXITING STATE
        switch(currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                //initiate
                if(damageCaster != null)
                {
                    damageCaster.DisableDamageCaster();
                }

                if (isPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Rolling:
                break;
            case CharacterState.Spawning:
                isInvincible = false;
                break;
        }

        //ENTERING STATE
        switch(newState)
        {
            case CharacterState.Normal:
                break ;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    transform.rotation = Quaternion.LookRotation(targetPlayer.position - transform.position);
                }
                animator.SetTrigger("attack");
                break ;
            case CharacterState.Dead:
                controller.enabled = false;
                animator.SetTrigger("dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                animator.SetTrigger("beingHit");
                if (isPlayer)
                {
                    isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Rolling:
                animator.SetTrigger("roll");
                break;
            case CharacterState.Spawning:
                isInvincible = true;
                currentSpawnTime = spawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }
        currentState = newState;

        Debug.Log(currentState);
    }

    //ROLL, Bind to event
    public void OnRollExit()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    //ATTACK, Bind to animation event
    public void EnableDamageCaster()
    {
        damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        damageCaster.DisableDamageCaster();
    }
    
    public void OnAttackExit()
    {
        //if()
        
        SwitchStateTo(CharacterState.Normal);

    }

    //BEING ATTACK, Bind to event
    public void OnBeingHitExit()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPosition = new Vector3())
    {
        if (isInvincible)
        {
            return;
        }
        if(health != null)
        {
            health.ApplyDamage(damage);
        }

        if(!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPosition);
        }

        StartCoroutine(MaterialBlink());    // start a coroutine

        if(isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPosition, 10f);
        }
    }

    private void AddImpact(Vector3 attackPosition, float impactForce)
    {
        Vector3 impactDirection = transform.position - attackPosition;
        impactDirection.Normalize();
        impactDirection.y = 0;
        impactOnCharacter = impactDirection * impactForce;
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    IEnumerator MaterialBlink()     //IEnumerable??  base class?
    {
        materialPropertyBlock.SetFloat("_blink", 0.4f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);         //reset after 0.2s

        materialPropertyBlock.SetFloat("_blink", 0f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_target = -10f;
        float dissolveHeight;

        materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        while(currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissolveTime/dissolveTimeDuration);
            materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            skinnedMeshRenderer.SetPropertyBlock (materialPropertyBlock);

            Debug.Log("Current dissolve height :" + dissolveHeight);
            yield return null;
        }
        DropItem();
    }

    public void DropItem()
    {
        if(itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }

    //ITEM PICK UP
    public void PickUpItem(PickUp item)
    {
        switch(item.type)
        {
            case PickUp.ItemType.Heal:
                health.AddHealth(item.value);
                GetComponent<PlayerVFXManager>().PlayGetHealVFX();
                break;
            case PickUp.ItemType.Coin:
                AddCoin(item.value);
                break;
        }
    }

    public void AddCoin(int value)
    {
        coin += value;
    }

    public void RotateToTarget()
    {
        if(currentState != CharacterState.Dead)
        {
            transform.LookAt(targetPlayer);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveDuration = spawnDuration;
        float currentDissloveTime = 0;
        float dissolveHeight_start = -10f;
        float dissolveHeight_target = 20f;
        float dissolveHeight;

        materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        while(currentDissloveTime < dissolveDuration)
        {
            currentDissloveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissloveTime/dissolveDuration);
            materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);
            yield return null;
        }

        materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        SwitchStateTo(CharacterState.Normal);
    }
}
