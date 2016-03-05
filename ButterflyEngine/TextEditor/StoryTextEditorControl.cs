using Sce.Atf;
using Sce.Atf.Adaptation;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ButterflyEngine
{
    public class StoryTextEditorControl : ScrollableControl
    {
        public StoryTextEditor Editor
        {
            get { return m_editor; }
        }

        public StoryDocument Document
        {
            get { return m_editor.Document; }
        }

        private StoryTextPainter m_painter;
        private StoryTextEditor m_editor;

        private bool isKeyDownHandled;

        public StoryTextEditorControl(StoryTextEditor editor)
        {
            m_editor = editor;
            m_painter = new StoryTextPainter(this);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.UserPaint, true);            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(m_painter != null)
                {
                    m_painter.Dispose();
                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            m_painter.RenderAll();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            m_painter.Resize(Size);

            base.OnSizeChanged(e);
        }

        protected override bool IsInputKey(Keys key)
        {
            return true;		
        }

        protected override bool IsInputChar(char c)
        {
            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            isKeyDownHandled = e.Handled;


            if (e.Control || e.Alt)
                return;

            if (!Document.IsReadOnly)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        InsertEnter();
                        break;
                    case Keys.Back:
                        DeleteBackwards();
                        break;
                    case Keys.Delete:
                        Delete();
                        break;
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!e.Handled && !isKeyDownHandled && e.KeyChar != (char)127)
            {
                if (((int)e.KeyChar) < 32)
                    return;

                if (!Document.IsReadOnly)
                {
                    switch ((Keys)(int)e.KeyChar)
                    {
                        default:
                            InsertText(e.KeyChar.ToString());
                            break;

                    }
                }
            }
        }

        private void InsertEnter()
        {
            InsertText("\n");
            OnChange();
        }

        private void Delete()
        {
            Story story = Document.Cast<Story>();
            if (story.StoryText.Length <= 1)
            {
                story.StoryText = "";
            }
            else
            {
                story.StoryText = story.StoryText.Substring(0, story.StoryText.Length - 1);
            }
            OnChange();
        }

        private void DeleteBackwards()
        {
            Delete();
            OnChange();
        }

        private void InsertText(string text)
        {
            Story story = Document.Cast<Story>();
            story.StoryText += text;
            OnChange();
        }

        public void OnChange()
        {
            Redraw();
        }

        private void Redraw()
        {
            Invalidate();
        }

    }
}
