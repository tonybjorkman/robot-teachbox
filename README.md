# robot-teachbox
Virtual teaching box for Melfa Movemaster industrial robotic arm.

![GUI](https://raw.githubusercontent.com/tonybjorkman/robot-teachbox/master/doc/screenshot_2020_08_20.png)

This is a WPF application which will send serial output according to the legacy Melfa Basic V commands for basic robot movements. 
This enables easy jogging of a robot without the physical teaching box connected and instead using a computer keyboard.

## Setup:

Select COM-port in drop-down list and press Connect prior to issuing any commands.

## Operation:

The program has different keymappings depending on which movement mode is selected, either XYZ or Join Jog.
Designed to be used with the numpad-keys.

| Key        | Command           | 
| ------------- |:-------------| 
| X      | Selects XYZ movement mode | 
| A      | Selects Joint movement mode      |   
| W | Outputs current pose of robot      |   
| O | 	Opens gripper |
|C | 	Closes gripper |
|Q | 	Queries for a composite movement of grapping object at polar coordinate.|
|P | 	Query for a composite movement of "pouring liquid" by moving the tooltip around the circumference of a 3D circle while rolling the tool.|
|Esc | 	Quits application|

### Movement Mode XYZ:
| Key        | Command           | 
| ------------- |:-------------| 
|5 | 	+Z|
|0 | 	-Z|
|2 | 	+X|
|8 | 	-X|
|6 | 	+Y|
|4 | 	-Y|
|Down | 	+ToolStraight|
|Up |   	-ToolStraight|


### Movement Mode Joints:
| Key        | Command           | 
| ------------- |:-------------| 
|0 | 	+Elbow|
|2 | 	-Elbow|
|5 | 	+Shoulder|
|8 | 	-Shoulder|
|6 | 	+Waist|
|4 | 	-Waist|
|Down | 	+Pitch|
|Up |   	-Pitch|
|Left | 	+Roll|
|Right |	-Roll|

