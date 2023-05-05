using System;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using UColor = CodeStage_Decrypter.EncrypterDecrypter.Color32;

namespace CodeStage_Decrypter
{
	public class ColorPicker : Button
	{
		public ColorPicker()
		{
			dialog = new ColorDialog();
			_colorImage = Properties.Resources.blank;
			Click += ColorPicker_OnClick;
			Validating += ColorPicker_Validating;
			
		}

		public override string Text { get => string.Empty; }
		new public Image Image
		{
			get => _colorImage;
		}


		public UColor Value { get; set; }
		private Color _formsColor;

		private readonly ColorDialog dialog;
		private readonly Image _colorImage;

		private void ColorPicker_OnClick(object sender, EventArgs e)
		{
			_formsColor = Color.FromArgb((int)(uint)(Value.a << 24) | (Value.r << 16) | (Value.g << 8) | (Value.b << 0));
			dialog.Color = _formsColor;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var color = dialog.Color;
				Value = new UColor(color.R, color.G, color.B, color.A);
			}
		}

		private void ColorPicker_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			uint color = (uint)_formsColor.ToArgb();
			UColor result;
			result.a = (byte)(color >> 24);
			result.r = (byte)(color >> 16);
			result.g = (byte)(color >> 8);
			result.b = (byte)(color >> 0);
			Value = result;
			BackColor = _formsColor;
		}
	}
}
