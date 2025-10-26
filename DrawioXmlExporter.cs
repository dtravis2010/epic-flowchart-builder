using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EpicFlowchartBuilder.Models;

namespace EpicFlowchartBuilder.Services
{
    /// <summary>
    /// Service for exporting questionnaire flowcharts to Draw.io XML format
    /// </summary>
    public class DrawioXmlExporter
    {
        private const string DrawioHost = "app.diagrams.net";
        private const int GridSize = 10;
        
        /// <summary>
        /// Exports a flowchart to Draw.io compatible XML
        /// </summary>
        public string ExportToDrawio(QuestionnaireFlowchart flowchart)
        {
            var xml = CreateDrawioDocument(flowchart);
            return xml.ToString();
        }

        /// <summary>
        /// Creates the complete Draw.io XML document
        /// </summary>
        private XElement CreateDrawioDocument(QuestionnaireFlowchart flowchart)
        {
            var mxfile = new XElement("mxfile",
                new XAttribute("host", DrawioHost),
                new XAttribute("modified", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                new XAttribute("agent", "Epic Flowchart Builder"),
                new XAttribute("version", "1.0.0"),
                new XAttribute("type", "device")
            );

            var diagram = new XElement("diagram",
                new XAttribute("name", flowchart.Name ?? "Questionnaire"),
                new XAttribute("id", flowchart.Id.ToString())
            );

            var model = new XElement("mxGraphModel",
                new XAttribute("dx", "1000"),
                new XAttribute("dy", "1000"),
                new XAttribute("grid", "1"),
                new XAttribute("gridSize", GridSize.ToString()),
                new XAttribute("guides", "1"),
                new XAttribute("tooltips", "1"),
                new XAttribute("connect", "1"),
                new XAttribute("arrows", "1"),
                new XAttribute("fold", "1"),
                new XAttribute("page", "1"),
                new XAttribute("pageScale", "1"),
                new XAttribute("pageWidth", "850"),
                new XAttribute("pageHeight", "1100")
            );

            var root = new XElement("root");
            
            // Add default parent cells (required by Draw.io)
            root.Add(new XElement("mxCell", new XAttribute("id", "0")));
            root.Add(new XElement("mxCell", 
                new XAttribute("id", "1"), 
                new XAttribute("parent", "0")));

            // Add all nodes
            foreach (var node in flowchart.Nodes)
            {
                root.Add(CreateNodeCell(node));
            }

            // Add all connections
            foreach (var connection in flowchart.Connections)
            {
                root.Add(CreateEdgeCell(connection, flowchart));
            }

            model.Add(root);
            diagram.Add(model);
            mxfile.Add(diagram);

            return mxfile;
        }

        /// <summary>
        /// Creates an mxCell element for a node
        /// </summary>
        private XElement CreateNodeCell(QuestionnaireNode node)
        {
            var style = GetNodeStyle(node.Type);
            
            var cell = new XElement("mxCell",
                new XAttribute("id", node.Id.ToString()),
                new XAttribute("value", EscapeXml(node.QuestionText)),
                new XAttribute("style", style),
                new XAttribute("vertex", "1"),
                new XAttribute("parent", "1")
            );

            var geometry = new XElement("mxGeometry",
                new XAttribute("x", node.Position.X.ToString("F0")),
                new XAttribute("y", node.Position.Y.ToString("F0")),
                new XAttribute("width", node.Size.Width.ToString("F0")),
                new XAttribute("height", node.Size.Height.ToString("F0")),
                new XAttribute("as", "geometry")
            );

            cell.Add(geometry);
            return cell;
        }

        /// <summary>
        /// Creates an mxCell element for a connection/edge
        /// </summary>
        private XElement CreateEdgeCell(QuestionnaireConnection connection, QuestionnaireFlowchart flowchart)
        {
            var style = GetEdgeStyle(connection);
            
            var cell = new XElement("mxCell",
                new XAttribute("id", connection.Id.ToString()),
                new XAttribute("value", EscapeXml(connection.ConditionLabel ?? "")),
                new XAttribute("style", style),
                new XAttribute("edge", "1"),
                new XAttribute("parent", "1"),
                new XAttribute("source", connection.SourceNodeId.ToString()),
                new XAttribute("target", connection.TargetNodeId.ToString())
            );

            var geometry = new XElement("mxGeometry",
                new XAttribute("relative", "1"),
                new XAttribute("as", "geometry")
            );

            // Add exit/entry points for better routing
            var sourceNode = flowchart.Nodes.FirstOrDefault(n => n.Id == connection.SourceNodeId);
            var targetNode = flowchart.Nodes.FirstOrDefault(n => n.Id == connection.TargetNodeId);
            
            if (sourceNode != null && targetNode != null)
            {
                // Calculate connection points based on node positions
                var exitPoint = CalculateExitPoint(sourceNode, targetNode);
                var entryPoint = CalculateEntryPoint(sourceNode, targetNode);
                
                if (exitPoint != null)
                {
                    geometry.Add(new XElement("mxPoint",
                        new XAttribute("as", "sourcePoint"),
                        new XAttribute("x", exitPoint.Value.X),
                        new XAttribute("y", exitPoint.Value.Y)
                    ));
                }
                
                if (entryPoint != null)
                {
                    geometry.Add(new XElement("mxPoint",
                        new XAttribute("as", "targetPoint"),
                        new XAttribute("x", entryPoint.Value.X),
                        new XAttribute("y", entryPoint.Value.Y)
                    ));
                }
            }

            cell.Add(geometry);
            return cell;
        }

        /// <summary>
        /// Gets the Draw.io style string for a node type
        /// </summary>
        private string GetNodeStyle(NodeType type)
        {
            return type switch
            {
                NodeType.Start => 
                    "ellipse;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;fontSize=12;fontStyle=1",
                
                NodeType.Decision => 
                    "rhombus;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;fontSize=11",
                
                NodeType.Question => 
                    "rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;fontSize=11",
                
                NodeType.Action => 
                    "rounded=0;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;fontSize=11",
                
                NodeType.End => 
                    "ellipse;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;fontSize=12;fontStyle=1",
                
                _ => "rounded=0;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11"
            };
        }

        /// <summary>
        /// Gets the Draw.io style string for an edge/connection
        /// </summary>
        private string GetEdgeStyle(QuestionnaireConnection connection)
        {
            var baseStyle = "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;";
            
            // Add label styling if condition label exists
            if (!string.IsNullOrWhiteSpace(connection.ConditionLabel))
            {
                baseStyle += "labelBackgroundColor=#ffffff;fontSize=10;fontColor=#000000;";
            }

            // Style "Yes" branches green, "No" branches red
            if (connection.ConditionLabel?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true)
            {
                baseStyle += "strokeColor=#82b366;";
            }
            else if (connection.ConditionLabel?.Trim().Equals("No", StringComparison.OrdinalIgnoreCase) == true)
            {
                baseStyle += "strokeColor=#b85450;";
            }
            else
            {
                baseStyle += "strokeColor=#000000;";
            }

            return baseStyle;
        }

        /// <summary>
        /// Calculates the optimal exit point from source node
        /// </summary>
        private (double X, double Y)? CalculateExitPoint(QuestionnaireNode source, QuestionnaireNode target)
        {
            var sourceCenterX = source.Position.X + source.Size.Width / 2;
            var sourceCenterY = source.Position.Y + source.Size.Height / 2;
            var targetCenterX = target.Position.X + target.Size.Width / 2;
            var targetCenterY = target.Position.Y + target.Size.Height / 2;

            // Determine exit side based on relative position
            if (targetCenterY > sourceCenterY + source.Size.Height / 2)
            {
                // Exit from bottom
                return (sourceCenterX, source.Position.Y + source.Size.Height);
            }
            else if (targetCenterX > sourceCenterX + source.Size.Width / 2)
            {
                // Exit from right
                return (source.Position.X + source.Size.Width, sourceCenterY);
            }
            else if (targetCenterX < sourceCenterX - source.Size.Width / 2)
            {
                // Exit from left
                return (source.Position.X, sourceCenterY);
            }
            else
            {
                // Exit from top
                return (sourceCenterX, source.Position.Y);
            }
        }

        /// <summary>
        /// Calculates the optimal entry point to target node
        /// </summary>
        private (double X, double Y)? CalculateEntryPoint(QuestionnaireNode source, QuestionnaireNode target)
        {
            var sourceCenterX = source.Position.X + source.Size.Width / 2;
            var sourceCenterY = source.Position.Y + source.Size.Height / 2;
            var targetCenterX = target.Position.X + target.Size.Width / 2;
            var targetCenterY = target.Position.Y + target.Size.Height / 2;

            // Determine entry side (opposite of exit logic)
            if (sourceCenterY < targetCenterY - target.Size.Height / 2)
            {
                // Enter from top
                return (targetCenterX, target.Position.Y);
            }
            else if (sourceCenterX < targetCenterX - target.Size.Width / 2)
            {
                // Enter from left
                return (target.Position.X, targetCenterY);
            }
            else if (sourceCenterX > targetCenterX + target.Size.Width / 2)
            {
                // Enter from right
                return (target.Position.X + target.Size.Width, targetCenterY);
            }
            else
            {
                // Enter from bottom
                return (targetCenterX, target.Position.Y + target.Size.Height);
            }
        }

        /// <summary>
        /// Escapes XML special characters
        /// </summary>
        private string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        /// <summary>
        /// Validates the export and returns any issues
        /// </summary>
        public List<string> ValidateExport(QuestionnaireFlowchart flowchart)
        {
            var issues = new List<string>();

            // Check for duplicate IDs
            var nodeIds = new HashSet<Guid>();
            foreach (var node in flowchart.Nodes)
            {
                if (!nodeIds.Add(node.Id))
                    issues.Add($"Duplicate node ID: {node.Id}");
            }

            // Check all connection references are valid
            foreach (var conn in flowchart.Connections)
            {
                if (!nodeIds.Contains(conn.SourceNodeId))
                    issues.Add($"Connection {conn.Id} references invalid source: {conn.SourceNodeId}");
                
                if (!nodeIds.Contains(conn.TargetNodeId))
                    issues.Add($"Connection {conn.Id} references invalid target: {conn.TargetNodeId}");
            }

            // Check for missing positions
            foreach (var node in flowchart.Nodes)
            {
                if (node.Position == null)
                    issues.Add($"Node {node.Id} has no position defined");
                
                if (node.Size == null)
                    issues.Add($"Node {node.Id} has no size defined");
            }

            return issues;
        }
    }
}
