namespace Notes.Forms
{
    partial class SigninForm
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
            label1 = new Label();
            txtUserName = new TextBox();
            label2 = new Label();
            txtPassword = new TextBox();
            button1 = new Button();
            label3 = new Label();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F);
            label1.Location = new Point(12, 79);
            label1.Name = "label1";
            label1.Size = new Size(121, 20);
            label1.TabIndex = 0;
            label1.Text = "UserName/Email";
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(138, 76);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(148, 23);
            txtUserName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F);
            label2.Location = new Point(12, 127);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 0;
            label2.Text = "Password";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(138, 124);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(148, 23);
            txtPassword.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(12, 233);
            button1.Name = "button1";
            button1.Size = new Size(274, 27);
            button1.TabIndex = 3;
            button1.Text = "Sign In";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("B Nazanin", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 178);
            label3.Location = new Point(110, 17);
            label3.Name = "label3";
            label3.Size = new Size(70, 28);
            label3.TabIndex = 0;
            label3.Text = "Sign In";
            // 
            // button2
            // 
            button2.Location = new Point(12, 266);
            button2.Name = "button2";
            button2.Size = new Size(274, 29);
            button2.TabIndex = 4;
            button2.Text = "Sign Up";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // SigninForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(298, 311);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(txtUserName);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(txtPassword);
            MaximizeBox = false;
            MaximumSize = new Size(314, 350);
            MinimumSize = new Size(314, 350);
            Name = "SigninForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SigninForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtUserName;
        private Label label2;
        private TextBox txtPassword;
        private Button button1;
        private Label label3;
        private Button button2;
    }
}