using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] string text;

        // References
        SpriteRenderer spriteRenderer;
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void UpdateSprite()
        {

        }

        #region Editor Scripts
#if UNITY_EDITOR
        public void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            EditorApplication.delayCall += UpdateSprite;
        }
#endif
#endregion
    }
}