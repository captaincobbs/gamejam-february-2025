using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class ButtonTimer : MonoBehaviour
    {
        [Header("Sprite")]
        public List<Sprite> Sprites;

        int timeRemaining;
        public int TurnsRemaining
        {
            get => timeRemaining;
            set
            {
                timeRemaining = value;
                SpriteRenderer.sprite = Sprites[TurnsRemaining + 1];
            }
        }

        SpriteRenderer spriteRenderer;
        SpriteRenderer SpriteRenderer
        {
            get
            {
                if (spriteRenderer == null)
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                }

                return spriteRenderer;
            }
        }
    }
}
