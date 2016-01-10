
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Applications;
using Sce.Atf.Dom;

namespace ButterflyEngine
{
    class CharacterSettingsContext : EditingContext,
        IListView,
        IItemView,
        IObservableContext,
        IInstancingContext,
        IEnumerableContext
    {
        /// <summary>
        /// Constructor</summary>
        public CharacterSettingsContext()
        {
            if (Reloaded == null) return; // suppress compiler warning
        }

        /// <summary>
        /// Performs initialization when the adapter is connected to the event context's DomNode.
        /// Raises the HistoryContext NodeSet event and performs custom processing.</summary>
        protected override void OnNodeSet()
        {
            DomNode.AttributeChanged += DomNode_AttributeChanged;
            DomNode.ChildInserted += DomNode_ChildInserted;
            DomNode.ChildRemoved += DomNode_ChildRemoved;

            base.OnNodeSet();
        }


        #region IListView Members

        /// <summary>
        /// Gets the items in the list</summary>
        public IEnumerable<object> Items
        {
            get { return GetChildList<object>(Schema.settingsType.charactersChild); }
        }

        /// <summary>
        /// Gets names for table columns</summary>
        public string[] ColumnNames
        {
            get { return new string[] { "Name".Localize(), "Age".Localize() }; }
        }

        #endregion
        
        #region IItemView Members

        /// <summary>
        /// Fills in or modifies the given display info for the item</summary>
        /// <param name="item">Item</param>
        /// <param name="info">Display info to update</param>
        public void GetInfo(object item, ItemInfo info)
        {
            Character character = item.As<Character>();
            info.Label = character.Name;

            info.Properties = new object[] { character.Age };
        }

        #endregion

        #region IObservableContext Members

        /// <summary>
        /// Event that is raised when an item is inserted</summary>
        public event EventHandler<ItemInsertedEventArgs<object>> ItemInserted;

        /// <summary>
        /// Event that is raised when an item is removed</summary>
        public event EventHandler<ItemRemovedEventArgs<object>> ItemRemoved;

        /// <summary>
        /// Event that is raised when an item is changed</summary>
        public event EventHandler<ItemChangedEventArgs<object>> ItemChanged;

        /// <summary>
        /// Event that is raised when the collection has been reloaded</summary>
        public event EventHandler Reloaded;

        #endregion


        #region IEnumerableContext Members

        /// <summary>
        /// Gets an enumeration of all of the items of this context</summary>
        IEnumerable<object> IEnumerableContext.Items
        {
            get { return Items; }
        }

        #endregion


        #region IInstancingContext Members

        /// <summary>
        /// Returns whether the context can copy the selection</summary>
        /// <returns>True iff the context can copy</returns>
        public bool CanCopy()
        {
            return Selection.Count > 0;
        }

        /// <summary>
        /// Copies the selection. Returns a data object representing the copied items.</summary>
        /// <returns>Data object representing the copied items; e.g., a
        /// System.Windows.Forms.IDataObject object</returns>
        public object Copy()
        {
            IEnumerable<DomNode> characters = Selection.AsIEnumerable<DomNode>();
            List<object> copies = new List<object>(DomNode.Copy(characters));
            return new DataObject(copies.ToArray());
        }

        /// <summary>
        /// Returns whether the context can insert the data object</summary>
        /// <param name="insertingObject">Data to insert; e.g., System.Windows.Forms.IDataObject</param>
        /// <returns>True iff the context can insert the data object</returns>
        public bool CanInsert(object insertingObject)
        {
            IDataObject dataObject = (IDataObject)insertingObject;
            object[] items = dataObject.GetData(typeof(object[])) as object[];
            if (items == null)
                return false;

            foreach (object item in items)
                if (!item.Is<Character>())
                    return false;

            return true;
        }

        /// <summary>
        /// Inserts the data object into the context</summary>
        /// <param name="insertingObject">Data to insert; e.g., System.Windows.Forms.IDataObject</param>
        public void Insert(object insertingObject)
        {
            IDataObject dataObject = (IDataObject)insertingObject;
            object[] items = dataObject.GetData(typeof(object[])) as object[];
            if (items == null)
                return;

            DomNode[] itemCopies = DomNode.Copy(items.AsIEnumerable<DomNode>());
            IList<Character> characters = this.Cast<Settings>().Characters;
            foreach (Character character in itemCopies.AsIEnumerable<Character>())
                characters.Add(character);

            Selection.SetRange(itemCopies);
        }

        /// <summary>
        /// Returns whether the context can delete the selection</summary>
        /// <returns>True iff the context can delete</returns>
        public bool CanDelete()
        {
            return Selection.Count > 0;
        }

        /// <summary>
        /// Deletes the selection</summary>
        public void Delete()
        {
            foreach (DomNode node in Selection.AsIEnumerable<DomNode>())
                node.RemoveFromParent();

            Selection.Clear();
        }

        #endregion

        public void Add(string name = "")
        {
            if(name == "")
            {
                name = "New Character".Localize();
            }
            IList<Character> characters = this.Cast<Settings>().Characters;
            
            Character character = Character.New(Schema.settingsType.charactersChild);
            character.Name = name;
            characters.Add(character);
        }


        private void DomNode_AttributeChanged(object sender, AttributeEventArgs e)
        {
            Character character = e.DomNode.As<Character>();
            if (character != null)
            {
                ItemChanged.Raise(this, new ItemChangedEventArgs<object>(character));
            }
        }

        private void DomNode_ChildInserted(object sender, ChildEventArgs e)
        {
            Character character = e.Child.As<Character>();
            if (character != null)
            {
                ItemInserted.Raise(this, new ItemInsertedEventArgs<object>(e.Index, character));
            }
        }

        private void DomNode_ChildRemoved(object sender, ChildEventArgs e)
        {
            Character character = e.Child.As<Character>();
            if (character != null)
            {
                ItemRemoved.Raise(this, new ItemRemovedEventArgs<object>(e.Index, character));
            }
        }
    }
}
