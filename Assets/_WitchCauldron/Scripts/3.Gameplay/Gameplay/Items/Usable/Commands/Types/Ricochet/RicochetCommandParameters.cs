using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Ricochet
{
    [CreateAssetMenu(
        fileName = "Ricochet Command",
        menuName = "Game/Gameplay/Items/Parameters/Ricochet",
        order = 3)]
    public class RicochetCommandParameters : UseCommandParameters
    {
        [field: Header("Targeting")]
        [field: SerializeField] public float StartRadius { get; private set; } = 1.5f;
        [field: SerializeField] public int BounceCount { get; private set; } = 3;
        [field: SerializeField] public float BounceRange { get; private set; } = 3f;
        [field: SerializeField] public bool CanHitSameEnemy { get; private set; }

        [field: Header("Animation")]
        [field: SerializeField] public GameObject ProjectileVisualPrefab { get; private set; }
        [field: SerializeField] public float JumpDuration { get; private set; } = 0.18f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 0.7f;
        [field: SerializeField] public bool RotateToMoveDirection { get; private set; } = true;
        [field: SerializeField] public float RotationOffset { get; private set; }

        [field: Header("Trail")]
        [field: SerializeField] public bool UseTrail { get; private set; } = true;
        [field: SerializeField] public Material TrailMaterial { get; private set; }
        [field: SerializeField] public float TrailTime { get; private set; } = 0.18f;
        [field: SerializeField] public float TrailStartWidth { get; private set; } = 0.24f;
        [field: SerializeField] public float TrailEndWidth { get; private set; } = 0f;
        [field: SerializeField] public Color TrailStartColor { get; private set; } = new(0.55f, 1f, 0.35f, 0.75f);
        [field: SerializeField] public Color TrailEndColor { get; private set; } = new(0.55f, 1f, 0.35f, 0f);

        [field: Header("Hit")]
        [field: SerializeField] public bool PlayImpactFxOnEachHit { get; private set; } = true;
        [field: SerializeField] public UseCommandParameters[] OnEachHitCommands { get; private set; }
    }
}
