using System;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Handler;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Damage
{
    public sealed class DamageCommandHandler : UseCommandHandler<DamageCommandParameters>
    {
        public override bool Handle(DamageCommandParameters p, Vector2 pos)
        {
            Debug.Log($"Do damage {p.Damage} at {pos}");
            return true;
        }
    }
}