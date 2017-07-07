===========================================================================================
READ ME for the Live Game Design Tool 
by Christian Stiehl
version: 0.1
===========================================================================================

This READ ME is for a Unity Tool that can be used to create, import and export
Micro-Machination Diagrams. The tool can be used standalone or be combined with a
game. 

===========================================================================================

Tool Keyboard Shortcuts:
CTRL + M: Toggles the tool on or off (useful when intigrating the tool into a game)(location in code: ToggleTool.cs)
CTRL + A: Adds new definition (tab) (location in code: TabManager.cs)
CTRL + C: Copy (copies selected node) (locaiton in code: ModelViewController.cs)
CTRL + V: Paste (pastes current copied node) (locaiton in code: ModelViewController.cs)
CTRL + X: Cut (copies selected node & deletes it) (locaiton in code: ModelViewController.cs)
Tab:  	  Cycles definitions (tabs) (location in code: Tabmanager.cs)
Delete:   Deletes selected object (nodes/edges) (location in code: SelectNodeButton.cs & SelectEdgeButton.cs)