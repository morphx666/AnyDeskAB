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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.treeViewItems = new AnyDeskAB.Controls.ScrollableTreeView();
            this.contextMenuStripOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep01toolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep02ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sep03ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.linkLabelConnect = new System.Windows.Forms.LinkLabel();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelAlias = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.contextMenuStripOptions.SuspendLayout();
            this.panelDetails.SuspendLayout();
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
            this.treeViewItems.Size = new System.Drawing.Size(453, 466);
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
            // panelDetails
            // 
            this.panelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetails.Controls.Add(this.linkLabelConnect);
            this.panelDetails.Controls.Add(this.textBoxDescription);
            this.panelDetails.Controls.Add(this.labelAlias);
            this.panelDetails.Controls.Add(this.labelID);
            this.panelDetails.Controls.Add(this.labelName);
            this.panelDetails.Location = new System.Drawing.Point(471, 14);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(3);
            this.panelDetails.Size = new System.Drawing.Size(269, 466);
            this.panelDetails.TabIndex = 1;
            // 
            // linkLabelConnect
            // 
            this.linkLabelConnect.AutoSize = true;
            this.linkLabelConnect.Location = new System.Drawing.Point(7, 41);
            this.linkLabelConnect.Name = "linkLabelConnect";
            this.linkLabelConnect.Size = new System.Drawing.Size(105, 17);
            this.linkLabelConnect.TabIndex = 5;
            this.linkLabelConnect.TabStop = true;
            this.linkLabelConnect.Text = "linkLabelConnect";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(10, 78);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(253, 135);
            this.textBoxDescription.TabIndex = 4;
            // 
            // labelAlias
            // 
            this.labelAlias.AutoSize = true;
            this.labelAlias.Location = new System.Drawing.Point(7, 58);
            this.labelAlias.Name = "labelAlias";
            this.labelAlias.Size = new System.Drawing.Size(63, 17);
            this.labelAlias.TabIndex = 3;
            this.labelAlias.Text = "labelAlias";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Location = new System.Drawing.Point(7, 24);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(48, 17);
            this.labelID.TabIndex = 1;
            this.labelID.Text = "labelID";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(6, 3);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(94, 21);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "labelName";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(752, 494);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.treeViewItems);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AnyDesk AddressBook";
            this.contextMenuStripOptions.ResumeLayout(false);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AnyDeskAB.Controls.ScrollableTreeView treeViewItems;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOptions;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep01toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep02ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sep03ToolStripMenuItem;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Label labelAlias;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.LinkLabel linkLabelConnect;
    }
}

