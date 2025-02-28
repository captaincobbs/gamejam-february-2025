using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class ButtonTimer
    {
        [Header("Sprite")]
        List<Sprite> Sprites;
        [NonSerialized] public int TimeRemaining;
    }
}
