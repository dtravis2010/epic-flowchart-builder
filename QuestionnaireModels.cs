using System;
using System.Collections.Generic;

namespace EpicFlowchartBuilder.Models
{
    /// <summary>
    /// Represents the type of questionnaire node
    /// </summary>
    public enum NodeType
    {
        Start,      // Entry point
        Question,   // Generic question node
        Decision,   // Yes/No or multi-choice decision
        Action,     // Instruction or outcome
        End         // Terminal point
    }

    /// <summary>
    /// Represents the input type for a question
    /// </summary>
    public enum InputType
    {
        None,           // For Start, End, Action nodes
        YesNo,          // Binary decision
        MultipleChoice, // Select from list
        FreeText,       // Text input
        Date,           // Date picker
        Number          // Numeric input
    }

    /// <summary>
    /// Represents a single node in the questionnaire flowchart
    /// </summary>
    public class QuestionnaireNode
    {
        public Guid Id { get; set; }
        public NodeType Type { get; set; }
        public string QuestionText { get; set; }
        public InputType InputType { get; set; }
        public List<string> ChoiceOptions { get; set; }
        public NodePosition Position { get; set; }
        public NodeSize Size { get; set; }
        public string Notes { get; set; } // Additional metadata or instructions

        public QuestionnaireNode()
        {
            Id = Guid.NewGuid();
            ChoiceOptions = new List<string>();
            Position = new NodePosition();
            Size = GetDefaultSize();
        }

        private NodeSize GetDefaultSize()
        {
            return Type switch
            {
                NodeType.Start => new NodeSize { Width = 120, Height = 60 },
                NodeType.Decision => new NodeSize { Width = 180, Height = 100 },
                NodeType.Question => new NodeSize { Width = 160, Height = 80 },
                NodeType.Action => new NodeSize { Width = 140, Height = 70 },
                NodeType.End => new NodeSize { Width = 120, Height = 60 },
                _ => new NodeSize { Width = 140, Height = 70 }
            };
        }

        /// <summary>
        /// Validates the node configuration
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(QuestionText))
                errors.Add($"Node {Id}: Question text is required");

            if (Type == NodeType.Decision && InputType == InputType.None)
                errors.Add($"Node {Id}: Decision nodes must have an input type");

            if (InputType == InputType.MultipleChoice && (ChoiceOptions == null || ChoiceOptions.Count < 2))
                errors.Add($"Node {Id}: Multiple choice questions must have at least 2 options");

            return errors;
        }
    }

    /// <summary>
    /// Represents a connection between nodes with optional conditional logic
    /// </summary>
    public class QuestionnaireConnection
    {
        public Guid Id { get; set; }
        public Guid SourceNodeId { get; set; }
        public Guid TargetNodeId { get; set; }
        public string ConditionLabel { get; set; } // e.g., "Yes", "No", "Option A"
        public SkipLogic Logic { get; set; }

        public QuestionnaireConnection()
        {
            Id = Guid.NewGuid();
            Logic = new SkipLogic();
        }
    }

    /// <summary>
    /// Defines skip logic rules for a connection
    /// </summary>
    public class SkipLogic
    {
        public string Condition { get; set; } // e.g., "answer == 'Yes'"
        public List<Guid> SkippedNodeIds { get; set; }

        public SkipLogic()
        {
            SkippedNodeIds = new List<Guid>();
        }
    }

    /// <summary>
    /// Node position on the canvas
    /// </summary>
    public class NodePosition
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    /// <summary>
    /// Node dimensions
    /// </summary>
    public class NodeSize
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }

    /// <summary>
    /// Complete questionnaire flowchart model
    /// </summary>
    public class QuestionnaireFlowchart
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Author { get; set; }
        public int Version { get; set; }
        
        public List<QuestionnaireNode> Nodes { get; set; }
        public List<QuestionnaireConnection> Connections { get; set; }
        
        public QuestionnaireFlowchart()
        {
            Id = Guid.NewGuid();
            Nodes = new List<QuestionnaireNode>();
            Connections = new List<QuestionnaireConnection>();
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Version = 1;
        }

        /// <summary>
        /// Validates the entire flowchart structure
        /// </summary>
        public FlowchartValidationResult Validate()
        {
            var result = new FlowchartValidationResult();

            // Validate individual nodes
            foreach (var node in Nodes)
            {
                var nodeErrors = node.Validate();
                result.Errors.AddRange(nodeErrors);
            }

            // Check for Start node
            var startNodes = Nodes.FindAll(n => n.Type == NodeType.Start);
            if (startNodes.Count == 0)
                result.Errors.Add("Flowchart must have a Start node");
            else if (startNodes.Count > 1)
                result.Errors.Add("Flowchart should have only one Start node");

            // Check for End node
            var endNodes = Nodes.FindAll(n => n.Type == NodeType.End);
            if (endNodes.Count == 0)
                result.Errors.Add("Flowchart must have at least one End node");

            // Validate connections
            foreach (var conn in Connections)
            {
                if (!Nodes.Exists(n => n.Id == conn.SourceNodeId))
                    result.Errors.Add($"Connection {conn.Id}: Source node not found");
                
                if (!Nodes.Exists(n => n.Id == conn.TargetNodeId))
                    result.Errors.Add($"Connection {conn.Id}: Target node not found");
            }

            // Check for orphaned nodes
            var reachableNodes = FindReachableNodes();
            var orphanedNodes = Nodes.FindAll(n => !reachableNodes.Contains(n.Id) && n.Type != NodeType.Start);
            foreach (var orphan in orphanedNodes)
            {
                result.Warnings.Add($"Node '{orphan.QuestionText}' is not reachable from Start");
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        /// <summary>
        /// Finds all nodes reachable from the Start node
        /// </summary>
        private HashSet<Guid> FindReachableNodes()
        {
            var reachable = new HashSet<Guid>();
            var startNode = Nodes.Find(n => n.Type == NodeType.Start);
            
            if (startNode == null)
                return reachable;

            var queue = new Queue<Guid>();
            queue.Enqueue(startNode.Id);
            reachable.Add(startNode.Id);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var outgoingConnections = Connections.FindAll(c => c.SourceNodeId == currentId);

                foreach (var conn in outgoingConnections)
                {
                    if (!reachable.Contains(conn.TargetNodeId))
                    {
                        reachable.Add(conn.TargetNodeId);
                        queue.Enqueue(conn.TargetNodeId);
                    }
                }
            }

            return reachable;
        }
    }

    /// <summary>
    /// Result of flowchart validation
    /// </summary>
    public class FlowchartValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public FlowchartValidationResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }
    }
}
