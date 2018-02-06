using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace z0r_scr
{
    public partial class scrForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        private static extern bool NativeGetKeyboardState([Out] byte[] keyStates);

        private static bool GetKeyboardState(byte[] keyStates)
        {
            if (keyStates == null)
                throw new ArgumentNullException("keyState");
            if (keyStates.Length != 256)
                throw new ArgumentException("The buffer must be 256 bytes long.", "keyState");
            return NativeGetKeyboardState(keyStates);
        }

        private static byte[] GetKeyboardState()
        {
            byte[] keyStates = new byte[256];
            if (!GetKeyboardState(keyStates))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return keyStates;
        }

        private static bool AnyKeyPressed()
        {
            byte[] keyState = GetKeyboardState();
            // skip the mouse buttons
            return keyState.Skip(8).Any(state => (state & 0x80) != 0);
        }

        private Point mouseLocation;
        private bool previewMode = false;

        public scrForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
        }

        public scrForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            previewMode = true;
        }

        private void scrForm_Load(object sender, EventArgs e)
        {
            if (!previewMode)
            {
                Cursor.Hide();
                TopMost = true;
            }
            ((Control)webBrowser1).Enabled = false;
            RandomLoop();
        }

        private void RandomLoop()
        {
            int loopcount = int.Parse(SettingsForm.LoadSettings());
            int loopnumber = new Random().Next(1, loopcount);
            webBrowser1.Navigate("http://dva.z0r.de/L/z0r-de_" + loopnumber + ".swf");
            linklabel.Text = "zor.de/" + loopnumber;
        }

        private void scrForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode && !mouseLocation.IsEmpty)
            {
                // Terminate if mouse is moved a significant distance
                if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                    Math.Abs(mouseLocation.Y - e.Y) > 5)
                    Application.Exit();
            }

            // Update current mouse location
            mouseLocation = e.Location;
        }

        private void scrForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void scrForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }
    }
}
