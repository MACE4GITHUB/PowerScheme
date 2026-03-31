namespace PowerScheme.Controls
{
    partial class ThresholdControl
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
            this.pollingIdleTimeValueLabel = new System.Windows.Forms.Label();
            this.pollingIdleTimeLabel = new System.Windows.Forms.Label();
            this.pollingIdleTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.pollingActiveTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.pollingActiveTimeValueLabel = new System.Windows.Forms.Label();
            this.pollingActiveTimeLabel = new System.Windows.Forms.Label();
            this.idleThresholdValueLabel = new System.Windows.Forms.Label();
            this.idleThresholdTrackBar = new System.Windows.Forms.TrackBar();
            this.idleThresholdLabel = new System.Windows.Forms.Label();
            this.okButton = new PowerScheme.Controls.BaseButton();
            this.defaultButton = new PowerScheme.Controls.BaseButton();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingIdleTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingActiveTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleThresholdTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeValueLabel, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeTrackBar, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeTrackBar, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeValueLabel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdValueLabel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdTrackBar, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.defaultButton, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(460, 215);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // pollingIdleTimeValueLabel
            // 
            this.pollingIdleTimeValueLabel.AutoSize = true;
            this.pollingIdleTimeValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingIdleTimeValueLabel.Location = new System.Drawing.Point(217, 122);
            this.pollingIdleTimeValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.pollingIdleTimeValueLabel.Name = "pollingIdleTimeValueLabel";
            this.pollingIdleTimeValueLabel.Size = new System.Drawing.Size(106, 45);
            this.pollingIdleTimeValueLabel.TabIndex = 9;
            this.pollingIdleTimeValueLabel.Text = "value3";
            this.pollingIdleTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pollingIdleTimeLabel
            // 
            this.pollingIdleTimeLabel.AutoSize = true;
            this.pollingIdleTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingIdleTimeLabel.Location = new System.Drawing.Point(10, 122);
            this.pollingIdleTimeLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.pollingIdleTimeLabel.Name = "pollingIdleTimeLabel";
            this.pollingIdleTimeLabel.Size = new System.Drawing.Size(199, 45);
            this.pollingIdleTimeLabel.TabIndex = 8;
            this.pollingIdleTimeLabel.Text = "PollingIdleTime";
            // 
            // pollingIdleTimeTrackBar
            // 
            this.pollingIdleTimeTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeTrackBar.Location = new System.Drawing.Point(327, 122);
            this.pollingIdleTimeTrackBar.Margin = new System.Windows.Forms.Padding(1, 10, 5, 1);
            this.pollingIdleTimeTrackBar.Maximum = 5000;
            this.pollingIdleTimeTrackBar.Minimum = 1000;
            this.pollingIdleTimeTrackBar.Name = "pollingIdleTimeTrackBar";
            this.pollingIdleTimeTrackBar.Size = new System.Drawing.Size(128, 45);
            this.pollingIdleTimeTrackBar.TabIndex = 7;
            this.pollingIdleTimeTrackBar.TickFrequency = 500;
            this.pollingIdleTimeTrackBar.Value = 1500;
            // 
            // pollingActiveTimeTrackBar
            // 
            this.pollingActiveTimeTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeTrackBar.Location = new System.Drawing.Point(327, 66);
            this.pollingActiveTimeTrackBar.Margin = new System.Windows.Forms.Padding(1, 10, 5, 1);
            this.pollingActiveTimeTrackBar.Maximum = 5000;
            this.pollingActiveTimeTrackBar.Minimum = 1000;
            this.pollingActiveTimeTrackBar.Name = "pollingActiveTimeTrackBar";
            this.pollingActiveTimeTrackBar.Size = new System.Drawing.Size(128, 45);
            this.pollingActiveTimeTrackBar.TabIndex = 6;
            this.pollingActiveTimeTrackBar.TickFrequency = 500;
            this.pollingActiveTimeTrackBar.Value = 1500;
            // 
            // pollingActiveTimeValueLabel
            // 
            this.pollingActiveTimeValueLabel.AutoSize = true;
            this.pollingActiveTimeValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingActiveTimeValueLabel.Location = new System.Drawing.Point(217, 66);
            this.pollingActiveTimeValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.pollingActiveTimeValueLabel.Name = "pollingActiveTimeValueLabel";
            this.pollingActiveTimeValueLabel.Size = new System.Drawing.Size(106, 45);
            this.pollingActiveTimeValueLabel.TabIndex = 5;
            this.pollingActiveTimeValueLabel.Text = "value2";
            this.pollingActiveTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pollingActiveTimeLabel
            // 
            this.pollingActiveTimeLabel.AutoSize = true;
            this.pollingActiveTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingActiveTimeLabel.Location = new System.Drawing.Point(10, 66);
            this.pollingActiveTimeLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.pollingActiveTimeLabel.Name = "pollingActiveTimeLabel";
            this.pollingActiveTimeLabel.Size = new System.Drawing.Size(199, 45);
            this.pollingActiveTimeLabel.TabIndex = 4;
            this.pollingActiveTimeLabel.Text = "PollingActiveTime";
            // 
            // idleThresholdValueLabel
            // 
            this.idleThresholdValueLabel.AutoSize = true;
            this.idleThresholdValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.idleThresholdValueLabel.Location = new System.Drawing.Point(217, 10);
            this.idleThresholdValueLabel.Margin = new System.Windows.Forms.Padding(5, 10, 3, 1);
            this.idleThresholdValueLabel.Name = "idleThresholdValueLabel";
            this.idleThresholdValueLabel.Size = new System.Drawing.Size(106, 45);
            this.idleThresholdValueLabel.TabIndex = 3;
            this.idleThresholdValueLabel.Text = "value1";
            this.idleThresholdValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // idleThresholdTrackBar
            // 
            this.idleThresholdTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdTrackBar.Location = new System.Drawing.Point(327, 10);
            this.idleThresholdTrackBar.Margin = new System.Windows.Forms.Padding(1, 10, 5, 1);
            this.idleThresholdTrackBar.Maximum = 200;
            this.idleThresholdTrackBar.Minimum = 1;
            this.idleThresholdTrackBar.Name = "idleThresholdTrackBar";
            this.idleThresholdTrackBar.Size = new System.Drawing.Size(128, 45);
            this.idleThresholdTrackBar.TabIndex = 0;
            this.idleThresholdTrackBar.TickFrequency = 15;
            this.idleThresholdTrackBar.Value = 1;
            // 
            // idleThresholdLabel
            // 
            this.idleThresholdLabel.AutoSize = true;
            this.idleThresholdLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.idleThresholdLabel.Location = new System.Drawing.Point(10, 10);
            this.idleThresholdLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 1);
            this.idleThresholdLabel.Name = "idleThresholdLabel";
            this.idleThresholdLabel.Size = new System.Drawing.Size(199, 45);
            this.idleThresholdLabel.TabIndex = 2;
            this.idleThresholdLabel.Text = "IdleThreshold";
            // 
            // okButton
            // 
            this.okButton.BorderThickness = 1;
            this.okButton.CornerRadius = 8;
            this.okButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(336, 183);
            this.okButton.Margin = new System.Windows.Forms.Padding(10, 15, 10, 15);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(5);
            this.okButton.Size = new System.Drawing.Size(114, 17);
            this.okButton.TabIndex = 12;
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
            this.defaultButton.Location = new System.Drawing.Point(10, 183);
            this.defaultButton.Margin = new System.Windows.Forms.Padding(10, 15, 10, 15);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Padding = new System.Windows.Forms.Padding(5);
            this.defaultButton.Size = new System.Drawing.Size(192, 17);
            this.defaultButton.TabIndex = 10;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            // 
            // ThresholdControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ThresholdControl";
            this.Size = new System.Drawing.Size(460, 215);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingIdleTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingActiveTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleThresholdTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label pollingIdleTimeValueLabel;
        private System.Windows.Forms.Label pollingIdleTimeLabel;
        private System.Windows.Forms.TrackBar pollingIdleTimeTrackBar;
        private System.Windows.Forms.TrackBar pollingActiveTimeTrackBar;
        private System.Windows.Forms.Label pollingActiveTimeValueLabel;
        private System.Windows.Forms.Label pollingActiveTimeLabel;
        private System.Windows.Forms.Label idleThresholdValueLabel;
        private System.Windows.Forms.TrackBar idleThresholdTrackBar;
        private System.Windows.Forms.Label idleThresholdLabel;
        private BaseButton defaultButton;
        private BaseButton okButton;
    }
}
