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

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using MM.Parser;
using MM.Model;
using MM;
using System;

/// <summary>
/// Opens a micro-machinations file and creates a visual diagram from it
/// </summary>
public class OpenFileScript : MonoBehaviour, MM.Runtime.Checking, MM.Parser.Parsing
{
    public UnityEngine.UI.Button yourButton;
    public string file;
    public string path = null;
    MM.Machine machine;
    public Program program;
    RMPatch.List<MM.Model.Definition> definitions;
    public TabManager tabman;
    public GameObject tab;
    public GameObject tempNode;

    public ModelController mc;

    List<GameObject> definitionNodes = new List<GameObject>();
    public List<GameObject> tabs = new List<GameObject>();
    int index = 1;

    /// <summary>
    /// Receives error/warning messages from the parser
    /// TODO: Use the tools own console instead of unity debugger.
    /// </summary>
    /// <param name="message">The error message received</param>
    public void receive(MM.Parser.ParserMessage message)
    {
        Debug.Log(message.toString());
    }
    /// <summary>
    /// Receives error/warning messages from the runtime model
    /// TODO: Use the tools own console instead of unity debugger.
    /// </summary>
    /// <param name="message">The error message received</param>
    public void receive(MM.Runtime.CheckerMessage message)
    {
        Debug.Log(message.toString());
    }

    /// <summary>
    /// Subscribes the TaskOnClick function to the buttons onClick event
    /// </summary>
    void Start()
    {
        machine = new MM.Machine(this, this, Console.Out, Console.Error);
        UnityEngine.UI.Button btn = yourButton.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
    /// <summary>
    /// When clicked opens the file browser, if a valid path is selected imports a micro-machinations file and creates visual diagram
    /// </summary>
    void TaskOnClick()
    {
        OpenFileDialog open = new OpenFileDialog();
        open.ShowDialog();
        path = open.FileName.ToString();
        if(path != null)
        {
            RMPatch.StringBuffer buf = new RMPatch.StringBuffer();
            program = machine.parseFile(path);
            definitions = program.getDefinitions();
            tabman.AddTab("Global");
            tab = tabman.GetLastTab();
            tabs.Add(tab);
            foreach (MM.Model.Node n1 in program.getNodes())
            {
                CreateNodes(n1, buf);
            }
            foreach (MM.Model.Definition def in definitions)
            {
                tabman.AddTab(def.getName().toString());
                tab = tabman.GetLastTab();
                tabs.Add(tab);
                
                foreach (MM.Model.Node node in def.getNodes())
                {
                    CreateNodes(node, buf);
                }

                foreach (GameObject obj in definitionNodes)
                {
                    obj.GetComponent<SelectNodeButton>().UpdateDefinitionReferences();
                }
            }
            foreach (MM.Model.Edge e1 in program.getEdges())
            {
                tab = tabs[0];
                CreateEdges(e1, buf);
            }
            foreach (MM.Model.Definition d1 in program.getDefinitions())
            {
                tab = tabs[index];
                index++;
                foreach (MM.Model.Edge edge in d1.getEdges())
                {
                    CreateEdges(edge, buf);
                }
            }
        }
        if(program != null)
        {
            mc.SetProgram(program);
            mc.SetMachine(machine);
        }
    }

    /// <summary>
    /// Creates a visual node based on the information in the MM.Model.Node given
    /// </summary>
    /// <param name="node">The Micro-Machinations node to be visualized</param>
    /// <param name="buf">String buffer</param>
    private void CreateNodes(MM.Model.Node node, RMPatch.StringBuffer buf)
    {
        Location2D.Coordinate tempcoord = node.getSource().getLocation2D().getCoordinates()[0];
        if (node.getBehavior() is PoolNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Pool, new Vector2(tempcoord.getX(), tempcoord.getY()));
            tempNode = tab.GetComponentInChildren<ModelViewController>().tempNode;
            PoolNodeBehavior pnb = (PoolNodeBehavior)node.getBehavior();
            tempNode.GetComponent<SelectNodeButton>().UpdateAt((int)pnb.getAt());
            if (pnb.getExp() != null)
            {
                buf.clear();
                pnb.getExp().toString(buf);
                tempNode.GetComponent<SelectNodeButton>().UpdateAdd(buf.toString());
            }
            if (pnb.getDefinition() != null)
            {
                tempNode.GetComponent<SelectNodeButton>().UpdateOfType(pnb.getDefinition().getName().toString());
                definitionNodes.Add(tempNode);
            }
        }
        else if (node.getBehavior() is SourceNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Source, new Vector2(tempcoord.getX(), tempcoord.getY()));
        }
        else if (node.getBehavior() is DrainNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Drain, new Vector2(tempcoord.getX(), tempcoord.getY()));
        }
        else if (node.getBehavior() is GateNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Gate, new Vector2(tempcoord.getX(), tempcoord.getY()));
        }
        else if (node.getBehavior() is ConverterNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Converter, new Vector2(tempcoord.getX(), tempcoord.getY()));
        }
        else if (node.getBehavior() is RefNodeBehavior)
        {
            tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Reference, new Vector2(tempcoord.getX(), tempcoord.getY()));
        }

        tempNode = tab.GetComponentInChildren<ModelViewController>().tempNode;
        if (tempNode)
        {
            tempNode.GetComponent<SelectNodeButton>().idNode = node;
            tempNode.GetComponent<SelectNodeButton>().label.text = node.getName().toString();
            if (node.getBehavior().getHow() == NodeBehavior.How.ANY)
            {
                switch (node.getBehavior().getAct())
                {
                    case NodeBehavior.Act.PULL:
                        tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Pull, How.Any);
                        break;
                    case NodeBehavior.Act.PUSH:
                        tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Push, How.Any);
                        break;
                }
            }
            else if (node.getBehavior().getHow() == NodeBehavior.How.ALL)
            {
                switch (node.getBehavior().getAct())
                {
                    case NodeBehavior.Act.PULL:
                        tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Pull, How.All);
                        break;
                    case NodeBehavior.Act.PUSH:
                        tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Push, How.All);
                        break;
                }
            }

            switch (node.getBehavior().getWhen())
            {
                case NodeBehavior.When.AUTO:
                    tempNode.GetComponent<SelectNodeButton>().UpdateWhen(When.Auto);
                    break;
                case NodeBehavior.When.PASSIVE:
                    tempNode.GetComponent<SelectNodeButton>().UpdateWhen(When.Passive);
                    break;
                case NodeBehavior.When.START:
                    tempNode.GetComponent<SelectNodeButton>().UpdateWhen(When.Start);
                    break;
                case NodeBehavior.When.USER:
                    tempNode.GetComponent<SelectNodeButton>().UpdateWhen(When.User);
                    break;
            }

            switch (node.getBehavior().getIO())
            {
                case NodeBehavior.IO.PRIVATE:
                    tempNode.GetComponent<SelectNodeButton>().UpdateIO(IO.Internal);
                    break;
                case NodeBehavior.IO.IN:
                    tempNode.GetComponent<SelectNodeButton>().UpdateIO(IO.In);
                    break;
                case NodeBehavior.IO.OUT:
                    tempNode.GetComponent<SelectNodeButton>().UpdateIO(IO.Out);
                    break;
                case NodeBehavior.IO.INOUT:
                    tempNode.GetComponent<SelectNodeButton>().UpdateIO(IO.InOut);
                    break;
            }
        }
    }

    /// <summary>
    /// Creates a visual edge based on the information in the MM.Model.Edge given
    /// </summary>
    /// <param name="edge">The Micro-Machinations edge to be visualized</param>
    /// <param name="buf">String buffer</param>
    private void CreateEdges(MM.Model.Edge edge, RMPatch.StringBuffer buf)
    {
        RMPatch.List<Location2D.Coordinate> coords;
        coords = edge.getSource().getLocation2D().getCoordinates();
        if (edge is MM.Model.StateEdge)
        {
            tab.GetComponentInChildren<ModelViewController>().AddState(new Vector2(coords[0].getX(), coords[0].getY()));
            for (int i = 0; i < coords.Count; i++)
            {
                tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(new Vector2(coords[i].getX(), coords[i].getY()), null);
            }
            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().UpdateSource(edge.getSrcName().toString());
            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().UpdateTarget(edge.getTgtName().toString());
            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().StopCurrentEdge();
            tab.GetComponentInChildren<ModelViewController>().tempState.transform.SetAsFirstSibling();
            if (edge.getName() != null)
            {
                tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().label.text = edge.getName().toString();
            }
            buf.clear();
            edge.getExp().toString(buf);
            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().exp.text = buf.toString();
            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().idedge = edge;
        }
        else if (edge is MM.Model.FlowEdge)
        {
            tab.GetComponentInChildren<ModelViewController>().AddEdge(new Vector2(coords[0].getX(), coords[0].getY()));
            for (int i = 0; i < coords.Count; i++)
            {
                tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(new Vector2(coords[i].getX(), coords[i].getY()), null);
            }
            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().UpdateSource(edge.getSrcName().toString());
            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().UpdateTarget(edge.getTgtName().toString());
            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().StopCurrentEdge();
            tab.GetComponentInChildren<ModelViewController>().tempEdge.transform.SetAsFirstSibling();
            if (edge.getName() != null)
            {
                tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().label.text = edge.getName().toString();
            }
            buf.clear();
            edge.getExp().toString(buf);
            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().exp.text = buf.toString();
            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().idedge = edge;
        }
    }
}
