using System.Collections.Generic;

using Sce.Atf.Dom;

namespace ButterflyEngine
{
    /// <summary>
    /// DomNode adapter for event sequence data</summary>
    public class Story : DomNodeAdapter
    {
        public string StoryText
        {
            get { return GetAttribute<string>(Schema.storyType.storyTextAttribute); }
            set { SetAttribute(Schema.storyType.storyTextAttribute, value); }
        }

        public Settings Settings
        {
            get { return GetChild<Settings>(Schema.storyType.settingsChild); }
            set { SetChild(Schema.storyType.settingsChild, value); }
        }
    }
}