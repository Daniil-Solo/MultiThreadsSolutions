
namespace Lab3_Task2_
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.App1 = new System.Windows.Forms.GroupBox();
            this.App1_button = new System.Windows.Forms.Button();
            this.App1_status = new System.Windows.Forms.Label();
            this.App1_text = new System.Windows.Forms.TextBox();
            this.App2 = new System.Windows.Forms.GroupBox();
            this.App2_status = new System.Windows.Forms.Label();
            this.App2_text = new System.Windows.Forms.TextBox();
            this.App2_button = new System.Windows.Forms.Button();
            this.App1.SuspendLayout();
            this.App2.SuspendLayout();
            this.SuspendLayout();
            // 
            // App1
            // 
            this.App1.Controls.Add(this.App1_button);
            this.App1.Controls.Add(this.App1_status);
            this.App1.Controls.Add(this.App1_text);
            this.App1.Location = new System.Drawing.Point(12, 12);
            this.App1.Name = "App1";
            this.App1.Size = new System.Drawing.Size(846, 196);
            this.App1.TabIndex = 0;
            this.App1.TabStop = false;
            this.App1.Text = "Приложение 1 - Писатель";
            // 
            // App1_button
            // 
            this.App1_button.Location = new System.Drawing.Point(630, 121);
            this.App1_button.Name = "App1_button";
            this.App1_button.Size = new System.Drawing.Size(94, 29);
            this.App1_button.TabIndex = 2;
            this.App1_button.Text = "Отправить";
            this.App1_button.UseVisualStyleBackColor = true;
            this.App1_button.Click += new System.EventHandler(this.App1_button_Click);
            // 
            // App1_status
            // 
            this.App1_status.AutoSize = true;
            this.App1_status.Location = new System.Drawing.Point(630, 43);
            this.App1_status.Name = "App1_status";
            this.App1_status.Size = new System.Drawing.Size(119, 20);
            this.App1_status.TabIndex = 1;
            this.App1_status.Text = "Статус писателя";
            // 
            // App1_text
            // 
            this.App1_text.Location = new System.Drawing.Point(6, 26);
            this.App1_text.Multiline = true;
            this.App1_text.Name = "App1_text";
            this.App1_text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.App1_text.Size = new System.Drawing.Size(607, 164);
            this.App1_text.TabIndex = 0;
            // 
            // App2
            // 
            this.App2.Controls.Add(this.App2_button);
            this.App2.Controls.Add(this.App2_status);
            this.App2.Controls.Add(this.App2_text);
            this.App2.Location = new System.Drawing.Point(12, 214);
            this.App2.Name = "App2";
            this.App2.Size = new System.Drawing.Size(846, 224);
            this.App2.TabIndex = 1;
            this.App2.TabStop = false;
            this.App2.Text = "Приложение 2 - Читатель";
            // 
            // App2_status
            // 
            this.App2_status.AutoSize = true;
            this.App2_status.Location = new System.Drawing.Point(630, 100);
            this.App2_status.Name = "App2_status";
            this.App2_status.Size = new System.Drawing.Size(117, 20);
            this.App2_status.TabIndex = 1;
            this.App2_status.Text = "Статус читателя";
            // 
            // App2_text
            // 
            this.App2_text.Location = new System.Drawing.Point(6, 26);
            this.App2_text.Multiline = true;
            this.App2_text.Name = "App2_text";
            this.App2_text.ReadOnly = true;
            this.App2_text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.App2_text.Size = new System.Drawing.Size(607, 192);
            this.App2_text.TabIndex = 0;
            // 
            // App2_button
            // 
            this.App2_button.Location = new System.Drawing.Point(630, 35);
            this.App2_button.Name = "App2_button";
            this.App2_button.Size = new System.Drawing.Size(94, 29);
            this.App2_button.TabIndex = 2;
            this.App2_button.Text = "Запустить";
            this.App2_button.UseVisualStyleBackColor = true;
            this.App2_button.Click += new System.EventHandler(this.App2_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 450);
            this.Controls.Add(this.App2);
            this.Controls.Add(this.App1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.App1.ResumeLayout(false);
            this.App1.PerformLayout();
            this.App2.ResumeLayout(false);
            this.App2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox App1;
        private System.Windows.Forms.GroupBox App2;
        private System.Windows.Forms.TextBox App1_text;
        private System.Windows.Forms.TextBox App2_text;
        private System.Windows.Forms.Button App1_button;
        private System.Windows.Forms.Label App1_status;
        private System.Windows.Forms.Label App2_status;
        private System.Windows.Forms.Button App2_button;
    }
}

