namespace PowerScheme.Controls
{
    partial class SleepControl
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
            this.applyButton = new PowerScheme.Controls.BaseButton();
            this.hibernateNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.okButton = new PowerScheme.Controls.BaseButton();
            this.hibernateValueLabel = new System.Windows.Forms.Label();
            this.hibernateLabel = new System.Windows.Forms.Label();
            this.sleepValueLabel = new System.Windows.Forms.Label();
            this.sleepLabel = new System.Windows.Forms.Label();
            this.defaultButton = new PowerScheme.Controls.BaseButton();
            this.sleepNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.powerSchemeComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hibernateNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleepNumericUpDown)).BeginInit();
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
            this.tableLayoutPanel.Controls.Add(this.hibernateNumericUpDown, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.hibernateValueLabel, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.hibernateLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.sleepValueLabel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.sleepLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.defaultButton, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.sleepNumericUpDown, 2, 1);
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
            // hibernateNumericUpDown
            // 
            this.hibernateNumericUpDown.BackColor = System.Drawing.Color.Black;
            this.hibernateNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hibernateNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hibernateNumericUpDown.ForeColor = System.Drawing.Color.White;
            this.hibernateNumericUpDown.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.hibernateNumericUpDown.Location = new System.Drawing.Point(341, 74);
            this.hibernateNumericUpDown.Margin = new System.Windows.Forms.Padding(1, 6, 5, 5);
            this.hibernateNumericUpDown.Maximum = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            this.hibernateNumericUpDown.Name = "hibernateNumericUpDown";
            this.hibernateNumericUpDown.Size = new System.Drawing.Size(114, 23);
            this.hibernateNumericUpDown.TabIndex = 3;
            this.hibernateNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.hibernateNumericUpDown.Value = new decimal(new int[] {
            18000,
            0,
            0,
            0});
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
            // hibernateValueLabel
            // 
            this.hibernateValueLabel.AutoSize = true;
            this.hibernateValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hibernateValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.hibernateValueLabel.Location = new System.Drawing.Point(226, 78);
            this.hibernateValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.hibernateValueLabel.Name = "hibernateValueLabel";
            this.hibernateValueLabel.Size = new System.Drawing.Size(111, 23);
            this.hibernateValueLabel.TabIndex = 0;
            this.hibernateValueLabel.Text = "value2";
            this.hibernateValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // hibernateLabel
            // 
            this.hibernateLabel.AutoSize = true;
            this.hibernateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hibernateLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.hibernateLabel.Location = new System.Drawing.Point(10, 78);
            this.hibernateLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.hibernateLabel.Name = "hibernateLabel";
            this.hibernateLabel.Size = new System.Drawing.Size(208, 23);
            this.hibernateLabel.TabIndex = 0;
            this.hibernateLabel.Text = "Hibernate";
            // 
            // sleepValueLabel
            // 
            this.sleepValueLabel.AutoSize = true;
            this.sleepValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sleepValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.sleepValueLabel.Location = new System.Drawing.Point(226, 44);
            this.sleepValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.sleepValueLabel.Name = "sleepValueLabel";
            this.sleepValueLabel.Size = new System.Drawing.Size(111, 23);
            this.sleepValueLabel.TabIndex = 0;
            this.sleepValueLabel.Text = "value1";
            this.sleepValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sleepLabel
            // 
            this.sleepLabel.AutoSize = true;
            this.sleepLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sleepLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.sleepLabel.Location = new System.Drawing.Point(10, 44);
            this.sleepLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.sleepLabel.Name = "sleepLabel";
            this.sleepLabel.Size = new System.Drawing.Size(208, 23);
            this.sleepLabel.TabIndex = 0;
            this.sleepLabel.Text = "Sleep";
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
            // sleepNumericUpDown
            // 
            this.sleepNumericUpDown.BackColor = System.Drawing.Color.Black;
            this.sleepNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sleepNumericUpDown.ForeColor = System.Drawing.Color.White;
            this.sleepNumericUpDown.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.sleepNumericUpDown.Location = new System.Drawing.Point(341, 40);
            this.sleepNumericUpDown.Margin = new System.Windows.Forms.Padding(1, 6, 5, 5);
            this.sleepNumericUpDown.Maximum = new decimal(new int[] {
            18000,
            0,
            0,
            0});
            this.sleepNumericUpDown.Name = "sleepNumericUpDown";
            this.sleepNumericUpDown.Size = new System.Drawing.Size(114, 23);
            this.sleepNumericUpDown.TabIndex = 2;
            this.sleepNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sleepNumericUpDown.Value = new decimal(new int[] {
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
            // SleepControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SleepControl";
            this.Size = new System.Drawing.Size(460, 144);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hibernateNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleepNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label hibernateValueLabel;
        private System.Windows.Forms.Label hibernateLabel;
        private System.Windows.Forms.Label sleepValueLabel;
        private System.Windows.Forms.Label sleepLabel;
        private BaseButton defaultButton;
        private BaseButton okButton;
        private System.Windows.Forms.NumericUpDown sleepNumericUpDown;
        private System.Windows.Forms.NumericUpDown hibernateNumericUpDown;
        private BaseButton applyButton;
        private System.Windows.Forms.Label powerSchemesLabel;
        private System.Windows.Forms.ComboBox powerSchemeComboBox;
    }
}
