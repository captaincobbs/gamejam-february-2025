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
        public void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            EditorApplication.delayCall += UpdateSprite;
        }
        #endregion
    }
}