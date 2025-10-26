# Quick Start Guide

## 5-Minute Getting Started

### Step 1: Build and Run (30 seconds)
```bash
# Windows
build.bat

# Linux/Mac
./build.sh

# Then run
cd bin/Release/net8.0-windows
EpicFlowchartBuilder.exe
```

### Step 2: Create Your First Flowchart (2 minutes)

1. **Add Start Node**
   - Click "⭕ Start" in toolbar
   - It appears on canvas

2. **Add a Decision Node**
   - Click "◇ Decision" in toolbar
   - Click the node to edit
   - Change text to "Are you pregnant?"

3. **Add Action Nodes**
   - Click "▭ Action" twice
   - Edit first: "Cannot proceed - refer to physician"
   - Edit second: "Cleared for procedure"

4. **Add End Node**
   - Click "⭕ End" in toolbar

5. **Auto Arrange**
   - Click "Arrange" button
   - Nodes organize automatically

### Step 3: Save and Export (1 minute)

1. **Validate**
   - Click "✓ Validate" button
   - Fix any errors shown

2. **Save**
   - File → Save
   - Choose filename
   - Saved as JSON

3. **Export to Draw.io**
   - File → Export to Draw.io
   - Open at https://app.diagrams.net
   - Continue editing there

### Step 4: Try AI Generation (Optional - 2 minutes)

1. **Get API Key**
   - Visit https://platform.openai.com/api-keys
   - Create new key
   - Copy it

2. **Generate Flowchart**
   - Paste API key in left panel
   - Click "🤖 Generate from Description"
   - Enter: "MRI screening with device check"
   - Click OK

3. **Review and Edit**
   - AI generates complete flowchart
   - Edit as needed
   - Validate and export

## Common First Tasks

### Create Pregnancy Screening
```
Use AI with this description:
"Create a pregnancy screening questionnaire for X-ray procedures:
1. Ask if patient is pregnant
2. If yes, stop procedure
3. If no, proceed to X-ray"
```

### Create Allergy Check
```
Manually add:
- Start: "Allergy Screening"
- Decision: "Any medication allergies?" (Yes/No)
  - Yes → Question: "Which medications?" (Free text)
  - No → Action: "No allergies recorded"
- End: "Screening Complete"
```

### Create Device Screening
```
Use AI with this description:
"MRI device screening:
1. Ask about implanted devices
2. If yes, ask device type (pacemaker, ICD, neurostimulator)
3. If yes, ask implant date
4. If no devices, clear for MRI
5. End screening"
```

## Tips for Success

### ✅ Do's
- ✅ Start with simple 3-5 node flowcharts
- ✅ Use AI for complex questionnaires
- ✅ Validate before exporting
- ✅ Save frequently
- ✅ Use auto-arrange for layout

### ❌ Don'ts
- ❌ Don't skip validation
- ❌ Don't create disconnected nodes
- ❌ Don't forget decision branch labels
- ❌ Don't exceed 50 nodes per flowchart
- ❌ Don't share API keys

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+N` | New Flowchart |
| `Ctrl+S` | Save |
| `Ctrl+O` | Open |
| `Ctrl+E` | Export to Draw.io |
| `Del` | Delete selected node |
| `F5` | Validate |
| `Ctrl+A` | Auto-arrange |

## Next Steps

1. **Read full README.md** - Complete documentation
2. **Try examples** - See `/Examples` folder
3. **Learn AI prompts** - Better generation results
4. **Explore Draw.io** - Advanced editing features

## Need Help?

- 🐛 **Bug?** Check Troubleshooting in README
- 💡 **Question?** See full documentation
- 🚀 **Feature request?** Open GitHub issue

---

**You're ready to go! Start building healthcare questionnaires!** 🎉
