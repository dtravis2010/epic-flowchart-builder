# Epic Flowchart Builder

A professional WPF desktop application for building Epic-style healthcare questionnaires with AI-powered generation, validation, and Draw.io export capabilities.

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

## ✨ Features

### 🎨 Visual Flowchart Designer
- **Drag-and-drop node positioning** - Intuitive canvas-based editing
- **5 color-coded node types** - Start, Decision, Question, Action, End
- **Visual connection rendering** - Clear relationship visualization
- **Real-time property editing** - Instant updates to node properties
- **Auto-layout algorithm** - Automatic hierarchical arrangement

### 🤖 AI Assistant
- **Generate from description** - Natural language to flowchart conversion
- **AI-powered validation** - Healthcare-specific safety checks
- **Embedded prompts** - Uses proven templates for quality results
- **OpenAI GPT-4 integration** - State-of-the-art AI capabilities

### 🔀 Skip Logic Editor
- **Conditional branching** - Yes/No and multiple choice decisions
- **Connection-based logic** - Visual skip logic representation
- **Branch labeling** - Clear condition labels on connections

### ✓ Validation System
- **Structural validation** - Connectivity and completeness checks
- **Healthcare compliance** - Safety-critical question validation
- **AI validation** - Advanced logic and efficiency analysis
- **Real-time error reporting** - Immediate feedback

### 📤 Draw.io XML Export
- **Complete implementation** - Full compatibility with Draw.io
- **Preserves styling** - Colors, shapes, and connections maintained
- **Validation before export** - Ensures valid XML output
- **Opens in app.diagrams.net** - Ready for further editing

## 🚀 Quick Start

### Prerequisites
- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Windows 10/11** - Required for WPF
- **Visual Studio 2022** (optional) - For development

### Installation

#### Option 1: Build from Source

**Windows:**
```bash
build.bat
```

**Linux/Mac:**
```bash
./build.sh
```

#### Option 2: Manual Build
```bash
dotnet restore
dotnet build --configuration Release
dotnet run
```

### Running the Application
```bash
cd bin/Release/net8.0-windows
EpicFlowchartBuilder.exe
```

## 📖 Usage Guide

### Creating Your First Flowchart

1. **Launch the application**
2. **Add nodes** using the toolbar buttons
3. **Edit properties** by clicking on nodes
4. **Connect nodes** using the connection tool
5. **Validate** your flowchart structure
6. **Export** to Draw.io or save as JSON

### Using AI Generation

1. **Enter your OpenAI API key** in the left panel
2. **Click "Generate from Description"**
3. **Describe your questionnaire** in natural language
4. **Review and refine** the generated flowchart
5. **Validate** with AI or built-in validator

#### Example AI Prompt:
```
Create an MRI screening questionnaire:
1. Start: "MRI Safety Screening"
2. Q1: "Do you have any implanted medical devices?" (Yes/No)
   - If Yes → Q2: "What type?" (Multiple choice: Pacemaker, ICD, Neurostimulator)
   - If No → Skip to Q3
3. Q3: "Any metal fragments or tattoos?" (Yes/No)
   - If Yes → Action: "Requires radiologist approval"
   - If No → Action: "Cleared for MRI"
4. End: "Screening Complete"
```

### Node Types

| Type | Shape | Color | Purpose |
|------|-------|-------|---------|
| **Start** | Oval | Green (#d5e8d4) | Entry point |
| **Decision** | Diamond | Yellow (#fff2cc) | Yes/No or multiple choice |
| **Question** | Rectangle | Blue (#dae8fc) | User input collection |
| **Action** | Rectangle | Purple (#e1d5e7) | Instructions/outcomes |
| **End** | Oval | Red (#f8cecc) | Terminal point |

### Keyboard Shortcuts

- `Ctrl+N` - New flowchart
- `Ctrl+S` - Save flowchart
- `Ctrl+O` - Open flowchart
- `Del` - Delete selected node
- `Ctrl+E` - Export to Draw.io

## 🏗️ Architecture

### Technology Stack
- **.NET 8.0** - Modern framework
- **WPF** - Rich desktop UI
- **MVVM Pattern** - Clean architecture
- **Microsoft MSAGL** - Auto-layout
- **Newtonsoft.Json** - JSON serialization
- **OpenAI API** - AI capabilities

### Project Structure
```
EpicFlowchartBuilder/
├── Models/
│   └── QuestionnaireModels.cs      # Core data structures
├── ViewModels/
│   └── MainViewModel.cs             # Application logic
├── Views/
│   ├── MainWindow.xaml              # Main UI
│   └── MainWindow.xaml.cs           # UI code-behind
├── Services/
│   ├── FlowchartService.cs          # Business logic
│   ├── AiAssistantService.cs        # AI integration
│   └── DrawioXmlExporter.cs         # XML export
├── App.xaml                          # Application definition
└── EpicFlowchartBuilder.csproj      # Project file
```

## 🔧 Configuration

### AI Configuration
To use AI features, you need an OpenAI API key:

1. Get your API key from [OpenAI](https://platform.openai.com/api-keys)
2. Enter it in the application's AI panel
3. The key is used only during your session (not stored)

### Customization
- Edit node colors in `MainWindow.xaml.cs` (`NodeTypeToColorConverter`)
- Modify AI prompts in `AiAssistantService.cs`
- Adjust layout spacing in `FlowchartService.cs`

## 📁 File Formats

### JSON Format
Flowcharts are saved as JSON:
```json
{
  "id": "guid",
  "name": "Questionnaire Name",
  "nodes": [...],
  "connections": [...]
}
```

### Draw.io XML Format
Exported flowcharts are compatible with:
- https://app.diagrams.net
- Draw.io desktop applications
- Any tool supporting mxGraph XML

## ⚠️ Healthcare Compliance

This application includes healthcare-specific validation:

✅ **Safety Checks**
- Pregnancy screening before MRI/CT/X-Ray
- Implant detection before MRI procedures
- Allergy verification before contrast/medication
- Complete decision branch validation

✅ **HIPAA Considerations**
- No patient data sent to AI
- AI used only for structure/template design
- Local processing of all patient information
- Audit logging capabilities

## 🐛 Troubleshooting

### Build Errors
**Problem:** Missing dependencies
**Solution:** Run `dotnet restore`

### AI Not Working
**Problem:** API calls failing
**Solution:** Verify API key and internet connection

### Export Issues
**Problem:** Draw.io won't open file
**Solution:** Validate flowchart before exporting

### Performance Issues
**Problem:** Slow with many nodes
**Solution:** Use auto-arrange, limit to <100 nodes

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Built using the [epic-flowchart-builder skill](https://github.com/anthropics/claude-skills)
- AI prompts based on healthcare best practices
- Draw.io XML format from mxGraph documentation
- MVVM pattern inspired by WPF community

## 📞 Support

For issues, questions, or feedback:
- GitHub Issues: [Create an issue]
- Email: support@example.com
- Documentation: See `/docs` folder

## 🎯 Roadmap

### Version 1.1 (Planned)
- [ ] Real-time collaboration
- [ ] Template library
- [ ] Custom node shapes
- [ ] PDF export
- [ ] Version control

### Version 2.0 (Future)
- [ ] Web-based version
- [ ] Mobile companion app
- [ ] EHR integration
- [ ] Advanced analytics
- [ ] Multi-language support

---

**Made with ❤️ for healthcare professionals**

Version 1.0.0 | Last Updated: 2025-10-25
