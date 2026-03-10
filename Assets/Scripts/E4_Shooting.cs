using UnityEngine;
using UnityEngine.InputSystem;

public class E4_Shooting : MonoBehaviour
{
    E4_InputAction input;

    [SerializeField]
    AudioClip[] Zoom;
    [SerializeField] AudioSource shootSFX;
    [SerializeField] AudioSource ZoomSFX;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;

    [SerializeField] Transform aim; // AimPosition
    [SerializeField] GameObject Rifle;
    [SerializeField] GameObject crossHair;
    [SerializeField] Transform aimClick;

    [SerializeField] Transform GunstartPos;
    [SerializeField] Transform AimstartPos;

    [SerializeField] Camera cam;
    [SerializeField] float aimFOV = 45f;
    [SerializeField] float normalFOV = 70f;
    [SerializeField] float fovSpeed = 10f;

    bool isAiming;

    private void Awake()
    {
        input = new E4_InputAction();
    }

    void Update()
    {
        float targetFOV = isAiming ? aimFOV : normalFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * fovSpeed
        );
    }
    private void OnEnable()
    {
        input.Player.Enable();

        input.Player.Fire.performed += OnShoot;

        input.Player.Aim.performed += StartAim;
        input.Player.Aim.canceled += StopAim;
    }

    private void OnDisable()
    {
        input.Player.Fire.performed -= OnShoot;

        input.Player.Aim.performed -= StartAim;
        input.Player.Aim.canceled -= StopAim;

        input.Player.Disable();
    }

    void OnShoot(InputAction.CallbackContext ctx)
    {
        shootSFX.Play();
        Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
    }

    void StartAim(InputAction.CallbackContext ctx)
    {
        isAiming = true;

        Rifle.transform.localPosition = aim.localPosition;
        Rifle.transform.localRotation = aim.localRotation;
        crossHair.transform.localPosition = aimClick.localPosition;
        ZoomSFX.clip = Zoom[0];
        ZoomSFX.Play();
    }

    void StopAim(InputAction.CallbackContext ctx)
    {
        isAiming = false;

        Rifle.transform.localPosition = GunstartPos.localPosition;
        Rifle.transform.localRotation = GunstartPos.localRotation;
        crossHair.transform.localPosition = AimstartPos.localPosition;
        ZoomSFX.clip = Zoom[1];
        ZoomSFX.Play();
    }
}