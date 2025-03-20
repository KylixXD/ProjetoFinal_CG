using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int weaponDamage;
    public float reloadTime = 1f;
    public float maxDistance = 100f;
    public int ammunition; // Munição atual no pente
    public int maximumAmmo; // Munição total disponível
    public int ammunitionToReload; // Capacidade máxima do pente

    private bool isReloading = false;
    private AudioSource audioSource; // Componente de áudio da arma
    public AudioClip shootSound; // Som de disparo

    void Start()
    {
        // Garante que a arma comece carregada
        if (ammunition == 0 && maximumAmmo > 0)
        {
            Reload();
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot(Vector3 targetPosition)
    {
        if (isReloading) return;

        if (ammunition <= 0)
        {
            Debug.Log("Sem munição! Precisa recarregar.");
            return;
        }

        Vector3 bulletDirection = (targetPosition - bulletSpawn.position).normalized;
        GameObject bulletObject = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(bulletDirection));
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, 0.2f); // Toca o som do disparo
        }

        if (bulletScript != null)
        {
            bulletScript.SetDamage(weaponDamage);
        }

        ammunition--; // Reduz a munição ao atirar
    }

    public void Reload()
    {
        if (isReloading || maximumAmmo <= 0 || ammunition == ammunitionToReload) return;

        isReloading = true;
        Debug.Log("Recarregando...");

        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        int bulletsToReload = Mathf.Min(ammunitionToReload - ammunition, maximumAmmo);
        ammunition += bulletsToReload;
        maximumAmmo -= bulletsToReload;

        isReloading = false;
        Debug.Log("Recarregado! Munição atual: " + ammunition + "/" + maximumAmmo);
    }
}
