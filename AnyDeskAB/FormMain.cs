using AnyDeskAB.Classes;
using AnyDeskAB.Classes.AnyDesk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AnyDeskAB {
    public partial class FormMain : Form {
        private List<Group> groups;

        private string adConfigFileName;
        private string adTraceFileName;
        private string adExeFileName;
        private string adThumbnailsFolder;
        private string settingsFileName;
        private readonly int maxConfBackups = 10;
        private long lastConfigUpdate;

        private readonly System.Threading.Timer filterTimer;

        private TreeNode selectedNode;
        private bool ignoreTextBoxEvents = false;
        private bool isDragging = false;
        private bool abortThreads = false;
        private Point draggingLocation;
        TreeNode dragginOverNode;

        private readonly Thread draggingMonitor;
        private readonly FileSystemWatcher adConfigMonitor;
        private readonly FileSystemWatcher adThumbnailMonitor;

        public FormMain() {
            InitializeComponent();

            this.FormClosing += delegate {
                adConfigMonitor.Dispose();
                adThumbnailMonitor.Dispose();
                filterTimer.Dispose();
            };

            if(SetupPaths()) {
                LoadAddressBook();

                selectedNode = TreeViewItems.Nodes[0];
                selectedNode.Expand();
                HandleNodeSelected(selectedNode);

                adConfigMonitor = new FileSystemWatcher((new FileInfo(adConfigFileName)).Directory.FullName) {
                    Filter = "*.conf",
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                adThumbnailMonitor = new FileSystemWatcher(adThumbnailsFolder) {
                    Filter = "*.png",
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };

                SetupEventhandlers();

                draggingMonitor = new Thread(DraggingMonitorLoop) {
                    IsBackground = true
                };
                draggingMonitor.Start();

                filterTimer = new System.Threading.Timer(new TimerCallback((s) => this.Invoke((MethodInvoker)delegate { UpdateUI(); })), null, Timeout.Infinite, Timeout.Infinite);
            } else {
                // TODO: Inform the user which paths are invalid
                MessageBox.Show("Unable to initialize", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #region Configuration Management
        private void SaveSettings(bool updateConf) {
            string xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                            <settings>
                                <groups>{string.Concat(from Group g in groups select g.ToXML().ToString())}</groups>
                                <expandedNodes>{string.Concat(from en in Helpers.GetExpandedNodes(TreeViewItems.Nodes) select $"<node>{(new XText(en)).ToString()}</node>")}</expandedNodes>
                            </settings>";

            XDocument.Parse(xml).Save(settingsFileName);
            if(updateConf)
                UpdateUserConf();
            else
                UpdateUI();
        }

        private void UpdateUserConf() {
            string originalConfig = File.ReadAllText(adConfigFileName);
            StringBuilder sb = new StringBuilder();
            List<Item> items = groups[0].GetAllItems();

            foreach(string line in Helpers.ReadLines(adConfigFileName)) {
                if(line.Contains("ad.roster.items")) {
                    string data = "ad.roster.items=";
                    foreach(Item i in items) {
                        data += $"{i.Address},{i.Id},{i.Alias},;";
                    }
                    sb.AppendLine(data.TrimEnd(';'));
                } else {
                    sb.AppendLine(line);
                }
            }

            string newConfig = sb.ToString();
            if(originalConfig == newConfig) return;

            int n = 0;
            string bakFileName;
            while(true) {
                bakFileName = $"{adConfigFileName}-{n:00}.bak";
                if(File.Exists(bakFileName)) {
                    n += 1;
                    if(n == maxConfBackups) {
                        File.Delete($"{adConfigFileName}-00.bak");
                        for(n = 1; n < maxConfBackups; n++) {
                            File.Move($"{adConfigFileName}-{n:00}.bak", $"{adConfigFileName}-{n - 1:00}.bak");
                        }
                        break;
                    }
                    continue;
                } else
                    break;
            }
            File.Copy(adConfigFileName, bakFileName);
            File.WriteAllText(adConfigFileName, newConfig);
        }

        private void SetupEventhandlers() {
            adConfigMonitor.Changed += (object o, FileSystemEventArgs e) => {
                if(e.FullPath == adConfigFileName && (DateTime.Now.Ticks - lastConfigUpdate > 500000)) {
                    lastConfigUpdate = DateTime.Now.Ticks;
                    this.Invoke((MethodInvoker)delegate {
                        while(true) {
                            try {
                                LoadAddressBook();
                                break;
                            } catch(IOException) {
                                Thread.Sleep(500);
                            } catch(Exception ex) {
                                MessageBox.Show($"An error has occurred while trying to load AnyDesk's Address Book: {ex.Message}",
                                    "Error loading AnyDesk's Address Book",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                    });
                }
            };

            adThumbnailMonitor.Changed += (object o, FileSystemEventArgs e) => {
                this.Invoke((MethodInvoker)delegate {
                    TreeNode selectedItem = TreeViewItems.SelectedNode;
                    if(selectedNode != null) {
                        Item i = (Item)selectedNode.Tag;
                        if(e.Name.Contains(i.ThumbnailId)) {
                            while(true) {
                                try {
                                    PictureBoxThumbnail.Image = GetThumbnail(i.ThumbnailId);
                                    break;
                                } catch(Exception) {
                                    Thread.Sleep(500);
                                }
                            }
                        }
                    }
                });
            };

            TreeViewItems.MouseDoubleClick += (object o, MouseEventArgs e) => {
                TreeNode n = TreeViewItems.GetNodeAt(e.Location);
                if(n == null || n.Tag is Group) return;
                Connect();
            };
            TreeViewItems.MouseDown += (object o, MouseEventArgs e) => {
                TreeNode n = TreeViewItems.GetNodeAt(e.Location);
                if(n == null || e.Button != MouseButtons.Right) return;
                TreeViewItems.SelectedNode = n;
            };
            TreeViewItems.MouseUp += delegate { isDragging = false; };
            TreeViewItems.DragLeave += delegate { isDragging = false; };
            TreeViewItems.DragEnter += delegate { isDragging = true; };
            TreeViewItems.AfterSelect += delegate {
                selectedNode = TreeViewItems.SelectedNode;
                if(selectedNode == null) return;
                HandleNodeSelected(selectedNode);
            };
            TreeViewItems.AfterLabelEdit += (object o, NodeLabelEditEventArgs e) => {
                if(!string.IsNullOrEmpty(e.Label)) {
                    ((ADItem)e.Node.Tag).Name = e.Label;
                    LabelName.Text = e.Label;
                    SaveSettings(true);
                }
            };
            TreeViewItems.BeforeLabelEdit += (object o, NodeLabelEditEventArgs e) => e.CancelEdit = (e.Node.Parent == null);
            TreeViewItems.ItemDrag += (object o, ItemDragEventArgs e) => {
                selectedNode = (TreeNode)e.Item;
                if(e.Button == MouseButtons.Left) {
                    isDragging = true;
                    TreeViewItems.SelectedNode = selectedNode;
                    TreeViewItems.DoDragDrop(selectedNode, DragDropEffects.Move);
                }
            };
            TreeViewItems.DragOver += (object o, DragEventArgs e) => {
                draggingLocation = TreeViewItems.PointToClient(new Point(e.X, e.Y));
                dragginOverNode = TreeViewItems.GetNodeAt(draggingLocation);
                if(dragginOverNode == null || dragginOverNode.Tag is Item) {
                    e.Effect = DragDropEffects.None;
                } else {
                    e.Effect = e.AllowedEffect;
                }
            };
            TreeViewItems.DragDrop += (object o, DragEventArgs e) => {
                dragginOverNode = TreeViewItems.GetNodeAt(TreeViewItems.PointToClient(new Point(e.X, e.Y)));
                if(dragginOverNode != null) {
                    Group tg = (Group)dragginOverNode.Tag;
                    string nodeText = selectedNode.Text;

                    if(selectedNode.Tag is Item si) {
                        ((Group)si.Parent).Items.Remove(si);
                        tg.Items.Add((Item)si.Clone(tg));
                    } else {
                        Group sg = (Group)selectedNode.Tag;
                        ((Group)sg.Parent).Groups.Remove(sg);
                        tg.Groups.Add((Group)sg.Clone(sg));
                    }
                    SaveSettings(false);
                    SelectNode(TreeViewItems.Nodes, nodeText);
                }
            };

            ConnectToolStripMenuItem.Click += delegate { Connect(); };
            AddItemToolStripMenuItem.Click += delegate { AddItem(); };
            AddGroupToolStripMenuItem.Click += delegate { AddGroup(); };
            RenameToolStripMenuItem.Click += delegate { selectedNode.BeginEdit(); };
            DeleteToolStripMenuItem.Click += delegate { DeleteItem(); };

            TextBoxDescription.TextChanged += delegate { if(!ignoreTextBoxEvents) ((Item)selectedNode.Tag).Description = TextBoxDescription.Text; };
            LinkLabelConnect.Click += delegate { Connect(); };

            this.FormClosing += delegate {
                abortThreads = true;
                while(draggingMonitor.ThreadState == System.Threading.ThreadState.Background) {
                    Thread.Sleep(100);
                }
                adConfigMonitor.Dispose();
                SaveSettings(true);
            };
            this.KeyDown += (object o, KeyEventArgs e) => {
                switch(e.KeyCode) {
                    case Keys.F2:
                        if(this.ActiveControl == TreeViewItems) selectedNode?.BeginEdit();
                        break;
                    case Keys.Enter:
                        Connect();
                        // Avoid the freaking bell...
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Delete:
                        if(this.ActiveControl == TreeViewItems) DeleteItem();
                        break;
                }
            };

            TextBoxFilter.TextChanged += (object o, EventArgs e) => filterTimer.Change(500, Timeout.Infinite);
        }

        void HandleNodeSelected(TreeNode n) {
            TreeViewItems.SelectedNode = n;

            if(n.Tag is Group) {
                ConnectToolStripMenuItem.Visible = false;
                Sep01toolStripMenuItem.Visible = false;
                DeleteToolStripMenuItem.Enabled = (n.Parent != null);
            } else {
                ConnectToolStripMenuItem.Visible = true;
                Sep01toolStripMenuItem.Visible = true;
                DeleteToolStripMenuItem.Enabled = true;
            }
            RenameToolStripMenuItem.Enabled = n.Parent != null;

            UpdateDetails(n);
        }

        private void LoadAddressBook() {
            groups = new List<Group>();
            Group adg = new Group(null, "");
            List<string> expandedNodes = new List<string>();

            foreach(string line in Helpers.ReadLines(adConfigFileName)) {
                if(line.Contains("ad.roster.items")) {
                    foreach(string token in line.Split('=')[1].Split(';')) {
                        string[] tokens = token.Split(',');
                        adg.Items.Add(new Item(null, tokens[1],
                                                     tokens[0],
                                                     tokens[2],
                                                     GetThumbnailId(tokens[0])));
                    }
                    break;
                }
            }

            if(File.Exists(settingsFileName)) {
                XDocument xDoc = XDocument.Load(settingsFileName);
                groups.Add(Group.FromXML(null, xDoc.Descendants("adItem").First()));

                Group rg = groups[0];

                // Remove any items that may have been removed from AnyDesk
                bool isDone = false;
                do {
                    isDone = true;
                    foreach(Item i in rg.GetAllItems()) {
                        if(!adg.ItemExists(i)) {
                            ((Group)i.Parent).Items.Remove(i);
                            isDone = false;
                            break;
                        }
                    }
                } while(!isDone);

                // Add any new items that may have been added from AnyDesk
                List<Item> rgis = rg.GetAllItems();
                foreach(Item i in adg.Items) if(!rg.ItemExists(i)) rg.Items.Add((Item)i.Clone(rg));

                // Update any items that may have been changed from AnyDesk
                foreach(Item i in rg.GetAllItems()) {
                    i.Alias = adg.FindItem(i).Alias;
                    i.ThumbnailId = adg.FindItem(i).ThumbnailId;
                }

                // Restore expanded nodes
                foreach(XElement xml in xDoc.Descendants("node")) expandedNodes.Add(xml.Value);

                // Remove items from the main 'AnyDesk' group that are already present in sub-groups
                rg.Groups.ForEach(g => RemoveItems(rg, g));
            }

            if(!groups.Any()) {
                groups.Add(new Group(null, "AnyDesk"));
                Group g = groups[0];
                foreach(Item i in adg.Items) g.Items.Add((Item)i.Clone(g));
            }

            UpdateUI();
            Helpers.SetExpandedNodes(TreeViewItems.Nodes, expandedNodes);
        }

        private string GetThumbnailId(string id) {
            string line;
            using(FileStream fs = new FileStream(adTraceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using(StreamReader sr = new StreamReader(fs)) {
                    while((line = sr.ReadLine()) != null) {
                        if(line.Contains($"Sending a connection request for address {id}")) {
                            while((line = sr.ReadLine()) != null) {
                                if(line.Contains($"Saved thumbnail")) {
                                    string[] tokens = line.Split('\\');
                                    return tokens[tokens.Length - 1].TrimEnd('.').Split('.')[0];
                                } else if(line.Contains("Sending a connection") || line.Contains("Stopping")) {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }

        private void RemoveItems(Group parent, Group child) {
            foreach(Group g in child.Groups) RemoveItems(parent, g);
            foreach(Item i in child.Items) if(parent.ItemExists(i)) parent.Items.Remove(i);
        }

        private void RestoreItems(Group child) {
            foreach(Group g in child.Groups) RestoreItems(g);
            foreach(Item i in child.Items) if(!groups[0].ItemExists(i)) groups[0].Items.Add(i);
        }

        private bool SetupPaths() {
            settingsFileName = Path.Combine((new DirectoryInfo(Application.UserAppDataPath)).Parent.FullName, "settings.dat");
            adConfigFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\AnyDesk\user.conf");
            adThumbnailsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\AnyDesk\thumbnails");
            adTraceFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\AnyDesk\ad.trace");
            adExeFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"AnyDesk\AnyDesk.exe");

            // Is the user like me and likes to install programs on a drive other that C:?
            foreach(DriveInfo di in DriveInfo.GetDrives()) {
                adExeFileName = di.Name.Substring(0, 1) + adExeFileName.Substring(1);
                if(File.Exists(adExeFileName)) break;
            }

            return File.Exists(adConfigFileName) &&
                   File.Exists(adExeFileName) &&
                   File.Exists(adTraceFileName) &&
                   Directory.Exists(adThumbnailsFolder);
        }
        #endregion

        #region TreeView Management
        private void UpdateUI() {
            List<string> expandedNodes = Helpers.GetExpandedNodes(TreeViewItems.Nodes);

            // Dirty trick to prevent flickering
            this.BackgroundImage = new Bitmap(TreeViewItems.Width, TreeViewItems.Height);
            TreeViewItems.DrawToBitmap((Bitmap)this.BackgroundImage, new Rectangle(TreeViewItems.Location, TreeViewItems.Size));

            TreeViewItems.Visible = false;
            TreeViewItems.BeginUpdate();
            TreeViewItems.Nodes.Clear();
            foreach(Group g in groups.OrderBy(grp => grp.Name)) AddGroupNode(g, null);

            if(selectedNode != null) SelectNode(TreeViewItems.Nodes, selectedNode.Text);
            Helpers.SetExpandedNodes(TreeViewItems.Nodes, expandedNodes);

            TreeViewItems.EndUpdate();
            TreeViewItems.Visible = true;
            this.BackgroundImage = null;
        }

        private void UpdateDetails(TreeNode n) {
            ignoreTextBoxEvents = true;
            LabelName.Text = n.Text;
            if(n.Tag is Item i) {
                LabelID.Text = i.Id;
                LinkLabelConnect.Text = i.Address;
                LabelAlias.Text = i.Alias;
                TextBoxDescription.Text = i.Description;
                TextBoxDescription.Visible = true;
                PictureBoxThumbnail.Image = GetThumbnail(i.ThumbnailId);
                PictureBoxThumbnail.Visible = true;
            } else {
                LabelID.Text = "";
                LinkLabelConnect.Text = "";
                LabelAlias.Text = "";
                TextBoxDescription.Visible = false;
                PictureBoxThumbnail.Visible = false;
            }
            ignoreTextBoxEvents = false;
        }

        private Image GetThumbnail(string thumbnailId) {
            string tn = Path.Combine(adThumbnailsFolder, thumbnailId) + ".png";
            if(File.Exists(tn)) {
                Image img = Image.FromFile(tn);
                Bitmap bmp = new Bitmap(img.Width, img.Height);
                using(Graphics g = Graphics.FromImage(bmp)) {
                    g.DrawImageUnscaled(img, 0, 0);
                }
                img.Dispose();
                return bmp;
            } else {
                return null;
            }
        }

        private TreeNode AddGroupNode(Group g, TreeNode parentNode) {
            string filter = TextBoxFilter.Text;

            TreeNode n = new TreeNode(g.Name) {
                NodeFont = new Font(this.Font, FontStyle.Bold),
                Tag = g
            };

            if(parentNode == null) {
                TreeViewItems.Nodes.Add(n);
            } else {
                parentNode.Nodes.Add(n);
            }

            TreeNode ntn;
            foreach(Item i in g.Items.OrderBy(it => it.Name)) {
                foreach(TreeNode tn in TreeViewItems.Nodes) {
                    if((tn.Tag is Item) && ((Item)tn.Tag) == i) {
                        Debugger.Break();
                    }
                }

                if(i.ToString().ToLower().Contains(filter)) {
                    ntn = n.Nodes.Add(i.ToString());
                    ntn.Tag = i;
                }
            }

            foreach(Group sg in g.Groups.OrderBy(sgt => sgt.Name)) AddGroupNode(sg, n);

            return n;
        }

        private void SelectNode(TreeNodeCollection nodes, string nodeText) {
            foreach(TreeNode n in nodes) {
                if(n.Text == nodeText) {
                    n.EnsureVisible();
                    TreeViewItems.SelectedNode = n;
                    selectedNode = n;
                    break;
                }
                SelectNode(n.Nodes, nodeText);
            }
        }

        private void DraggingMonitorLoop() {
            const int scrollMargin = 8;
            TreeNode lastOver = null;
            int overItemCount = 0;
            while(!abortThreads) {
                if(isDragging && dragginOverNode != null) {
                    try {
                        if(draggingLocation.Y <= scrollMargin) {
                            TreeViewItems.ScrollUp();
                        } else if(draggingLocation.Y >= TreeViewItems.Height - scrollMargin) {
                            TreeViewItems.ScrollDown();
                        } else if(lastOver != dragginOverNode) {
                            this.Invoke((MethodInvoker)delegate {
                                if(dragginOverNode.Nodes.Count > 0 && !dragginOverNode.IsExpanded) {
                                    lastOver = dragginOverNode;
                                    overItemCount = 0;
                                } else {
                                    lastOver = null;
                                }
                            });
                        } else if(lastOver == dragginOverNode && lastOver != null && ++overItemCount >= 10) {
                            this.Invoke((MethodInvoker)delegate {
                                dragginOverNode.Expand();
                                lastOver = null;
                            });
                        }
                    } catch { }
                }
                Thread.Sleep(100);
            }
        }
        #endregion

        #region User Actions
        private void AddItem() {
            MessageBox.Show("Not Implemented", "Add Item", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddGroup() {
            if(selectedNode.Tag is Item) selectedNode = selectedNode.Parent;
            Group pg = ((Group)selectedNode.Tag);
            Group ng = new Group(pg, "<New Group>");
            pg.Groups.Add(ng);
            selectedNode = AddGroupNode(ng, selectedNode);
            TreeViewItems.SelectedNode = selectedNode;
            selectedNode.Expand();
            selectedNode.BeginEdit();
        }

        private void DeleteItem() {
            string nodeType;
            string extra = "";
            if(selectedNode.Tag is Item) {
                nodeType = "Connection Item";
            } else {
                nodeType = "Group";
                extra = $"{Environment.NewLine}{Environment.NewLine}Items under the {nodeType} '{selectedNode.Text}' will be moved to the root {nodeType} 'AnyDesk'";
            }
            DialogResult ans = MessageBox.Show($"Are you sure you want to delete the {nodeType} '{selectedNode.Text}'?{extra}",
                                                "Delete Confirmation",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if(ans == DialogResult.Yes) {
                if(selectedNode.Tag is Item i) {
                    Group g = ((Group)i.Parent);
                    if(g != groups[0]) groups[0].Items.Add((Item)i.Clone(i.Parent));
                    g.Items.Remove(i);
                } else {
                    Group g = (Group)selectedNode.Tag;
                    RestoreItems(g);
                    ((Group)g.Parent).Groups.Remove(g);
                }
                SaveSettings(true);
                UpdateUI();
            }
        }

        private void Connect() {
            if(selectedNode?.Tag is Item i) {
                Process.Start(adExeFileName, i.Address);
            }
        }
        #endregion
    }
}