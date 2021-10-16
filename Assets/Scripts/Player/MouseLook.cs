using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleSimulator.PlayerScripts {
	public class MouseLook : MonoBehaviour
	{

		public float sensitivity = 100.0f;

		private float rotationX = 0.0f;
		private float rotationY = 0.0f;

		void Awake() {
			Cursor.lockState = CursorLockMode.Locked;
		}

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	    	if (Input.GetKeyDown(KeyCode.LeftAlt)) {
	    		if (Cursor.lockState == CursorLockMode.Locked) {
	    			Cursor.lockState = CursorLockMode.None;
	    		} else {
	    			Cursor.lockState = CursorLockMode.Locked;
	    		}
	    	}

	    	if (Cursor.lockState == CursorLockMode.Locked) {
	    		float mouseX = Input.GetAxis("Mouse X") * sensitivity;
		        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

		        rotationX -= mouseY;
		        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
		        rotationY += mouseX;

		        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
	    	}
	    }
	}
}