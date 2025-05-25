using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI
{
    [RequireComponent(typeof(Image))]
    public class ShootCooldownIndicatorController : MonoBehaviour
    {
        public PlayerTankController playerController;
        [SerializeField] private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            image.fillAmount = 1 - (Mathf.Clamp01(playerController.ShootCooldown) / playerController.MaxShootCooldown);
        }
    }
}