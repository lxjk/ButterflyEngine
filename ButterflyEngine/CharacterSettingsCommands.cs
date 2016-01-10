using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Applications;
using Sce.Atf.Input;


namespace ButterflyEngine
{
    [Export]
    [Export(typeof(IInitializable))]
    [Export(typeof(ICommandClient))]
    [Export(typeof(IContextMenuCommandProvider))]
    public class CharacterSettingsCommands :
        IInitializable,
        ICommandClient,
        IContextMenuCommandProvider
    {

        /// <summary>
        /// Commands enumeration</summary>
        public enum Commands
        {
            AddCharacter
        }

        public enum Groups
        {
            Character
        }
        
        private readonly ICommandService m_commandService;
        private readonly IContextRegistry m_contextRegistry;
        
        [ImportingConstructor]
        public CharacterSettingsCommands(ICommandService commandService, IContextRegistry contextRegistry)
        {
            m_commandService = commandService;
            m_contextRegistry = contextRegistry;
        }

        #region IInitializable Members

        void IInitializable.Initialize()
        {
            // Register edit menu commands
            m_commandService.RegisterCommand(
                Commands.AddCharacter,
                StandardMenu.Edit,
                Groups.Character,
                "Add Character".Localize(),
                "Add new character".Localize(),
                Keys.None,
                null,
                CommandVisibility.ApplicationMenu | CommandVisibility.ContextMenu | CommandVisibility.Menu,
                this);
            
        }

        #endregion


        #region ICommandClient Members

        /// <summary>
        /// Checks whether the client can do the command, if it handles it</summary>
        /// <param name="tag">Command to be done</param>
        /// <returns>True iff client can do the command</returns>
        public bool CanDoCommand(object tag)
        {
            if (tag is Commands)
            {
                CharacterSettingsContext context = m_contextRegistry.GetActiveContext<CharacterSettingsContext>();
                return context != null;
            }
            return false;
        }

        /// <summary>
        /// Does the command</summary>
        /// <param name="tag">Command to be done</param>
        public void DoCommand(object tag)
        {
            switch ((Commands)tag)
            {
                case Commands.AddCharacter:
                    CharacterSettingsContext context = m_contextRegistry.GetActiveContext<CharacterSettingsContext>();
                    if (context != null)
                    {
                        context.DoTransaction(delegate
                        {
                            context.Add();
                        }, "Add New Character".Localize());
                    }
                    break;
            }
        }

        /// <summary>
        /// Updates command state for given command</summary>
        /// <param name="commandTag">Command</param>
        /// <param name="commandState">Command info to update</param>
        public void UpdateCommand(object commandTag, Sce.Atf.Applications.CommandState commandState) { }

        #endregion

        #region IContextMenuCommandProvider Members

        /// <summary>
        /// Gets command tags for context menu (right click) commands</summary>
        /// <param name="context">Context containing target object</param>
        /// <param name="target">Right clicked object, or null if none</param>
        /// <returns>Command tags for context menu</returns>
        public IEnumerable<object> GetCommands(object context, object target)
        {
            if (context.Is<CharacterSettingsContext>())
            {
                return new object[]
                    {
                        Commands.AddCharacter,
                    };
            }
            return EmptyEnumerable<object>.Instance;
        }

        #endregion
    }
}
