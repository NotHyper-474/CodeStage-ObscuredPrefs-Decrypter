using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Threading;
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
		private RegistryKey gameKey;

		private string strPart = "De";
		private string strPartOld = "En";

		private static readonly Icon encryptIcon;
		private static readonly Icon decryptIcon;

		private bool valueMode;

		private const string CUR_USER = @"HKEY_CURRENT_USER";

		public DecrypterForm()
		{
			InitializeComponent();
		}

		static DecrypterForm()
		{
			decryptIcon = Properties.Resources.decIcon;
			encryptIcon = Properties.Resources.encIcon;
		}

		public uint GetPrefsHash(string key)
		{
			uint hash = 5381;
			foreach (char c in key)
				hash = hash * 33 ^ c;
			return hash;
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			cryptoKeyBox.Text = Properties.Settings.Default.SavedKey;
			if (!string.IsNullOrEmpty(Properties.Settings.Default.RegPath))
				registryPathBox.Text = Properties.Settings.Default.RegPath;
			else Properties.Settings.Default.RegPath = registryPathBox.Text;
		}

		private void DecrypterForm_Enter(object sender, EventArgs e)
		{
			Opacity = 1.00;
		}

		private void DecrypterForm_Leave(object sender, EventArgs e)
		{
			Opacity = 0.60;
		}

		#region Thread-Safe Methods
		internal void ProgressBarSet(int value)
		{
			if (progressBar1.InvokeRequired)
				progressBar1.Invoke(new Action(() => progressBar1.Value = value));
			else progressBar1.Value = value;
		}

		internal void ProgressBarIncrement(int value)
		{
			if (progressBar1.InvokeRequired)
				progressBar1.Invoke(new Action(() => progressBar1.Increment(value)));
			else progressBar1.Increment(value);
		}

		internal void ProgressBarMaximumSet(int max)
		{
			if (progressBar1.InvokeRequired)
				progressBar1.Invoke(new Action(() => progressBar1.Maximum = max));
			else progressBar1.Maximum = max;
		}

		internal void ListViewAdd(ListView view, ListViewItem value)
		{
			if (view.InvokeRequired)
				view.Invoke(new Action(() => view.Items.Add(value)));
			else view.Items.Add(value);
		}

		internal ListViewItem ListViewItemGet(ListView view, int idx)
		{
			return (ListViewItem)view.Invoke(new Func<ListViewItem>(() => view.Items[idx]));
		}

		internal void ListViewItemTextSet(ListView view, int idx, string text)
		{
			if (view.InvokeRequired)
				view.Invoke(new Action(() => view.Items[idx].Text = text));
			else view.Items[idx].Text = text;
		}
		#endregion

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

#if ENCRYPTER_DECRYPTER
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
#endif
			Thread t = new Thread(() =>
			{
				for (int i = 0; i < keyViewList.Items.Count; i++)
				{
					string itemText = ListViewItemGet(keyViewList, i).Text;
					if (itemText.ToLower().Contains("unity") || itemText.ToLower().Contains("screenmanager")
					|| itemText.ToLower().Contains("resolutiondialog"))
						continue;
					string b64 = itemText[..itemText.IndexOf("_h")];
					if (!Base64Utils.IsBase64(b64) || b64 == "9978e9f39c218d674463dab9dc728bd6")
						continue;
					string decName = EncrypterDecrypter.Decrypt(b64, key) + "_h";
					ListViewItemTextSet(keyViewList, i, decName + GetPrefsHash(decName));
				}
				Invoke(new Action(() => decencButton.Enabled = false));
			});
			t.Start();
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

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.RegPath = registryPathBox.Text;
			Properties.Settings.Default.Save();
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			loadButton.Enabled = false;
			Thread.Sleep(50);
			Thread t = new Thread(delegate ()
			{
				registryPathBox.Text = registryPathBox.Text.Replace(CUR_USER, string.Empty);
				gameKey = Registry.CurrentUser.OpenSubKey(registryPathBox.Text);

				if (gameKey == null)
				{
					MessageBox.Show("Could not open registry key", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				ArrayList values = new ArrayList(gameKey.ValueCount);
				keyViewList.Invoke(new Action(() => keyViewList.Items.Clear()));
				ProgressBarSet(0);
				ProgressBarMaximumSet(gameKey.ValueCount);
				foreach (var name in gameKey.GetValueNames())
				{
					RegistryValueKind kind = gameKey.GetValueKind(name);
					object value = gameKey.GetValue(name);
					switch (kind)
					{
						case RegistryValueKind.None:
							values.Add(null);
							break;
						case RegistryValueKind.Unknown:
							values.Add("[UNSUPPORTED TYPE]");
							break;
						case RegistryValueKind.String:
						case RegistryValueKind.MultiString:
						case RegistryValueKind.ExpandString:
						case RegistryValueKind.DWord:
						case RegistryValueKind.QWord:
							values.Add(value);
							break;
						case RegistryValueKind.Binary:
							values.Add(Encoding.ASCII.GetString((byte[])value));
							break;
						default:
							break;
					}

					ListViewItem item = new ListViewItem(name);
					item.SubItems.Add(values[progressBar1.Value].ToString());
					ListViewAdd(keyViewList, item);
					ProgressBarIncrement(1);
				}
				Invoke(new Action(() => loadButton.Enabled = true));
				Invoke(new Action(() => decencButton.Enabled = true));
				Invoke(new Action(() => getKeyButton.Enabled = true));
			});
			t.Start();
		}

		private void getKeyButton_Click(object sender, EventArgs e)
		{
			try
			{
				var b64 = Encoding.ASCII.GetString((byte[])gameKey.GetValue("9978e9f39c218d674463dab9dc728bd6_h2592231670"));
				cryptoKeyBox.Text = Base64Utils.FromBase64ToString(b64.Replace("\0", ""));
				MessageBox.Show("Key successfully found.", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			catch (Exception)
			{
				MessageBox.Show("Key could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
