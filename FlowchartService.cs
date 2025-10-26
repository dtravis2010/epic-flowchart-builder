using System;
using System.Collections.Generic;
using System.Linq;
using EpicFlowchartBuilder.Models;

namespace EpicFlowchartBuilder.Services
{
    /// <summary>
    /// Service for managing flowchart business logic
    /// </summary>
    public class FlowchartService
    {
        private Random _random = new Random();

        /// <summary>
        /// Creates a new flowchart with Start and End nodes
        /// </summary>
        public QuestionnaireFlowchart CreateNewFlowchart(string name)
        {
            var flowchart = new QuestionnaireFlowchart
            {
                Name = name,
                Description = "New questionnaire flowchart",
                Author = Environment.UserName
            };

            // Add Start node
            var startNode = new QuestionnaireNode
            {
                Type = NodeType.Start,
                QuestionText = "Start",
                InputType = InputType.None,
                Position = new NodePosition { X = 400, Y = 50 },
                Size = new NodeSize { Width = 120, Height = 60 }
            };

            // Add End node
            var endNode = new QuestionnaireNode
            {
                Type = NodeType.End,
                QuestionText = "End",
                InputType = InputType.None,
                Position = new NodePosition { X = 400, Y = 400 },
                Size = new NodeSize { Width = 120, Height = 60 }
            };

            flowchart.Nodes.Add(startNode);
            flowchart.Nodes.Add(endNode);

            return flowchart;
        }

        /// <summary>
        /// Auto-arranges nodes in a hierarchical layout
        /// </summary>
        public void AutoArrangeNodes(QuestionnaireFlowchart flowchart)
        {
            var startNode = flowchart.Nodes.FirstOrDefault(n => n.Type == NodeType.Start);
            if (startNode == null) return;

            // Build graph structure
            var levels = BuildNodeLevels(flowchart, startNode);

            // Position nodes by level
            double startX = 100;
            double startY = 50;
            double verticalSpacing = 150;
            double horizontalSpacing = 200;

            foreach (var (level, nodes) in levels.Select((n, i) => (i, n)))
            {
                double levelY = startY + level * verticalSpacing;
                double totalWidth = nodes.Count * horizontalSpacing;
                double levelStartX = startX + (800 - totalWidth) / 2; // Center nodes

                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = flowchart.Nodes.First(n => n.Id == nodes[i]);
                    node.Position.X = levelStartX + i * horizontalSpacing;
                    node.Position.Y = levelY;
                }
            }
        }

        /// <summary>
        /// Builds level-based structure for layout
        /// </summary>
        private List<List<Guid>> BuildNodeLevels(QuestionnaireFlowchart flowchart, QuestionnaireNode startNode)
        {
            var levels = new List<List<Guid>>();
            var visited = new HashSet<Guid>();
            var queue = new Queue<(Guid nodeId, int level)>();

            queue.Enqueue((startNode.Id, 0));
            visited.Add(startNode.Id);

            while (queue.Count > 0)
            {
                var (nodeId, level) = queue.Dequeue();

                // Ensure level exists
                while (levels.Count <= level)
                {
                    levels.Add(new List<Guid>());
                }

                levels[level].Add(nodeId);

                // Find connected nodes
                var connections = flowchart.Connections.Where(c => c.SourceNodeId == nodeId);
                foreach (var conn in connections)
                {
                    if (!visited.Contains(conn.TargetNodeId))
                    {
                        visited.Add(conn.TargetNodeId);
                        queue.Enqueue((conn.TargetNodeId, level + 1));
                    }
                }
            }

            return levels;
        }

        /// <summary>
        /// Adds a new node to the flowchart
        /// </summary>
        public QuestionnaireNode AddNode(QuestionnaireFlowchart flowchart, NodeType type, double x, double y)
        {
            var node = new QuestionnaireNode
            {
                Type = type,
                QuestionText = GetDefaultText(type),
                InputType = GetDefaultInputType(type),
                Position = new NodePosition { X = x, Y = y },
                Size = GetDefaultSizeForType(type)
            };

            flowchart.Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Removes a node and its connections
        /// </summary>
        public void RemoveNode(QuestionnaireFlowchart flowchart, Guid nodeId)
        {
            var node = flowchart.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null)
            {
                flowchart.Nodes.Remove(node);
                flowchart.Connections.RemoveAll(c => c.SourceNodeId == nodeId || c.TargetNodeId == nodeId);
            }
        }

        /// <summary>
        /// Adds a connection between two nodes
        /// </summary>
        public QuestionnaireConnection AddConnection(QuestionnaireFlowchart flowchart, Guid sourceId, Guid targetId, string label = "")
        {
            var connection = new QuestionnaireConnection
            {
                SourceNodeId = sourceId,
                TargetNodeId = targetId,
                ConditionLabel = label
            };

            flowchart.Connections.Add(connection);
            return connection;
        }

        /// <summary>
        /// Removes a connection
        /// </summary>
        public void RemoveConnection(QuestionnaireFlowchart flowchart, Guid connectionId)
        {
            var connection = flowchart.Connections.FirstOrDefault(c => c.Id == connectionId);
            if (connection != null)
            {
                flowchart.Connections.Remove(connection);
            }
        }

        private string GetDefaultText(NodeType type)
        {
            return type switch
            {
                NodeType.Start => "Start",
                NodeType.End => "End",
                NodeType.Decision => "New Decision",
                NodeType.Question => "New Question",
                NodeType.Action => "New Action",
                _ => "New Node"
            };
        }

        private InputType GetDefaultInputType(NodeType type)
        {
            return type switch
            {
                NodeType.Decision => InputType.YesNo,
                NodeType.Question => InputType.FreeText,
                _ => InputType.None
            };
        }

        private NodeSize GetDefaultSizeForType(NodeType type)
        {
            return type switch
            {
                NodeType.Start => new NodeSize { Width = 120, Height = 60 },
                NodeType.Decision => new NodeSize { Width = 180, Height = 100 },
                NodeType.Question => new NodeSize { Width = 160, Height = 80 },
                NodeType.Action => new NodeSize { Width = 140, Height = 70 },
                NodeType.End => new NodeSize { Width = 120, Height = 60 },
                _ => new NodeSize { Width = 140, Height = 70 }
            };
        }
    }
}
