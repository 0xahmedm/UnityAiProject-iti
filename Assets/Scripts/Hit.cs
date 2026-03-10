using Unity.VisualScripting;
using UnityEngine;

public class Hit : MonoBehaviour
{
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private RectTransform panel;
    [SerializeField] GameObject zombie;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] Transform explosionTransform;
    [SerializeField] AudioSource death;
    [SerializeField] GameObject canvasDamage;
    [SerializeField] Animator animator;
    [SerializeField] GameObject damageCanvas;
    [SerializeField] GameObject winCanvas;
    [SerializeField] GameObject ZombieHP;
    E4_InputAction input;
    bool isHit= false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isHit = animator.GetBool("isAttacking");
        canvasDamage.SetActive(isHit);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        if (collision.collider.CompareTag("Bullet"))
        {
            collision.gameObject.SetActive(false);
            GameObject blood = Instantiate(bloodEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(blood, 2f);
            Debug.Log("contact");

            ReducePanelWidth();

            if (panel.rect.width <= 0)
            {
                death.Play();
                zombie.SetActive(false);
                damageCanvas.SetActive(false);
                winCanvas.SetActive(true);
                ZombieHP.SetActive(false) ;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                input.Disable();
                Instantiate(explosionEffect, explosionTransform.position, Quaternion.identity);
            }
        }
    }

    void ReducePanelWidth()
    {
        float currentWidth = panel.rect.width;
        float newWidth = currentWidth - 5f;

        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }
}
