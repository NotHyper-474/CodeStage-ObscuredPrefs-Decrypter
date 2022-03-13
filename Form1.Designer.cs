using System.Windows.Forms;

namespace CodeStage_Decrypter
{
	public partial class DecrypterForm
	{
		private Label label1;
		private TextBox cryptoKeyBox;
		private TextBox textBox;
		private CheckBox encryptModeCheck;
		private Button decencButton;
		private CheckBox valueModeCheck;
		private TextBox resultBox;

		/// <summary>
		/// <p>Automatically generated by Visual Studio</p>
		/// <p><b>DO NOT MODIFY</b></p>
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecrypterForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cryptoKeyBox = new System.Windows.Forms.TextBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.encryptModeCheck = new System.Windows.Forms.CheckBox();
            this.decencButton = new System.Windows.Forms.Button();
            this.valueModeCheck = new System.Windows.Forms.CheckBox();
            this.resultBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cryptoKeyBox
            // 
            resources.ApplyResources(this.cryptoKeyBox, "cryptoKeyBox");
            this.cryptoKeyBox.Name = "cryptoKeyBox";
            this.cryptoKeyBox.TextChanged += new System.EventHandler(this.cryptoKeyBox_TextChanged);
            // 
            // textBox
            // 
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.Name = "textBox";
            // 
            // encryptModeCheck
            // 
            resources.ApplyResources(this.encryptModeCheck, "encryptModeCheck");
            this.encryptModeCheck.Name = "encryptModeCheck";
            this.encryptModeCheck.UseVisualStyleBackColor = true;
            this.encryptModeCheck.CheckedChanged += new System.EventHandler(this.encryptModeCheck_CheckedChanged);
            // 
            // decencButton
            // 
            resources.ApplyResources(this.decencButton, "decencButton");
            this.decencButton.Name = "decencButton";
            this.decencButton.UseVisualStyleBackColor = true;
            this.decencButton.Click += new System.EventHandler(this.decencButton_Click);
            // 
            // valueModeCheck
            // 
            resources.ApplyResources(this.valueModeCheck, "valueModeCheck");
            this.valueModeCheck.Name = "valueModeCheck";
            this.valueModeCheck.UseVisualStyleBackColor = true;
            this.valueModeCheck.CheckedChanged += new System.EventHandler(this.valueModeCheck_CheckedChanged);
            // 
            // resultBox
            // 
            this.resultBox.HideSelection = false;
            resources.ApplyResources(this.resultBox, "resultBox");
            this.resultBox.Name = "resultBox";
            this.resultBox.ReadOnly = true;
            // 
            // DecrypterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resultBox);
            this.Controls.Add(this.valueModeCheck);
            this.Controls.Add(this.decencButton);
            this.Controls.Add(this.encryptModeCheck);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.cryptoKeyBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DecrypterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Enter += new System.EventHandler(this.DecrypterForm_Enter);
            this.Leave += new System.EventHandler(this.DecrypterForm_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}