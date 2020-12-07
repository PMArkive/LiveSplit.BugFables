
namespace LiveSplit.BugFables.UI
{
  partial class SettingsUserControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.rdbStartEndOnly = new System.Windows.Forms.RadioButton();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rdbGlitchless = new System.Windows.Forms.RadioButton();
      this.grpSplits = new System.Windows.Forms.GroupBox();
      this.lblVersion = new System.Windows.Forms.Label();
      this.btnSetCurrentSplits = new System.Windows.Forms.Button();
      this.flowSplits = new System.Windows.Forms.FlowLayoutPanel();
      this.groupBox1.SuspendLayout();
      this.grpSplits.SuspendLayout();
      this.SuspendLayout();
      // 
      // rdbStartEndOnly
      // 
      this.rdbStartEndOnly.AutoSize = true;
      this.rdbStartEndOnly.Location = new System.Drawing.Point(6, 25);
      this.rdbStartEndOnly.Name = "rdbStartEndOnly";
      this.rdbStartEndOnly.Size = new System.Drawing.Size(163, 24);
      this.rdbStartEndOnly.TabIndex = 0;
      this.rdbStartEndOnly.TabStop = true;
      this.rdbStartEndOnly.Text = "Start and end only";
      this.rdbStartEndOnly.UseVisualStyleBackColor = true;
      this.rdbStartEndOnly.CheckedChanged += new System.EventHandler(this.rdbStartEndOnly_CheckedChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.rdbGlitchless);
      this.groupBox1.Controls.Add(this.rdbStartEndOnly);
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(427, 62);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Mode";
      // 
      // rdbGlitchless
      // 
      this.rdbGlitchless.AutoSize = true;
      this.rdbGlitchless.Location = new System.Drawing.Point(175, 25);
      this.rdbGlitchless.Name = "rdbGlitchless";
      this.rdbGlitchless.Size = new System.Drawing.Size(122, 24);
      this.rdbGlitchless.TabIndex = 1;
      this.rdbGlitchless.TabStop = true;
      this.rdbGlitchless.Text = "Select Splits";
      this.rdbGlitchless.UseVisualStyleBackColor = true;
      this.rdbGlitchless.CheckedChanged += new System.EventHandler(this.rdbGlitchless_CheckedChanged);
      // 
      // grpSplits
      // 
      this.grpSplits.Controls.Add(this.lblVersion);
      this.grpSplits.Controls.Add(this.btnSetCurrentSplits);
      this.grpSplits.Controls.Add(this.flowSplits);
      this.grpSplits.Location = new System.Drawing.Point(3, 83);
      this.grpSplits.Name = "grpSplits";
      this.grpSplits.Size = new System.Drawing.Size(694, 687);
      this.grpSplits.TabIndex = 2;
      this.grpSplits.TabStop = false;
      this.grpSplits.Text = "Splits";
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.Location = new System.Drawing.Point(2, 649);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(108, 20);
      this.lblVersion.TabIndex = 3;
      this.lblVersion.Text = "Version X.X.X";
      // 
      // btnSetCurrentSplits
      // 
      this.btnSetCurrentSplits.Location = new System.Drawing.Point(375, 640);
      this.btnSetCurrentSplits.Name = "btnSetCurrentSplits";
      this.btnSetCurrentSplits.Size = new System.Drawing.Size(313, 38);
      this.btnSetCurrentSplits.TabIndex = 3;
      this.btnSetCurrentSplits.Text = "Set current splits to all enabled splits";
      this.btnSetCurrentSplits.UseVisualStyleBackColor = true;
      this.btnSetCurrentSplits.Click += new System.EventHandler(this.btnSetCurrentSplits_Click);
      // 
      // flowSplits
      // 
      this.flowSplits.AutoScroll = true;
      this.flowSplits.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowSplits.Location = new System.Drawing.Point(6, 34);
      this.flowSplits.MinimumSize = new System.Drawing.Size(0, 600);
      this.flowSplits.Name = "flowSplits";
      this.flowSplits.Size = new System.Drawing.Size(682, 600);
      this.flowSplits.TabIndex = 0;
      this.flowSplits.WrapContents = false;
      // 
      // SettingsUserControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.grpSplits);
      this.Controls.Add(this.groupBox1);
      this.Name = "SettingsUserControl";
      this.Size = new System.Drawing.Size(700, 779);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.grpSplits.ResumeLayout(false);
      this.grpSplits.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RadioButton rdbStartEndOnly;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton rdbGlitchless;
    private System.Windows.Forms.GroupBox grpSplits;
    private System.Windows.Forms.FlowLayoutPanel flowSplits;
    private System.Windows.Forms.Button btnSetCurrentSplits;
    private System.Windows.Forms.Label lblVersion;
  }
}
