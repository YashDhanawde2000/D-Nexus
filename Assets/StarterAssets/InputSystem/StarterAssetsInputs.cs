using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{


		// Player Inputs
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool reload;
		public bool swapGun;
		public bool equipGun;
		public bool heal;
		public bool ultimate;

		// UI Inputs
		[Header("UI Input Values")]
		public bool pause;
		public bool resume;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        #region Input Events

#if ENABLE_INPUT_SYSTEM

        #region Player Input Events
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}

		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}

		public void OnSwapGun(InputValue value)
		{
			SwapGunInput(value.isPressed);
		}

		public void OnToggleEquip(InputValue value)
		{
			EquipGunInput(value.isPressed);
		}

		public void OnHeal(InputValue value)
		{
			HealInput(value.isPressed);
		}

		public void OnUseUltimate(InputValue value)
		{
			UltimateInput(value.isPressed);
		}

#endregion

        #region UI Input Events
        // UI Events
        public void OnPause(InputValue value)
		{
			PauseGameInput(value.isPressed);
		}

		public void OnResume(InputValue value)
		{
			ResumeGameInput(value.isPressed);
		}
        #endregion

#endif

        #endregion Input Events



        #region Input Functions

        #region Player Input Functions
        // Player Map
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void SwapGunInput(bool newSwapGunState)
		{
			swapGun = newSwapGunState;
		}

		public void EquipGunInput(bool newEquipGunState)
		{
			equipGun = newEquipGunState;
		}

		public void HealInput(bool newHealState)
		{
			heal = newHealState;
		}

		public void UltimateInput(bool newUltimateState)
		{
			ultimate = newUltimateState;
		}

        #endregion Player Input Functions

        #region UI Input Functions
        // UI Input Map
        public void PauseGameInput(bool newPauseGameState)
		{
			pause = newPauseGameState;
		}
		public void ResumeGameInput(bool newResumeGameState)
		{
			resume = newResumeGameState;
		}
        #endregion UI Input Functions

        #endregion Input Functions

        #region Cursor Functions
        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
        #endregion
    }

}