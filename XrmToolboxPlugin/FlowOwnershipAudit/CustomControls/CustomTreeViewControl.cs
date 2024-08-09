using System;
using System.Windows.Forms;

namespace WhatBreaksIf
{
    // overriden TreeView to avoid a windows bug with doubleclicking checkboxes
    // https://stackoverflow.com/questions/6130297/c-how-to-avoid-treenode-check-from-happening-on-a-double-click-event
    internal class CustomTreeViewControl : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            // Suppress WM_LBUTTONDBLCLK
            if (m.Msg == 0x203) { m.Result = IntPtr.Zero; }
            else base.WndProc(ref m);
        }
    }
}
