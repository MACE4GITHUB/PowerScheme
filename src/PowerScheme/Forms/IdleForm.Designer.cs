namespace PowerScheme.Forms
{
    partial class IdleForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.idleThresholdTrackBar = new System.Windows.Forms.TrackBar();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.defaultButton = new System.Windows.Forms.Button();
            this.pollingIdleTimeValueLabel = new System.Windows.Forms.Label();
            this.pollingIdleTimeLabel = new System.Windows.Forms.Label();
            this.pollingIdleTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.pollingActiveTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.pollingActiveTimeValueLabel = new System.Windows.Forms.Label();
            this.pollingActiveTimeLabel = new System.Windows.Forms.Label();
            this.idleThresholdValueLabel = new System.Windows.Forms.Label();
            this.captionLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.idleThresholdLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.idleThresholdTrackBar)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingIdleTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingActiveTimeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // idleThresholdTrackBar
            // 
            this.idleThresholdTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdTrackBar.Location = new System.Drawing.Point(307, 45);
            this.idleThresholdTrackBar.Margin = new System.Windows.Forms.Padding(3, 10, 10, 3);
            this.idleThresholdTrackBar.Maximum = 200;
            this.idleThresholdTrackBar.Minimum = 1;
            this.idleThresholdTrackBar.Name = "idleThresholdTrackBar";
            this.idleThresholdTrackBar.Size = new System.Drawing.Size(113, 45);
            this.idleThresholdTrackBar.TabIndex = 0;
            this.idleThresholdTrackBar.TickFrequency = 15;
            this.idleThresholdTrackBar.Value = 1;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.defaultButton, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeValueLabel, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeLabel, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.pollingIdleTimeTrackBar, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeTrackBar, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeValueLabel, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.pollingActiveTimeLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdValueLabel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdTrackBar, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.captionLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.okButton, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.idleThresholdLabel, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(430, 265);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // defaultButton
            // 
            this.defaultButton.BackColor = System.Drawing.Color.SlateBlue;
            this.defaultButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultButton.FlatAppearance.BorderSize = 0;
            this.defaultButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue;
            this.defaultButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumSlateBlue;
            this.defaultButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.defaultButton.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.defaultButton.Location = new System.Drawing.Point(10, 219);
            this.defaultButton.Margin = new System.Windows.Forms.Padding(10, 10, 10, 20);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.defaultButton.Size = new System.Drawing.Size(178, 26);
            this.defaultButton.TabIndex = 10;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = false;
            // 
            // pollingIdleTimeValueLabel
            // 
            this.pollingIdleTimeValueLabel.AutoSize = true;
            this.pollingIdleTimeValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingIdleTimeValueLabel.Location = new System.Drawing.Point(208, 161);
            this.pollingIdleTimeValueLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.pollingIdleTimeValueLabel.Name = "pollingIdleTimeValueLabel";
            this.pollingIdleTimeValueLabel.Size = new System.Drawing.Size(93, 38);
            this.pollingIdleTimeValueLabel.TabIndex = 9;
            this.pollingIdleTimeValueLabel.Text = "value3";
            this.pollingIdleTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pollingIdleTimeLabel
            // 
            this.pollingIdleTimeLabel.AutoSize = true;
            this.pollingIdleTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingIdleTimeLabel.Location = new System.Drawing.Point(20, 161);
            this.pollingIdleTimeLabel.Margin = new System.Windows.Forms.Padding(20, 10, 3, 10);
            this.pollingIdleTimeLabel.Name = "pollingIdleTimeLabel";
            this.pollingIdleTimeLabel.Size = new System.Drawing.Size(175, 38);
            this.pollingIdleTimeLabel.TabIndex = 8;
            this.pollingIdleTimeLabel.Text = "PollingIdleTime";
            // 
            // pollingIdleTimeTrackBar
            // 
            this.pollingIdleTimeTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingIdleTimeTrackBar.Location = new System.Drawing.Point(307, 161);
            this.pollingIdleTimeTrackBar.Margin = new System.Windows.Forms.Padding(3, 10, 10, 3);
            this.pollingIdleTimeTrackBar.Maximum = 5000;
            this.pollingIdleTimeTrackBar.Minimum = 1000;
            this.pollingIdleTimeTrackBar.Name = "pollingIdleTimeTrackBar";
            this.pollingIdleTimeTrackBar.Size = new System.Drawing.Size(113, 45);
            this.pollingIdleTimeTrackBar.TabIndex = 7;
            this.pollingIdleTimeTrackBar.TickFrequency = 500;
            this.pollingIdleTimeTrackBar.Value = 1500;
            // 
            // pollingActiveTimeTrackBar
            // 
            this.pollingActiveTimeTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeTrackBar.Location = new System.Drawing.Point(307, 103);
            this.pollingActiveTimeTrackBar.Margin = new System.Windows.Forms.Padding(3, 10, 10, 3);
            this.pollingActiveTimeTrackBar.Maximum = 5000;
            this.pollingActiveTimeTrackBar.Minimum = 1000;
            this.pollingActiveTimeTrackBar.Name = "pollingActiveTimeTrackBar";
            this.pollingActiveTimeTrackBar.Size = new System.Drawing.Size(113, 45);
            this.pollingActiveTimeTrackBar.TabIndex = 6;
            this.pollingActiveTimeTrackBar.TickFrequency = 500;
            this.pollingActiveTimeTrackBar.Value = 1500;
            // 
            // pollingActiveTimeValueLabel
            // 
            this.pollingActiveTimeValueLabel.AutoSize = true;
            this.pollingActiveTimeValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingActiveTimeValueLabel.Location = new System.Drawing.Point(208, 103);
            this.pollingActiveTimeValueLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.pollingActiveTimeValueLabel.Name = "pollingActiveTimeValueLabel";
            this.pollingActiveTimeValueLabel.Size = new System.Drawing.Size(93, 38);
            this.pollingActiveTimeValueLabel.TabIndex = 5;
            this.pollingActiveTimeValueLabel.Text = "value2";
            this.pollingActiveTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pollingActiveTimeLabel
            // 
            this.pollingActiveTimeLabel.AutoSize = true;
            this.pollingActiveTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pollingActiveTimeLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.pollingActiveTimeLabel.Location = new System.Drawing.Point(20, 103);
            this.pollingActiveTimeLabel.Margin = new System.Windows.Forms.Padding(20, 10, 3, 10);
            this.pollingActiveTimeLabel.Name = "pollingActiveTimeLabel";
            this.pollingActiveTimeLabel.Size = new System.Drawing.Size(175, 38);
            this.pollingActiveTimeLabel.TabIndex = 4;
            this.pollingActiveTimeLabel.Text = "PollingActiveTime";
            // 
            // idleThresholdValueLabel
            // 
            this.idleThresholdValueLabel.AutoSize = true;
            this.idleThresholdValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdValueLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.idleThresholdValueLabel.Location = new System.Drawing.Point(208, 45);
            this.idleThresholdValueLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.idleThresholdValueLabel.Name = "idleThresholdValueLabel";
            this.idleThresholdValueLabel.Size = new System.Drawing.Size(93, 38);
            this.idleThresholdValueLabel.TabIndex = 3;
            this.idleThresholdValueLabel.Text = "value1";
            this.idleThresholdValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // captionLabel
            // 
            this.captionLabel.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.captionLabel, 2);
            this.captionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.captionLabel.Location = new System.Drawing.Point(10, 10);
            this.captionLabel.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(291, 15);
            this.captionLabel.TabIndex = 0;
            this.captionLabel.Text = "IdleOptions";
            // 
            // okButton
            // 
            this.okButton.AutoSize = true;
            this.okButton.BackColor = System.Drawing.Color.SlateBlue;
            this.okButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue;
            this.okButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumSlateBlue;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.okButton.Location = new System.Drawing.Point(314, 219);
            this.okButton.Margin = new System.Windows.Forms.Padding(10, 10, 10, 20);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.okButton.Size = new System.Drawing.Size(106, 26);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // idleThresholdLabel
            // 
            this.idleThresholdLabel.AutoSize = true;
            this.idleThresholdLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idleThresholdLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.idleThresholdLabel.Location = new System.Drawing.Point(20, 45);
            this.idleThresholdLabel.Margin = new System.Windows.Forms.Padding(20, 10, 3, 10);
            this.idleThresholdLabel.Name = "idleThresholdLabel";
            this.idleThresholdLabel.Size = new System.Drawing.Size(175, 38);
            this.idleThresholdLabel.TabIndex = 2;
            this.idleThresholdLabel.Text = "IdleThreshold";
            // 
            // IdleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(430, 265);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IdleForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.idleThresholdTrackBar)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingIdleTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pollingActiveTimeTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar idleThresholdTrackBar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label captionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label idleThresholdLabel;
        private System.Windows.Forms.Label idleThresholdValueLabel;
        private System.Windows.Forms.Label pollingActiveTimeLabel;
        private System.Windows.Forms.Label pollingActiveTimeValueLabel;
        private System.Windows.Forms.TrackBar pollingActiveTimeTrackBar;
        private System.Windows.Forms.TrackBar pollingIdleTimeTrackBar;
        private System.Windows.Forms.Label pollingIdleTimeLabel;
        private System.Windows.Forms.Label pollingIdleTimeValueLabel;
        private System.Windows.Forms.Button defaultButton;
    }
}