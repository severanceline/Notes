using DocumentFormat.OpenXml.Spreadsheet;
using Notes.DataAccess;
using Notes.Session;
using Notes.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notes.Forms
{
    public partial class SigninForm : Form
    {
        private UserRepository _userRepository;
        public SigninForm()
        {
            _userRepository = new UserRepository();
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SignupForm form = new SignupForm();
            this.Hide();
            form.Show();
            form.FormClosed += MainFormClosed;       
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string usernameOrEmail = txtUserName.Text.Trim();
            string password = txtPassword.Text;

            var result = _userRepository.LoginUser(usernameOrEmail, password);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            MessageBox.Show(
                "Welcome " + result.LoggedInUser.Username + "!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            if (result.Success)
            {
                UserSession.Login(result.LoggedInUser);

                LoginStorage.SaveUser(result.LoggedInUser.Id);
            }
            // باز کردن فرم اصلی برنامه
            MainForm mainForm = new MainForm();
            this.Hide(); 
            mainForm.Show();
            mainForm.FormClosed += MainFormClosed;
        }
        private void MainFormClosed(object s, FormClosedEventArgs args)
        {
            Application.Exit();
        }
    }
}
