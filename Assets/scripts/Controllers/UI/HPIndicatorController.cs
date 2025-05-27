using Managers;
using UnityEngine;

namespace Controllers.UI
{
    public class HPIndicatorController : MonoBehaviour
    {
        public PlayerTankController playerController;
        [SerializeField] private RectTransform rectTransform;

        private float _defaultWidth;

        private void Start()
        {
            _defaultWidth = rectTransform.rect.width * playerController.MaxHealth;
        }

        private void Update()
        {
            float part = _defaultWidth * (playerController.Health / playerController.MaxHealth);
            rectTransform.sizeDelta = new Vector2(part, rectTransform.sizeDelta.y);
        }
    }
}