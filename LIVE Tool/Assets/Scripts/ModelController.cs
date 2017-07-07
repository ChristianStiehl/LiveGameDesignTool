using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RMPatch;
using System;
using MM;
using MMParser;
/// <summary>
/// Communication between Visual model and technical Micro-Machinations model.
/// </summary>
public class ModelController : MonoBehaviour, MM.Runtime.Checking, MM.Parser.Parsing {
    public MM.Model.Program prog = null;//local program (obsolete)
    public MM.Machine machine;
    System.IO.TextWriter output;
    System.IO.TextWriter error;
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
    public void Start()
    {
        machine = new MM.Machine(this, this, output, error);
    }

    /// <summary>
    /// Adds an edge to the model
    /// </summary>
    /// <param name="edgebehavior">The type of edge added (Flow or State)</param>
    /// <param name="defName">Name of the definition this edge was added to</param>
    /// <param name="newedgeobj">The unity object of the added edge</param>
    public void AddEdge(Behavior edgebehavior, string defName, GameObject newedgeobj)
    {
        MM.Model.Definition targetDef = machine.getProgram();
        if (machine.getProgram().getDefinitions().size() != 0)
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == defName)
                {
                    targetDef = d;
                }
            }
        }
        RMPatch.List<MM.Model.Location2D.Coordinate> loclist = new RMPatch.List<MM.Model.Location2D.Coordinate>();
        for (int i = 0; i < newedgeobj.GetComponent<SelectEdgeButton>().edgeCorners.Count; i++)
        {
            MM.Model.Location2D.Coordinate pos = new MM.Model.Location2D.Coordinate((int)newedgeobj.GetComponent<SelectEdgeButton>().edgeCorners[i].transform.position.x, (int)newedgeobj.GetComponent<SelectEdgeButton>().edgeCorners[i].transform.position.y);
            loclist.add(pos);
        }
        MM.Model.Source edgesrc;
        edgesrc = machine.createSourceLocation(defName, loclist);
        MM.Model.Node srcnode = null;
        MM.Model.Node trgtnode = null;
        foreach (MM.Model.Node n in targetDef.getNodes())
        {
            if(n.getName() != null)
            {
                if (n.getName().toString() == newedgeobj.GetComponent<SelectEdgeButton>().source)
                {
                    srcnode = n;
                }
                else if (n.getName().toString() == newedgeobj.GetComponent<SelectEdgeButton>().target)
                {
                    trgtnode = n;
                }
            }   
        }
        switch (edgebehavior)
        {
            case Behavior.Flow:
                newedgeobj.GetComponent<SelectEdgeButton>().idedge = machine.addFlowEdge(targetDef, edgesrc, srcnode, trgtnode);
                break;
            case Behavior.State:
                newedgeobj.GetComponent<SelectEdgeButton>().idedge = machine.addStateEdge(targetDef, edgesrc, srcnode, trgtnode);
                break;
        }
    }

    /// <summary>
    /// Adds a node to the model
    /// </summary>
    /// <param name="addedNodeType">Behavior of the new node (Pool, Source, Drain etc.)</param>
    /// <param name="newNodePos">Position of the new node on screen</param>
    /// <param name="defName">Name of the definition this node was added to</param>
    /// <param name="newnodeobj">The unity object of the added node</param>
    public void AddNode(Behavior addedNodeType, Vector2 newNodePos, string defName, GameObject newnodeobj)
    {
        MM.Model.Definition targetDef = machine.getProgram();
        if(machine.getProgram().getDefinitions().size() != 0)
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == defName)
                {
                    targetDef = d;
                }
            }
        }
        MM.Model.Location2D.Coordinate pos = new MM.Model.Location2D.Coordinate((int)newNodePos.x, (int)newNodePos.y);
        RMPatch.List<MM.Model.Location2D.Coordinate> loclist = new RMPatch.List<MM.Model.Location2D.Coordinate>();
        loclist.add(pos);
        MM.Model.Source nodesrc;
        nodesrc = machine.createSourceLocation(defName, loclist);
        switch (addedNodeType)
        {
            case Behavior.Pool:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addPoolNode(targetDef, nodesrc);
                break;
            case Behavior.Source:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addSourceNode(targetDef, nodesrc);
                break;
            case Behavior.Drain:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addDrainNode(targetDef, nodesrc);
                break;
            case Behavior.Gate:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addGateNode(targetDef, nodesrc);
                break;
            case Behavior.Converter:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addConverterNode(targetDef, nodesrc);
                break;
            case Behavior.Reference:
                newnodeobj.GetComponent<SelectNodeButton>().idNode = machine.addRefNode(targetDef, nodesrc);
                break;
        }
    }

    /// <summary>
    /// Updates the max variable of a specific node
    /// </summary>
    /// <param name="newmax">New value for max</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    /// <param name="updatedNode">The node model object that needs to be changed</param>
    public void UpdateNodeMax(int newmax, string tabname, MM.Model.Node updatedNode)
    {
        if (tabname == "Global")
        {
            machine.setPoolNodeMax(machine.getProgram(), updatedNode, (uint)newmax);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setPoolNodeMax(d, updatedNode, (uint)newmax);
                }
            }
        }
    }

    /// <summary>
    /// Updates the add variable of a specific node
    /// </summary>
    /// <param name="newadd">New value for add</param>
    /// <param name="updatedNode">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeAdd(string newadd, MM.Model.Node updatedNode, string tabname)
    {
        if (tabname == "Global")
        {
            machine.setPoolNodeAdd(machine.getProgram(), updatedNode, newadd);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setPoolNodeAdd(d, updatedNode, newadd);
                }
            }
        }
    }

    /// <summary>
    /// Updates the typeof variable of a specific node
    /// </summary>
    /// <param name="newdefname">The name of the definition typeof needs to be</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeDefinition(string newdefname, MM.Model.Node newnodeobj, string tabname)
    {
        if(newdefname != null)
        {
            if (tabname == "Global")
            {
                foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
                {
                    if (d.getName().toString() == newdefname)
                    {
                        machine.setPoolNodeType(machine.getProgram(), newnodeobj, d);
                    }
                }
            }
            else
            {
                foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
                {
                    if (d.getName().toString() == tabname)
                    {
                        foreach (MM.Model.Definition d1 in machine.getProgram().getDefinitions())
                        {
                            if (d1.getName().toString() == newdefname)
                            {
                                machine.setPoolNodeType(d, newnodeobj, d1);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (tabname == "Global")
            {
                 machine.setPoolNodeType(machine.getProgram(), newnodeobj, null);
            }
            else
            {
                foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
                {
                    if (d.getName().toString() == tabname)
                    {
                        machine.setPoolNodeType(d, newnodeobj, null);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the how variable of a specific node
    /// </summary>
    /// <param name="newhow">New value for how</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeHow(How newhow, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            if(newhow == How.All)
            {
                machine.setNodeHow(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.How.ALL);
            }
            else
            {
                machine.setNodeHow(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.How.ANY);
            }
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    if (newhow == How.All)
                    {
                        machine.setNodeHow(d, newnodeobj, MM.Model.NodeBehavior.How.ALL);
                    }
                    else
                    {
                        machine.setNodeHow(d, newnodeobj, MM.Model.NodeBehavior.How.ANY);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the act variable of a specific node
    /// </summary>
    /// <param name="newact">New value for act</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeAct(Act newact, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            if (newact == Act.Pull)
            {
                machine.setNodeAct(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.Act.PULL);
            }
            else
            {
                machine.setNodeAct(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.Act.PUSH);
            }
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    if (newact == Act.Pull)
                    {
                        machine.setNodeAct(d, newnodeobj, MM.Model.NodeBehavior.Act.PULL);
                    }
                    else
                    {
                        machine.setNodeAct(d, newnodeobj, MM.Model.NodeBehavior.Act.PUSH);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the when variable of a specific node
    /// </summary>
    /// <param name="newwhen">New value for when</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeWhen(When newwhen, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            switch (newwhen)
            {
                case When.Auto:
                    machine.setNodeWhen(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.When.AUTO);
                    break;
                case When.Passive:
                    machine.setNodeWhen(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.When.PASSIVE);
                    break;
                case When.Start:
                    machine.setNodeWhen(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.When.START);
                    break;
                case When.User:
                    machine.setNodeWhen(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.When.USER);
                    break;
            }
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    switch (newwhen)
                    {
                        case When.Auto:
                            machine.setNodeWhen(d, newnodeobj, MM.Model.NodeBehavior.When.AUTO);
                            break;
                        case When.Passive:
                            machine.setNodeWhen(d, newnodeobj, MM.Model.NodeBehavior.When.PASSIVE);
                            break;
                        case When.Start:
                            machine.setNodeWhen(d, newnodeobj, MM.Model.NodeBehavior.When.START);
                            break;
                        case When.User:
                            machine.setNodeWhen(d, newnodeobj, MM.Model.NodeBehavior.When.USER);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the io variable of a specific node
    /// </summary>
    /// <param name="newio">New value for io</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeIO(IO newio, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            switch (newio)
            {
                case IO.In:
                    machine.setNodeIO(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.IO.IN);
                    break;
                case IO.Out:
                    machine.setNodeIO(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.IO.OUT);
                    break;
                case IO.InOut:
                    machine.setNodeIO(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.IO.INOUT);
                    break;
                case IO.Internal:
                    machine.setNodeIO(machine.getProgram(), newnodeobj, MM.Model.NodeBehavior.IO.PRIVATE);
                    break;
            }
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    switch (newio)
                    {
                        case IO.In:
                            machine.setNodeIO(d, newnodeobj, MM.Model.NodeBehavior.IO.IN);
                            break;
                        case IO.Out:
                            machine.setNodeIO(d, newnodeobj, MM.Model.NodeBehavior.IO.OUT);
                            break;
                        case IO.InOut:
                            machine.setNodeIO(d, newnodeobj, MM.Model.NodeBehavior.IO.INOUT);
                            break;
                        case IO.Internal:
                            machine.setNodeIO(d, newnodeobj, MM.Model.NodeBehavior.IO.PRIVATE);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the at variable of a specific node
    /// </summary>
    /// <param name="newat">New value for at</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeAt(int newat, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            machine.setPoolNodeAt(machine.getProgram(), newnodeobj, (uint)newat);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setPoolNodeAt(d, newnodeobj, (uint)newat);
                }
            }
        }
    }

    /// <summary>
    /// Updates the name of a specific node
    /// </summary>
    /// <param name="newname">New name</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeName(string newname, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            machine.setNodeName(machine.getProgram(), newnodeobj, newname);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setNodeName(d, newnodeobj, newname);
                }
            }
        }
    }

    /// <summary>
    /// Updates the behavior of a specific node
    /// </summary>
    /// <param name="newtype">New behavior</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodeBehavior(Behavior newtype, MM.Model.Node newnodeobj, string tabname)
    {
        if (tabname == "Global")
        {
            MM.Model.NodeBehavior behavior = new MM.Model.PoolNodeBehavior();
            switch (newtype)
            {
                case Behavior.Pool:
                    behavior = new MM.Model.PoolNodeBehavior();
                    break;
                case Behavior.Source:
                    behavior = new MM.Model.SourceNodeBehavior();
                    break;
                case Behavior.Drain:
                    behavior = new MM.Model.DrainNodeBehavior();
                    break;
                case Behavior.Gate:
                    behavior = new MM.Model.GateNodeBehavior();
                    break;
                case Behavior.Converter:
                    behavior = new MM.Model.ConverterNodeBehavior();
                    break;
                case Behavior.Reference:
                    behavior = new MM.Model.RefNodeBehavior();
                    break;
            }
            machine.setNodeBehavior(machine.getProgram(), newnodeobj, behavior);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    MM.Model.NodeBehavior behavior = new MM.Model.PoolNodeBehavior();
                    switch (newtype)
                    {
                        case Behavior.Pool:
                            behavior = new MM.Model.PoolNodeBehavior();
                            break;
                        case Behavior.Source:
                            behavior = new MM.Model.SourceNodeBehavior();
                            break;
                        case Behavior.Drain:
                            behavior = new MM.Model.DrainNodeBehavior();
                            break;
                        case Behavior.Gate:
                            behavior = new MM.Model.GateNodeBehavior();
                            break;
                        case Behavior.Converter:
                            behavior = new MM.Model.ConverterNodeBehavior();
                            break;
                        case Behavior.Reference:
                            behavior = new MM.Model.RefNodeBehavior();
                            break;
                    }
                    machine.setNodeBehavior(d, newnodeobj, behavior);
                }
            }
        }
    }

    /// <summary>
    /// Updates the position of a specific node
    /// </summary>
    /// <param name="newPos">New position</param>
    /// <param name="newnodeobj">The node model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed node is in</param>
    public void UpdateNodePosition(Vector2 newPos, string tabname, MM.Model.Node newnodeobj)
    {
        if (tabname == "Global")
        {
            foreach (MM.Model.Node n1 in machine.getProgram().getNodes())
            {
                if (n1 == newnodeobj)
                {
                    MM.Model.Location2D.Coordinate pos = new MM.Model.Location2D.Coordinate((int)newPos.x, (int)newPos.y);
                    machine.setNodePosition(machine.getProgram(), newnodeobj, pos);
                }
            }
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    MM.Model.Location2D.Coordinate pos = new MM.Model.Location2D.Coordinate((int)newPos.x, (int)newPos.y);
                    machine.setNodePosition(d, newnodeobj, pos);
                }
            }
        }
    }

    /// <summary>
    /// Updates the name of a specific edge
    /// </summary>
    /// <param name="newname">New name</param>
    /// <param name="newedgeobj">The edge model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed edge is in</param>
    public void UpdateEdgeName(string newname, MM.Model.Edge newedgeobj, string tabname)
    {
        if (tabname == "Global")
        {
            machine.setEdgeName(machine.getProgram(), newedgeobj, newname);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setEdgeName(d, newedgeobj, newname);
                }
            }
        }
    }

    /// <summary>
    /// Updates the source of a specific edge
    /// </summary>
    /// <param name="newsourcename">Name of the new source</param>
    /// <param name="newedgeobj">The edge model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed edge is in</param>
    public void UpdateEdgeSource(string newsourcename, MM.Model.Edge newedgeobj, string tabname)
    {
        if (tabname == "Global")
        {
            MM.Model.Node srcnode = null;
            foreach (MM.Model.Node n in machine.getProgram().getNodes())
            {
                if (n.getName()!= null)
                {
                    if (n.getName().toString() == newsourcename)
                    {
                        srcnode = n;
                    }
                }
            }
            machine.setEdgeSourceNode(machine.getProgram(), newedgeobj, srcnode);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    MM.Model.Node srcnode = null;
                    foreach (MM.Model.Node n in d.getNodes())
                    {
                        if(n.getName() != null)
                        {
                            if (n.getName().toString() == newsourcename)
                            {
                                srcnode = n;
                            }
                            else if (n.getBehavior() is MM.Model.PoolNodeBehavior)
                            {
                                MM.Model.PoolNodeBehavior pnb = (MM.Model.PoolNodeBehavior)n.getBehavior();
                                MM.Model.Name name = new MM.Model.Name(n.getSource(), newsourcename);
                                if (pnb.getInterface(name) != null)
                                {
                                    srcnode = pnb.getInterface(name);
                                }
                            }
                        }
                    }
                    machine.setEdgeSourceNode(d, newedgeobj, srcnode);
                }
            }
        }
    }

    /// <summary>
    /// Updates the target of a specific edge
    /// </summary>
    /// <param name="newtargetname">Name of the new target</param>
    /// <param name="newedgeobj">The edge model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed edge is in</param>
    public void UpdateEdgeTarget(string newtargetname, MM.Model.Edge newedgeobj, string tabname)
    {
        if (tabname == "Global")
        {
            MM.Model.Node tgtnode = null;
            foreach (MM.Model.Node n in machine.getProgram().getNodes())
            {
                if(n.getName() != null)
                {
                    if (n.getName().toString() == newtargetname)
                    {
                        tgtnode = n;
                    }
                }
            }
            machine.setEdgeTargetNode(machine.getProgram(), newedgeobj, tgtnode);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    MM.Model.Node tgtnode = null;
                    foreach (MM.Model.Node n in d.getNodes())
                    {
                        if (n.getName() != null)
                        {
                            if (n.getName().toString() == newtargetname)
                            {
                                tgtnode = n;
                            }
                            else if (n.getBehavior() is MM.Model.PoolNodeBehavior)
                            {
                                MM.Model.PoolNodeBehavior pnb = (MM.Model.PoolNodeBehavior)n.getBehavior();
                                MM.Model.Name name = new MM.Model.Name(n.getSource(), newtargetname);
                                if (pnb.getInterface(name) != null)
                                {
                                    tgtnode = pnb.getInterface(name);
                                }
                            }
                        }
                    }
                    machine.setEdgeTargetNode(d, newedgeobj, tgtnode);
                }
            }
        }
    }

    /// <summary>
    /// Updates the expression of a specific edge
    /// </summary>
    /// <param name="newexpression">New expression</param>
    /// <param name="newedgeobj">The edge model object that needs to be changed</param>
    /// <param name="tabname">Name of the definition the changed edge is in</param>
    public void UpdateEdgeExpression(string newexpression, MM.Model.Edge newedgeobj, string tabname)
    {
        if (tabname == "Global")
        {
            machine.setEdgeExpression(machine.getProgram(), newedgeobj, newexpression);
        }
        else
        {
            foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
            {
                if (d.getName().toString() == tabname)
                {
                    machine.setEdgeExpression(d, newedgeobj, newexpression);
                }
            }
        }
    }
    
    /// <summary>
    /// Creates the program in the machine
    /// </summary>
    public void AddProgram()
    {
        machine.createProgram();
    }

    /// <summary>
    /// Checks if there is a program (if not calls AddProgram) then adds a definition to it.
    /// </summary>
    /// <param name="name"></param>
    public void AddDefinition(string name)
    {
        if (machine.getProgram() == null)
        {
            AddProgram();
        }
        MM.Model.Definition def;
        def = machine.addDefinition(machine.getProgram(), machine.getProgram().getSource());
        machine.setDefinitionName(machine.getProgram(), def, name);
    }

    /// <summary>
    /// Updates the name of a definition
    /// </summary>
    /// <param name="oldName">Old name of the definition used to identify it</param>
    /// <param name="newName">New name of the definition</param>
    public void UpdateDefinition(string oldName, string newName)
    {
        foreach(MM.Model.Definition d in machine.getProgram().getDefinitions())
        {
            if(d.getName().toString() == oldName)
            {
                machine.setDefinitionName(machine.getProgram(), d, newName);
            }
        }
    }

    /// <summary>
    /// Removes the target definition from the model
    /// </summary>
    /// <param name="defname">Definition name that needs to be removed</param>
    public void DeleteDefinition(string defname)
    {
        foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
        {
            if (d.getName().toString() == defname)
            {
                machine.removeDefinition(machine.getProgram(), d);
            }
        }
    }

    /// <summary>
    /// Removes the target node from the model
    /// </summary>
    /// <param name="removednode">Node object that needs to be removed</param>
    /// <param name="tabname">Definition name containing the target node</param>
    public void DeleteNode(MM.Model.Node removednode, string tabname)
    {
        if(tabname == "Global")
        {
            machine.removeNode(machine.getProgram(), removednode);
        }
        foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
        {
                if(d.getName().toString() == tabname)
                {
                    machine.removeNode(d, removednode);
                }
         }
    }

    /// <summary>
    /// Removes the target flow edge from the model
    /// </summary>
    /// <param name="removededge">Flow edge object that needs to be removed</param>
    /// <param name="tabname">Definition name containing the target edge</param>
    public void DeleteFlowEdge(MM.Model.FlowEdge removededge, string tabname)
    {
        if (tabname == "Global")
        {
            machine.removeFlowEdge(machine.getProgram(), removededge);
        }
        foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
        {
            if (d.getName().toString() == tabname)
            {
                machine.removeFlowEdge(d, removededge);
            }
        }
    }

    /// <summary>
    /// Removes the target state edge from the model
    /// </summary>
    /// <param name="removededge">State edge object that needs to be removed</param>
    /// <param name="tabname">Definition name containing the target edge</param>
    public void DeleteStateEdge(MM.Model.StateEdge removededge, string tabname)
    {
        if (tabname == "Global")
        {
            machine.removeStateEdge(machine.getProgram(), removededge);
        }
        foreach (MM.Model.Definition d in machine.getProgram().getDefinitions())
        {
            if (d.getName().toString() == tabname)
            {
                machine.removeStateEdge(d, removededge);
            }
        }
    }

    /// <summary>
    /// Set local program to p
    /// </summary>
    /// <param name="p">Micro-Machinations model program </param>
    public void SetProgram(MM.Model.Program p)
    {
        prog = p;
    }

    /// <summary>
    /// Set local machine to m
    /// </summary>
    /// <param name="m">Micro-Machinations machine</param>
    public void SetMachine(MM.Machine m)
    {
        machine = m;
    }

    /// <summary>
    /// Print model to text file 
    /// </summary>
    /// <param name="filepath">File path for text file</param>
    public void ExportToFile(string filepath)
    {
        if (machine.getProgram() != null)
        {
            machine.storeProgram(filepath);
        }
        else
        {
            Debug.Log("Program is empty, cant write to file");
        }
    }
}
