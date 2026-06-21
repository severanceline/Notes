namespace Notes.Forms
{
    partial class SignupForm
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
            btnSignUp = new Button();
            btnSignIn = new Button();
            txtPassword = new TextBox();
            txtUserName = new TextBox();
            lblPassword = new Label();
            lblUserName = new Label();
            lblTitle = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            txtFullName = new TextBox();
            lblName = new Label();
            SuspendLayout();
            // 
            // btnSignUp
            // 
            btnSignUp.Location = new Point(12, 239);
            btnSignUp.Name = "btnSignUp";
            btnSignUp.Size = new Size(274, 29);
            btnSignUp.TabIndex = 4;
            btnSignUp.Text = "Sign Up";
            btnSignUp.UseVisualStyleBackColor = true;
            btnSignUp.Click += btnSignUp_Click;
            // 
            // btnSignIn
            // 
            btnSignIn.Location = new Point(12, 274);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new Size(274, 27);
            btnSignIn.TabIndex = 5;
            btnSignIn.Text = "Sign In";
            btnSignIn.UseVisualStyleBackColor = true;
            btnSignIn.Click += btnSignIn_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(110, 202);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(176, 23);
            txtPassword.TabIndex = 3;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(110, 113);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(176, 23);
            txtUserName.TabIndex = 1;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 11.25F);
            lblPassword.Location = new Point(12, 205);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(70, 20);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "Password";
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Segoe UI", 11.25F);
            lblUserName.Location = new Point(12, 116);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(78, 20);
            lblUserName.TabIndex = 4;
            lblUserName.Text = "UserName";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("B Nazanin", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 178);
            lblTitle.Location = new Point(113, 17);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(79, 28);
            lblTitle.TabIndex = 5;
            lblTitle.Text = "Sign Up";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(110, 156);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(176, 23);
            txtEmail.TabIndex = 2;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 11.25F);
            lblEmail.Location = new Point(12, 159);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 10;
            lblEmail.Text = "Email";
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(110, 73);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(176, 23);
            txtFullName.TabIndex = 0;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 11.25F);
            lblName.Location = new Point(12, 76);
            lblName.Name = "lblName";
            lblName.Size = new Size(76, 20);
            lblName.TabIndex = 12;
            lblName.Text = "Full Name";
            // 
            // SignupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(298, 311);
            Controls.Add(txtFullName);
            Controls.Add(lblName);
            Controls.Add(txtEmail);
            Controls.Add(lblEmail);
            Controls.Add(btnSignUp);
            Controls.Add(btnSignIn);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(lblPassword);
            Controls.Add(lblUserName);
            Controls.Add(lblTitle);
            MaximizeBox = false;
            MaximumSize = new Size(314, 350);
            MinimumSize = new Size(314, 350);
            Name = "SignupForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SignupForm";
            Load += SignupForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSignUp;
        private Button btnSignIn;
        private TextBox txtPassword;
        private TextBox txtUserName;
        private Label lblPassword;
        private Label lblUserName;
        private Label lblTitle;
        private TextBox txtEmail;
        private Label lblEmail;
        private TextBox txtFullName;
        private Label lblName;
    }
}