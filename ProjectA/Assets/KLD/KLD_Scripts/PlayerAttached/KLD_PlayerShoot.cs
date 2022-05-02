using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class KLD_PlayerShoot : MonoBehaviour
{
    //private references
    KLD_PlayerAim playerAim;

    [Header("Public References")]
    [SerializeField] Transform canon;
    [SerializeField] Text ammoText;
    [SerializeField] KLD_TouchInputs touchInputs;
    [SerializeField] Button reloadButton;
    [SerializeField] Animator animator;

    [Header("Weapon"), Space(10)]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] KLD_WeaponSO weapon;

    [Header("Shooting Parameters"), Space(10)]
    [SerializeField] float zombieVerticalOffset = 1.5f;
    [SerializeField] LayerMask layerMask;

    bool canReload = false;

    //private local fields
    Vector3 impactPosition = Vector3.zero;
    Vector3 shootDirection = Vector3.zero;
    Vector3 selectedZombiePos = Vector3.zero;
    int bulletsToShoot = 0;
    int missingBullets = 0;

    //weapon delays
    float curShootDelay = 0f;
    float curBurstDelay = 0f;

    //weapon data
    int curBullets = 0;

    //animation
    //[HideInInspector] public bool isReloading;
    [ReadOnly] public bool isReloading = false;
    [HideInInspector] public bool isAiming = false;
    [HideInInspector] public bool isShooting;

    public enum WeaponState
    {
        HOLD,
        AIMING,
        SHOOTING,
        RELOADING,
        RELOADING_BPB
    }
    WeaponState weaponState = WeaponState.HOLD;


    //weapon mesh and anims references
    [Header("Weapon Mesh/Anims"), Space(10)]
    [SerializeField] Transform weaponHolderParent;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] TwoBoneIKConstraint rightHandIK;


    void Awake()
    {
        playerAim = GetComponent<KLD_PlayerAim>();
        InitWeaponMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
        weapon.ValidateValues();
        curBullets = weapon.GetCurAttributes().magazineSize;
        playerAim.targetPosAngleOffset = weapon.angleOffset;
        StartCoroutine(DelayedStart());
        UpdateUI();
        //InitWeaponMesh();
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        weapon.PoolBullets();
    }

    void OnEnable()
    {
        touchInputs.onReloadButton += Reload;
    }

    void OnDisable()
    {
        touchInputs.onReloadButton -= Reload;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessIsAimingAndShooting();

        AnimateWeaponState();

        canReload = !isReloading && curBullets < weapon.GetCurAttributes().magazineSize;
        reloadButton.interactable = canReload;

        if (isShooting && curBullets > 0 && !isReloading)
        {
            if (curShootDelay > weapon.shootDelay)
            {
                StartCoroutine(ShootCoroutine());
                curShootDelay = 0f;
            }
        }

        curShootDelay += Time.deltaTime;
        curBurstDelay += Time.deltaTime;
    }

    IEnumerator ShootCoroutine()
    {
        bulletsToShoot = weapon.GetCurAttributes().isBuckshot ?
        1 : weapon.GetCurAttributes().bulletsPerShot;

        for (int i = 0; i < bulletsToShoot; i++)
        {
            if (curBullets > 0)
            {
                DoShot();

                curBullets--;

                UpdateUI();

                if (i < bulletsToShoot - 1)
                    yield return new WaitForSeconds(weapon.GetCurAttributes().timeBetweenBullets);
            }
        }
    }

    void DoShot()
    {
        if (playerAim.GetSelectedZombie() != null)
        {
            selectedZombiePos = playerAim.GetSelectedZombie().transform.position;

            selectedZombiePos.y += zombieVerticalOffset;

            shootDirection = selectedZombiePos - canon.position;
        }
        else
        {
            shootDirection = canon.forward;
        }
        weapon.bullet.Shoot(weapon, canon.position, shootDirection, layerMask);
    }

    public void Reload()
    {
        if (canReload)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }


    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        if (weapon.reloadType == ReloadType.MAGAZINE)
        {
            yield return new WaitForSeconds(weapon.GetCurAttributes().reloadSpeed);
            curBullets = weapon.GetCurAttributes().magazineSize;
        }
        else if (weapon.reloadType == ReloadType.BULLET_PER_BULLET)
        {
            missingBullets = weapon.GetCurAttributes().magazineSize - curBullets;
            for (int i = 0; i < missingBullets + 1; i++)
            {
                curBullets++;
                UpdateUI();
                yield return new WaitForSeconds(weapon.GetCurAttributes().reloadSpeed);
            }
        }
        isReloading = false;
    }

    void UpdateUI()
    {
        ammoText.text = $"{curBullets} / {weapon.GetCurAttributes().magazineSize}";
    }

    float curShootAnimDelay = 0f;
    void ProcessIsAimingAndShooting()
    {
        isAiming = playerAim.GetIsPressingAimJoystick() && playerAim.GetSelectedZombie() != null ||
         playerAim.GetIsPressingAimJoystick() && playerAim.GetInputAimVector().sqrMagnitude > 0.1f;

        if (!isAiming)
        {
            isShooting = false;
            curShootAnimDelay = 0f;
        }
        else if (isAiming && !isShooting)
        {
            curShootAnimDelay += Time.deltaTime;
            if (curShootAnimDelay > weapon.shootAnimDelay)
            {
                isShooting = true;
            }
        }
    }


    void AnimateWeaponState()
    {
        if (isReloading)
        {
            weaponState = (weapon.reloadType == ReloadType.MAGAZINE ? WeaponState.RELOADING : WeaponState.RELOADING_BPB);
        }
        else if (!playerAim.GetIsPressingAimJoystick())
        {
            weaponState = WeaponState.HOLD;
        }
        else if (isAiming && !isShooting)
        {
            weaponState = WeaponState.AIMING;
        }
        else if (isAiming && isShooting)
        {
            weaponState = WeaponState.SHOOTING;
        }
        animator.SetInteger("weaponState", (int)weaponState);
    }



    public WeaponState GetWeaponState()
    {
        return weaponState;
    }



    #region Weapon Mesh and anims Initialization

    GameObject instantiedWH;
    KLD_WeaponHolder weaponHolder;

    void InitWeaponMesh()
    {
        if (weaponHolderParent.childCount > 3) { Destroy(weaponHolderParent.GetChild(3).gameObject); }

        //leftHandIK.enabled = false;
        //rightHandIK.enabled = false;

        instantiedWH = Instantiate(weapon.weaponHolder, Vector3.zero, Quaternion.identity, weaponHolderParent);

        instantiedWH.name = "WeaponHolder";

        instantiedWH.transform.localPosition = weapon.weaponHolder.transform.position;
        instantiedWH.transform.localRotation = weapon.weaponHolder.transform.rotation;


        weaponHolder = instantiedWH.GetComponent<KLD_WeaponHolder>();

        rightHandIK.data.target = weaponHolder.leftHandle;
        leftHandIK.data.target = weaponHolder.rightHandle;

        canon = weaponHolder.canon;


        animator.runtimeAnimatorController = weapon.animatorOverrideController;


        animator.enabled = false;
        animator.enabled = true;

        rigBuilder.Build();

        animator.enabled = false;
        animator.enabled = true;

        //leftHandIK.enabled = true;
        //rightHandIK.enabled = true;

    }

    #endregion
}
