using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Global Script
//Maintains state of different options, and handles toggling various UI elements
public class GlobalScript : MonoBehaviour {

	//Options
	//Head controls for translation and scaling
	public bool headControls;
	//Uniform/non-uniform scaling
	public bool uniformScaling;
	//Gravity on/off
	public bool useGravity;

	//UI elements for options
	public Canvas optionsCanvas;
	public Text headContText;
	public Text uniformContText;
	public Text gravityContText;

	//UI element for instructions
	public Canvas instrCanvas;
	public float instrTimer;

	//The interaction mode text
	public Text modeText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Keyboard / Mouse input handling
		// Toggle the option and update the text UI element 
		if (Input.GetKeyUp (KeyCode.H)) {
			headControls = !headControls;
			if(headControls){
				headContText.text = "Head Controls: On"; 
			}
			else{
				headContText.text = "Head Controls: Off";
			}
		}
		if (Input.GetKeyUp (KeyCode.U)) {
			uniformScaling = !uniformScaling;
			if(uniformScaling){
				uniformContText.text = "Uniform Scaling: On";
			}
			else{
				uniformContText.text = "Uniform Scaling: Off";
			}
		}
		if (Input.GetKeyUp (KeyCode.J)) {
			useGravity = !useGravity;
			toggleGravity();
			if(useGravity){
				gravityContText.text = "Gravity: On";
			}
			else{
				gravityContText.text = "Gravity: Off";
			}
		}
		if (Input.GetKeyUp (KeyCode.O)) {
			toggleOptions ();
		}
		if (Input.GetKeyUp (KeyCode.I)) {
			toggleInstructions ();
		}

		//Hide the instructions after a certain amount of time
		if (instrTimer > 0) {
			instrTimer -= Time.deltaTime;
			if (instrTimer <= 0) {
				toggleInstructions ();
			}
		}
	}

	public void toggleGravity(){
		//Forces don't affect an object with a rigidbody attached when isKinematic = true,
		//so we use this attribute to fake the turning of gravity on or off
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Manip");
		for(int i=0; i<objs.Length; i++){
			GameObject go = objs[i];
			go.rigidbody.isKinematic = !useGravity;
		}
	}

	//Show/Hide options
	void toggleOptions(){
		optionsCanvas.enabled = !optionsCanvas.enabled;
	}

	//Show/Hide instructions and hide/show the interaction mode text
	void toggleInstructions(){
		instrCanvas.enabled = !instrCanvas.enabled;
		modeText.enabled = !modeText.enabled;
	}
	
}
