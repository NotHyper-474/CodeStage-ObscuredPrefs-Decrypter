using System.Windows.Forms;

namespace CodeStage_Decrypter
{
	public partial class DecrypterForm
	{
		/// <summary>
		/// <p>Automatically generated by Visual Studio</p>
		/// <p><b>DO NOT MODIFY</b></p>
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecrypterForm));
            this.cryptoKeyBox = new System.Windows.Forms.TextBox();
            this.decencButton = new System.Windows.Forms.Button();
            this.regGroupBox = new System.Windows.Forms.GroupBox();
            this.getKeyButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.registryPathBox = new System.Windows.Forms.TextBox();
            this.dumpGroupBox = new System.Windows.Forms.GroupBox();
            this.saveDumpButton = new System.Windows.Forms.Button();
            this.openDialogButton1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dumpSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.keyViewList = new BrightIdeasSoftware.ObjectListView();
            this.nameColumn = new BrightIdeasSoftware.OLVColumn();
            this.valueColumn = new BrightIdeasSoftware.OLVColumn();
            this.regGroupBox.SuspendLayout();
            this.dumpGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyViewList)).BeginInit();
            this.SuspendLayout();
            // 
            // cryptoKeyBox
            // 
            resources.ApplyResources(this.cryptoKeyBox, "cryptoKeyBox");
            this.cryptoKeyBox.Name = "cryptoKeyBox";
            this.cryptoKeyBox.TextChanged += new System.EventHandler(this.cryptoKeyBox_TextChanged);
            // 
            // decencButton
            // 
            resources.ApplyResources(this.decencButton, "decencButton");
            this.decencButton.Name = "decencButton";
            this.decencButton.UseVisualStyleBackColor = true;
            this.decencButton.Click += new System.EventHandler(this.decencButton_Click);
            // 
            // regGroupBox
            // 
            this.regGroupBox.Controls.Add(this.getKeyButton);
            this.regGroupBox.Controls.Add(this.loadButton);
            this.regGroupBox.Controls.Add(this.progressBar1);
            this.regGroupBox.Controls.Add(this.label2);
            this.regGroupBox.Controls.Add(this.label1);
            this.regGroupBox.Controls.Add(this.decencButton);
            this.regGroupBox.Controls.Add(this.registryPathBox);
            this.regGroupBox.Controls.Add(this.cryptoKeyBox);
            resources.ApplyResources(this.regGroupBox, "regGroupBox");
            this.regGroupBox.Name = "regGroupBox";
            this.regGroupBox.TabStop = false;
            // 
            // getKeyButton
            // 
            resources.ApplyResources(this.getKeyButton, "getKeyButton");
            this.getKeyButton.Name = "getKeyButton";
            this.getKeyButton.UseVisualStyleBackColor = true;
            this.getKeyButton.Click += new System.EventHandler(this.getKeyButton_Click);
            // 
            // loadButton
            // 
            resources.ApplyResources(this.loadButton, "loadButton");
            this.loadButton.Name = "loadButton";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // registryPathBox
            // 
            resources.ApplyResources(this.registryPathBox, "registryPathBox");
            this.registryPathBox.Name = "registryPathBox";
            this.registryPathBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dumpGroupBox
            // 
            this.dumpGroupBox.Controls.Add(this.saveDumpButton);
            this.dumpGroupBox.Controls.Add(this.openDialogButton1);
            this.dumpGroupBox.Controls.Add(this.textBox1);
            this.dumpGroupBox.Controls.Add(this.label3);
            resources.ApplyResources(this.dumpGroupBox, "dumpGroupBox");
            this.dumpGroupBox.Name = "dumpGroupBox";
            this.dumpGroupBox.TabStop = false;
            // 
            // saveDumpButton
            // 
            resources.ApplyResources(this.saveDumpButton, "saveDumpButton");
            this.saveDumpButton.Name = "saveDumpButton";
            this.saveDumpButton.UseVisualStyleBackColor = true;
            this.saveDumpButton.Click += new System.EventHandler(this.saveDumpButton_Click);
            // 
            // openDialogButton1
            // 
            resources.ApplyResources(this.openDialogButton1, "openDialogButton1");
            this.openDialogButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openDialogButton1.Name = "openDialogButton1";
            this.openDialogButton1.UseVisualStyleBackColor = true;
            this.openDialogButton1.Click += new System.EventHandler(this.openDialogButton1_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // dumpSaveDialog
            // 
            this.dumpSaveDialog.FileName = "dump";
            resources.ApplyResources(this.dumpSaveDialog, "dumpSaveDialog");
            // 
            // keyViewList
            // 
            this.keyViewList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.keyViewList.CellEditUseWholeCell = false;
            this.keyViewList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.valueColumn});
            this.keyViewList.Cursor = System.Windows.Forms.Cursors.Default;
            this.keyViewList.GridLines = true;
            this.keyViewList.HideSelection = false;
            resources.ApplyResources(this.keyViewList, "keyViewList");
            this.keyViewList.Name = "keyViewList";
            this.keyViewList.ShowGroups = false;
            this.keyViewList.UseCellFormatEvents = true;
            this.keyViewList.View = System.Windows.Forms.View.Details;
            this.keyViewList.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.keyViewList_FormatCell);
            // 
            // nameColumn
            // 
            this.nameColumn.AspectName = "Name";
            this.nameColumn.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            resources.ApplyResources(this.nameColumn, "nameColumn");
            // 
            // valueColumn
            // 
            this.valueColumn.AspectName = "Value";
            this.valueColumn.IsEditable = false;
            resources.ApplyResources(this.valueColumn, "valueColumn");
            // 
            // DecrypterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.keyViewList);
            this.Controls.Add(this.dumpGroupBox);
            this.Controls.Add(this.regGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DecrypterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Activated += new System.EventHandler(this.DecrypterForm_Activated);
            this.Deactivate += new System.EventHandler(this.DecrypterForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DecrypterForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.regGroupBox.ResumeLayout(false);
            this.regGroupBox.PerformLayout();
            this.dumpGroupBox.ResumeLayout(false);
            this.dumpGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyViewList)).EndInit();
            this.ResumeLayout(false);

		}

        private TextBox cryptoKeyBox;
        private Button decencButton;
        private GroupBox regGroupBox;
        private Label label1;
        private TextBox registryPathBox;
        private Label label2;
        private System.ComponentModel.IContainer components;
        private Button loadButton;
        private ProgressBar progressBar1;
        private Button getKeyButton;
        private GroupBox dumpGroupBox;
        private Label label3;
        private SaveFileDialog dumpSaveDialog;
        private TextBox textBox1;
        private Button openDialogButton1;
        private BrightIdeasSoftware.ObjectListView keyViewList;
        private BrightIdeasSoftware.OLVColumn nameColumn;
        private BrightIdeasSoftware.OLVColumn valueColumn;
        private Button saveDumpButton;
    }
}