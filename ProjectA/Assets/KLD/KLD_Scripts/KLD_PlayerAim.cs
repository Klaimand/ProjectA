using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KLD_PlayerAim : MonoBehaviour
{
    [SerializeField] KLD_TouchInputs inputs;
    [SerializeField] Animator animator;
    [SerializeField] KLD_PlayerController controller;

    Rigidbody rb;

    [SerializeField] KLD_ZombieList zombieList;
    [SerializeField] Transform defaultTarget = null;


    [SerializeField] bool isPressingAimJoystick = false;
    [SerializeField, ReadOnly] Vector2 inputAimVector = Vector2.zero;
    Vector3 inputAimVector3 = Vector3.zero;
    Vector3 worldAimVector3 = Vector3.zero;




    [SerializeField, ReadOnly] KLD_ZombieAttributes selectedZombie = null;

    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] KLD_AimBehavior aimBehavior;

    Vector3 targetPos = Vector3.zero;

    Vector3 debugPosa = Vector3.zero;
    Vector3 debugPosb = Vector3.zero;

    [SerializeField, ReadOnly] KLD_PlayerAttributes playerAttributes;

    //shooting
    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool isShooting;

    //animation
    enum WeaponState
    {
        HOLD,
        AIMING,
        SHOOTING,
        RELOADING
    }
    WeaponState weaponState = WeaponState.HOLD;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPlayerAttributes();
    }

    // Update is called once per frame
    void Update()
    {
        isPressingAimJoystick = inputs.IsJoystickPressed(1);
        inputAimVector = inputs.GetJoystickNormalizedVector(1);

        ProcessPlayerAttributeAimVector();

        DoAim();

        DrawSelectedLine();

        isShooting = isPressingAimJoystick && selectedZombie != null;
        AnimateWeaponState();
    }

    void ProcessPlayerAttributeAimVector()
    {
        if (isPressingAimJoystick && inputAimVector.sqrMagnitude > 0.05f)
        {
            inputAimVector3.x = inputAimVector.x;
            inputAimVector3.y = 0f;
            inputAimVector3.z = inputAimVector.y;

            inputAimVector3 = controller.refTransform.transform.rotation * inputAimVector3;
        }
        else
        {
            inputAimVector3 = controller.refTransform.forward;
        }

        //playerAttributes.worldAimDirection.x = inputAimVector3.x;
        //playerAttributes.worldAimDirection.y = inputAimVector3.z;
        playerAttributes.worldAimDirection = inputAimVector3;

        Debug.DrawRay(transform.position, inputAimVector3, Color.magenta);
    }

    void DoAim()
    {
        selectedZombie = aimBehavior.GetZombieToTarget(zombieList.GetZombies(), playerAttributes);

        //targetPos = selectedZombie != null && isPressingAimJoystick ?
        //selectedZombie.transform.position :
        //transform.position + rb.velocity;

        if (selectedZombie != null && isPressingAimJoystick)
        {
            targetPos = selectedZombie.transform.position;
        }
        else if (rb.velocity.sqrMagnitude > 0.1f)
        {
            targetPos = transform.position + rb.velocity;
        }
        else
        {
            targetPos = transform.position + transform.forward;
        }

        targetPos.y = transform.position.y;
        transform.LookAt(targetPos, Vector3.up);
    }

    void DrawSelectedLine()
    {
        debugPosa = transform.position;
        debugPosb = targetPos;
        debugPosa.y = 1f;
        debugPosb.y = 1f;
        Debug.DrawLine(debugPosa, debugPosb, Color.red);
    }

    void InitPlayerAttributes()
    {
        if (playerAttributes == null)
        {
            playerAttributes = new KLD_PlayerAttributes { transform = this.transform };
        }
        else if (playerAttributes.transform == null)
        {
            playerAttributes.transform = transform;
        }
    }

    void AnimateWeaponState()
    {
        if (isReloading)
        {
            weaponState = WeaponState.RELOADING;
        }
        else if (!isPressingAimJoystick)
        {
            weaponState = WeaponState.HOLD;
        }
        else if (!isShooting)
        {
            weaponState = WeaponState.AIMING;
        }
        else
        {
            weaponState = WeaponState.SHOOTING;
        }
        animator.SetInteger("weaponState", (int)weaponState);
    }
}

[System.Serializable]
public class KLD_PlayerAttributes
{
    public Transform transform = null;
    public Vector3 worldAimDirection = Vector3.zero;
}