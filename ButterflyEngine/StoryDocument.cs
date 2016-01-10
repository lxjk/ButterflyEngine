using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Applications;
using Sce.Atf.Dom;
using Sce.Atf.Controls.SyntaxEditorControl;

namespace ButterflyEngine
{
    public class StoryDocument : DomDocument
    {

        /// <summary>
        /// Gets main editor interface</summary>
        public ISyntaxEditorControl Editor
        {
            get { return m_editor; }
        }

        /// <summary>
        /// Gets editor Control</summary>
        public Control Control
        {
            get { return (Control)m_editor; }
        }

        private readonly ISyntaxEditorControl m_editor;


        /// <summary>
        /// Gets ControlInfo</summary>
        public ControlInfo ControlInfo
        {
            get { return m_controlInfo; }
            set { m_controlInfo = value; }
        }

        private ControlInfo m_controlInfo;


        // <summary>
        /// Constructor</summary>
        public StoryDocument()
        {
            m_editor = TextEditorFactory.CreateSyntaxHighlightingEditor();
        }

        public void Init(Uri uri)
        {         
            if (uri == null)
                throw new ArgumentNullException("uri");

            string filePath = uri.LocalPath;
            string fileName = Path.GetFileName(filePath);

            Control ctrl = (Control)m_editor;
            ctrl.Tag = this;

            m_editor.EditorTextChanged += editor_EditorTextChanged;

            Story story = this.Cast<Story>();
            if (story.Settings == null)
            {
                story.Settings = Settings.New(Schema.storyType.settingsChild);
            }

            story.Settings.DomNode.AttributeChanged += DomNode_AttributeChanged;
            story.Settings.DomNode.ChildInserted += DomNode_ChildInserted;
            story.Settings.DomNode.ChildRemoved += DomNode_ChildRemoved;

            MakeSyntaxXml();

            m_controlInfo = new ControlInfo(fileName, filePath, StandardControlGroup.Center);
            // tell ControlHostService this control should be considered a document in the menu, 
            // and using the full path of the document for menu text to avoid adding a number in the end 
            // in control header,  which is not desirable for documents that have the same name 
            // but located at different directories.
            m_controlInfo.IsDocument = true;

            Uri = uri;

        }

        public void Read()
        {
            Story story = this.As<Story>();
            if(story != null)
            {
                m_editor.Text = story.StoryText;
                m_editor.Dirty = false;
            }
        }

        public void Write()
        {
            Story story = this.As<Story>();
            if (story != null)
            {
                story.StoryText = m_editor.Text;
                m_editor.Dirty = false;
            }
        }

        private void editor_EditorTextChanged(object sender, EditorTextChangedEventArgs e)
        {
            bool dirty = m_editor.Dirty;
            if (dirty != Dirty)
            {
                Dirty = dirty;
            }
        }


        /// <summary>
        /// Gets whether the document is read-only</summary>
        public override bool IsReadOnly 
        {
            get { return m_editor.ReadOnly; }
        }


        /// <summary>
        /// Gets a string identifying the type of the resource to the end-user</summary>
        public override string Type
        {
            get { return ButterflyEngine.Editor.DocumentClientInfo.FileType; }
        }

        /// <summary>
        /// Raises the UriChanged event and performs custom processing</summary>
        /// <param name="e">Event args</param>
        protected override void OnUriChanged(UriChangedEventArgs e)
        {
            UpdateControlInfo();

            base.OnUriChanged(e);
        }

        /// <summary>
        /// Raises the DirtyChanged event and performs custom processing</summary>
        /// <param name="e">Event args</param>
        protected override void OnDirtyChanged(EventArgs e)
        {
            UpdateControlInfo();

            base.OnDirtyChanged(e);
        }

        private void UpdateControlInfo()
        {
            string filePath = Uri.LocalPath;
            string fileName = Path.GetFileName(filePath);
            if (Dirty)
                fileName += "*";
            
            m_controlInfo.Name = fileName;
            m_controlInfo.Description = filePath;
        }


        private void DomNode_AttributeChanged(object sender, AttributeEventArgs e)
        {
            MakeSyntaxXml();
        }

        private void DomNode_ChildInserted(object sender, ChildEventArgs e)
        {
            MakeSyntaxXml();
        }

        private void DomNode_ChildRemoved(object sender, ChildEventArgs e)
        {
            MakeSyntaxXml();
        }

        private void MakeSyntaxXml()
        {

            MemoryStream stream = new MemoryStream();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(@"<SyntaxLanguage Key=""Cg"" LanguageDefinitionVersion=""4.0"" Secure=""True"" xmlns = ""http://ActiproSoftware/SyntaxEditor/4.0/LanguageDefinition"" > </SyntaxLanguage>");
            {
                XmlNode styleListNode = xml.CreateNode(XmlNodeType.Element, "Styles", null);
                {
                    XmlNode styleNode = xml.CreateNode(XmlNodeType.Element, "Style", null);
                    styleNode.AppendAttribute(xml, "Key", "CharacterNameStyle");
                    styleNode.AppendAttribute(xml, "ForeColor", "Blue");
                    styleListNode.AppendChild(styleNode);
                }
                xml.DocumentElement.AppendChild(styleListNode);

                XmlNode stateListNode = xml.CreateNode(XmlNodeType.Element, "States", null);
                {
                    XmlNode stateNode = xml.CreateNode(XmlNodeType.Element, "State", null);
                    stateNode.AppendAttribute(xml, "Key", "DefaultState");
                    {
                        XmlNode patternGroupListNode = xml.CreateNode(XmlNodeType.Element, "PatternGroups", null);
                        {
                            XmlNode patternNode = xml.CreateNode(XmlNodeType.Element, "ExplicitPatternGroup", null);
                            patternNode.AppendAttribute(xml, "TokenKey", "CharacterNameToken");
                            patternNode.AppendAttribute(xml, "Style", "CharacterNameStyle");
                            //patternNode.AppendAttribute(xml, "PatternValue", "Test");
                            //patternNode.AppendAttribute(xml, "LookAhead", @"{NonWordMacro}|\z");
                            patternNode.AppendAttribute(xml, "CaseSensitivity", "Sensitive");
                            {
                                XmlNode explicitePatternNode = xml.CreateNode(XmlNodeType.Element, "ExplicitPatterns", null);
                                StringBuilder sb = new StringBuilder();
                                Story story = this.Cast<Story>();
                                if (story.Settings != null)
                                {
                                    foreach(Character character in story.Settings.Characters)
                                    {
                                        sb.Append(character.Name);
                                        sb.Append(" ");
                                        foreach (string altName in character.AltNames)
                                        {
                                            sb.Append(altName);
                                            sb.Append(" ");
                                        }
                                    }
                                }
                                explicitePatternNode.InnerText = sb.ToString();
                                patternNode.AppendChild(explicitePatternNode);
                            }
                            patternGroupListNode.AppendChild(patternNode);
                        }
                        stateNode.AppendChild(patternGroupListNode);
                    }
                    stateListNode.AppendChild(stateNode);
                }
                xml.DocumentElement.AppendChild(stateListNode);
            }
            System.Console.WriteLine(xml.InnerXml);
            xml.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            m_editor.SetLanguage(stream);

            Assembly thisAssem = Assembly.GetExecutingAssembly();
            Stream strm = null;
            const string langPath = "ButterflyEngine.schemas.";
            strm = thisAssem.GetManifestResourceStream(langPath + "CgDefinition.xml");
            //m_editor.SetLanguage(strm);
        }


    }
}
