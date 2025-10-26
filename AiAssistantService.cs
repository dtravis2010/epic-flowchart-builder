using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EpicFlowchartBuilder.Models;

namespace EpicFlowchartBuilder.Services
{
    /// <summary>
    /// Service for AI-powered flowchart generation and validation
    /// </summary>
    public class AiAssistantService
    {
        private readonly HttpClient _httpClient;
        private const string OpenAiEndpoint = "https://api.openai.com/v1/chat/completions";

        public AiAssistantService()
        {
            _httpClient = new HttpClient();
        }

        public void SetApiKey(string apiKey)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        /// <summary>
        /// Generate a flowchart from natural language description
        /// </summary>
        public async Task<QuestionnaireFlowchart> GenerateFlowchartAsync(string description, string apiKey = null)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                SetApiKey(apiKey);
            }

            var systemPrompt = GetGenerationSystemPrompt();
            var userPrompt = $@"Create a questionnaire flowchart with the following structure:

{description}

Requirements:
- Include all node types needed (Start, Questions, Decisions, Actions, End)
- Implement skip logic as described
- Ensure all paths lead to an end node
- Use appropriate input types for each question
- Label decision branches clearly

Return the JSON structure only.";

            var response = await CallOpenAI(systemPrompt, userPrompt);
            
            // Parse the JSON response
            var flowchartData = JsonConvert.DeserializeObject<FlowchartGenerationResponse>(response);
            
            return ConvertToQuestionnaireFlowchart(flowchartData);
        }

        /// <summary>
        /// Validate flowchart logic using AI
        /// </summary>
        public async Task<FlowchartValidationResult> ValidateFlowchartAsync(QuestionnaireFlowchart flowchart, string apiKey = null)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                SetApiKey(apiKey);
            }

            var systemPrompt = GetValidationSystemPrompt();
            var flowchartJson = SerializeFlowchart(flowchart);
            var userPrompt = $@"Validate this questionnaire flowchart for logic errors and safety issues:

{flowchartJson}

Check for:
- Structural integrity (connectivity, reachability)
- Logic completeness (all branches defined)
- Clinical safety (critical questions not skipped)
- Efficiency opportunities (redundant steps)

Return the validation report in JSON format.";

            var response = await CallOpenAI(systemPrompt, userPrompt);
            
            // Parse validation response
            var validationData = JsonConvert.DeserializeObject<AiValidationResponse>(response);
            
            return ConvertToValidationResult(validationData);
        }

        private async Task<string> CallOpenAI(string systemPrompt, string userPrompt)
        {
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                max_tokens = 4000
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(OpenAiEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OpenAiResponse>(responseBody);

            return result.Choices[0].Message.Content;
        }

        private string GetGenerationSystemPrompt()
        {
            return @"You are an expert healthcare questionnaire designer specializing in Epic scheduling workflows. 

Your task is to convert natural language descriptions of questionnaires into structured JSON that represents a flowchart with nodes and connections.

Output Format Requirements:
1. Return ONLY valid JSON, no additional text
2. Use the exact schema provided below
3. Assign unique GUIDs to each node and connection
4. Implement skip logic by connecting decision branches to the appropriate target nodes
5. Label all decision branches clearly (e.g., 'Yes', 'No', 'Option A')

JSON Schema:
{
  ""nodes"": [
    {
      ""id"": ""string (GUID)"",
      ""type"": ""Start|Decision|Question|Action|End"",
      ""questionText"": ""string"",
      ""inputType"": ""YesNo|MultipleChoice|FreeText|Date|None"",
      ""choiceOptions"": [""string array - only for MultipleChoice""],
      ""position"": {""x"": number, ""y"": number},
      ""size"": {""width"": number, ""height"": number}
    }
  ],
  ""connections"": [
    {
      ""id"": ""string (GUID)"",
      ""sourceId"": ""string (GUID)"",
      ""targetId"": ""string (GUID)"",
      ""conditionLabel"": ""string (e.g., 'Yes', 'No', 'Option A')""
    }
  ]
}

Positioning Guidelines:
- Start node at (200, 40)
- Vertical spacing: 120px between levels
- Horizontal spacing: 150px between parallel branches
- Use hierarchical top-down layout
- Center-align decision diamond shapes

Node Sizing:
- Start/End: 120x60
- Question: 160x80
- Decision: 180x100
- Action: 140x70

Healthcare Best Practices:
- Always validate skip logic makes clinical sense
- Ensure critical safety questions are never skipped
- Group related medical questions together
- End branches should have clear next actions";
        }

        private string GetValidationSystemPrompt()
        {
            return @"You are a healthcare questionnaire logic validator with expertise in Epic workflows and clinical decision pathways.

Your task is to analyze flowchart JSON data and identify any logical issues, structural problems, or potential safety concerns.

Analysis Areas:
1. **Graph Connectivity**: Ensure all nodes are reachable from Start and lead to End
2. **Branch Completeness**: Verify all decision nodes have all branches defined
3. **Skip Logic Validation**: Check that skip logic makes clinical sense
4. **Safety Checks**: Identify critical questions that should never be skipped
5. **Efficiency**: Spot redundant questions or unnecessary branching

Output Format:
Return JSON with this exact structure:
{
  ""isValid"": boolean,
  ""errors"": [
    {
      ""nodeId"": ""string (GUID or null)"",
      ""severity"": ""Error|Warning|Info"",
      ""category"": ""Connectivity|Logic|Safety|Efficiency|Structure"",
      ""message"": ""string - clear description of issue"",
      ""suggestion"": ""string - how to fix it""
    }
  ],
  ""suggestions"": [
    ""string - general improvements""
  ],
  ""summary"": {
    ""totalNodes"": number,
    ""totalConnections"": number,
    ""decisionPoints"": number,
    ""endPoints"": number,
    ""criticalIssues"": number,
    ""warnings"": number
  }
}

Validation Rules:
1. Every flowchart must have exactly one Start node
2. Every flowchart must have at least one End node
3. All nodes must be reachable from Start (no orphans)
4. All leaf nodes must eventually lead to an End node
5. Decision nodes must have 2+ outgoing connections
6. Each decision branch must be labeled
7. No circular dependencies (unless intentionally marked as loops)
8. Multiple choice questions must have branches for all options
9. Safety-critical questions (implants, allergies, pregnancy) must never be bypassed

Healthcare-Specific Rules:
- Pregnancy screening should not be skipped for MRI/CT
- Device implant questions must be asked before MRI
- Allergy questions should appear before medication/contrast administration
- Critical safety answers should have clear escalation paths";
        }

        private string SerializeFlowchart(QuestionnaireFlowchart flowchart)
        {
            var data = new
            {
                nodes = flowchart.Nodes,
                connections = flowchart.Connections
            };
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        private QuestionnaireFlowchart ConvertToQuestionnaireFlowchart(FlowchartGenerationResponse data)
        {
            var flowchart = new QuestionnaireFlowchart
            {
                Name = "AI Generated Flowchart",
                Description = "Generated by AI Assistant"
            };

            // Add nodes
            foreach (var node in data.Nodes)
            {
                flowchart.Nodes.Add(new QuestionnaireNode
                {
                    Id = Guid.Parse(node.Id),
                    Type = Enum.Parse<NodeType>(node.Type),
                    QuestionText = node.QuestionText,
                    InputType = Enum.Parse<InputType>(node.InputType),
                    ChoiceOptions = node.ChoiceOptions ?? new System.Collections.Generic.List<string>(),
                    Position = new NodePosition { X = node.Position.X, Y = node.Position.Y },
                    Size = new NodeSize { Width = node.Size.Width, Height = node.Size.Height }
                });
            }

            // Add connections
            foreach (var conn in data.Connections)
            {
                flowchart.Connections.Add(new QuestionnaireConnection
                {
                    Id = Guid.Parse(conn.Id),
                    SourceNodeId = Guid.Parse(conn.SourceId),
                    TargetNodeId = Guid.Parse(conn.TargetId),
                    ConditionLabel = conn.ConditionLabel
                });
            }

            return flowchart;
        }

        private FlowchartValidationResult ConvertToValidationResult(AiValidationResponse data)
        {
            var result = new FlowchartValidationResult
            {
                IsValid = data.IsValid
            };

            foreach (var error in data.Errors)
            {
                if (error.Severity == "Error")
                {
                    result.Errors.Add($"{error.Category}: {error.Message} - {error.Suggestion}");
                }
                else
                {
                    result.Warnings.Add($"{error.Category}: {error.Message} - {error.Suggestion}");
                }
            }

            return result;
        }

        // DTOs for JSON responses
        private class OpenAiResponse
        {
            public Choice[] Choices { get; set; }
        }

        private class Choice
        {
            public Message Message { get; set; }
        }

        private class Message
        {
            public string Content { get; set; }
        }

        private class FlowchartGenerationResponse
        {
            public NodeData[] Nodes { get; set; }
            public ConnectionData[] Connections { get; set; }
        }

        private class NodeData
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string QuestionText { get; set; }
            public string InputType { get; set; }
            public System.Collections.Generic.List<string> ChoiceOptions { get; set; }
            public PositionData Position { get; set; }
            public SizeData Size { get; set; }
        }

        private class PositionData
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        private class SizeData
        {
            public double Width { get; set; }
            public double Height { get; set; }
        }

        private class ConnectionData
        {
            public string Id { get; set; }
            public string SourceId { get; set; }
            public string TargetId { get; set; }
            public string ConditionLabel { get; set; }
        }

        private class AiValidationResponse
        {
            public bool IsValid { get; set; }
            public ErrorData[] Errors { get; set; }
            public string[] Suggestions { get; set; }
        }

        private class ErrorData
        {
            public string NodeId { get; set; }
            public string Severity { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public string Suggestion { get; set; }
        }
    }
}
