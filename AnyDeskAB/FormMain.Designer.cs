namespace AnyDeskAB {
    partial class FormMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.treeViewItems = new System.Windows.Forms.TreeView();
            this.contextMenuStripOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep01toolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep02ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep03ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewItems
            // 
            this.treeViewItems.AllowDrop = true;
            this.treeViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewItems.ContextMenuStrip = this.contextMenuStripOptions;
            this.treeViewItems.HideSelection = false;
            this.treeViewItems.LabelEdit = true;
            this.treeViewItems.Location = new System.Drawing.Point(12, 14);
            this.treeViewItems.Name = "treeViewItems";
            this.treeViewItems.Size = new System.Drawing.Size(548, 466);
            this.treeViewItems.TabIndex = 0;
            // 
            // contextMenuStripOptions
            // 
            this.contextMenuStripOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.sep01toolStripMenuItem,
            this.addGroupToolStripMenuItem,
            this.addItemToolStripMenuItem,
            this.sep02ToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.sep03ToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStripOptions.Name = "contextMenuStripGroups";
            this.contextMenuStripOptions.Size = new System.Drawing.Size(133, 132);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.connectToolStripMenuItem.Text = "Connect...";
            // 
            // sep01toolStripMenuItem
            // 
            this.sep01toolStripMenuItem.Name = "sep01toolStripMenuItem";
            this.sep01toolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.addGroupToolStripMenuItem.Text = "Add Group";
            // 
            // addItemToolStripMenuItem
            // 
            this.addItemToolStripMenuItem.Name = "addItemToolStripMenuItem";
            this.addItemToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.addItemToolStripMenuItem.Text = "Add Item";
            // 
            // sep02ToolStripMenuItem
            // 
            this.sep02ToolStripMenuItem.Name = "sep02ToolStripMenuItem";
            this.sep02ToolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.renameToolStripMenuItem.Text = "Rename...";
            // 
            // sep03ToolStripMenuItem
            // 
            this.sep03ToolStripMenuItem.Name = "sep03ToolStripMenuItem";
            this.sep03ToolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(572, 494);
            this.Controls.Add(this.treeViewItems);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AnyDesk AddressBook";
            this.contextMenuStripOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewItems;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOptions;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep01toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep02ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep03ToolStripMenuItem;
    }
}

