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

    public void receive(MM.Parser.ParserMessage message)
    {
        Debug.Log(message.toString());
    }
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
    /// When clicked opens the file browser
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
                if (n1.getBehavior() is PoolNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Pool, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                    tempNode = tab.GetComponentInChildren<ModelViewController>().tempNode;
                    PoolNodeBehavior pnb = (PoolNodeBehavior)n1.getBehavior();
                    tempNode.GetComponent<SelectNodeButton>().UpdateAt((int)pnb.getAt());
                    if (pnb.getExp() != null)
                    {
                        buf.clear();
                        pnb.getExp().toString(buf);
                        tempNode.GetComponent<SelectNodeButton>().UpdateAdd(buf.toString());
                    }
                    //tempNode.GetComponent<SelectNodeButton>().UpdateMax((int)pnb.getMax());
                    if (pnb.getDefinition() != null)
                    {
                        tempNode.GetComponent<SelectNodeButton>().UpdateOfType(pnb.getDefinition().getName().toString());
                        definitionNodes.Add(tempNode);
                        // tempNode.GetComponent<SelectNodeButton>().UpdateDefinitionReferences(pnb.getDefinition().getName().toString());
                    }
                    //TODO pool specific stuff
                }
                else if (n1.getBehavior() is SourceNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Source, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                }
                else if (n1.getBehavior() is DrainNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Drain, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                }
                else if (n1.getBehavior() is GateNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Gate, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                }
                else if (n1.getBehavior() is ConverterNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Converter, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                }
                else if (n1.getBehavior() is RefNodeBehavior)
                {
                    tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Reference, new Vector2(n1.getSource().getLocation2D().getCoordinates()[0].getX(), n1.getSource().getLocation2D().getCoordinates()[0].getY()));
                }

                tempNode = tab.GetComponentInChildren<ModelViewController>().tempNode;
                if (tempNode)
                {
                    tempNode.GetComponent<SelectNodeButton>().idNode = n1;
                    tempNode.GetComponent<SelectNodeButton>().label.text = n1.getName().toString();
                    if (n1.getBehavior().getHow() == NodeBehavior.How.ANY)
                    {
                        switch (n1.getBehavior().getAct())
                        {
                            case NodeBehavior.Act.PULL:
                                tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Pull, How.Any);
                                break;
                            case NodeBehavior.Act.PUSH:
                                tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Push, How.Any);
                                break;
                        }
                    }
                    else if (n1.getBehavior().getHow() == NodeBehavior.How.ALL)
                    {
                        switch (n1.getBehavior().getAct())
                        {
                            case NodeBehavior.Act.PULL:
                                tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Pull, How.All);
                                break;
                            case NodeBehavior.Act.PUSH:
                                tempNode.GetComponent<SelectNodeButton>().UpdateActHow(Act.Push, How.All);
                                break;
                        }
                    }

                    switch (n1.getBehavior().getWhen())
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

                    switch (n1.getBehavior().getIO())
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
            foreach (MM.Model.Definition def in definitions)
            {
                tabman.AddTab(def.getName().toString());
                tab = tabman.GetLastTab();
                tabs.Add(tab);
                //create all nodes
                foreach (MM.Model.Node node in def.getNodes())
                {
                    if (node.getBehavior() is PoolNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Pool, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
                        tempNode = tab.GetComponentInChildren<ModelViewController>().tempNode;
                        PoolNodeBehavior pnb = (PoolNodeBehavior)node.getBehavior();
                        tempNode.GetComponent<SelectNodeButton>().UpdateAt((int)pnb.getAt());
                        if (pnb.getExp() != null)
                        {
                            buf.clear();
                            pnb.getExp().toString(buf);
                            tempNode.GetComponent<SelectNodeButton>().UpdateAdd(buf.toString());
                        }
                        tempNode.GetComponent<SelectNodeButton>().UpdateMax((int)pnb.getMax());
                        if (pnb.getDefinition() != null)
                        {
                            tempNode.GetComponent<SelectNodeButton>().UpdateOfType(pnb.getDefinition().getName().toString());
                            definitionNodes.Add(tempNode);
                        }
                    }
                    else if (node.getBehavior() is SourceNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Source, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
                    }
                    else if (node.getBehavior() is DrainNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Drain, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
                    }
                    else if (node.getBehavior() is GateNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Gate, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
                    }
                    else if (node.getBehavior() is ConverterNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Converter, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
                    }
                    else if (node.getBehavior() is RefNodeBehavior)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddNode(Behavior.Reference, new Vector2(node.getSource().getLocation2D().getCoordinates()[0].getX(), node.getSource().getLocation2D().getCoordinates()[0].getY()));
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
                }//end node foreach

                foreach (GameObject obj in definitionNodes)
                {
                    obj.GetComponent<SelectNodeButton>().UpdateDefinitionReferences();
                }
            }//end definition foreach
            foreach (MM.Model.Edge e1 in program.getEdges())
            {
                    tab = tabs[0];
                    RMPatch.List<Location2D.Coordinate> coords;
                    coords = e1.getSource().getLocation2D().getCoordinates();
                    if (e1 is MM.Model.StateEdge)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddState(new Vector2(coords[0].getX(), coords[0].getY()));
                        for (int i = 0; i < coords.Count; i++)
                        {
                            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(new Vector2(coords[i].getX(), coords[i].getY()), null);
                        }
                        tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().UpdateSource(e1.getSrcName().toString());
                        tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().UpdateTarget(e1.getTgtName().toString());
                        tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().StopCurrentEdge();
                        tab.GetComponentInChildren<ModelViewController>().tempState.transform.SetAsFirstSibling();
                        if (e1.getName() != null)
                        {
                            tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().label.text = e1.getName().toString();
                        }
                        buf.clear();
                        e1.getExp().toString(buf);
                        tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().exp.text = buf.toString();
                        tab.GetComponentInChildren<ModelViewController>().tempState.GetComponent<SelectEdgeButton>().idedge = e1;
                }
                else if (e1 is MM.Model.FlowEdge)
                    {
                        tab.GetComponentInChildren<ModelViewController>().AddEdge(new Vector2(coords[0].getX(), coords[0].getY()));
                        for (int i = 0; i < coords.Count; i++)
                        {
                            tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(new Vector2(coords[i].getX(), coords[i].getY()), null);
                        }
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().UpdateSource(e1.getSrcName().toString());
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().UpdateTarget(e1.getTgtName().toString());
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().StopCurrentEdge();
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.transform.SetAsFirstSibling();
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().label.text = e1.getName().toString();
                        buf.clear();
                        e1.getExp().toString(buf);
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().exp.text = buf.toString();
                        tab.GetComponentInChildren<ModelViewController>().tempEdge.GetComponent<SelectEdgeButton>().idedge = e1;
                }
            }
            foreach (MM.Model.Definition d1 in program.getDefinitions())
            {
                tab = tabs[index];
                index++;
                foreach (MM.Model.Edge edge in d1.getEdges())
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
                }//end edge foreach
            }
        }
        if(program != null)
        {
            mc.SetProgram(program);
            mc.SetMachine(machine);
        }
    }//end task on click
}
