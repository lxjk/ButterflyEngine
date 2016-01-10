
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Xml.Schema;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Controls.PropertyEditing;
using Sce.Atf.Dom;

using PropertyDescriptor = Sce.Atf.Dom.PropertyDescriptor;

namespace ButterflyEngine
{
    /// <summary>
    /// Loads the event schema, registers data extensions on the DOM types, and annotates
    /// the types with display information and PropertyDescriptors.</summary>
    [Export(typeof(SchemaLoader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SchemaLoader : XmlSchemaTypeLoader
    {
        /// <summary>
        /// Constructor</summary>
        public SchemaLoader()
        {
            // set resolver to locate embedded .xsd file
            SchemaResolver = new ResourceStreamResolver(Assembly.GetExecutingAssembly(), "ButterflyEngine/schemas");
            Load("story.xsd");
        }

        /// <summary>
        /// Gets the schema type collection</summary>
        public XmlSchemaTypeCollection TypeCollection
        {
            get { return m_typeCollection; }
        }
        private XmlSchemaTypeCollection m_typeCollection;

        protected override void OnSchemaSetLoaded(XmlSchemaSet schemaSet)
        {
            foreach (XmlSchemaTypeCollection typeCollection in GetTypeCollections())
            {
                m_typeCollection = typeCollection;
                Schema.Initialize(typeCollection);

                // register extensions
                Schema.storyType.Type.Define(new ExtensionInfo<Story>());
                Schema.storyType.Type.Define(new ExtensionInfo<StoryDocument>());
                Schema.storyType.Type.Define(new ExtensionInfo<StoryContext>());
                Schema.storyType.Type.Define(new ExtensionInfo<ReferenceValidator>());
                Schema.storyType.Type.Define(new ExtensionInfo<UniqueIdValidator>());
                Schema.storyType.Type.Define(new ExtensionInfo<DomNodeQueryable>());

                Schema.settingsType.Type.Define(new ExtensionInfo<Settings>());
                Schema.settingsType.Type.Define(new ExtensionInfo<CharacterSettingsContext>());

                Schema.characterType.Type.Define(new ExtensionInfo<Character>());
                
                // Enable metadata driven property editing for events and resources
                var creator = new AdapterCreator<CustomTypeDescriptorNodeAdapter>();
                Schema.characterType.Type.AddAdapterCreator(creator);

                
                Schema.characterType.Type.SetTag(
                    new PropertyDescriptorCollection(
                        new PropertyDescriptor[] {
                            new AttributePropertyDescriptor(
                                "Name".Localize(),
                                Schema.characterType.nameAttribute,
                                null,
                                "Name".Localize(),
                                false),
                           new AttributePropertyDescriptor(
                               "Alternative Names".Localize(),
                               Schema.characterType.altNamesAttribute,
                               null,
                               "List of Alternative Names".Localize(),
                               false,
                               new StringArrayEditor()),
                            new AttributePropertyDescriptor(
                                "Age".Localize(),
                                Schema.characterType.ageAttribute,
                                null,
                                "Age".Localize(),
                                false),
                    }));

            }
        }
    }
}
