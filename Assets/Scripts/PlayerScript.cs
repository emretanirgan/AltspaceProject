using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//PlayerScript handles all interaction between the player and objects in the scene
//The different interaction methods are translation, rotation and scaling
//The user can either interact through mouse & keyboard or a gamepad
//Depending on options, the player's head position can also be used to interact with objects
public class PlayerScript : MonoBehaviour {

	//The object that the player is looking at/interacting with
	GameObject selectedObj;

	//Global game object contains information such as options
	//Interaction is different based on some of these options
	public GameObject global;
	GlobalScript gs;

	//The transform of the center of the two eyes
	Transform head;
	//Used for head controls
	Vector3 lastPos;
	Vector3 deltaHeadPos;
	
	public bool debugMode;

	//UI Text element that shows the current mode
	public Text modeText;

	//List of interaction modes
	//Easy to add a new one and just handle it in Update()
	public enum InteractMode{Translate, Rotate, Scale, None};

	//The current interaction mode
	public InteractMode mode;

	// Use this for initialization
	void Start () {
		deltaHeadPos = new Vector3(0.0f,0.0f,0.0f);
		head = this.GetComponent<OVRCameraRig>().centerEyeAnchor;
		gs = global.GetComponent<GlobalScript> ();
		mode = InteractMode.None;
		modeText.text = "Interaction Mode: " + mode.ToString();
	}

	// Update is called once per frame
	void Update () {
		//Cast ray out of the center of the two cameras
		Vector3 pos = head.position;
		Vector3 fwd = head.forward;
		RaycastHit hit;

		//Debugging - Draw the ray cast out of the head
		if (debugMode) {
			Debug.DrawRay (pos, fwd * 100);
		}

		// Keyboard / Mouse input handling
		if (selectedObj) {
			if (Input.GetButtonUp ("ToggleInteract")) {
				switchMode ();
			}
			if (Input.GetButtonUp ("NextMode")) {
				nextMode();
			}
			//Throw the object back
			if(Input.GetKeyUp (KeyCode.K)){
				selectedObj.rigidbody.isKinematic = false;
				selectedObj.rigidbody.AddForce(fwd*100, ForceMode.Impulse);
				mode = InteractMode.None;
			}
		}

		//Compute difference between last head position and current position
		//Used for head controls if enabled
		deltaHeadPos = head.position - lastPos;
		lastPos = head.position;

		//Interact with objects based on current interaction mode
		switch (mode){
		case InteractMode.None:
			if (Physics.Raycast (pos, fwd, out hit, 1000)) {
				if(debugMode){
					Debug.Log ("Hit an object!");
				}
				if ( hit.collider.gameObject.tag == "Manip" ){
					GameObject hitObj = hit.collider.gameObject;
					//Looking at a new object now
					if(selectedObj != hitObj){
						//Hide the outline of the previous object
						if(selectedObj){
							selectedObj.renderer.material.SetFloat("_Outline", 0.0f);
							//selectedObj.renderer.material.color -= new Color(0.1f,0.1f,0.1f);
							//selectedObj.renderer.material.color = Color.gray;
						}
						//Update the selected object, and show an outline
						selectedObj = hitObj;
						selectedObj.renderer.material.SetFloat("_Outline", 0.005f);
						//selectedObj.renderer.material.color = Color.red;
					}
				}
			}
			else{//The ray shooting out from the head doesn't hit any objects anymore 
				if(selectedObj){
					selectedObj.renderer.material.SetFloat("_Outline", 0.0f);
					//selectedObj.renderer.material.color = Color.gray;
				}
			}
			break;

		case InteractMode.Rotate:
			//Mouse rotation controls, click and drag up & down or left & right
			if(Input.GetKey (KeyCode.Mouse0)){
				selectedObj.transform.Rotate (fwd, Input.GetAxis("Mouse X")* -3.0f, Space.World);
				selectedObj.transform.Rotate (Vector3.Cross(fwd,head.up), Input.GetAxis("Mouse Y")* -3.0f, Space.World);
			}

			//Gamepad rotation controls
			selectedObj.transform.Rotate (fwd, Input.GetAxis("RightHoriz")* -6.0f, Space.World);
			selectedObj.transform.Rotate (Vector3.Cross(fwd,head.up), Input.GetAxis("RightVert")* -6.0f, Space.World);
			break;

		case InteractMode.Scale:
			//Mouse input scale controls
			//If uniform scaling is on, click and drag will scale object in all axes
			//If uniform scaling is off, left&right dragging will scale in X and up&down will scale in Y
			if(Input.GetKey (KeyCode.Mouse0)){
				float xaxis = Input.GetAxis("Mouse X");
				float yaxis = Input.GetAxis ("Mouse Y");
				Vector3 incVec;
				if(gs.uniformScaling){
					incVec = new Vector3(xaxis+yaxis,xaxis+yaxis,xaxis+yaxis);
				}
				else{
					incVec = new Vector3(xaxis,yaxis,0.0f);
				}
				selectedObj.transform.localScale += incVec;
			}

			//Gamepad scale controls
			float gamepadX = Input.GetAxis ("RightHoriz");
			float gamepadY = Input.GetAxis ("RightVert");
			Vector3 gamepadScale;
			if(gs.uniformScaling){
				gamepadScale = new Vector3(gamepadX+gamepadY,gamepadX+gamepadY,gamepadX+gamepadY);
			}
			else{
				gamepadScale = new Vector3(gamepadX,gamepadX,0.0f);
			}
			selectedObj.transform.localScale += gamepadScale;

			//Head positioning scale controls
			//If uniform scaling is on, moving head forward and back will scale the object up or down
			//If uniform scaling is off, the object will scale based on which axis the user's head is moving in
			if(gs.headControls){
				if(gs.uniformScaling){
					Vector3 incVec = new Vector3(deltaHeadPos[2],deltaHeadPos[2],deltaHeadPos[2]);
					selectedObj.transform.localScale += incVec*-8.0f;
				}
				else{
					selectedObj.transform.localScale += new Vector3(deltaHeadPos[0]*-5.0f, deltaHeadPos[1]*5.0f, deltaHeadPos[2]*-5.0f);
				}
			}
			break;

		case InteractMode.Translate:
			//Mouse input translation controls
			//Clicking and dragging up and down will move object forward or back
			if(Input.GetKey (KeyCode.Mouse0)){
				selectedObj.transform.Translate ( fwd * Input.GetAxis("Mouse Y"), Space.World);
			}
			selectedObj.transform.Translate ( fwd * Input.GetAxis("RightVert"), Space.World);

			//Head positioning translation controls
			//Moving the head forward will push the object away from the user, while moving the 
			//head back will pull the object towards the user
			if(gs.headControls){
				selectedObj.transform.Translate ( fwd * deltaHeadPos[2] * -50, Space.World);
			}
			break;
		}
	}

	//Called to turn the interaction mode off or on
	//Default mode when on is Translate
	public void switchMode() {
		if (mode == InteractMode.None) {
			selectedObj.transform.parent = head;
			mode = InteractMode.Translate;
			lastPos = head.position;
			selectedObj.rigidbody.isKinematic = true;
		} else {
			selectedObj.transform.parent = null;
			if(gs.useGravity){
				selectedObj.rigidbody.isKinematic = false;
			}
			mode = InteractMode.None;
		}
		modeText.text = "Interaction Mode: " + mode.ToString();
	}

	//Move to the next interaction mode
	public void nextMode() {
		if (mode == InteractMode.Translate) {
			selectedObj.transform.parent = null;
			mode = InteractMode.Rotate;
		} else if (mode == InteractMode.Rotate) {
			mode = InteractMode.Scale;
		} else if (mode == InteractMode.Scale) {
			selectedObj.transform.parent = head;
			mode = InteractMode.Translate;
		} else {
			mode = InteractMode.Translate;
		}
		modeText.text = "Interaction Mode: " + mode.ToString();
	}

}
