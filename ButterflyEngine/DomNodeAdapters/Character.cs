using System.Collections.Generic;

using Sce.Atf.Adaptation;
using Sce.Atf.Dom;

namespace ButterflyEngine
{
    /// <summary>
    /// DomNode adapter for event sequence data</summary>
    public class Character : DomNodeAdapter
    {
        public static Character New(ChildInfo childInfo)
        {
            DomNode node = new DomNode(Schema.characterType.Type, childInfo);
            return node.Cast<Character>();
        }

        public string Name
        {
            get { return GetAttribute<string>(Schema.characterType.nameAttribute); }
            set { SetAttribute(Schema.characterType.nameAttribute, value); }
        }

        public IList<string> AltNames
        {
            get { return GetAttribute<IList<string>>(Schema.characterType.altNamesAttribute); }
        }

        public int Age
        {
            get { return GetAttribute<int>(Schema.characterType.ageAttribute); }
            set { SetAttribute(Schema.characterType.ageAttribute, value); }
        }
    }
}