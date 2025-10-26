# 📑 File Index - Epic Flowchart Builder

Complete guide to every file in the project.

## 🎯 Entry Points

| File | Purpose | When to Read |
|------|---------|--------------|
| **START-HERE.md** | Project entry point | First time visiting |
| **QUICKSTART.md** | 5-minute tutorial | Ready to build |
| **README.md** | Complete documentation | Want full details |

---

## 📁 Project Root Files

### Build & Configuration

| File | Type | Description |
|------|------|-------------|
| `EpicFlowchartBuilder.csproj` | XML | Project configuration, NuGet dependencies |
| `build.bat` | Batch | Windows build script |
| `build.sh` | Shell | Linux/Mac build script |
| `App.xaml` | XAML | Application definition and global styles |
| `App.xaml.cs` | C# | Application startup logic |

**When to edit:**
- `*.csproj` - Adding dependencies
- `build.*` - Custom build steps
- `App.xaml` - Global styling

---

## 📁 Models/ (Data Layer)

| File | Lines | Description |
|------|-------|-------------|
| `QuestionnaireModels.cs` | 260 | Core data structures |

**Contains:**
- `NodeType` enum - Start, Decision, Question, Action, End
- `InputType` enum - YesNo, MultipleChoice, FreeText, etc.
- `QuestionnaireNode` class - Individual flowchart nodes
- `QuestionnaireConnection` class - Connections between nodes
- `SkipLogic` class - Conditional branching rules
- `NodePosition` & `NodeSize` - Layout data
- `QuestionnaireFlowchart` class - Complete flowchart
- `FlowchartValidationResult` class - Validation output

**When to edit:**
- Adding new node types
- Adding new input types
- Extending validation logic
- Adding metadata fields

---

## 📁 ViewModels/ (Presentation Logic)

| File | Lines | Description |
|------|-------|-------------|
| `MainViewModel.cs` | 450+ | Application state and commands |

**Contains:**
- `MainViewModel` class - Primary view model
- Properties for current flowchart, nodes, connections
- Command implementations (New, Save, Load, Export, etc.)
- AI integration methods
- Event handling logic
- `RelayCommand` & `RelayCommand<T>` helpers

**When to edit:**
- Adding new commands
- Changing application behavior
- Adding new properties
- Modifying AI integration

---

## 📁 Views/ (User Interface)

| File | Type | Lines | Description |
|------|------|-------|-------------|
| `MainWindow.xaml` | XAML | 250+ | Main UI layout |
| `MainWindow.xaml.cs` | C# | 100+ | UI code-behind |

**MainWindow.xaml contains:**
- Menu bar (File, Edit, Validate, Help)
- Toolbar (Quick actions, node buttons)
- Three-panel layout:
  - Left: AI Assistant panel
  - Center: Canvas for flowchart
  - Right: Properties panel
- Status bar

**MainWindow.xaml.cs contains:**
- Event handlers (Exit, About, etc.)
- Value converters:
  - `NodeTypeToColorConverter` - Node colors
  - `NullToBooleanConverter` - IsEnabled bindings

**When to edit:**
- Changing UI layout
- Adding new panels
- Modifying color schemes
- Adding new converters

---

## 📁 Services/ (Business Logic)

### FlowchartService.cs (280 lines)

**Purpose:** Core flowchart operations

**Key Methods:**
- `CreateNewFlowchart()` - Initialize new flowchart
- `AutoArrangeNodes()` - Hierarchical layout algorithm
- `AddNode()` - Create and add nodes
- `RemoveNode()` - Delete nodes and connections
- `AddConnection()` - Create connections
- `RemoveConnection()` - Delete connections

**When to edit:**
- Changing layout algorithm
- Adding node operations
- Modifying default sizes

---

### AiAssistantService.cs (430+ lines)

**Purpose:** AI integration with OpenAI

**Key Methods:**
- `GenerateFlowchartAsync()` - Create flowchart from description
- `ValidateFlowchartAsync()` - AI-powered validation
- `SetApiKey()` - Configure API authentication
- `CallOpenAI()` - HTTP API communication
- `GetGenerationSystemPrompt()` - Embedded generation prompt
- `GetValidationSystemPrompt()` - Embedded validation prompt

**Embedded Prompts:**
- Generation prompt (healthcare-aware)
- Validation prompt (safety checks)
- JSON schema specifications
- Positioning guidelines

**When to edit:**
- Updating AI prompts
- Changing AI models
- Modifying parsing logic
- Adding new AI features

---

### DrawioXmlExporter.cs (347 lines)

**Purpose:** Export to Draw.io XML format

**Key Methods:**
- `ExportToDrawio()` - Generate XML string
- `CreateDrawioDocument()` - Build XML structure
- `CreateNodeCell()` - Node XML elements
- `CreateEdgeCell()` - Connection XML elements
- `GetNodeStyle()` - Style mapping for nodes
- `GetEdgeStyle()` - Style mapping for edges
- `CalculateExitPoint()` - Connection routing
- `CalculateEntryPoint()` - Connection routing
- `ValidateExport()` - Pre-export validation

**When to edit:**
- Changing node colors
- Modifying shapes
- Adjusting connection routing
- Adding new styles

---

## 📁 Examples/

| File | Type | Description |
|------|------|-------------|
| `MRI-Screening-Example.json` | JSON | Sample MRI screening questionnaire |

**Contains:**
- 9 nodes (Start, 2 Decisions, 2 Questions, 4 Actions, End)
- 10 connections
- Skip logic examples
- Healthcare safety patterns
- Proper positioning

**When to use:**
- Learning the JSON format
- Understanding skip logic
- Testing the application
- Template for new questionnaires

---

## 📁 Documentation/

| File | Lines | Purpose |
|------|-------|---------|
| `START-HERE.md` | 200+ | Entry point for all users |
| `QUICKSTART.md` | 150+ | 5-minute tutorial |
| `README.md` | 500+ | Complete user documentation |
| `PROJECT-SUMMARY.md` | 600+ | Technical deep-dive |
| `FILE-INDEX.md` | This file | Navigate the project |

### START-HERE.md
- Quick start paths
- System requirements
- Learning path
- First task guide
- Common questions

### QUICKSTART.md
- 5-minute getting started
- Step-by-step tutorial
- Common first tasks
- Keyboard shortcuts
- Quick tips

### README.md
- Complete feature list
- Installation guide
- Usage examples
- Architecture overview
- Troubleshooting
- Configuration
- File formats
- Healthcare compliance
- Roadmap

### PROJECT-SUMMARY.md
- Project statistics
- Architecture details
- Design decisions
- File structure
- Technical implementation
- Performance metrics
- Future enhancements
- Testing guide
- Development guidelines

---

## 🔍 File Dependencies

```
App.xaml → Views/MainWindow.xaml
    ↓
MainWindow.xaml → MainViewModel
    ↓
MainViewModel → Services (FlowchartService, AiAssistantService)
    ↓
Services → Models (QuestionnaireModels)
    ↓
DrawioXmlExporter → Models
```

---

## 📊 File Size Guide

| Size | Files | Purpose |
|------|-------|---------|
| < 100 lines | App.xaml.cs, build scripts | Configuration |
| 100-300 lines | MainWindow.xaml.cs, QuestionnaireModels.cs | Core components |
| 300-500 lines | MainViewModel.cs, AiAssistantService.cs, DrawioXmlExporter.cs | Major features |
| 500+ lines | README.md, PROJECT-SUMMARY.md | Documentation |

---

## 🎯 Quick Reference

### I want to...

**...change node colors**
→ `Views/MainWindow.xaml.cs` - `NodeTypeToColorConverter`

**...modify AI prompts**
→ `Services/AiAssistantService.cs` - `GetGenerationSystemPrompt()`

**...change layout algorithm**
→ `Services/FlowchartService.cs` - `AutoArrangeNodes()`

**...add new node types**
→ `Models/QuestionnaireModels.cs` - `NodeType` enum

**...customize UI**
→ `Views/MainWindow.xaml` - XAML layout

**...add new commands**
→ `ViewModels/MainViewModel.cs` - Command definitions

**...modify export format**
→ `Services/DrawioXmlExporter.cs` - XML generation

**...add dependencies**
→ `EpicFlowchartBuilder.csproj` - PackageReference

---

## 📝 Editing Guidelines

### Code Files (.cs)
1. Follow MVVM pattern
2. Document public methods
3. Use meaningful names
4. Handle exceptions
5. Keep methods focused

### XAML Files (.xaml)
1. Use data binding
2. Minimize code-behind
3. Apply styles consistently
4. Comment complex layouts

### Documentation (.md)
1. Keep format consistent
2. Update version dates
3. Verify links work
4. Include examples

---

## 🔄 File Relationships

### Data Flow
```
User Input → MainWindow.xaml
    ↓
Event → MainWindow.xaml.cs
    ↓
Command → MainViewModel
    ↓
Service Call → FlowchartService/AiService
    ↓
Model Update → QuestionnaireModels
    ↓
Property Change → UI Update
```

### Save/Load Flow
```
Save: 
QuestionnaireFlowchart → JSON → File System

Load:
File System → JSON → QuestionnaireFlowchart → UI
```

### Export Flow
```
QuestionnaireFlowchart → DrawioXmlExporter
    ↓
Validation → XML Generation
    ↓
Style Mapping → Connection Routing
    ↓
XML File → Draw.io
```

---

## 📈 Maintenance Checklist

### When updating code:
- [ ] Update XML docs
- [ ] Test all commands
- [ ] Verify validation
- [ ] Check export format
- [ ] Update version numbers
- [ ] Update README.md
- [ ] Test build scripts

### When adding features:
- [ ] Update MainViewModel
- [ ] Add to README.md
- [ ] Create examples
- [ ] Add to QUICKSTART.md
- [ ] Update screenshots
- [ ] Document in code

---

## 🎓 Learning Order

### For Users:
1. START-HERE.md
2. QUICKSTART.md
3. README.md (Usage section)
4. Examples/

### For Developers:
1. PROJECT-SUMMARY.md
2. Models/QuestionnaireModels.cs
3. Services/*.cs
4. ViewModels/MainViewModel.cs
5. Views/*.xaml

---

## 📞 Getting Help

**Can't find a file?** Use your IDE's search (Ctrl+P in VS Code)

**Don't understand a file?** Check PROJECT-SUMMARY.md

**Want to modify something?** Look in "I want to..." section above

**Need more details?** Read the comments in the actual files

---

**Last Updated:** October 25, 2025  
**Total Files:** 17 source + 5 documentation = 22 files  
**Total Lines:** ~7,000 lines (code + docs)  

---

*This index covers all files in Epic Flowchart Builder v1.0.0*
