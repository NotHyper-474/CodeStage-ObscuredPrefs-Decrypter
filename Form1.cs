using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeStage_Decrypter
{
	public partial class DecrypterForm : Form
	{
		private string strPart = "De";
		private string strPartOld = "En";

		private static readonly Icon encryptIcon;
		private static readonly Icon decryptIcon;

		private bool valueMode;

		public DecrypterForm()
		{
			InitializeComponent();
		}

		static DecrypterForm()
		{
			decryptIcon = Properties.Resources.decIcon;
			encryptIcon = Properties.Resources.encIcon;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			cryptoKeyBox.Text = Properties.Settings.Default.SavedKey;
		}

		private void DecrypterForm_Enter(object sender, EventArgs e)
		{
			Opacity = 1.00;
		}

		private void DecrypterForm_Leave(object sender, EventArgs e)
		{
			Opacity = 0.60;
		}

		private void encryptModeCheck_CheckedChanged(object sender, EventArgs e)
		{
			strPartOld = strPart;
			if (strPart == "De")
			{
				strPart = "En";
				Icon = encryptIcon;
			}
			else
			{
				strPart = "De";
				Icon = decryptIcon;
			}

			textBox.PlaceholderText = textBox.PlaceholderText.Replace(strPartOld.ToLower(), strPart.ToLower());
			decencButton.Text = decencButton.Text.Replace(strPartOld, strPart);
		}

		private void decencButton_Click(object sender, EventArgs e)
		{
			string text = textBox.Text;
			char[] key = cryptoKeyBox.Text.ToCharArray();

			if (encryptModeCheck.Checked)
			{
				if (!valueMode)
					resultBox.Text = EncrypterDecrypter.Encrypt(text, key);
				else resultBox.Text = EncrypterDecrypter.EncryptValue(text, new string(key));
			}
			else
			{
				if (valueMode)
				{
					byte[] result = EncrypterDecrypter.DecryptData(new string(key), text);
					if ((result != null) && Encoding.UTF8.GetString(result) == "SAVES_TAMPERED")
						MessageBox.Show("Could not decrypt value. The value was probably tampered with.");
					var type = EncrypterDecrypter.GetRawDataType(text);
					//MessageBox.Show(Enum.GetName(typeof(EncrypterDecrypter.DataType), type));
					resultBox.Text = result.Length > 0 ? Encoding.UTF8.GetString(result) : string.Empty;
					return;
				}
				if (!Base64Utils.IsBase64(text))
				{
					MessageBox.Show("Invalid Base64 text. Decrypting without decoding.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					resultBox.Text = new string(EncrypterDecrypter.EncryptDecrypt(text.ToCharArray(), key));
					return;
				}
				resultBox.Text = EncrypterDecrypter.Decrypt(text, key);
			}
		}

		private void cryptoKeyBox_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SavedKey = cryptoKeyBox.Text;
			Properties.Settings.Default.Save();
		}

		private void valueModeCheck_CheckedChanged(object sender, EventArgs e)
		{
			valueMode = valueModeCheck.Checked;
		}
	}
}
