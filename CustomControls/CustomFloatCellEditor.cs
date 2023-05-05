using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStage_Decrypter
{
	public class CustomFloatCellEditor : FloatCellEditor
	{
		public CustomFloatCellEditor() : base()
		{
			base.DecimalPlaces = 10;
			base.Minimum = -9999999m;
			base.Maximum = 9999999m;
		}
	}
}
