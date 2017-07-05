using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RMPatch;
using System;
using MM;
using MMParser;
/// <summary>
/// The behind the scenes model and inner workings
/// </summary>
public class ModelController : MonoBehaviour, MM.Runtime.Checking, MM.Parser.Parsing {
    public MM.Model.Program prog = null;
    public MM.Machine machine;
    System.IO.TextWriter output;
    System.IO.TextWriter error;
    public void receive(MM.Parser.ParserMessage message)
    {
        Debug.Log(message.toString());
    }
    public void receive(MM.Runtime.CheckerMessage message)
    {
        Debug.Log(message.toString());
    }
    public void Start()
    {
        machine = new MM.Machine(this, this, output, error);
    }

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
    //Update node property functions
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

    

    public void AddProgram()
    {
        /*MM.Model.Location tloc = new MM.Model.Location();
        MM.Model.Location2D vloc = new MM.Model.Location2D();
        MM.Model.Source s = new MM.Model.Source(tloc, vloc);
        machine.getProgram() = new MM.Model.Program(s);*/
        machine.createProgram();
    }
    public void AddDefinition(string name)
    {
        if (machine.getProgram() == null)
        {
            AddProgram();
        }
        /*
        MM.Model.Source s2 = machine.getProgram().getSource();
        MM.Model.Name n = new MM.Model.Name(s2, name);
        MM.Model.Definition d = new MM.Model.Definition(s2, n);
        machine.getProgram().addElement(d);*/
        MM.Model.Definition def;
        def = machine.addDefinition(machine.getProgram(), machine.getProgram().getSource());
        machine.setDefinitionName(machine.getProgram(), def, name);
    }
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

    public void SetProgram(MM.Model.Program p)
    {
        prog = p;
    }

    public void SetMachine(MM.Machine m)
    {
        machine = m;
    }

    public void ExportToFile(string filepath)
    {
        if (machine.getProgram() != null)
        {
            /*RMPatch.StringBuffer buf = new RMPatch.StringBuffer();
            buf.clear();
            prog.toString(buf, 0);
            System.IO.File.WriteAllText(filepath, buf.toString());*/
            machine.storeProgram(filepath);
        }
        else
        {
            Debug.Log("Program is empty, cant write to file");
        }
    }
}
