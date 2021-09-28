using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.PlayerScripts {
    public class PlayerMovement : MonoBehaviour
    {

    	public float movementSpeed = 10.0f;
    	public float sprintSpeed = 20.0f;
        public float currentSpeed;

    	private CharacterController controller;
    	private Vector3 move;
    	private float y = 0.0f;
        private bool _isLeftShiftHeld;

    	void Awake() {
    		controller = GetComponent<CharacterController>();
    		currentSpeed = movementSpeed;
            _isLeftShiftHeld = false;
    	}

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (_isLeftShiftHeld) {
                currentSpeed = sprintSpeed;
            } else {
                currentSpeed = movementSpeed;
            }

        	if (Input.GetKeyDown(KeyCode.LeftShift)) {
        		currentSpeed = sprintSpeed;
                _isLeftShiftHeld = true;
        	}
        	if (Input.GetKeyUp(KeyCode.LeftShift)) {
        		currentSpeed = movementSpeed;
                _isLeftShiftHeld = false;
        	}

            float x = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
            y = 0.0f;
            if (Input.GetKey(KeyCode.Space)) {
            	y = currentSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.C)) {
            	y = -currentSpeed * Time.deltaTime;
            }

            move = (transform.forward * z) + (transform.right * x);
            move.y += y;
            move.y = Mathf.Clamp(move.y, -currentSpeed * Time.deltaTime, currentSpeed * Time.deltaTime);
            controller.Move(move);
        }
    }
}