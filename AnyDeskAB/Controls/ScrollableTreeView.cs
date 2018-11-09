using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AnyDeskAB.Controls {
    public partial class ScrollableTreeView : TreeView {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private IntPtr myHandle;

        public ScrollableTreeView() {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.HandleCreated += delegate { myHandle = this.Handle; };
        }

        public void ScrollUp() {
            SendMessage(myHandle, 277, (IntPtr)0, (IntPtr)0);
        }

        public void ScrollDown() {
            SendMessage(myHandle, 277, (IntPtr)1, (IntPtr)0);
        }
    }
}
