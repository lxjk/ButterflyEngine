using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sce.Atf.Direct2D;
using Sce.Atf.VectorMath;

namespace ButterflyEngine
{
    public partial class FormTest : Form
    {

        private Canvas2d m_canvas;

        public FormTest()
        {

            D2dFactory.EnableResourceSharing(this.Handle);
            m_canvas = new Canvas2d();
            m_canvas.Dock = DockStyle.Fill;
            Controls.Add(m_canvas);

            this.ClientSize = new Size(1280, 900);
        }
    }


    /// <summary>
    /// Canvas for displaying graphics</summary>
    public class Canvas2d : Control
    {
        
        private D2dHwndGraphics m_d2dGraphics;

        private D2dTextFormat m_generalTextFormat;

        private string testString = "yooo";

        public Canvas2d()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            this.ResizeRedraw = true;

            m_d2dGraphics = D2dFactory.CreateD2dHwndGraphics(this.Handle);

            CreateSharedResources();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Render();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            m_d2dGraphics.Resize(Size);

            base.OnSizeChanged(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!e.Handled && e.KeyChar != (char)127)
            {
                if (((int)e.KeyChar) < 32)
                    return;

                testString += e.KeyChar.ToString();
                Invalidate();
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
        }


        private void Render()
        {
            m_d2dGraphics.BeginDraw();
            m_d2dGraphics.Clear(Color.White);

            m_d2dGraphics.AntialiasMode = D2dAntialiasMode.PerPrimitive;
            

            m_d2dGraphics.DrawText
                (testString, m_generalTextFormat,
                new PointF(1, 1), Color.Blue);


            m_d2dGraphics.EndDraw();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_d2dGraphics.Dispose();
                ReleaseSharedResources();
            }
            base.Dispose(disposing);
        }
    }
}
