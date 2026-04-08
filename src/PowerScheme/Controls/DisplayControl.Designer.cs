namespace PowerScheme.Controls
{
    partial class DisplayControl
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.powerSchemesLabel = new System.Windows.Forms.Label();
            this.turnOffLockedDisplayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.turnOffLockedDisplayValueLabel = new System.Windows.Forms.Label();
            this.turnOffLockedDisplayLabel = new System.Windows.Forms.Label();
            this.turnOffDisplayValueLabel = new System.Windows.Forms.Label();
            this.turnOffDisplayLabel = new System.Windows.Forms.Label();
            this.turnOffDisplayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.powerSchemeComboBox = new System.Windows.Forms.ComboBox();
            this.applyButton = new PowerScheme.Controls.BaseButton();
            this.okButton = new PowerScheme.Controls.BaseButton();
            this.defaultButton = new PowerScheme.Controls.BaseButton();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.turnOffLockedDisplayNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.turnOffDisplayNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.powerSchemesLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.applyButton, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.turnOffLockedDisplayNumericUpDown, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.turnOffLockedDisplayValueLabel, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.turnOffLockedDisplayLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.turnOffDisplayValueLabel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.turnOffDisplayLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.defaultButton, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.turnOffDisplayNumericUpDown, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.powerSchemeComboBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(460, 144);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // powerSchemesLabel
            // 
            this.powerSchemesLabel.AutoSize = true;
            this.powerSchemesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powerSchemesLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.powerSchemesLabel.Location = new System.Drawing.Point(10, 10);
            this.powerSchemesLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.powerSchemesLabel.Name = "powerSchemesLabel";
            this.powerSchemesLabel.Size = new System.Drawing.Size(208, 23);
            this.powerSchemesLabel.TabIndex = 6;
            this.powerSchemesLabel.Text = "Power Schemes";
            // 
            // turnOffLockedDisplayNumericUpDown
            // 
            this.turnOffLockedDisplayNumericUpDown.BackColor = System.Drawing.Color.Black;
            this.turnOffLockedDisplayNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.turnOffLockedDisplayNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffLockedDisplayNumericUpDown.ForeColor = System.Drawing.Color.White;
            this.turnOffLockedDisplayNumericUpDown.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.turnOffLockedDisplayNumericUpDown.Location = new System.Drawing.Point(341, 74);
            this.turnOffLockedDisplayNumericUpDown.Margin = new System.Windows.Forms.Padding(1, 6, 5, 5);
            this.turnOffLockedDisplayNumericUpDown.Maximum = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            this.turnOffLockedDisplayNumericUpDown.Name = "turnOffLockedDisplayNumericUpDown";
            this.turnOffLockedDisplayNumericUpDown.Size = new System.Drawing.Size(114, 23);
            this.turnOffLockedDisplayNumericUpDown.TabIndex = 3;
            this.turnOffLockedDisplayNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.turnOffLockedDisplayNumericUpDown.Value = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            // 
            // turnOffLockedDisplayValueLabel
            // 
            this.turnOffLockedDisplayValueLabel.AutoSize = true;
            this.turnOffLockedDisplayValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffLockedDisplayValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.turnOffLockedDisplayValueLabel.Location = new System.Drawing.Point(226, 78);
            this.turnOffLockedDisplayValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.turnOffLockedDisplayValueLabel.Name = "turnOffLockedDisplayValueLabel";
            this.turnOffLockedDisplayValueLabel.Size = new System.Drawing.Size(111, 23);
            this.turnOffLockedDisplayValueLabel.TabIndex = 0;
            this.turnOffLockedDisplayValueLabel.Text = "value2";
            this.turnOffLockedDisplayValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // turnOffLockedDisplayLabel
            // 
            this.turnOffLockedDisplayLabel.AutoSize = true;
            this.turnOffLockedDisplayLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffLockedDisplayLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.turnOffLockedDisplayLabel.Location = new System.Drawing.Point(10, 78);
            this.turnOffLockedDisplayLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.turnOffLockedDisplayLabel.Name = "turnOffLockedDisplayLabel";
            this.turnOffLockedDisplayLabel.Size = new System.Drawing.Size(208, 23);
            this.turnOffLockedDisplayLabel.TabIndex = 0;
            this.turnOffLockedDisplayLabel.Text = "Turn off the display when locked";
            // 
            // turnOffDisplayValueLabel
            // 
            this.turnOffDisplayValueLabel.AutoSize = true;
            this.turnOffDisplayValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffDisplayValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.turnOffDisplayValueLabel.Location = new System.Drawing.Point(226, 44);
            this.turnOffDisplayValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.turnOffDisplayValueLabel.Name = "turnOffDisplayValueLabel";
            this.turnOffDisplayValueLabel.Size = new System.Drawing.Size(111, 23);
            this.turnOffDisplayValueLabel.TabIndex = 0;
            this.turnOffDisplayValueLabel.Text = "value1";
            this.turnOffDisplayValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // turnOffDisplayLabel
            // 
            this.turnOffDisplayLabel.AutoSize = true;
            this.turnOffDisplayLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffDisplayLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.turnOffDisplayLabel.Location = new System.Drawing.Point(10, 44);
            this.turnOffDisplayLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.turnOffDisplayLabel.Name = "turnOffDisplayLabel";
            this.turnOffDisplayLabel.Size = new System.Drawing.Size(208, 23);
            this.turnOffDisplayLabel.TabIndex = 0;
            this.turnOffDisplayLabel.Text = "Turn off the display";
            // 
            // turnOffDisplayNumericUpDown
            // 
            this.turnOffDisplayNumericUpDown.BackColor = System.Drawing.Color.Black;
            this.turnOffDisplayNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turnOffDisplayNumericUpDown.ForeColor = System.Drawing.Color.White;
            this.turnOffDisplayNumericUpDown.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.turnOffDisplayNumericUpDown.Location = new System.Drawing.Point(341, 40);
            this.turnOffDisplayNumericUpDown.Margin = new System.Windows.Forms.Padding(1, 6, 5, 5);
            this.turnOffDisplayNumericUpDown.Maximum = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            this.turnOffDisplayNumericUpDown.Name = "turnOffDisplayNumericUpDown";
            this.turnOffDisplayNumericUpDown.Size = new System.Drawing.Size(114, 23);
            this.turnOffDisplayNumericUpDown.TabIndex = 2;
            this.turnOffDisplayNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.turnOffDisplayNumericUpDown.Value = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            // 
            // powerSchemeComboBox
            // 
            this.tableLayoutPanel.SetColumnSpan(this.powerSchemeComboBox, 2);
            this.powerSchemeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powerSchemeComboBox.DropDownHeight = 90;
            this.powerSchemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.powerSchemeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.powerSchemeComboBox.FormattingEnabled = true;
            this.powerSchemeComboBox.IntegralHeight = false;
            this.powerSchemeComboBox.Location = new System.Drawing.Point(226, 8);
            this.powerSchemeComboBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 3);
            this.powerSchemeComboBox.MaxDropDownItems = 6;
            this.powerSchemeComboBox.Name = "powerSchemeComboBox";
            this.powerSchemeComboBox.Size = new System.Drawing.Size(229, 23);
            this.powerSchemeComboBox.TabIndex = 1;
            // 
            // applyButton
            // 
            this.applyButton.BorderThickness = 1;
            this.applyButton.CornerRadius = 8;
            this.applyButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applyButton.FlatAppearance.BorderSize = 0;
            this.applyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.applyButton.ForeColor = System.Drawing.Color.White;
            this.applyButton.Location = new System.Drawing.Point(239, 112);
            this.applyButton.Margin = new System.Windows.Forms.Padding(18, 10, 1, 15);
            this.applyButton.Name = "applyButton";
            this.applyButton.Padding = new System.Windows.Forms.Padding(5);
            this.applyButton.Size = new System.Drawing.Size(100, 17);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.BorderThickness = 1;
            this.okButton.CornerRadius = 8;
            this.okButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(350, 112);
            this.okButton.Margin = new System.Windows.Forms.Padding(10, 10, 10, 15);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(5);
            this.okButton.Size = new System.Drawing.Size(100, 17);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // defaultButton
            // 
            this.defaultButton.BorderThickness = 1;
            this.defaultButton.CornerRadius = 8;
            this.defaultButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultButton.FlatAppearance.BorderSize = 0;
            this.defaultButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.defaultButton.ForeColor = System.Drawing.Color.White;
            this.defaultButton.Location = new System.Drawing.Point(10, 112);
            this.defaultButton.Margin = new System.Windows.Forms.Padding(10, 10, 30, 15);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Padding = new System.Windows.Forms.Padding(5);
            this.defaultButton.Size = new System.Drawing.Size(181, 17);
            this.defaultButton.TabIndex = 4;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            // 
            // DisplayControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DisplayControl";
            this.Size = new System.Drawing.Size(460, 144);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.turnOffLockedDisplayNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.turnOffDisplayNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label turnOffLockedDisplayValueLabel;
        private System.Windows.Forms.Label turnOffLockedDisplayLabel;
        private System.Windows.Forms.Label turnOffDisplayValueLabel;
        private System.Windows.Forms.Label turnOffDisplayLabel;
        private BaseButton defaultButton;
        private BaseButton okButton;
        private System.Windows.Forms.NumericUpDown turnOffDisplayNumericUpDown;
        private System.Windows.Forms.NumericUpDown turnOffLockedDisplayNumericUpDown;
        private BaseButton applyButton;
        private System.Windows.Forms.Label powerSchemesLabel;
        private System.Windows.Forms.ComboBox powerSchemeComboBox;
    }
}
