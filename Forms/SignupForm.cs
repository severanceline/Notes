using Noots.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Noots.Forms
{
    public partial class SignupForm : Form
    {
        private UserRepository _userRepository;
        public SignupForm()
        {
            _userRepository = new UserRepository();
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
        }

        private void SignupForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            SigninForm form = new SigninForm();
            form.Show();
            this.Hide();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            var result = _userRepository.RegisterUser(username, email, password, fullName);

            if (result.Success)
            {
                MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SigninForm form = new SigninForm();
                form.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
