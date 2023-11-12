using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.Scripting.API
{
    public interface IUiModelActions
    {
        /// <summary>
        /// Creates a new, empty model and loads it into Tabular Editor. This method implicitly calls <see cref="Close(bool)"/> before loading the new model.
        /// </summary>
        /// <param name="compatibilityLevel"></param>
        /// <param name="compatibilityMode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        void New(int compatibilityLevel = 0, CompatibilityMode compatibilityMode = CompatibilityMode.Unknown, string name = "Semantic Model");

        /// <summary>
        /// Closes the model currently open in Tabular Editor. If the Tabular Editor UI is showing and <paramref name="noPrompt"/>>
        /// is false, a confirmation prompt is shown to the user, in case the model has unsaved changes. If the user chooses the
        /// "Cancel" option, the method throws a <see cref="ScriptCanceledException"/>. If the UI is not showing, the model is
        /// closed without saving any changes. If no model is currently loaded, this method does nothing.
        /// </summary>
        void Close(bool noPrompt = false);

        /// <summary>
        /// If the Tabular Editor UI is showing, shows the Open Model From File dialog letting the user choose a model file to open.
        /// If the dialog is canceled, the method throws a <see cref="ScriptCanceledException"/>. Once a model is loaded, you can
        /// access it through the <see cref="ScriptHost.Model"/> property. If the UI is not showing, this method throws a
        /// <see cref="NoUiException"/>. This method implicitly calls <see cref="Close(bool)"/> before opening the new model.
        /// </summary>
        void OpenFromFile();

        /// <summary>
        /// Loads the specified model metadata file into Tabular Editor. Once a model is loaded, you can
        /// access it through the <see cref="ScriptHost.Model"/> property. 
        /// This method implicitly calls <see cref="Close(bool)"/> before opening the new model.
        /// </summary>
        /// <param name="fileName">The path to the database.json or TMDL folder, or the path to the model.bim file</param>
        void OpenFromFile(string fileName);

        /// <summary>
        /// If the Tabular Editor UI is showing, shows the Open Model From Database dialog letting the user choose a model to open.
        /// If the dialog is canceled, the method throws a <see cref="ScriptCanceledException"/>. Once a model is loaded, you can
        /// access it through the <see cref="ScriptHost.Model"/> property. If the UI is not showing, this method throws a
        /// <see cref="NoUiException"/>. This method implicitly calls <see cref="Close(bool)"/> before opening the new model.
        /// </summary>
        void OpenFromDB();

        /// <summary>
        /// Loads the model specified by the XMLA connection string and database name. If no database name is specified, it is
        /// assumed that the AS instance only holds a single database. Otherwise, an exception is thrown. This method supports
        /// authenticating with MFA only when the Tabular Editor UI is showing.
        /// This method implicitly calls <see cref="Close(bool)"/> before opening the new model.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        void OpenFromDB(string connectionString, string databaseName = null);

        /// <summary>
        /// Saves the changes back to the currently loaded model. If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        void Save();

        /// <summary>
        /// If the Tabular Editor UI is showing, shows the Save As dialog, letting the user choose the file name and location to save the
        /// model metadata. If the UI is not showing, this method throws a <see cref="NoUiException"/>. If the dialog is canceled, no save
        /// is performed, and the method returns false.
        /// </summary>
        bool SaveAs(SaveMode saveMode = SaveMode.Default);

        /// <summary>
        /// Saves the model metadata to the specified file name. If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveOptions"></param>
        /// <returns></returns>
        void SaveAs(string fileName, SaveOptions saveOptions = null);

        /// <summary>
        /// Shows the Tabular Editor Deployment Wizard for the currently loaded model. If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// If the deployment fails, a <see cref="DeploymentException"/> is thrown.
        /// </summary>
        void Deploy();

        /// <summary>
        /// Deploys the currently loaded model against the specified XMLA connection string and database name, using the specified deployment options. 
        /// If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// If the deployment fails, a <see cref="DeploymentException"/> is thrown. 
        /// </summary>
        void Deploy(Model model, string connectionString, string databaseName, DeployOptions deployOptions = null);

        /// <summary>
        /// Gets the deployment TMSL script that would be executed by calling the equivalent <see cref="Deploy"/> method.
        /// <paramref name="connectionString"/> and <paramref name="databaseName"/> can be set to null if the <see cref="DeployOptions.AllowOverwrite"/>
        /// property is false, indicating a new deployment. If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        string GetDeployTmsl(Model model, string connectionString, string databaseName, DeployOptions deployOptions = null);

        /// <summary>
        /// Runs the Best Practice Analyzer on the currently loaded model, and returns a list of <see cref="AnalyzerResult"/> objects representing
        /// rule violations. If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<AnalyzerResult> Analyze();

        /// <summary>
        /// Runs the Best Practice Analyzer on the currently loaded model using the specified rule file(s), and returns a list of <see cref="AnalyzerResult"/> objects representing
        /// rule violations. If no model is loaded, this throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        IReadOnlyList<AnalyzerResult> Analyze(params string[] rules);
    }

    public interface IMetadataActions
    {
        /// <summary>
        /// Creates a new, empty model and returns a <see cref="Model"/> object representation of the metadata.
        /// </summary>
        /// <param name="compatibilityLevel"></param>
        /// <param name="compatibilityMode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Model New(int compatibilityLevel = 0, CompatibilityMode compatibilityMode = CompatibilityMode.Unknown, string name = "Semantic Model");

        /// <summary>
        /// Loads the specified model metadata file and returns a <see cref="Model"/> object representation of the metadata.
        /// </summary>
        /// <param name="fileName">The path to the database.json or TMDL folder, or the path to the model.bim file</param>
        Model LoadFromFile(string fileName);

        /// <summary>
        /// Loads the model specified by the XMLA connection string and database name and returns a <see cref="Model"/> object
        /// represenation of the metadata. If no database name is specified, it is
        /// assumed that the AS instance only holds a single database. Otherwise, an exception is thrown. This method supports
        /// authenticating with MFA only when the Tabular Editor UI is showing.
        /// </summary>
        Model LoadFromDB(string connectionString, string databaseName = null);

        /// <summary>
        /// Saves the changes back to the specified model metadata source (file or database). Throws an exception if the model
        /// was created using <see cref="New(int, CompatibilityMode)"/> (in that case, call <see cref="SaveAs(Model, string, SaveOptions)"/> instead).
        /// </summary>
        void Save(Model model);

        /// <summary>
        /// Saves the model metadata to the specified file name. If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveOptions"></param>
        /// <returns></returns>
        void SaveAs(Model model, string fileName, SaveOptions saveOptions = null);

        /// <summary>
        /// Deploys the specified model against the specified XMLA connection string and database name, using the specified deployment options.
        /// If the deployment fails, a <see cref="DeploymentException"/> is thrown.
        /// </summary>
        void Deploy(Model model, string connectionString, string databaseName, DeployOptions deployOptions = null);

        /// <summary>
        /// Gets the deployment TMSL script that would be executed by calling the equivalent <see cref="Deploy"/> method.
        /// <paramref name="connectionString"/> and <paramref name="databaseName"/> can be set to null if the <see cref="DeployOptions.AllowOverwrite"/>
        /// property is false, indicating a new deployment.
        /// </summary>
        string GetDeployTmsl(Model model, string connectionString, string databaseName, DeployOptions deployOptions = null);

        /// <summary>
        /// Runs the Best Practice Analyzer on the specified model, and returns a list of <see cref="AnalyzerResult"/> objects representing
        /// rule violations.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<AnalyzerResult> Analyze(Model model);

        /// <summary>
        /// Runs the Best Practice Analyzer on the specified model using the specified rule file(s), and returns a list of <see cref="AnalyzerResult"/> objects representing
        /// rule violations.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        IReadOnlyList<AnalyzerResult> Analyze(Model model, params string[] rules);
    }

    public interface IUiActions
    {
        /// <summary>
        /// Actions for opening, saving, deploying and closing the model loaded in Tabular Editor's UI.
        /// </summary>
        IUiModelActions Model { get; }

        /// <summary>
        /// The text shown in the status bar while the script is executed.
        /// </summary>
        string StatusText { get; set; }

        /// <summary>
        /// When set to a value between 0 and 1, a progress indicator will be displayed in the status bar while the script is executed.
        /// </summary>
        double? Progress { get; set; }

        /// <summary>
        /// Actions for retrieving information about the current document, cursor position, tokens, etc. as well as modifying its content.
        /// null if no document is currently open.
        /// </summary>
        IDocumentActions CurrentDocument { get; set; }
    }

    public interface IDocumentActions
    {
        /// <summary>
        /// The document content type.
        /// </summary>
        DocumentType Type { get; }

        /// <summary>
        /// The source object from which the document was generated. For example, if the document represents the DAX expression
        /// of a measure, the measure would be the <see cref="ExpressionSource"/>.
        /// </summary>
        TabularNamedObject ExpressionSource { get; }

        /// <summary>
        /// Some source objects can have multiple DAX expression properties. If the document represents a specific property, 
        /// the <see cref="ExpressionProperty"/> specifies which one.
        /// </summary>
        ExpressionProperty? ExpressionProperty { get; }

        /// <summary>
        /// The start position of the current selection within the document
        /// </summary>
        int SelectionStart { get; }

        /// <summary>
        /// The length of the current selection within the document
        /// </summary>
        int SelectionLength { get; }

        /// <summary>
        /// A string containing the current selection
        /// </summary>
        string SelectedText { get; }

        /// <summary>
        /// Sets the start and length of the current selection within the document
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void SetSelection(int start, int length);
        
        /// <summary>
        /// Replaces the specified range of the document with the specified text.
        /// </summary>
        void ReplaceRange(int start, int length, string text);

        /// <summary>
        /// If the document contains DAX, retrieve a list of <see cref="DaxToken"/> objects representing the DAX content, including whitespace and comment tokens.
        /// Returns null if the document does not contain DAX.
        /// </summary>
        IReadOnlyList<DaxToken> DaxTokens { get; }

        /// <summary>
        /// If the document contains DAX, retrieves the <see cref="DaxToken"/> at the specified position.
        /// Returns null if the document does not contain DAX.
        /// </summary>
        DaxToken GetDaxTokenAtPos(int position);

        /// <summary>
        /// Whether the document content can currently be updated.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// The document content represented as a string.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Indicates if changes were made to the content, which have not been saved to the document source (the model metadata).
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Save content changes to the document source (the model metadata).
        /// </summary>
        void Accept();

        /// <summary>
        /// Discard content changes.
        /// </summary>
        void Cancel();
    }

    public enum DocumentType
    {
        DaxExpression,
        DaxScript,
        DaxQuery,
        Sql,
        M,
        CSharp
    }

    public interface IScriptApi
    {
        IUiActions UI { get; }
        IMetadataActions Metadata { get; }
    }

    public record class SaveOptions(SaveMode Mode);

    public record class DeployOptions(
        bool AllowOverwrite = false,
        bool DeployConnections = false,
        bool DeployPartitions = false,
        bool SkipRefreshPolicyPartitions = true,
        bool DeployRoles = true,
        bool DeployRoleMembers = false
        );

    public enum SaveMode
    {
        Default,
        TMSL,
        Folder,
        TMDL
    }
}
