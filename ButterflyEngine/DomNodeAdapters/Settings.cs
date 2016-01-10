using System.Collections.Generic;

using Sce.Atf.Adaptation;
using Sce.Atf.Dom;

namespace ButterflyEngine
{
    /// <summary>
    /// DomNode adapter for event sequence data</summary>
    public class Settings : DomNodeAdapter
    {
        public static Settings New(ChildInfo childInfo)
        {
            DomNode node = new DomNode(Schema.settingsType.Type, childInfo);
            return node.Cast<Settings>();
        }

        public IList<Character> Characters
        {
            get { return GetChildList<Character>(Schema.settingsType.charactersChild); }
        }
    }
}