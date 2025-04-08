# Unity_test_code
Simple code for unity test, to start with unity C#. 
Using the Inspector, you can now define your conversation by populating the Dialogue Nodes list. For example:

Node 0:

Speaker Text: Character 1

Dialogue: السلام عليكم

Animation Trigger: Wave

Camera Focus: Set to your CameraFocus1 object

Dialogue Clip: (Assign an audio clip if available)

Choices: Leave empty for auto-advance.

Node 1:

Speaker Text: Character 2

Dialogue: وعليكم السلام

Animation Trigger: Nod

Camera Focus: Set to your CameraFocus2 object

Choices: Leave empty for auto-advance.

Node 2:

Speaker Text: Character 1

Dialogue: كيف حالك؟

Camera Focus: Set back to CameraFocus1

Choices: Add two choices:

Choice A: بخير، الحمد لله → NextNodeIndex = 3

Choice B: لست بخير → NextNodeIndex = 4

Node 3 (Branch A):

Speaker Text: Character 2

Dialogue: بخير، الحمد لله

(Further dialogue can be appended here.)

Node 4 (Branch B):

Speaker Text: Character 2

Dialogue: أوه، مع الأسف

(Add further follow-up if desired.)
