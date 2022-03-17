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

        private RegistryKey gameKey;
        private RegistryKey editKey;

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

            //ObjectListView.EditorRegistry.Register(typeof(EncrypterDecrypter.Color32), typeof(ColorControl));
            nameColumn.AspectName = "HashedName";
        }

        private void DecrypterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if DEBUG
            FreeConsole();
#endif
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

        internal static void ListViewAdd(ObjectListView view, PrefsKey item)
        {
            if (view.InvokeRequired)
                view.Invoke(() => view.AddObject(item));
            else view.AddObject(item);
        }

        internal static ListViewItem ListViewItemGet(ListView view, int idx)
        {
            return (ListViewItem)view.Invoke(new Func<ListViewItem>(() => view.Items[idx]));
        }

        internal void ListViewSubItemTextSet(ListView view, int idx, int subIdx, string value)
        {
            Invoke(new Action(() => view.Items[idx].SubItems[subIdx].Text = value));
        }

        internal void ListViewItemTextSet(ListView view, int idx, string text)
        {
            if (view.InvokeRequired)
                view.Invoke(new Action(() => view.Items[idx].Text = text));
            else view.Items[idx].Text = text;
        }
        #endregion

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
            Thread t = new Thread(() =>
            {
                int i = 0;
                this.Invoke(() => keyViewList.BeginUpdate());
                ProgressBarSet(0);
                var objs = keyViewList.Objects;
                this.Invoke(() => decencButton.Enabled = false);
                foreach (var p in objs)
                {
                    var item = (PrefsKey)p;
                    string prefsKey = item.Name;
                    if (ignoredKeys.Contains(item))
                        goto INC;
                    if (prefsKey == "9978e9f39c218d674463dab9dc728bd6")
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
                    ProgressBarIncrement(1);
                    i++;
                }
                this.Invoke(() => keyViewList.EndUpdate());
                this.Invoke(() => keyViewList.SetObjects(objs, false));
                this.Invoke(() => valueColumn.IsEditable = true);
                this.Invoke(() => decencButton.Enabled = true);
                this.Invoke(() => ToggleMode());
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
                    this.Invoke(() => registryPathBox.Text = registryPathBox.Text.Replace(CUR_USER, string.Empty));
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
                    this.Invoke(() => keyViewList.ClearObjects());
                    this.Invoke(() => valueColumn.IsEditable = false);
                    this.Invoke(() => keyViewList.CellEditStarting -= keyViewList_EditStarting);
                    this.Invoke(() => keyViewList.CellEditFinishing -= keyViewList_EditFinished);
                    ProgressBarSet(0);
                    ProgressBarMaximumSet(gameKey.ValueCount);
                    this.Invoke(() => keyViewList.BeginUpdate());
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
                        int idx = progressBar1.Value;
                        var cleanName = name.Remove(name.IndexOf("_h"));
                        var newKey = new PrefsKey(cleanName, values[idx], out _);
                        if (cleanName.ToLower().Contains("unity") || cleanName.ToLower().Contains("screenmanager")
                            || cleanName.ToLower().Contains("resolutiondialog"))
                            ignoredKeys.Add(newKey);
                        ListViewAdd(keyViewList, newKey);
                        ProgressBarIncrement(1);
                    }
                    this.Invoke(() => keyViewList.CellEditStarting += keyViewList_EditStarting);
                    this.Invoke(() => keyViewList.CellEditFinished += keyViewList_EditFinished);

                    this.Invoke(() => keyViewList.EndUpdate());
                    this.Invoke(() => loadButton.Enabled = true);
                    this.Invoke(() => decencButton.Enabled = true);
                    this.Invoke(() => getKeyButton.Enabled = true);
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
            //nameColumn.AspectName = nameof(PrefsKey.Name);
            var key = args.RowObject as PrefsKey;
            key.isEditing = true;
            args.Control.Text = key.Name;
        }

        private void keyViewList_EditFinished(object sender, CellEditEventArgs args)
        {
            if (args.Column == nameColumn)
            {
                //nameColumn.AspectName = nameof(PrefsKey.HashedName);
                (args.RowObject as PrefsKey).isEditing = false;
            }

            keyViewList.Update();
        }

        private void getKeyButton_Click(object sender, EventArgs e)
        {
            try
            {
                var b64 = Encoding.ASCII.GetString((byte[])gameKey.GetValue("9978e9f39c218d674463dab9dc728bd6_h2592231670"));
                cryptoKeyBox.Text = Base64Utils.FromBase64ToString(b64.TrimEnd('\0'));
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
            if (string.IsNullOrEmpty(textBox1.Text))
                return;
            const string path = "Software\\NH474DECRYPTER\\TEMP";
            editKey = Registry.CurrentUser.CreateSubKey(path);
            foreach (var item in keyViewList.Objects)
            {
                PrefsKey save = (PrefsKey)item;
                object value = save.Value;
                if (value is string str)
                    value = Encoding.ASCII.GetBytes(str);
                editKey.SetValue(save.HashedName, value);
            }
            editKey.Flush();
            Registry.CurrentUser.Close();
            System.Diagnostics.Process.Start("reg", $"export /y HKCU\\{path} " + textBox1.Text);
            //Registry.CurrentUser.DeleteSubKeyTree("software\\NH474DECRYPTER");
        }
    }
}
