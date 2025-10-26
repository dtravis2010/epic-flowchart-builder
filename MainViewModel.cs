using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EpicFlowchartBuilder.Models;
using EpicFlowchartBuilder.Services;
using Newtonsoft.Json;

namespace EpicFlowchartBuilder.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FlowchartService _flowchartService;
        private readonly AiAssistantService _aiService;
        private readonly DrawioXmlExporter _xmlExporter;

        private QuestionnaireFlowchart _currentFlowchart;
        private QuestionnaireNode _selectedNode;
        private string _statusMessage;
        private string _aiApiKey;

        public MainViewModel()
        {
            _flowchartService = new FlowchartService();
            _aiService = new AiAssistantService();
            _xmlExporter = new DrawioXmlExporter();

            // Initialize with new flowchart
            CurrentFlowchart = _flowchartService.CreateNewFlowchart("New Questionnaire");

            // Commands
            NewFlowchartCommand = new RelayCommand(ExecuteNewFlowchart);
            SaveFlowchartCommand = new RelayCommand(ExecuteSaveFlowchart);
            LoadFlowchartCommand = new RelayCommand(ExecuteLoadFlowchart);
            ExportToDrawioCommand = new RelayCommand(ExecuteExportToDrawio);
            ValidateFlowchartCommand = new RelayCommand(ExecuteValidateFlowchart);
            AiGenerateCommand = new RelayCommand(ExecuteAiGenerate);
            AiValidateCommand = new RelayCommand(ExecuteAiValidate);
            AddNodeCommand = new RelayCommand<string>(ExecuteAddNode);
            DeleteNodeCommand = new RelayCommand(ExecuteDeleteNode);
            AddConnectionCommand = new RelayCommand(ExecuteAddConnection);
            AutoArrangeCommand = new RelayCommand(ExecuteAutoArrange);
        }

        public QuestionnaireFlowchart CurrentFlowchart
        {
            get => _currentFlowchart;
            set
            {
                _currentFlowchart = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Nodes));
                OnPropertyChanged(nameof(Connections));
            }
        }

        public ObservableCollection<QuestionnaireNode> Nodes => 
            new ObservableCollection<QuestionnaireNode>(CurrentFlowchart?.Nodes ?? new System.Collections.Generic.List<QuestionnaireNode>());

        public ObservableCollection<QuestionnaireConnection> Connections =>
            new ObservableCollection<QuestionnaireConnection>(CurrentFlowchart?.Connections ?? new System.Collections.Generic.List<QuestionnaireConnection>());

        public QuestionnaireNode SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string AiApiKey
        {
            get => _aiApiKey;
            set
            {
                _aiApiKey = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand NewFlowchartCommand { get; }
        public ICommand SaveFlowchartCommand { get; }
        public ICommand LoadFlowchartCommand { get; }
        public ICommand ExportToDrawioCommand { get; }
        public ICommand ValidateFlowchartCommand { get; }
        public ICommand AiGenerateCommand { get; }
        public ICommand AiValidateCommand { get; }
        public ICommand AddNodeCommand { get; }
        public ICommand DeleteNodeCommand { get; }
        public ICommand AddConnectionCommand { get; }
        public ICommand AutoArrangeCommand { get; }

        private void ExecuteNewFlowchart()
        {
            CurrentFlowchart = _flowchartService.CreateNewFlowchart("New Questionnaire");
            StatusMessage = "Created new flowchart";
        }

        private void ExecuteSaveFlowchart()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = ".json"
                };

                if (dialog.ShowDialog() == true)
                {
                    var json = JsonConvert.SerializeObject(CurrentFlowchart, Formatting.Indented);
                    File.WriteAllText(dialog.FileName, json);
                    StatusMessage = $"Saved to {dialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteLoadFlowchart()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    var json = File.ReadAllText(dialog.FileName);
                    CurrentFlowchart = JsonConvert.DeserializeObject<QuestionnaireFlowchart>(json);
                    StatusMessage = $"Loaded from {dialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteExportToDrawio()
        {
            try
            {
                // Validate first
                var issues = _xmlExporter.ValidateExport(CurrentFlowchart);
                if (issues.Any())
                {
                    var message = "Export validation found issues:\n" + string.Join("\n", issues);
                    MessageBox.Show(message, "Validation Issues", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var xml = _xmlExporter.ExportToDrawio(CurrentFlowchart);

                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Draw.io XML files (*.drawio)|*.drawio|XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    DefaultExt = ".drawio"
                };

                if (dialog.ShowDialog() == true)
                {
                    File.WriteAllText(dialog.FileName, xml);
                    StatusMessage = $"Exported to Draw.io format: {dialog.FileName}";
                    MessageBox.Show($"Successfully exported to Draw.io!\n\nOpen the file at: https://app.diagrams.net", 
                        "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteValidateFlowchart()
        {
            var result = CurrentFlowchart.Validate();

            var message = result.IsValid 
                ? "✓ Flowchart is valid!" 
                : "✗ Validation errors found:\n" + string.Join("\n", result.Errors);

            if (result.Warnings.Any())
            {
                message += "\n\nWarnings:\n" + string.Join("\n", result.Warnings);
            }

            StatusMessage = result.IsValid ? "Validation passed" : "Validation failed";
            MessageBox.Show(message, "Validation Result", 
                MessageBoxButton.OK, 
                result.IsValid ? MessageBoxImage.Information : MessageBoxImage.Warning);
        }

        private async void ExecuteAiGenerate()
        {
            if (string.IsNullOrWhiteSpace(AiApiKey))
            {
                MessageBox.Show("Please enter your OpenAI API key first.", "API Key Required", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var description = Microsoft.VisualBasic.Interaction.InputBox(
                "Describe the questionnaire you want to create:",
                "AI Flowchart Generation",
                "Create an MRI screening questionnaire with device implant questions");

            if (string.IsNullOrWhiteSpace(description)) return;

            try
            {
                StatusMessage = "Generating flowchart with AI...";
                CurrentFlowchart = await _aiService.GenerateFlowchartAsync(description, AiApiKey);
                StatusMessage = "AI generation complete!";
                MessageBox.Show("Flowchart generated successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = "AI generation failed";
                MessageBox.Show($"Error generating flowchart: {ex.Message}", "AI Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteAiValidate()
        {
            if (string.IsNullOrWhiteSpace(AiApiKey))
            {
                MessageBox.Show("Please enter your OpenAI API key first.", "API Key Required", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                StatusMessage = "Validating with AI...";
                var result = await _aiService.ValidateFlowchartAsync(CurrentFlowchart, AiApiKey);

                var message = result.IsValid 
                    ? "✓ AI Validation passed!" 
                    : "✗ AI found issues:\n" + string.Join("\n", result.Errors);

                if (result.Warnings.Any())
                {
                    message += "\n\nWarnings:\n" + string.Join("\n", result.Warnings);
                }

                StatusMessage = result.IsValid ? "AI validation passed" : "AI validation found issues";
                MessageBox.Show(message, "AI Validation Result", 
                    MessageBoxButton.OK, 
                    result.IsValid ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                StatusMessage = "AI validation failed";
                MessageBox.Show($"Error during AI validation: {ex.Message}", "AI Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddNode(string nodeTypeStr)
        {
            if (Enum.TryParse<NodeType>(nodeTypeStr, out var nodeType))
            {
                var node = _flowchartService.AddNode(CurrentFlowchart, nodeType, 
                    100 + new Random().Next(500), 
                    100 + new Random().Next(400));
                
                OnPropertyChanged(nameof(Nodes));
                StatusMessage = $"Added {nodeType} node";
            }
        }

        private void ExecuteDeleteNode()
        {
            if (SelectedNode != null)
            {
                _flowchartService.RemoveNode(CurrentFlowchart, SelectedNode.Id);
                SelectedNode = null;
                OnPropertyChanged(nameof(Nodes));
                OnPropertyChanged(nameof(Connections));
                StatusMessage = "Node deleted";
            }
        }

        private void ExecuteAddConnection()
        {
            // This would typically open a dialog to select source and target nodes
            MessageBox.Show("Select two nodes to connect them.", "Add Connection", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteAutoArrange()
        {
            _flowchartService.AutoArrangeNodes(CurrentFlowchart);
            OnPropertyChanged(nameof(Nodes));
            StatusMessage = "Nodes auto-arranged";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
