using UnityEngine;
using UnityEngine.SceneManagement;
using Animancer;

namespace Characters.States
{
    public class FlinchState : CharacterState
    {
        [SerializeField, Range(0, 1)]
        [Tooltip("The character's speed is multiplied by this value while flinching")]
        private float _FlinchMovementSpeedMultiplier;

        /// <inheritdoc/>
        public override float MovementSpeedMultiplier => Character.Health.CurrentHealth > 0 ? _FlinchMovementSpeedMultiplier : 0;

        /// <inheritdoc/>
        public override bool CanTurn => false;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The animation to play when the character gets hit by an attack")]
        private ClipTransition _FlinchAnimation;
        public ClipTransition FlinchAnimation => _FlinchAnimation;

        [SerializeField]
        [Tooltip("The animation to play when the character's health reaches 0")]
        private ClipTransition _DieAnimation;
        public ClipTransition DieAnimation => _DieAnimation;

        /************************************************************************************************************************/

        private void Awake()
        {
            _FlinchAnimation.Events.OnEnd += Character.StateMachine.ForceSetDefaultState;
            _DieAnimation.Events.OnEnd += () => Destroy(Character.gameObject);

            Character.Health.OnHitReceived += (hit) =>
            {
                if (hit.damage > 0)
                    Character.StateMachine.ForceSetState(this);
            };

            Character.Health.OnCurrentHealthChanged += (oldValue, newValue) =>
            {
                if (newValue <= 0)
                    Character.StateMachine.ForceSetState(this);
            };
        }

        /************************************************************************************************************************/

        public override void OnEnterState()
        {
            base.OnEnterState();
            var animation = Character.Health.CurrentHealth > 0 ? _FlinchAnimation : _DieAnimation;
            Character.Animancer.Play(animation);
        }

        /************************************************************************************************************************/

        public override bool CanExitState => false;

        /************************************************************************************************************************/

        /// <summary>Reloads the current scene.</summary>
        /// <remarks>
        /// This method isn't used in code, but in the example player prefab has it assigned to the end event of the
        /// <see cref="DieAnimation"/>. In a real game, you would need a script to properly handle the player's death.
        /// </remarks>
        public void ReloadCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scene.path, default);
#else
            SceneManager.LoadScene(scene.buildIndex);
#endif
        }
    }
}
