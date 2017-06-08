using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using AnyDeskAB.Classes;
using AnyDeskAB.Classes.AnyDesk;
using System.Diagnostics;
using System.Xml.Linq;

namespace AnyDeskAB {
    public partial class FormMain : Form {
        private List<Group> groups;

        private string adConfigFileName;
        private string adExeFileName;
        private string settingsFileName;
        private int maxConfBackups = 10;

        private TreeNode selectedNode;
        private bool ignoreTextBoxEvents = false;
        private bool isDragging = false;
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
                    EnableRaisingEvents = true
                };
                SetupEventhandlers();

                draggingMonitor = new Thread(DraggingMonitorLoop) {
                    IsBackground = true
                };
                draggingMonitor.Start();
            } else {
                MessageBox.Show("Unable to initialize", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void DraggingMonitorLoop() {
            const int scrollMargin = 8;
            TreeNode lastOver = null;
            int overItemCount = 0;
            while(true) {
                if(isDragging && dragginOverNode != null) {
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
                }
                Thread.Sleep(100);
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

            foreach(string line in ReadFile.ReadLines(adConfigFileName)) {
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
                if(e.FullPath == adConfigFileName) this.Invoke((MethodInvoker)delegate { LoadAddressBook(); });
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
                adConfigMonitor.Dispose();
                SaveSettings(true);
            };
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

            foreach(string line in ReadFile.ReadLines(adConfigFileName)) {
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
                bool isDone;
                do {
                    isDone = true;
                    foreach(Item i in rg.GetAllItems()) {
                        if(!adg.ItemExists(i)) {
                            ((Group)i.Parent).Items.Remove(i);
                            isDone = false;
                        }
                    }
                } while(!isDone);

                // Add any new items that may have been added from AnyDesk
                List<Item> rgis = rg.GetAllItems();
                foreach(Item i in adg.Items) {
                    if(!rg.ItemExists(i)) rg.Items.Add((Item)i.Clone(rg));
                }

                // Restore expanded nodes
                foreach(XElement xml in xDoc.Descendants("node")) {
                    expandedNodes.Add(xml.Value);
                }
            }

            if(!groups.Any()) {
                groups.Add(new Group(null, "AnyDesk"));
                Group g = groups[0];
                foreach(Item i in adg.Items) g.Items.Add((Item)i.Clone(g));
            }

            UpdateUI();
            Helpers.SetExpandedNodes(treeViewItems.Nodes, expandedNodes);
        }

        private bool SetupPaths() {
            settingsFileName = Path.Combine((new DirectoryInfo(Application.UserAppDataPath)).Parent.FullName, "settings.dat");
            adConfigFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\AnyDesk\user.conf");
            adExeFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"AnyDesk\AnyDesk.exe");

            // Is the user like me and likes to install programs on multiple drives?
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
            foreach(Group g in groups.OrderBy(grp => grp.Name)) {
                AddGroupNode(g, null);
            }

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
            TreeNode n = new TreeNode(g.Name) {
                NodeFont = new Font(this.Font, FontStyle.Bold),
                Tag = g
            };

            if(parentNode == null) {
                treeViewItems.Nodes.Add(n);
            } else {
                parentNode.Nodes.Add(n);
            }

            foreach(Item i in g.Items.OrderBy(it => it.Name)) {
                n.Nodes.Add(i.ToString()).Tag = i;
            }

            foreach(Group sg in g.Groups.OrderBy(sgt => sgt.Name)) {
                AddGroupNode(sg, n);
            }

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
                    ((Group)i.Parent).Items.Remove(i);
                } else {
                    nodeType = "Group";
                    Group g = (Group)selectedNode.Tag;
                    while(g.Items.Any()) {
                        groups[0].Items.Add((Item)g.Items[0].Clone(groups[0]));
                        g.Items.Remove(g.Items[0]);
                    }
                    while(g.Groups.Any()) {
                        groups[0].Groups.Add((Group)g.Groups[0].Clone(groups[0]));
                        g.Groups.Remove(g.Groups[0]);
                    }
                    ((Group)g.Parent).Groups.Remove(g);
                }
                SaveSettings(true);
                UpdateUI();
            }
        }

        private void Connect() {
            Process.Start(adExeFileName, ((Item)selectedNode.Tag).Address);
        }
        #endregion
    }
}