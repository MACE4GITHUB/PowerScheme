namespace PowerSchemes
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextLeftMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextRightMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // contextLeftMenuStrip
            // 
            this.contextLeftMenuStrip.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.contextLeftMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextLeftMenuStrip.Name = "contextLeftMenuStrip";
            this.contextLeftMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // contextRightMenuStrip
            // 
            this.contextRightMenuStrip.Name = "contextRightMenuStrip";
            this.contextRightMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 90);
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Text = "PowerScheme";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextLeftMenuStrip;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextRightMenuStrip;
    }
}

