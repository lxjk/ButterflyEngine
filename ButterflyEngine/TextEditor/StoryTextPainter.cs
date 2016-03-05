
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Sce.Atf.Direct2D;
using Sce.Atf.Adaptation;

namespace ButterflyEngine
{
    public class StoryTextPainter : IDisposable
    {

        private StoryTextEditorControl m_control;
        private D2dHwndGraphics m_d2dGraphics;

        private D2dTextFormat m_generalTextFormat;


        public StoryTextPainter(StoryTextEditorControl control)
        {
            m_control = control;
            D2dFactory.EnableResourceSharing(m_control.Handle);
            m_d2dGraphics = D2dFactory.CreateD2dHwndGraphics(m_control.Handle);
            CreateSharedResources();
        }

        public void Dispose()
        {
            m_d2dGraphics.Dispose();
            ReleaseSharedResources();
        }

        private void CreateSharedResources()
        {
            // create text format object for various test modes
            m_generalTextFormat = D2dFactory.CreateTextFormat("Calibri"
                , D2dFontWeight.Bold, D2dFontStyle.Normal, D2dFactory.FontSizeToPixel(16));

            m_generalTextFormat.WordWrapping = D2dWordWrapping.Wrap;
        }

        private void ReleaseSharedResources()
        {
            m_generalTextFormat.Dispose();
        }

        public void Resize(Size size)
        {
            m_d2dGraphics.Resize(size);
        }


        public void RenderAll()
        {
            m_d2dGraphics.BeginDraw();
            m_d2dGraphics.Clear(Color.White);

            m_d2dGraphics.AntialiasMode = D2dAntialiasMode.PerPrimitive;

            Story story = m_control.Document.Cast<Story>();

            m_d2dGraphics.DrawText
                (story.StoryText, m_generalTextFormat,
                new PointF(1, 1), Color.Blue);


            m_d2dGraphics.EndDraw();
        }


    }
}
