/******************************************************************************
 * Copyright (c) 2013-2017, Amsterdam University of Applied Sciences (HvA), 
 * Firebrush Studios and Centrum Wiskunde & Informatica (CWI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Contributors:
 *   * Christian Stiehl - christian.stiehl@hva.nl - HvA / Firebrush Studios
 *   * Riemer van Rozen - rozen@cwi.nl - HvA / CWI
 ******************************************************************************/
===========================================================================================
READ ME for the Live Game Design Tool 
by Christian Stiehl
version: 0.3
===========================================================================================

This READ ME is for a Unity Tool that can be used to create, import and export
Micro-Machination Diagrams. The tool can be used standalone or be combined with a
game. 

The executable of the tool is built for Windows 10.

The tool is made in Unity version 5.5.1f1

===========================================================================================

Tool Keyboard Shortcuts:
CTRL + M: Toggles the tool on or off (useful when integrating the tool into a game)(location in code: ToggleTool.cs)
CTRL + A: Adds new definition (tab) (location in code: TabManager.cs)
CTRL + C: Copy (copies selected node) (location in code: ModelViewController.cs)
CTRL + V: Paste (pastes current copied node) (location in code: ModelViewController.cs)
CTRL + X: Cut (copies selected node & deletes it) (location in code: ModelViewController.cs)
Tab:  	  Cycles definitions (tabs) (location in code: Tabmanager.cs)
Delete:   Deletes selected object (nodes/edges) (location in code: SelectNodeButton.cs & SelectEdgeButton.cs)