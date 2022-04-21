using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using static CodeStage_Decrypter.EncrypterDecrypter;

namespace CodeStage_Decrypter
{
	public class Color32Renderer : BaseRenderer
	{
		public override bool OptionalRender(Graphics g, Rectangle r)
		{
			if (RowObject.GetType() != typeof(Color32))
				return false;
			Color32 c = (Color32)RowObject;
			g.DrawRectangle(new Pen(System.Drawing.Color.FromArgb(c.a, c.r, c.g, c.b)), r);
			return false;
		}
	}
}
