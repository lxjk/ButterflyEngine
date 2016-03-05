using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ButterflyEngine
{
    public class StoryTextEditor
    {

        public Control Control
        {
            get { return m_Control; }
        }

        public StoryDocument Document
        {
            get { return m_Document; }
        }

        private StoryDocument m_Document;
        private StoryTextEditorControl m_Control;

        public StoryTextEditor(StoryDocument document)
        {
            m_Document = document;
            m_Control = new StoryTextEditorControl(this);
        }
    }
}
