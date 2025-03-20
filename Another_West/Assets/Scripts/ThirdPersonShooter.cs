using UnityEngine;
using StarterAssets;
using TMPro;

public class ThirdPersonShooter : MonoBehaviour
{
    private GameObject player;
    public Transform debugTransform;
    public GameObject aimCamera;
    public float rotateSpeed = 15;
    public TMP_Text ammoText; // Referência à UI de munição
    private StarterAssetsInputs input;
    private Camera mainCam;
    private ThirdPersonController tpc;
    private Animator animator;
    private Weapon equippedWeapon;

    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        mainCam = Camera.main;
        tpc = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        equippedWeapon = GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        if (equippedWeapon == null) return;

        Vector3 aimPosition = Vector3.zero;
        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCam.ScreenPointToRay(screenCenterPos);

        if (Physics.Raycast(ray, out RaycastHit hit, equippedWeapon.maxDistance))
        {
            debugTransform.position = hit.point;
            aimPosition = hit.point;
        }
        else
        {
            debugTransform.position = ray.origin + ray.direction * equippedWeapon.maxDistance;
            aimPosition = ray.origin + ray.direction * equippedWeapon.maxDistance;
        }

        if (input.aim)
        {
            animator.SetLayerWeight(1, 1);
            tpc.SetRotateOnMove(false);
            aimCamera.SetActive(true);
            float yawCamera = mainCam.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), Time.deltaTime * rotateSpeed);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
            aimCamera.SetActive(false);
            tpc.SetRotateOnMove(true);
        }

        // Garantir que o disparo só aconteça se estiver mirando e pressionando o botão de disparo
        if (input.shoot && input.aim)
        {
            input.shoot = false; // Reseta o input de disparo
            equippedWeapon.Shoot(aimPosition); // Dispara se estiver mirando
        }

        // Recarregamento
        if (input.reload)
        {
            input.reload = false;
            equippedWeapon.Reload();
        }

        // Atualizar a UI de munição
        UpdateAmmoUI();
    }

    // Atualiza a UI de munição
    public void UpdateAmmoUI()
    {
        if (ammoText != null && equippedWeapon != null)
        {
            ammoText.text = $"{equippedWeapon.ammunition}/{equippedWeapon.maximumAmmo}";
        }
    }
}
