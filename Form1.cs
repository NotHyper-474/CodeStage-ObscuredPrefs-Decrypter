using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace CodeStage_Decrypter
{
	public partial class DecrypterForm : Form
	{
		public readonly HashSet<PrefsKey> ignoredKeys;

		private readonly Dictionary<string, Type> aliasToType = new Dictionary<string, Type>()
		{
			{ "Int", typeof(int) },
			{ "Float", typeof(float) },
			{ "String", typeof(string) },
		};

		private RegistryKey gameKey;

		private string strPart = "De";
		private string strPartOld = "En";

		private static readonly Icon encryptIcon;
		private static readonly Icon decryptIcon;

		private bool encryptMode = false;

		private const string CUR_USER = @"HKEY_CURRENT_USER\";

		public DecrypterForm()
		{
			ignoredKeys = new HashSet<PrefsKey>();
			InitializeComponent();
		}

		static DecrypterForm()
		{
			decryptIcon = Properties.Resources.decIcon;
			encryptIcon = Properties.Resources.encIcon;
		}

#if DEBUG
		[DllImport("kernel32", SetLastError = false, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeConsole();
#endif

		private void Form1_Load(object sender, EventArgs e)
		{
			cryptoKeyBox.Text = Properties.Settings.Default.SavedKey;
			if (!string.IsNullOrEmpty(Properties.Settings.Default.RegPath))
				registryPathBox.Text = Properties.Settings.Default.RegPath;
			else Properties.Settings.Default.RegPath = registryPathBox.Text;

			ObjectListView.EditorRegistry.Register(typeof(float), typeof(CustomFloatCellEditor));
			ObjectListView.EditorRegistry.Register(typeof(EncrypterDecrypter.Color32), typeof(ColorPicker));

			nameColumn.AspectName = "HashedName";
			valueColumn.Renderer = new Color32Renderer();
		}

		private void DecrypterForm_FormClosing(object sender, FormClosingEventArgs e)
		{
#if DEBUG
			FreeConsole();
#endif
		}

		private void ToggleMode()
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
			decencButton.Text = decencButton.Text.Replace(strPartOld, strPart);
			encryptMode = !encryptMode;
		}

		private void decencButton_Click(object sender, EventArgs e)
		{
			var key = cryptoKeyBox.Text;
			if (key.Length == 0)
			{
				MessageBox.Show("Key cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Thread t = new Thread(() =>
			{
				int i = 0;
				this.Invoke(() =>
				{
					keyViewList.BeginUpdate();
					progressBar1.Value = 0;
				});
				var objs = keyViewList.Objects;
				this.Invoke(() => decencButton.Enabled = false);
				foreach (var p in objs)
				{
					var item = (PrefsKey)p;
					string prefsKey = item.Name;
					if (ignoredKeys.Contains(item))
						goto INC;
					if (prefsKey == EncrypterDecrypter.PrefsKey)
					{
						ignoredKeys.Add(item);
						goto INC;
					}
					// Encryption/Decryption
					if (encryptMode)
					{
						item.Encrypt(key);
					}
					else
					{
						// Make sure that the keys that failed won't get encrypted afterwards
						if (!item.Decrypt(key))
							ignoredKeys.Add(item);
						if (!IsNotGibberish(item.Name))
							Console.WriteLine("Possible PrefsKey decrypted with incorrect key: " + item.Name);
					}
				INC:
					this.Invoke(() => progressBar1.Increment(1));
					i++;
				}
				this.Invoke(() =>
				{
					keyViewList.EndUpdate();
					keyViewList.SetObjects(objs, false);
					valueColumn.IsEditable = true;
					decencButton.Enabled = true;
					ToggleMode();
				});
			});
			t.Start();
		}

		private static bool IsErrorCode(object value)
		{
			if (value == null) return false;
			return value.ToString() == Encoding.UTF8.GetString(EncrypterDecrypter.savesTampered)
					|| value.ToString() == Encoding.UTF8.GetString(EncrypterDecrypter.olderVersion);
		}

		private static bool IsNotGibberish(string str)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(str, @"\b[\w']+\b");
		}

		private void SaveDump(string savePath, PrefsKey[] keys = null)
		{
			if (string.IsNullOrEmpty(savePath))
			{
				MessageBox.Show("No path specified.");
				return;
			}
			const string path = @"software\NH474DECRYPTER\TEMP";
			PrefsKey[] dump;
			RegistryKey editKey = Registry.CurrentUser.CreateSubKey(path);
			keyViewList.Objects ??= Array.Empty<PrefsKey>();
			if (keys != null && keys.Length > 0)
				dump = keys;
			else dump = keyViewList.Objects.Cast<PrefsKey>().ToArray();
			foreach (var item in dump)
			{
				PrefsKey save = item;
				object value = save.Value;
				value ??= (byte)0;
				if (value is bool bol)
					value = BitConverter.GetBytes(bol);
				if (value is string str)
					value = Encoding.ASCII.GetBytes(str);
				if (value is float f)
				{
					editKey.SetValue(save.HashedName, BitConverter.ToInt64(BitConverter.GetBytes((double)f)), RegistryValueKind.QWord);
					continue;
				}
				if (value is double d)
				{
					editKey.SetValue(save.HashedName, BitConverter.ToInt64(BitConverter.GetBytes(d)), RegistryValueKind.QWord);
				}
				editKey.SetValue(save.HashedName, value);
			}
			editKey.Flush();
			Process reg = new Process();
			reg.StartInfo.FileName = "cmd.exe";
			reg.StartInfo.Arguments = $@"/c reg export HKCU\{path} """ + savePath + "\" /y";
			reg.StartInfo.CreateNoWindow = false;
			reg.Start();
			reg.WaitForExit();
			var file = File.ReadAllText(savePath);
			// 9 is index after "SOFTWARE\"
			file = file.Replace(path[9..], registryPathBox.Text[9..]);
			// Hack for float values
			file = file.Replace("hex(b)", "hex(4)");
			File.WriteAllText(savePath, file);

			Registry.CurrentUser.DeleteSubKeyTree("software\\NH474DECRYPTER");
			Registry.CurrentUser.Close();
			MessageBox.Show("Dump saved. Notice that if you merge it you might overwrite the original registry data.");
		}

		private void cryptoKeyBox_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SavedKey = cryptoKeyBox.Text;
			Properties.Settings.Default.Save();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.RegPath = registryPathBox.Text;
			Properties.Settings.Default.Save();
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			Thread t = new Thread(() =>
				{
					this.Invoke(() =>
					{
						string path = registryPathBox.Text.Replace(CUR_USER, string.Empty);
						path = path.Replace('/', '\\');
						registryPathBox.Text = path;
					});
					gameKey = Registry.CurrentUser.OpenSubKey(registryPathBox.Text);

					if (gameKey == null)
					{
						MessageBox.Show("Could not open registry key", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					this.Invoke(() => loadButton.Enabled = false);
					if (encryptMode)
						this.Invoke(() => ToggleMode());

					ArrayList values = new ArrayList(gameKey.ValueCount);
					ignoredKeys.Clear();
					// I was too stupid to see I could just put everything inside one Invoke() call, smh
					this.Invoke(() =>
					{
						keyViewList.ClearObjects();
						valueColumn.IsEditable = false;
						keyViewList.CellRightClick -= keyViewList_CellRightClick;
						keyViewList.CellEditStarting -= keyViewList_EditStarting;
						keyViewList.CellEditFinishing -= keyViewList_EditFinished;
						progressBar1.Value = 0;
						progressBar1.Maximum = gameKey.ValueCount;
						keyViewList.BeginUpdate();
					});
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
							case RegistryValueKind.DWord:
								try
								{
									float f = (float)BitConverter.ToDouble(BitConverter.GetBytes((long)value));
									values.Add(f);
								}
								catch
								{
									values.Add(value);
								}
								break;
							case RegistryValueKind.String:
							case RegistryValueKind.MultiString:
							case RegistryValueKind.ExpandString:
							case RegistryValueKind.QWord:
								values.Add(value);
								break;
							case RegistryValueKind.Binary:
								values.Add(Encoding.ASCII.GetString((byte[])value));
								break;
							default:
								break;
						}

						int idx = progressBar1.Value;
						var cleanName = name.Remove(name.IndexOf("_h"));
						var newKey = new PrefsKey(cleanName, values[idx], out _);
						var lcName = cleanName.ToLower();
						if (lcName.Contains("unity") || lcName.Contains("screenmanager")
							|| lcName.Contains("resolutiondialog"))
							ignoredKeys.Add(newKey);
						this.Invoke(() =>
						{
							keyViewList.AddObject(newKey);
							progressBar1.Increment(1);
						});
					}
					this.Invoke(() =>
					{
						keyViewList.CellRightClick += keyViewList_CellRightClick;
						keyViewList.CellEditStarting += keyViewList_EditStarting;
						keyViewList.CellEditFinished += keyViewList_EditFinished;

						keyViewList.EndUpdate();
						loadButton.Enabled = true;
						decencButton.Enabled = true;
						getKeyButton.Enabled = true;
					});
				});
			t.Start();
		}

		private void keyViewList_FormatCell(object sender, FormatCellEventArgs e)
		{
			e.Item.UseItemStyleForSubItems = false;
			if (e.Column != valueColumn)
				return;
			if (IsErrorCode(e.CellValue))
				e.SubItem.ForeColor = Color.Red;
		}

		private void keyViewList_EditStarting(object sender, CellEditEventArgs args)
		{
			if (args.Column == valueColumn)
			{
				if (IsErrorCode(((PrefsKey)args.RowObject).Value))
				{
					MessageBox.Show("Cannot edit this value.\n\n" + ((PrefsKey)args.RowObject).Value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					args.Cancel = true;
				}
				return;
			}
			var key = args.RowObject as PrefsKey;
			key.isEditing = true;
			args.Control.Text = key.Name;
		}

		#region keyViewList Methods

		private void keyViewList_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Delete:
					if (keyViewList.SelectedObjects.Count > 0)
					{
						int newMax = progressBar1.Maximum - keyViewList.SelectedObjects.Count;
						newMax = Math.Clamp(newMax, 0, int.MaxValue);
						progressBar1.Maximum = newMax;
						progressBar1.Value = newMax;
						keyViewList.RemoveObjects(keyViewList.SelectedObjects);
					}
					break;
			}
		}

		private void keyViewList_EditFinished(object sender, CellEditEventArgs args)
		{
			if (args.Column == nameColumn)
			{
				(args.RowObject as PrefsKey).isEditing = false;
			}

			keyViewList.Update();
		}

		private void keyViewList_CellRightClick(object s, CellRightClickEventArgs e)
		{
			seeDecryptedNameToolStripMenuItem.Available = !encryptMode &&
				keyViewList.SelectedObject != null;
			dumpThisKeyToolStripMenuItem.Available = keyViewList.SelectedObjects.Count > 0;
			dumpThisKeyToolStripMenuItem.Name = keyViewList.SelectedObjects.Count == 1 ?
				"Dump Selected Key" : "Dump Selected Keys";
			cellContextMenu.Show(Cursor.Position);
		}
		#endregion

		#region KeyEdit Context Menu Methods
		private void createKeyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripItem c)
			{
				aliasToType.TryGetValue(c.Text, out Type type);
				object value;
				if (type.IsClass)
					value = Activator.CreateInstance(type, Array.Empty<char>());
				else
					value = Activator.CreateInstance(type);
				keyViewList.AddObject(new PrefsKey("New " + c.Text, value));
			}
			else throw new InvalidOperationException("Sender is not a " + nameof(ToolStripItem));
		}
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var obj = (IList)keyViewList.Objects;
			var key = keyViewList.SelectedObject;
			if (key != null)
			{
				obj.Remove(key);
			}
			keyViewList.Objects = obj;
		}


		private void seeDecryptedNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PrefsKey prefsKey = (PrefsKey)keyViewList.SelectedObject;
			if (!Base64Utils.IsBase64(prefsKey.Name))
			{
				MessageBox.Show("Key has invalid Base64 name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			MessageBox.Show(EncrypterDecrypter.Decrypt(prefsKey.Name, cryptoKeyBox.Text.ToArray()));
		}


		private void dumpThisKeyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (dumpSaveDialog.ShowDialog() == DialogResult.OK)
			{
				PrefsKey[] keys = new PrefsKey[keyViewList.SelectedObjects.Count];
				for (int i = 0; i < keyViewList.SelectedObjects.Count; i++)
					keys[i] = (PrefsKey)keyViewList.SelectedObjects[i];
				SaveDump(dumpSaveDialog.FileName, keys);
			}
		}
		#endregion

		private void getKeyButton_Click(object sender, EventArgs e)
		{
			try
			{
				PrefsKey key = new PrefsKey(
					EncrypterDecrypter.PrefsKey,
					null
					);
				key.Value = Encoding.ASCII.GetString((byte[])gameKey.GetValue(key.HashedName));
				key.Value = ((string)key.Value).TrimEnd(char.MinValue);
				cryptoKeyBox.Text = Base64Utils.FromBase64ToString((string)key.Value);
				MessageBox.Show("Key found successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			catch (Exception)
			{
				MessageBox.Show("Key could not be found. Game probably uses older key and version or it doesn't use ACTk at all.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void DecrypterForm_Activated(object sender, EventArgs e)
		{
			Opacity = 1.00;
		}

		private void DecrypterForm_Deactivate(object sender, EventArgs e)
		{
			//Opacity = 0.75;
		}

		private void openDialogButton1_Click(object sender, EventArgs e)
		{
			dumpSaveDialog.FileOk += (sender, args) => textBox1.Text = dumpSaveDialog.FileName;
			dumpSaveDialog.ShowDialog();
		}

		private void saveDumpButton_Click(object sender, EventArgs e)
		{
			SaveDump(textBox1.Text);
		}

		private void versionUpDown_ValueChanged(object sender, EventArgs e)
		{
			EncrypterDecrypter.Version = (byte)versionUpDown.Value;
		}
	}
}
