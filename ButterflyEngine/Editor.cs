using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Applications;
using Sce.Atf.Dom;

namespace ButterflyEngine
{
    [Export(typeof(IDocumentClient))]
    [Export(typeof(Editor))]
    [Export(typeof(IInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Editor : IDocumentClient, IControlHostClient, IInitializable, ICommandClient
    {
        private ICommandService m_commandService;
        private IControlHostService m_controlHostService;
        private IDocumentService m_documentService;
        private IDocumentRegistry m_documentRegistry;
        private IFileDialogService m_fileDialogService;
        private IContextRegistry m_contextRegistry;
        private SchemaLoader m_schemaLoader;

        [Import(AllowDefault = true)]
        private IStatusService m_statusService = null;
        
        /// <summary>
        /// Constructor</summary>
        /// <param name="commandService">Command service</param>
        /// <param name="controlHostService">Control host service</param>
        /// <param name="documentService">Document service</param>
        /// <param name="documentRegistry">Document registry used to track documents</param>
        /// <param name="fileDialogService">File dialog service</param>
        [ImportingConstructor]
        public Editor(
            ICommandService commandService,
            IControlHostService controlHostService,
            IDocumentService documentService,
            IDocumentRegistry documentRegistry,
            IFileDialogService fileDialogService,
            IContextRegistry contextRegistry,
            SchemaLoader schemaLoader
            )
        {
            m_commandService = commandService;
            m_controlHostService = controlHostService;
            m_documentService = documentService;
            m_documentRegistry = documentRegistry;
            m_fileDialogService = fileDialogService;
            m_contextRegistry = contextRegistry;
            m_schemaLoader = schemaLoader;
        }

        #region IInitializable Members

        public void Initialize()
        {
            // register commands
            m_commandService.RegisterCommand(CommandInfo.EditUndo, this);
            m_commandService.RegisterCommand(CommandInfo.EditRedo, this);
            m_commandService.RegisterCommand(CommandInfo.EditCut, this);
            m_commandService.RegisterCommand(CommandInfo.EditCopy, this);
            m_commandService.RegisterCommand(CommandInfo.EditPaste, this);
            m_commandService.RegisterCommand(CommandInfo.EditDelete, this);

            m_commandService.RegisterCommand(
                Command.FindReplace,
                StandardMenu.Edit,
                StandardCommandGroup.EditOther,
                "Find and Replace...".Localize(),
                "Find and replace text".Localize(),
                Keys.None,
                Resources.FindImage,
                CommandVisibility.Menu,
                this);

            m_commandService.RegisterCommand(
                Command.Goto,
                StandardMenu.Edit,
                StandardCommandGroup.EditOther,
                "Go to...".Localize(),
                "Go to line".Localize(),
                Keys.None,
                null,
                CommandVisibility.Menu,
                this);
        }

        #endregion

        #region IControlHostClient Members

        /// <summary>
        /// Notifies the client that its Control has been activated. Activation occurs when
        /// the Control gets focus, or a parent "host" Control gets focus.</summary>
        /// <param name="control">Client Control that was activated</param>
        /// <remarks>This method is only called by IControlHostService if the Control was previously
        /// registered for this IControlHostClient.</remarks>
        public void Activate(Control control)
        {
            if (control.Tag is StoryDocument)
            {
                IDocument doc = (IDocument)control.Tag;
                m_documentRegistry.ActiveDocument = doc;

               StoryContext context = doc.As<StoryContext>();
                m_contextRegistry.ActiveContext = context;

                m_commandService.SetActiveClient(this);
            }
        }

        /// <summary>
        /// Notifies the client that its Control has been deactivated. Deactivation occurs when
        /// another Control or "host" Control gets focus.</summary>
        /// <param name="control">Client Control that was deactivated</param>
        /// <remarks>This method is only called by IControlHostService if the Control was previously
        /// registered for this IControlHostClient.</remarks>
        public void Deactivate(Control control)
        {
            m_commandService.SetActiveClient(null);
        }

        /// <summary>
        /// Requests permission to close the client's Control</summary>
        /// <param name="control">Client Control to be closed</param>
        /// <returns>True if the Control can close, or false to cancel</returns>
        /// <remarks>
        /// 1. This method is only called by IControlHostService if the Control was previously
        /// registered for this IControlHostClient.
        /// 2. If true is returned, the IControlHostService calls its own
        /// UnregisterControl. The IControlHostClient has to call RegisterControl again
        /// if it wants to re-register this Control.</remarks>
        public bool Close(Control control)
        {
            StoryDocument document = control.Tag as StoryDocument;
            if (document != null)
                return m_documentService.Close(document);

            return true;
        }

        #endregion

        #region ICommandClient Members

        /// <summary>
        /// Checks whether the client can do the command, if it handles it</summary>
        /// <param name="commandTag">Command to be done</param>
        /// <returns>True iff client can do the command</returns>
        public bool CanDoCommand(object commandTag)
        {
            bool canDo = false;
            StoryDocument doc = m_documentRegistry.ActiveDocument as StoryDocument;
            if (commandTag is StandardCommand)
            {
                if (doc != null)
                {
                    //switch ((StandardCommand)commandTag)
                    //{
                    //    case StandardCommand.EditUndo:
                    //        canDo = doc.Editor.CanUndo;
                    //        break;

                    //    case StandardCommand.EditRedo:
                    //        canDo = doc.Editor.CanRedo;
                    //        break;

                    //    case StandardCommand.EditCut:
                    //    case StandardCommand.EditDelete:
                    //        canDo = doc.Editor.HasSelection && !doc.Editor.ReadOnly;
                    //        break;

                    //    case StandardCommand.EditCopy:
                    //        canDo = doc.Editor.HasSelection;
                    //        break;

                    //    case StandardCommand.EditPaste:
                    //        canDo = doc.Editor.CanPaste;
                    //        break;
                    //}
                }
            }
            else if (commandTag is Command)
            {
                switch ((Command)commandTag)
                {
                    case Command.Goto:
                    case Command.FindReplace:
                        canDo = doc != null;
                        break;

                    default:
                        canDo = true;
                        break;

                }
            }

            return canDo;
        }

        /// <summary>
        /// Does the command</summary>
        /// <param name="commandTag">Command to be done</param>
        public void DoCommand(object commandTag)
        {
            StoryDocument activeDocument = m_documentRegistry.ActiveDocument as StoryDocument;
            if (commandTag is StandardCommand)
            {
                //switch ((StandardCommand)commandTag)
                //{
                //    case StandardCommand.EditUndo:
                //        activeDocument.Editor.Undo();
                //        break;

                //    case StandardCommand.EditRedo:
                //        activeDocument.Editor.Redo();
                //        break;

                //    case StandardCommand.EditCut:
                //        activeDocument.Editor.Cut();
                //        break;

                //    case StandardCommand.EditCopy:
                //        activeDocument.Editor.Copy();
                //        break;

                //    case StandardCommand.EditPaste:
                //        activeDocument.Editor.Paste();
                //        break;

                //    case StandardCommand.EditDelete:
                //        activeDocument.Editor.Delete();
                //        break;
                //}
            }
            else if (commandTag is Command)
            {
                switch ((Command)commandTag)
                {
                    //case Command.FindReplace:
                    //    activeDocument.Editor.ShowFindReplaceForm();
                    //    break;

                    //case Command.Goto:
                    //    activeDocument.Editor.ShowGoToLineForm();
                    //    break;
                }
            }
        }

        /// <summary>
        /// Updates command state for given command</summary>
        /// <param name="commandTag">Command</param>
        /// <param name="commandState">Command info to update</param>
        public void UpdateCommand(object commandTag, CommandState commandState)
        {
        }

        #endregion

        private void ShowStatus(string message)
        {
            if (m_statusService != null)
                m_statusService.ShowStatus(message);
        }

        private enum Command
        {
            FindReplace,
            Goto
        }


        #region IDocumentClient Members

        /// <summary>
        /// Gets information about the document client, such as the file type and file
        /// extensions it supports, whether or not it allows multiple documents to be open, etc.</summary>
        public DocumentClientInfo Info
        {
            get { return DocumentClientInfo; }
        }

        /// <summary>
        /// Information about the document client</summary>
        public static DocumentClientInfo DocumentClientInfo = new DocumentClientInfo(
            "Story".Localize(),
            new string[] { ".xml", ".sto" },
            Sce.Atf.Resources.DocumentImage,
            Sce.Atf.Resources.FolderImage,
            true)
        {
            DefaultExtension = ".xml" //avoids requiring the user to choose a filename when creating a new doc
        };

        /// <summary>
        /// Returns whether the client can open or create a document at the given URI</summary>
        /// <param name="uri">Document URI</param>
        /// <returns>True iff the client can open or create a document at the given URI</returns>
        public bool CanOpen(Uri uri)
        {
            return DocumentClientInfo.IsCompatibleUri(uri);
        }

        /// <summary>
        /// Opens or creates a document at the given URI</summary>
        /// <param name="uri">Document URI</param>
        /// <returns>Document, or null if the document couldn't be opened or created</returns>
        public IDocument Open(Uri uri)
        {
            DomNode node = null;
            string filePath = uri.LocalPath;
            string fileName = Path.GetFileName(filePath);

            if (File.Exists(filePath))
            {
                // read existing document using standard XML reader
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    DomXmlReader reader = new DomXmlReader(m_schemaLoader);
                    node = reader.Read(stream, uri);
                }
            }
            else
            {
                // create new document by creating a Dom node of the root type defined by the schema
                node = new DomNode(Schema.storyType.Type, Schema.storyRootElement);
            }

            StoryDocument document = null;
            if (node != null)
            {
                // Initialize Dom extensions now that the data is complete
                node.InitializeExtensions();

                // set document URI
                document = node.As<StoryDocument>();
                document.Init(uri);
                document.Read();

                // show the ListView control
                m_controlHostService.RegisterControl(document.Control, document.ControlInfo, this);
            }

            return document;
        }

        /// <summary>
        /// Makes the document visible to the user</summary>
        /// <param name="document">Document to show</param>
        public void Show(IDocument document)
        {
            StoryDocument doc = document.As<StoryDocument>();
            m_controlHostService.Show(doc.Control);
        }

        /// <summary>
        /// Saves the document at the given URI</summary>
        /// <param name="document">Document to save</param>
        /// <param name="uri">New document URI</param>
        public void Save(IDocument document, Uri uri)
        {
            string filePath = uri.LocalPath;
            FileMode fileMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.OpenOrCreate;
            using (FileStream stream = new FileStream(filePath, fileMode))
            {
                DomXmlWriter writer = new DomXmlWriter(m_schemaLoader.TypeCollection);
                StoryDocument storyDocument = (StoryDocument)document;
                if(storyDocument != null)
                {
                    storyDocument.Write();
                    writer.Write(storyDocument.DomNode, stream, uri);
                }
            }
        }

        /// <summary>
        /// Closes the document and removes any views of it from the UI</summary>
        /// <param name="document">Document to close</param>
        public void Close(IDocument document)
        {
            StoryDocument doc = document.As<StoryDocument>();
            m_controlHostService.UnregisterControl(doc.Control);
            
            // close the document
            m_documentRegistry.Remove(document);
        }

        #endregion
        

    }
}
