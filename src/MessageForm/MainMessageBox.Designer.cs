namespace MessageForm
{
    partial class MainMessageBox
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
            UnsubscribeAllEvents();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._plFooter = new System.Windows.Forms.Panel();
            this._flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this._plIcon = new System.Windows.Forms.Panel();
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._plHeader = new System.Windows.Forms.Panel();
            this._lblMessage = new System.Windows.Forms.Label();
            this._labelTimerTick = new System.Windows.Forms.Label();
            this._plFooter.SuspendLayout();
            this._plIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this._plHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // _plFooter
            // 
            this._plFooter.BackColor = System.Drawing.Color.DimGray;
            this._plFooter.Controls.Add(this._labelTimerTick);
            this._plFooter.Controls.Add(this._flpButtons);
            this._plFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._plFooter.Location = new System.Drawing.Point(0, 131);
            this._plFooter.Name = "_plFooter";
            this._plFooter.Size = new System.Drawing.Size(474, 40);
            this._plFooter.TabIndex = 1;
            // 
            // _flpButtons
            // 
            this._flpButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this._flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._flpButtons.Location = new System.Drawing.Point(80, 0);
            this._flpButtons.Name = "_flpButtons";
            this._flpButtons.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._flpButtons.Size = new System.Drawing.Size(394, 40);
            this._flpButtons.TabIndex = 0;
            // 
            // _plIcon
            // 
            this._plIcon.Controls.Add(this._pictureBox);
            this._plIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this._plIcon.Location = new System.Drawing.Point(0, 0);
            this._plIcon.Name = "_plIcon";
            this._plIcon.Padding = new System.Windows.Forms.Padding(20, 20, 0, 0);
            this._plIcon.Size = new System.Drawing.Size(80, 131);
            this._plIcon.TabIndex = 2;
            // 
            // _pictureBox
            // 
            this._pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pictureBox.Location = new System.Drawing.Point(20, 20);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(60, 111);
            this._pictureBox.TabIndex = 0;
            this._pictureBox.TabStop = false;
            // 
            // _plHeader
            // 
            this._plHeader.Controls.Add(this._lblMessage);
            this._plHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this._plHeader.Location = new System.Drawing.Point(80, 0);
            this._plHeader.Name = "_plHeader";
            this._plHeader.Padding = new System.Windows.Forms.Padding(5, 20, 10, 20);
            this._plHeader.Size = new System.Drawing.Size(394, 131);
            this._plHeader.TabIndex = 3;
            // 
            // _lblMessage
            // 
            this._lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblMessage.Location = new System.Drawing.Point(5, 20);
            this._lblMessage.Name = "_lblMessage";
            this._lblMessage.Size = new System.Drawing.Size(379, 91);
            this._lblMessage.TabIndex = 0;
            this._lblMessage.Text = "Message";
            // 
            // _labelTimerTick
            // 
            this._labelTimerTick.AutoSize = true;
            this._labelTimerTick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labelTimerTick.ForeColor = System.Drawing.Color.White;
            this._labelTimerTick.Location = new System.Drawing.Point(19, 12);
            this._labelTimerTick.Name = "_labelTimerTick";
            this._labelTimerTick.Size = new System.Drawing.Size(35, 13);
            this._labelTimerTick.TabIndex = 1;
            this._labelTimerTick.Text = "1440";
            // 
            // MainMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(474, 171);
            this.Controls.Add(this._plHeader);
            this.Controls.Add(this._plIcon);
            this.Controls.Add(this._plFooter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainMessageBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainMessageBox";
            this._plFooter.ResumeLayout(false);
            this._plFooter.PerformLayout();
            this._plIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this._plHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel _plFooter;
        private System.Windows.Forms.FlowLayoutPanel _flpButtons;
        private System.Windows.Forms.Panel _plIcon;
        private System.Windows.Forms.Panel _plHeader;
        private System.Windows.Forms.Label _lblMessage;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Label _labelTimerTick;
    }
}