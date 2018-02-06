using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace z0r_scr
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            textBox1.Text = LoadSettings();
        }

        private void SaveSettings()
        {
            // Create or get existing Registry subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\z0r_Screensaver");
            key.SetValue("loopcount", textBox1.Text);
        }

        public static string LoadSettings()
        {
            // Get the value stored in the Registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\z0r_Screensaver");
            if (key == null)
                return "7911";
            else
                return (string)key.GetValue("loopcount");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }
    }
}
