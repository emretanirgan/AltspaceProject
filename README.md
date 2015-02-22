# AltspaceVR Object Manipulation Project
#### by Emre Tanirgan

Welcome to my AltspaceVR project! This is a basic Unity scene that demonstrates various methods of interacting with objects in virtual reality. I've tested it with my DK2 on a computer running the 0.4.4 Oculus Runtime, so it should work as long as you have 0.4.4. It was built using the Unity version 4.6.2f1.

The project contains all source code along with a built executable called AltspaceProjectFinal.

### Instructions

When you run the project you'll find yourself in an environment surrounded by various objects. (It's freezing at Penn right now, so that definitely influenced some of my choices in texturing the ground and choosing a background.)

You can start looking around. Looking at an object will highlight it, and at that point you will be able to start interacting with the object.Yaw controls using the mouse and the analog stick are disabled, so you can use comfort mode rotation using Q & E and the Right and Left Bumpers. 
You will be able to start interacting with the object you're looking at once you hit Right Click or A on the Gamepad. Press Ctrl or B on the Gamepad to cycle between different interaction modes, which are Translate, Rotate and Scale. Hitting Right Click or A again will turn off interaction mode, and you will be able to look at and highlight different objects again. 

In any interaction mode, clicking and dragging your left mouse button or controlling your right analog stick will result in different actions. Here are the different modes and what you can do in them:

#### Translate Mode
In this mode, the object will basically be stuck to your head, and you can move around or look around to move the object with you. Clicking and dragging up & down with the left mouse button will push the object back or forward. Likewise, going up and down with the right analog will result in the same movement.

#### Rotate Mode
In this mode, you can click and drag left & right or up & down to rotate the object in 2 axes. The same effect can be achieved using the right analog stick as well.

#### Scale Mode
In this mode, you can click and drag your left mouse button to scale the object up or down. 

There is a UI element in the view at all times that displays the current interaction mode the player is in. 

### Additional Options & Interactions:

I threw in a couple of options in the project as I was trying out different interaction methods. They're not necessarily intuitive but I thought some of them were pretty interesting to experiment with. Here are the additional options I put in:

#### Head Position Controls: 
I think this one is pretty interesting. If turned on, this mode basically allows you to use positional tracking as a way to interact with objects. In the Translate Mode, you can push the object back by moving your head forward, and pull the object towards you by moving your head back. In the Scale Mode, if scaling is uniform you can move your head forward and back to scale the object up or down. If scaling is not uniform, moving your head in different axes will scale the object up or down in those axes. The default setting for this option is Off, but I'd suggest turning it on at some point to try it out.

#### Gravity: 
I added this in there just so I could see what it'd be like if gravity was acting on the objects. This option is Off by default. If you start interacting with an object, gravity turns off for that object until you exit interaction mode. You can also throw objects if gravity is on. 

#### Uniform Scaling: 
This option is On by default. It basically controls whether scaling is done to all axes at once, or on an individual axis basis. If it is Off, dragging the mouse left & right and up & down will result in scaling the object in different axes. If head controls are on, you can scale the object in all 3 axes.

#### Keyboard Inputs for different commands:

* O - Toggle options 
* H - Toggle head position controls
* J - Toggle gravity
* U - Toggle uniform scaling
* I - Toggle instructions
* K - (If gravity is on) Throw object --> a little buggy, didn't spend a lot of time tweaking the constant that multiplies the impulse force

### Technical/Implementation Details

There are two scripts that handle most of the behavior: PlayerScript and GlobalScript. GlobalScript is attached to the Global object, and manages player interaction with options and other behavior such as toggling instructions or options. PlayerScript handles all the object interaction.An enum called InteractMode defines the different interaction modes (None, Translate, Scale, Rotate), and based on what mode the player is currently in, Update() handles input differently. PlayerScript is attached to the OVRCameraRig object. 

To add an object in the scene that the user can start interacting with, it has to be tagged "Manip" and needs a collider and rigidbody. I made a material to get the highlighting effect, using a Diffused Outline shader I found online. When the user looks at an object, we change the outline width to get the highlight effect. I made a prefab called ManipPrefab to package all of this with a simple cube mesh. However, as you can tell from the scene, the interaction works with more complex meshes as well.

I followed a tutorial to get a basic UI up and running with the 4.6 UI system and Oculus, and then tweaked some things on my own to get the results I wanted. All of the UI elements are children of the CenterEyeAnchor object since I wanted them to stay fixed in the user's view.

The boat, earth and moon models/textures along with the skybox were all imported from the Unity Asset Store/online. 

### Known Issues/Bugs

Every once in a while the object you're looking at might not get highlighted. If this happens, just look around to other objects and it should go back to normal. 

When you turn gravity on, nothing seems to happen. However when you interact with an object and exit interaction mode, you'll see it fall on the ground. I'm not sure why this is happening, because toggling gravity on/off toggles the isKinematic property of the rigidbody on the objects, and the objects should just fall right after you toggle gravity on. 

Interacting with objects while gravity is on might result in weird collisions. One reason of this is because I made the colliders of the objects a bit bigger than they should be, to make it easier for the user to look at objects and select them. 

If you run the scene from the editor, it might throw a warning about the camera frustum. I think this is caused by the method I'm using to display the UI, and it doesn't seem to affect anything negatively. I followed a tutorial to get the Unity 4.6 UI to work with the OVR scripts (this was my first time using the 4.6 UI), so I didn't spend a lot of time to look into what exactly was going wrong. 

Enjoy!