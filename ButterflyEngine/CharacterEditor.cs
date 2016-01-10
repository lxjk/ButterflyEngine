
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Applications;
using Sce.Atf.Dom;
using Sce.Atf.Controls.PropertyEditing;

namespace ButterflyEngine
{
    [Export(typeof(IInitializable))]
    [Export(typeof(Editor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CharacterEditor : IInitializable, IControlHostClient
    {

        private IControlHostService m_controlHostService;
        private ICommandService m_commandService;
        private IContextRegistry m_contextRegistry;

        [Import(AllowDefault = true)]
        private ISettingsService m_settingsService = null;

        [Import(AllowDefault = true)]
        private IStatusService m_statusService = null;

        [ImportMany]
        private IEnumerable<Lazy<IContextMenuCommandProvider>> m_contextMenuCommandProviders = null;
                
        private StoryContext m_storyContext;
        private CharacterSettingsContext m_CharacterSettingsContext;
        private ListView m_characterListView;
        private ListViewAdapter m_characteListViewAdapter;
        private ControlInfo m_characteControlInfo;

        /// <summary>
        /// Constructor</summary>
        /// <param name="controlHostService">Control host service for registering the ListView</param>
        /// <param name="commandService">Optional command service for running the context menu</param>
        /// <param name="contextRegistry">Context registry</param>
        [ImportingConstructor]
        public CharacterEditor(
            IControlHostService controlHostService,
            ICommandService commandService,
            IContextRegistry contextRegistry)
        {
            m_controlHostService = controlHostService;
            m_commandService = commandService;
            m_contextRegistry = contextRegistry;

            m_contextRegistry.ActiveContextChanged += contextRegistry_ActiveContextChanged;
        }

        /// <summary>
        /// Finishes initializing component by creating ListView and initializing it, subscribing to drag events,
        /// and registering the control</summary>
        void IInitializable.Initialize()
        {
            m_characterListView = new ListView();
            //m_characterListView.SmallImageList = ResourceUtil.GetImageList16();
            m_characterListView.AllowDrop = false;
            m_characterListView.MultiSelect = true;
            m_characterListView.AllowColumnReorder = true;
            m_characterListView.LabelEdit = true;
            m_characterListView.Dock = DockStyle.Top;
            
            m_characterListView.MouseUp += characterListView_MouseUp;
            m_characteListViewAdapter = new ListViewAdapter(m_characterListView);
            m_characteListViewAdapter.LabelEdited +=
                resourcesListViewAdapter_LabelEdited;

            m_characteControlInfo = new ControlInfo(
                "Characters".Localize(),
                "Characters".Localize(),
                StandardControlGroup.Right);

            m_controlHostService.RegisterControl(m_characterListView, m_characteControlInfo, this);

            if (m_settingsService != null)
            {
                m_settingsService.RegisterSettings(this, new BoundPropertyDescriptor(this, () => ListViewSettings, "ListViewSettings", "", ""));
            }
        }

        /// <summary>
        /// Gets or sets the resource ListView state; used to persist that state
        /// with the settings service</summary>
        public string ListViewSettings
        {
            get { return m_characteListViewAdapter.Settings; }
            set { m_characteListViewAdapter.Settings = value; }
        }

        #region IControlHostClient Members

        /// <summary>
        /// Notifies the client that its Control has been activated. Activation occurs when
        /// the Control gets focus, or a parent "host" Control gets focus.</summary>
        /// <param name="control">Client Control that was activated</param>
        /// <remarks>This method is only called by IControlHostService if the Control was previously
        /// registered for this IControlHostClient.</remarks>
        void IControlHostClient.Activate(Control control)
        {
            if(m_CharacterSettingsContext != null)
            {
                m_contextRegistry.ActiveContext = m_CharacterSettingsContext;
            }
        }

        /// <summary>
        /// Notifies the client that its Control has been deactivated. Deactivation occurs when
        /// another Control or "host" Control gets focus.</summary>
        /// <param name="control">Client Control that was deactivated</param>
        /// <remarks>This method is only called by IControlHostService if the Control was previously
        /// registered for this IControlHostClient.</remarks>
        void IControlHostClient.Deactivate(Control control)
        {
            // nothing to do if our control is deactivated
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
        bool IControlHostClient.Close(Control control)
        {
            // always allow control to close
            return true;
        }

        #endregion

        private void contextRegistry_ActiveContextChanged(object sender, EventArgs e)
        {
            // make sure we're always tracking the most recently active EventSequenceContext
            StoryContext context = m_contextRegistry.GetMostRecentContext<StoryContext>();
            if (m_storyContext != context)
            {
                m_storyContext = context;
                Story story = m_storyContext.Cast<Story>();
                if(story.Settings == null)
                {
                    //DomNode node = new DomNode(Schema.settingsType.Type, Schema.storyType.settingsChild);
                    //story.Cast<DomNode>().SetChild(Schema.storyType.settingsChild, node);
                    story.Settings = Settings.New(Schema.storyType.settingsChild);
                }
                m_CharacterSettingsContext = story.Settings.Cast<CharacterSettingsContext>();
                m_characteListViewAdapter.ListView = m_CharacterSettingsContext;
            }
        }

        private void characterListView_MouseUp(object sender, MouseEventArgs e)
        {
            // in case of right click 
            if (e.Button == MouseButtons.Right)
            {
                Control control = sender as Control;
                object target = null;

                IEnumerable<object> commands =
                    m_contextMenuCommandProviders.GetValues().GetCommands(m_CharacterSettingsContext, target);

                Point screenPoint = control.PointToScreen(new Point(e.X, e.Y));
                m_commandService.RunContextMenu(commands, screenPoint);
            }
        }

        private void resourcesListViewAdapter_LabelEdited(object sender, LabelEditedEventArgs<object> e)
        {
            Character character = e.Item.As<Character>();
            m_storyContext.DoTransaction(delegate
            {
                character.Name = e.Label;
            }, "Rename Character".Localize());

            if (m_statusService != null)
                m_statusService.ShowStatus("Rename Character".Localize());
        }
    }
}
