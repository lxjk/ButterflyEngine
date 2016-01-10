//Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.

using System;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using Sce.Atf;
using Sce.Atf.Applications;
using Sce.Atf.Applications.NetworkTargetServices;

using HelpAboutCommand = Sce.Atf.Applications.HelpAboutCommand;

namespace ButterflyEngine
{
    /// <summary>
    /// This is a target manager sample application.
    /// TargetManager shows how to use the TargetEnumerationService to discover, add, configure and select targets. 
    /// Targets are network endpoints, such as TCP/IP addresses, PS3 DevKits (to be added) or Vita DevKits. 
    /// TargetEnumerationService is implemented as a MEF component that supports the ITargetConsumer interface 
    /// for querying and enumerating targets. 
    /// It consumes target providers created by the application.
    /// For more information, see https://github.com/SonyWWS/ATF/wiki/ATF-Target-Manager-Sample.
    /// For a discussion of this sample's programming, see 
    /// https://github.com/SonyWWS/ATF/wiki/Target-Manager-Programming-Discussion. </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application</summary>
        [STAThread]
        static void Main()
        {
            // important to call these before creating application host
            Application.EnableVisualStyles();
            Application.DoEvents(); // see http://www.codeproject.com/buglist/EnableVisualStylesBug.asp?df=100&forumid=25268&exp=0&select=984714

            // Set up localization support early on, so that user-readable strings will be localized
            //  during the initialization phase below. Use XML files that are embedded resources.
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;
            Localizer.SetStringLocalizer(new EmbeddedResourceStringLocalizer());

            var catalog = new TypeCatalog(
                typeof(SettingsService),                // persistent settings and user preferences dialog
                typeof(StatusService),                  // status bar at bottom of main Form
                typeof(CommandService),                 // handles commands in menus and toolbars
                typeof(ControlHostService),             // docking control host
                typeof(WindowLayoutService),            // multiple window layout support
                typeof(WindowLayoutServiceCommands),    // window layout commands
                typeof(FileDialogService),              // standard Windows file dialogs
                typeof(AutoDocumentService),            // opens documents from last session, or creates a new document, on startup
                typeof(Outputs),                        // service that provides static methods for writing to IOutputWriter objects.
                typeof(OutputService),                  // rich text box for displaying error and warning messages. Implements IOutputWriter.
                
                typeof(RecentDocumentCommands),         // standard recent document commands in File menu
                typeof(StandardFileCommands),           // standard File menu commands for New, Open, Save, SaveAs, Close
                typeof(StandardFileExitCommand),        // standard File exit menu command
                typeof(StandardEditCommands),           // standard Edit menu commands for copy/paste
                typeof(StandardEditHistoryCommands),    // standard Edit menu commands for undo/redo
                typeof(StandardSelectionCommands),      // standard Edit menu selection commands
                typeof(HelpAboutCommand),               // Help -> About command

                typeof(UnhandledExceptionService),      // catches unhandled exceptions, displays info, and gives user a chance to save
                typeof(ContextRegistry),                // central context registry with change notification
                typeof(DocumentRegistry),               // central document registry with change notification
                typeof(MainWindowTitleService),         // tracks document changes and updates main form title
                typeof(TabbedControlSelector),          // enable ctrl-tab selection of documents and controls within the app
                typeof(DefaultTabCommands),             // provides the default commands related to document tab Controls

                typeof(PropertyEditor),                 // property grid for editing selected objects
                //typeof(GridPropertyEditor),             // grid control for editing selected objects
                typeof(PropertyEditingCommands),        // commands for PropertyEditor and GridPropertyEditor, like Reset,
                                                        //  Reset All, Copy Value, Paste Value, Copy All, Paste All

                typeof(Editor),                         // code editor component
                typeof(SchemaLoader),                   // loads schema and extends types
                typeof(CharacterEditor),
                typeof(CharacterSettingsCommands)

             );

            // Set up the MEF container with these components
            var container = new CompositionContainer(catalog);

            var toolStripContainer = new ToolStripContainer();
            toolStripContainer.Dock = DockStyle.Fill;

            var mainForm = new MainForm(toolStripContainer);
            mainForm.Icon = GdiUtil.CreateIcon(ResourceUtil.GetImage(Sce.Atf.Resources.AtfIconImage));

            mainForm.Text = "Butterfly Engine".Localize();

            var batch = new CompositionBatch();
            batch.AddPart(mainForm);
            container.Compose(batch);

            // To make the tab commands (e.g., "Copy Full Path", "Open Containing Folder") available, we have to change
            //  the default behavior to work with this sample app's unusual Editor. In most cases, an editor like this
            //  would implement IDocumentClient and this customization of DefaultTabCommands wouldn't be necessary.
            var tabCommands = container.GetExportedValue<DefaultTabCommands>();
            tabCommands.IsDocumentControl = controlInfo => controlInfo.Client is Editor;

            // Initialize components that require it. Initialization often can't be done in the constructor,
            //  or even after imports have been satisfied by MEF, since we allow circular dependencies between
            //  components, via the System.Lazy class. IInitializable allows components to defer some operations
            //  until all MEF composition has been completed.
            container.InitializeAll();

            // Show the main form and start message handling. The main Form Load event provides a final chance
            //  for components to perform initialization and configuration.
            Application.Run(mainForm);

            // Give components a chance to clean up.
            container.Dispose();

        }
    }
}
