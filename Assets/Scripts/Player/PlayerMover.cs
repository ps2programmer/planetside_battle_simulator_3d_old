using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.PlayerScripts 
{
    public class PlayerMover : MonoBehaviour, IPlayerMover
    {

        /* 
            The PlayerMovementHandler class handles the logic of how values are changed for the most part, this
            class uses those values returned by PlayerMovementHandler and feeds them to the character controller
            component to move the player. This class is a MonoBehaviour whereas the PlayerMovementHandler is not.
            This allows editor unit tests to be created to test that player movement works since editor unit tests
            don't work with MonoBehaviours (see Tests folder for the test scripts). IPlayerMover is not necessary
            to have but I made it to get into the habit of using more interfaces. Basically, the entire purpose of
            separating this logic into IPlayerMover and PlayerMovementHandler is to learn good practices for setting
            up unit tests in Unity, this is the very first time I have used unit tests so I began with something simple.
            This is based on following some YouTube tutorials.
        */

    	public float normalSpeed { get; set; }
    	public float leftShiftHeldSpeed { get; set; }
    	public float currentSpeed { get; set; }
        public bool updateSpeed { get; set; }
        
        private PlayerMovementHandler _playerMovementHandler;
    	private CharacterController _characterController;
        private bool _toggleToLeftShiftHeldSpeed;

    	void Awake() {
    		normalSpeed = 40.0f;
    		leftShiftHeldSpeed = 60.0f;
            currentSpeed = normalSpeed;
            updateSpeed = false;
    		_playerMovementHandler = new PlayerMovementHandler();
    		_characterController = GetComponent<CharacterController>();
            _toggleToLeftShiftHeldSpeed = false;
    	}

        void Update()
        {
        	if (Input.GetKeyDown(KeyCode.LeftShift)) {
        		_toggleToLeftShiftHeldSpeed = true;
        		currentSpeed = _playerMovementHandler.ChangeBetweenLeftShiftHeldSpeedAndNormalSpeed(normalSpeed, leftShiftHeldSpeed, _toggleToLeftShiftHeldSpeed);
        	}
        	if (Input.GetKeyUp(KeyCode.LeftShift)) {
        		_toggleToLeftShiftHeldSpeed = false;
        		currentSpeed = _playerMovementHandler.ChangeBetweenLeftShiftHeldSpeedAndNormalSpeed(normalSpeed, leftShiftHeldSpeed, _toggleToLeftShiftHeldSpeed);
        	}
            // updateSpeed flag is used when speed is updated in the Settings menu
            if (updateSpeed) {
                updateSpeed = false;
                if (_toggleToLeftShiftHeldSpeed) {
                    currentSpeed = leftShiftHeldSpeed;
                } else {
                    currentSpeed = normalSpeed;
                }
            }

        	float distance = currentSpeed * Time.deltaTime;
        	float x = Input.GetAxis("Horizontal") * distance;
        	float z = Input.GetAxis("Vertical") * distance;
        	float y = 0.0f;
        	if (Input.GetKey(KeyCode.Space)) {
        		y = distance;
        	}
        	if (Input.GetKey(KeyCode.C)) {
        		y = -distance;
        	}

        	Vector3 move = _playerMovementHandler.CalculateMovement(x, y, z, transform.forward, transform.right, distance);
        	_characterController.Move(move);
        }
    }
}