using AnyDeskAB.Classes;
using AnyDeskAB.Classes.AnyDesk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AnyDeskAB {
    public partial class FormMain : Form {
        private List<Group> groups;

        private string adConfigFileName;
        private string adExeFileName;
        private string settingsFileName;
        private int maxConfBackups = 10;
        private long lastConfigUpdate;

        private System.Threading.Timer filterTimer;

        private TreeNode selectedNode;
        private bool ignoreTextBoxEvents = false;
        private bool isDragging = false;
        private bool abortThreads = false;
        private Point draggingLocation;
        TreeNode dragginOverNode;

        private Thread draggingMonitor;

        FileSystemWatcher adConfigMonitor;

        public FormMain() {
            InitializeComponent();

            // TODO: Inform the user some paths are invalid and quit the program
            if(SetupPaths()) {
                LoadAddressBook();

                selectedNode = treeViewItems.Nodes[0];
                selectedNode.Expand();
                HandleNodeSelected(selectedNode);

                adConfigMonitor = new FileSystemWatcher((new FileInfo(adConfigFileName)).Directory.FullName) {
                    Filter = "*.conf",
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
                MessageBox.Show("Unable to initialize", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #region Configuration Management
        private void SaveSettings(bool updateConf) {
            string xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                            <settings>
                                <groups>{string.Concat(from Group g in groups select g.ToXML().ToString())}</groups>
                                <expandedNodes>{string.Concat(from en in Helpers.GetExpandedNodes(treeViewItems.Nodes) select $"<node>{en}</node>")}</expandedNodes>
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
                    this.Invoke((MethodInvoker)delegate { LoadAddressBook(); });
                }
            };

            treeViewItems.MouseDoubleClick += (object o, MouseEventArgs e) => {
                TreeNode n = treeViewItems.GetNodeAt(e.Location);
                if(n == null || n.Tag is Group) return;
                Connect();
            };
            treeViewItems.MouseDown += (object o, MouseEventArgs e) => {
                TreeNode n = treeViewItems.GetNodeAt(e.Location);
                if(n == null || e.Button != MouseButtons.Right) return;
                treeViewItems.SelectedNode = n;
            };
            treeViewItems.MouseUp += delegate { isDragging = false; };
            treeViewItems.DragLeave += delegate { isDragging = false; };
            treeViewItems.DragEnter += delegate { isDragging = true; };
            treeViewItems.AfterSelect += delegate {
                selectedNode = treeViewItems.SelectedNode;

                if(selectedNode == null) return;
                HandleNodeSelected(selectedNode);
            };
            treeViewItems.AfterLabelEdit += (object o, NodeLabelEditEventArgs e) => {
                if(e.Label != null && e.Label != "") {
                    ((ADItem)e.Node.Tag).Name = e.Label;
                    SaveSettings(true);
                }
            };
            treeViewItems.BeforeLabelEdit += (object o, NodeLabelEditEventArgs e) => e.CancelEdit = (e.Node.Parent == null);
            treeViewItems.ItemDrag += (object o, ItemDragEventArgs e) => {
                selectedNode = (TreeNode)e.Item;
                if(e.Button == MouseButtons.Left) {
                    isDragging = true;
                    treeViewItems.SelectedNode = selectedNode;
                    treeViewItems.DoDragDrop(selectedNode, DragDropEffects.Move);
                }
            };
            treeViewItems.DragOver += (object o, DragEventArgs e) => {
                draggingLocation = treeViewItems.PointToClient(new Point(e.X, e.Y));
                dragginOverNode = treeViewItems.GetNodeAt(draggingLocation);
                if(dragginOverNode == null || dragginOverNode.Tag is Item) {
                    e.Effect = DragDropEffects.None;
                } else {
                    e.Effect = e.AllowedEffect;
                }
            };
            treeViewItems.DragDrop += (object o, DragEventArgs e) => {
                dragginOverNode = treeViewItems.GetNodeAt(treeViewItems.PointToClient(new Point(e.X, e.Y)));
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
                    SelectNode(treeViewItems.Nodes, nodeText);
                }
            };

            connectToolStripMenuItem.Click += delegate { Connect(); };
            addItemToolStripMenuItem.Click += delegate { AddItem(); };
            addGroupToolStripMenuItem.Click += delegate { AddGroup(); };
            renameToolStripMenuItem.Click += delegate { selectedNode.BeginEdit(); };
            deleteToolStripMenuItem.Click += delegate { DeleteItem(); };

            textBoxDescription.TextChanged += delegate { if(!ignoreTextBoxEvents) ((Item)selectedNode.Tag).Description = textBoxDescription.Text; };
            linkLabelConnect.Click += delegate { Connect(); };

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
                        if(this.ActiveControl == treeViewItems) selectedNode?.BeginEdit();
                        break;
                    case Keys.Enter:
                        Connect();
                        // Avoid the freaking bell...
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Delete:
                        if(this.ActiveControl == treeViewItems) DeleteItem();
                        break;
                }
            };

            textBoxFilter.TextChanged += (object o, EventArgs e) => filterTimer.Change(500, Timeout.Infinite);
        }

        void HandleNodeSelected(TreeNode n) {
            treeViewItems.SelectedNode = n;

            if(n.Tag is Group) {
                connectToolStripMenuItem.Visible = false;
                sep01toolStripMenuItem.Visible = false;
                deleteToolStripMenuItem.Enabled = (n.Parent != null);
            } else {
                connectToolStripMenuItem.Visible = true;
                sep01toolStripMenuItem.Visible = true;
                deleteToolStripMenuItem.Enabled = true;
            }
            renameToolStripMenuItem.Enabled = n.Parent != null;

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
                        adg.Items.Add(new Item(null, tokens[1], tokens[0], tokens[2]));
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
                foreach(Item i in rg.GetAllItems()) i.Alias = adg.FindItem(i).Alias;

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
            Helpers.SetExpandedNodes(treeViewItems.Nodes, expandedNodes);
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
            adExeFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"AnyDesk\AnyDesk.exe");

            // Is the user like me and likes to install programs on a drive other that C:?
            foreach(DriveInfo di in DriveInfo.GetDrives()) {
                adExeFileName = di.Name.Substring(0, 1) + adExeFileName.Substring(1);
                if(File.Exists(adExeFileName)) break;
            }

            return File.Exists(adConfigFileName) && File.Exists(adExeFileName);
        }
        #endregion

        #region TreeView Management
        private void UpdateUI() {
            List<string> expandedNodes = Helpers.GetExpandedNodes(treeViewItems.Nodes);

            // Dirty trick to prevent flickering
            this.BackgroundImage = new Bitmap(treeViewItems.Width, treeViewItems.Height);
            treeViewItems.DrawToBitmap((Bitmap)this.BackgroundImage, new Rectangle(treeViewItems.Location, treeViewItems.Size));

            treeViewItems.Visible = false;
            treeViewItems.BeginUpdate();
            treeViewItems.Nodes.Clear();
            foreach(Group g in groups.OrderBy(grp => grp.Name)) AddGroupNode(g, null);

            if(selectedNode != null) SelectNode(treeViewItems.Nodes, selectedNode.Text);
            Helpers.SetExpandedNodes(treeViewItems.Nodes, expandedNodes);

            treeViewItems.EndUpdate();
            treeViewItems.Visible = true;
            this.BackgroundImage = null;
        }

        private void UpdateDetails(TreeNode n) {
            ignoreTextBoxEvents = true;
            labelName.Text = n.Text;
            if(n.Tag is Item i) {
                labelID.Text = i.Id;
                linkLabelConnect.Text = i.Address;
                labelAlias.Text = i.Alias;
                textBoxDescription.Text = i.Description;
                textBoxDescription.Visible = true;
            } else {
                labelID.Text = "";
                linkLabelConnect.Text = "";
                labelAlias.Text = "";
                textBoxDescription.Visible = false;
            }
            ignoreTextBoxEvents = false;
        }

        private TreeNode AddGroupNode(Group g, TreeNode parentNode) {
            string filter = textBoxFilter.Text;

            TreeNode n = new TreeNode(g.Name) {
                NodeFont = new Font(this.Font, FontStyle.Bold),
                Tag = g
            };

            if(parentNode == null) {
                treeViewItems.Nodes.Add(n);
            } else {
                parentNode.Nodes.Add(n);
            }

            TreeNode ntn;
            foreach(Item i in g.Items.OrderBy(it => it.Name)) {
                foreach(TreeNode tn in treeViewItems.Nodes) {
                    if((tn.Tag is Item) && ((Item)tn.Tag) == i) {
                        int a = 1; // STOP?
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
                    treeViewItems.SelectedNode = n;
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
                            treeViewItems.ScrollUp();
                        } else if(draggingLocation.Y >= treeViewItems.Height - scrollMargin) {
                            treeViewItems.ScrollDown();
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
            treeViewItems.SelectedNode = selectedNode;
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