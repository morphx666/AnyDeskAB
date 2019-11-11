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
            this.ContextMenuStripOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Sep01toolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.AddGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Sep02ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.RenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Sep03ToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelDetails = new System.Windows.Forms.Panel();
            this.PictureBoxThumbnail = new System.Windows.Forms.PictureBox();
            this.LinkLabelConnect = new System.Windows.Forms.LinkLabel();
            this.TextBoxDescription = new System.Windows.Forms.TextBox();
            this.LabelAlias = new System.Windows.Forms.Label();
            this.LabelID = new System.Windows.Forms.Label();
            this.LabelName = new System.Windows.Forms.Label();
            this.TextBoxFilter = new System.Windows.Forms.TextBox();
            this.TreeViewItems = new AnyDeskAB.Controls.ScrollableTreeView();
            this.ContextMenuStripOptions.SuspendLayout();
            this.PanelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // ContextMenuStripOptions
            // 
            this.ContextMenuStripOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripMenuItem,
            this.Sep01toolStripMenuItem,
            this.AddGroupToolStripMenuItem,
            this.AddItemToolStripMenuItem,
            this.Sep02ToolStripMenuItem,
            this.RenameToolStripMenuItem,
            this.Sep03ToolStripMenuItem,
            this.DeleteToolStripMenuItem});
            this.ContextMenuStripOptions.Name = "contextMenuStripGroups";
            this.ContextMenuStripOptions.Size = new System.Drawing.Size(133, 132);
            // 
            // ConnectToolStripMenuItem
            // 
            this.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem";
            this.ConnectToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.ConnectToolStripMenuItem.Text = "Connect...";
            // 
            // Sep01toolStripMenuItem
            // 
            this.Sep01toolStripMenuItem.Name = "Sep01toolStripMenuItem";
            this.Sep01toolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // AddGroupToolStripMenuItem
            // 
            this.AddGroupToolStripMenuItem.Name = "AddGroupToolStripMenuItem";
            this.AddGroupToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.AddGroupToolStripMenuItem.Text = "Add Group";
            // 
            // AddItemToolStripMenuItem
            // 
            this.AddItemToolStripMenuItem.Name = "AddItemToolStripMenuItem";
            this.AddItemToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.AddItemToolStripMenuItem.Text = "Add Item";
            // 
            // Sep02ToolStripMenuItem
            // 
            this.Sep02ToolStripMenuItem.Name = "Sep02ToolStripMenuItem";
            this.Sep02ToolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // RenameToolStripMenuItem
            // 
            this.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem";
            this.RenameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.RenameToolStripMenuItem.Text = "Rename...";
            // 
            // Sep03ToolStripMenuItem
            // 
            this.Sep03ToolStripMenuItem.Name = "Sep03ToolStripMenuItem";
            this.Sep03ToolStripMenuItem.Size = new System.Drawing.Size(129, 6);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.DeleteToolStripMenuItem.Text = "Delete";
            // 
            // PanelDetails
            // 
            this.PanelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelDetails.Controls.Add(this.PictureBoxThumbnail);
            this.PanelDetails.Controls.Add(this.LinkLabelConnect);
            this.PanelDetails.Controls.Add(this.TextBoxDescription);
            this.PanelDetails.Controls.Add(this.LabelAlias);
            this.PanelDetails.Controls.Add(this.LabelID);
            this.PanelDetails.Controls.Add(this.LabelName);
            this.PanelDetails.Location = new System.Drawing.Point(471, 14);
            this.PanelDetails.Name = "PanelDetails";
            this.PanelDetails.Padding = new System.Windows.Forms.Padding(3);
            this.PanelDetails.Size = new System.Drawing.Size(269, 466);
            this.PanelDetails.TabIndex = 1;
            // 
            // PictureBoxThumbnail
            // 
            this.PictureBoxThumbnail.BackColor = System.Drawing.SystemColors.Control;
            this.PictureBoxThumbnail.Location = new System.Drawing.Point(10, 219);
            this.PictureBoxThumbnail.Name = "PictureBoxThumbnail";
            this.PictureBoxThumbnail.Size = new System.Drawing.Size(128, 128);
            this.PictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PictureBoxThumbnail.TabIndex = 6;
            this.PictureBoxThumbnail.TabStop = false;
            // 
            // LinkLabelConnect
            // 
            this.LinkLabelConnect.AutoSize = true;
            this.LinkLabelConnect.Location = new System.Drawing.Point(7, 41);
            this.LinkLabelConnect.Name = "LinkLabelConnect";
            this.LinkLabelConnect.Size = new System.Drawing.Size(108, 17);
            this.LinkLabelConnect.TabIndex = 5;
            this.LinkLabelConnect.TabStop = true;
            this.LinkLabelConnect.Text = "LinkLabelConnect";
            // 
            // TextBoxDescription
            // 
            this.TextBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxDescription.Location = new System.Drawing.Point(10, 78);
            this.TextBoxDescription.Multiline = true;
            this.TextBoxDescription.Name = "TextBoxDescription";
            this.TextBoxDescription.Size = new System.Drawing.Size(253, 135);
            this.TextBoxDescription.TabIndex = 4;
            // 
            // LabelAlias
            // 
            this.LabelAlias.AutoSize = true;
            this.LabelAlias.Location = new System.Drawing.Point(7, 58);
            this.LabelAlias.Name = "LabelAlias";
            this.LabelAlias.Size = new System.Drawing.Size(66, 17);
            this.LabelAlias.TabIndex = 3;
            this.LabelAlias.Text = "LabelAlias";
            // 
            // LabelID
            // 
            this.LabelID.AutoSize = true;
            this.LabelID.Location = new System.Drawing.Point(7, 24);
            this.LabelID.Name = "LabelID";
            this.LabelID.Size = new System.Drawing.Size(51, 17);
            this.LabelID.TabIndex = 1;
            this.LabelID.Text = "LabelID";
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelName.Location = new System.Drawing.Point(6, 3);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(97, 21);
            this.LabelName.TabIndex = 0;
            this.LabelName.Text = "LabelName";
            // 
            // TextBoxFilter
            // 
            this.TextBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TextBoxFilter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TextBoxFilter.Location = new System.Drawing.Point(12, 455);
            this.TextBoxFilter.Name = "TextBoxFilter";
            this.TextBoxFilter.Size = new System.Drawing.Size(453, 25);
            this.TextBoxFilter.TabIndex = 2;
            // 
            // TreeViewItems
            // 
            this.TreeViewItems.AllowDrop = true;
            this.TreeViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeViewItems.ContextMenuStrip = this.ContextMenuStripOptions;
            this.TreeViewItems.HideSelection = false;
            this.TreeViewItems.LabelEdit = true;
            this.TreeViewItems.Location = new System.Drawing.Point(12, 14);
            this.TreeViewItems.Name = "TreeViewItems";
            this.TreeViewItems.Size = new System.Drawing.Size(453, 435);
            this.TreeViewItems.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(752, 494);
            this.Controls.Add(this.TextBoxFilter);
            this.Controls.Add(this.PanelDetails);
            this.Controls.Add(this.TreeViewItems);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AnyDesk AddressBook";
            this.ContextMenuStripOptions.ResumeLayout(false);
            this.PanelDetails.ResumeLayout(false);
            this.PanelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxThumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AnyDeskAB.Controls.ScrollableTreeView TreeViewItems;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStripOptions;
        private System.Windows.Forms.ToolStripMenuItem AddGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator Sep01toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator Sep02ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator Sep03ToolStripMenuItem;
        private System.Windows.Forms.Panel PanelDetails;
        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.Label LabelID;
        private System.Windows.Forms.Label LabelAlias;
        private System.Windows.Forms.TextBox TextBoxDescription;
        private System.Windows.Forms.LinkLabel LinkLabelConnect;
        private System.Windows.Forms.TextBox TextBoxFilter;
        private System.Windows.Forms.PictureBox PictureBoxThumbnail;
    }
}

