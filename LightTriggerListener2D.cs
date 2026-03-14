using UnityEngine;

namespace shootstar
{
    [RequireComponent(typeof(Collider2D))]
    public class LightTriggerListener2D : MonoBehaviour
    {
        private bool _playerInside;
        private ILightReactive _reactive;

        private void Awake()
        {
            _reactive = GetComponent<ILightReactive>();
        }

        private void OnEnable()
        {
            ChangeColorManager.OnLightColorChanged += OnColorChanged;
        }

        private void OnDisable()
        {
            ChangeColorManager.OnLightColorChanged -= OnColorChanged;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Debug.Log($"[Listener] Player Enter {name}");
            _playerInside = true;

            // ⭐ 核心：第一次进入就同步当前颜色
            var colorMgr = other.GetComponent<ChangeColorManager>();
            Debug.Log($"[Listener] CurrentColor = {colorMgr?.CurrentColor}");
            if (colorMgr != null && _reactive != null)
            {
                Debug.Log($"[Listener] ApplyLight on enter");
                _reactive.ApplyLight(colorMgr.CurrentColor);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInside = false;
        }

        private void OnColorChanged(ColorType color)
        {
            if (!_playerInside) return;
            _reactive?.ApplyLight(color);
        }
    }
}
