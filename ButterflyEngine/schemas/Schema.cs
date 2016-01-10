// -------------------------------------------------------------------------------------------------------------------
// Generated code, do not edit
// Command Line:  DomGen "story.xsd" "Schema.cs" "ButterflyEngine_Story_1_0" "ButterflyEngine"
// -------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Sce.Atf.Dom;

namespace ButterflyEngine
{
    public static class Schema
    {
        public const string NS = "ButterflyEngine_Story_1_0";

        public static void Initialize(XmlSchemaTypeCollection typeCollection)
        {
            Initialize((ns,name)=>typeCollection.GetNodeType(ns,name),
                (ns,name)=>typeCollection.GetRootElement(ns,name));
        }

        public static void Initialize(IDictionary<string, XmlSchemaTypeCollection> typeCollections)
        {
            Initialize((ns,name)=>typeCollections[ns].GetNodeType(name),
                (ns,name)=>typeCollections[ns].GetRootElement(name));
        }

        private static void Initialize(Func<string, string, DomNodeType> getNodeType, Func<string, string, ChildInfo> getRootElement)
        {
            storyType.Type = getNodeType("ButterflyEngine_Story_1_0", "storyType");
            storyType.storyTextAttribute = storyType.Type.GetAttributeInfo("storyText");
            storyType.settingsChild = storyType.Type.GetChildInfo("settings");

            settingsType.Type = getNodeType("ButterflyEngine_Story_1_0", "settingsType");
            settingsType.charactersChild = settingsType.Type.GetChildInfo("characters");

            characterType.Type = getNodeType("ButterflyEngine_Story_1_0", "characterType");
            characterType.nameAttribute = characterType.Type.GetAttributeInfo("name");
            characterType.altNamesAttribute = characterType.Type.GetAttributeInfo("altNames");
            characterType.ageAttribute = characterType.Type.GetAttributeInfo("age");

            storyRootElement = getRootElement(NS, "story");
        }

        public static class storyType
        {
            public static DomNodeType Type;
            public static AttributeInfo storyTextAttribute;
            public static ChildInfo settingsChild;
        }

        public static class settingsType
        {
            public static DomNodeType Type;
            public static ChildInfo charactersChild;
        }

        public static class characterType
        {
            public static DomNodeType Type;
            public static AttributeInfo nameAttribute;
            public static AttributeInfo altNamesAttribute;
            public static AttributeInfo ageAttribute;
        }

        public static ChildInfo storyRootElement;
    }
}
