﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private TreeNode selectedNode;

        FileSystemWatcher adConfigMonitor;

        public FormMain() {
            InitializeComponent();

            // TODO: Inform the user some paths are invalid and quit the program
            SetupPaths();
            LoadAddressBook();
            treeViewItems.Nodes[0].Expand();

            adConfigMonitor = new FileSystemWatcher((new FileInfo(adConfigFileName)).Directory.FullName) {
                Filter = "*.conf",
                EnableRaisingEvents = true
            };
            SetupEventhandlers();
        }

        private void SaveSettings() {
            string xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                            <settings>
                                <groups>{string.Concat(from Group g in groups select g.ToXML().ToString())}</groups>
                            </settings>";

            XDocument.Parse(xml).Save(settingsFileName);
            UpdateUserConf();
            //UpdateUI();
        }

        private void UpdateUserConf() {
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

            int n = 0;
            string bakFileName;
            while(true) {
                bakFileName = $"{adConfigFileName}-{n:00}.bak";
                if(File.Exists(bakFileName)) {
                    n += 1;
                    continue;
                } else
                    break;
            }
            File.Copy(adConfigFileName, bakFileName);
            File.WriteAllText(adConfigFileName, sb.ToString());
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

                if(n.Tag is Group) {
                    connectToolStripMenuItem.Visible = false;
                    sep01toolStripMenuItem.Visible = false;
                    deleteToolStripMenuItem.Enabled = (selectedNode.Parent != null);
                } else {
                    connectToolStripMenuItem.Visible = true;
                    sep01toolStripMenuItem.Visible = true;
                    deleteToolStripMenuItem.Enabled = true;
                }
            };
            treeViewItems.AfterSelect += delegate { selectedNode = treeViewItems.SelectedNode; };
            treeViewItems.AfterLabelEdit += (object o, NodeLabelEditEventArgs e) => ((ADItem)e.Node.Tag).Name = e.Label;
            treeViewItems.ItemDrag += (object o, ItemDragEventArgs e) => {
                // TODO: Add support to drag groups
                selectedNode = (TreeNode)e.Item;
                if(e.Button == MouseButtons.Left) {
                    treeViewItems.SelectedNode = selectedNode;
                    treeViewItems.DoDragDrop(selectedNode, DragDropEffects.Move);
                }
            };
            treeViewItems.DragOver += (object o, DragEventArgs e) => {
                TreeNode overNode = treeViewItems.GetNodeAt(treeViewItems.PointToClient(new Point(e.X, e.Y)));
                if(overNode == null || overNode.Tag is Item) {
                    e.Effect = DragDropEffects.None;
                } else {
                    e.Effect = e.AllowedEffect;
                }
            };
            treeViewItems.DragDrop += (object o, DragEventArgs e) => {
                TreeNode overNode = treeViewItems.GetNodeAt(treeViewItems.PointToClient(new Point(e.X, e.Y)));
                if(overNode != null) {
                    Group tg = (Group)overNode.Tag;

                    if(selectedNode.Tag is Item si) {
                        ((Group)si.Parent).Items.Remove(si);
                        tg.Items.Add((Item)si.Clone(tg));
                    } else {
                        Group sg = (Group)selectedNode.Tag;
                        ((Group)sg.Parent).Groups.Remove(sg);
                        tg.Groups.Add((Group)sg.Clone(sg));
                    }
                    SaveSettings();
                }
            };

            connectToolStripMenuItem.Click += delegate { Connect(); };
            addGroupToolStripMenuItem.Click += delegate {
                if(selectedNode.Tag is Item) selectedNode = selectedNode.Parent;
                Group pg = ((Group)selectedNode.Tag);
                Group g = new Group(pg, "<New Group>");
                pg.Groups.Add(g);
                selectedNode = AddGroupNode(g, selectedNode);
                treeViewItems.SelectedNode = selectedNode;
                selectedNode.Expand();
                selectedNode.BeginEdit();

            };
            renameToolStripMenuItem.Click += delegate { selectedNode.BeginEdit(); };
            deleteToolStripMenuItem.Click += delegate { DeleteItem(); };

            this.FormClosing += delegate {
                adConfigMonitor.Dispose();
                SaveSettings();
            };
        }

        private void AddItem() {
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
                SaveSettings();
                UpdateUI();
            }
        }

        private void Connect() {
            Process.Start(adExeFileName, ((Item)selectedNode.Tag).Address);
        }

        private void LoadAddressBook() {
            groups = new List<Group>();
            Group adg = new Group(null, "");

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
            }

            if(!groups.Any()) {
                groups.Add(new Group(null, "AnyDesk"));
                Group g = groups[0];
                foreach(Item i in adg.Items) g.Items.Add((Item)i.Clone(g));
            }

            UpdateUI();
        }

        private void UpdateUI() {
            List<string> expandedNodes = GetExpandedNodes(treeViewItems.Nodes);

            // Dirty trick to prevent flickering
            this.BackgroundImage = new Bitmap(treeViewItems.Width, treeViewItems.Height);
            treeViewItems.DrawToBitmap((Bitmap)this.BackgroundImage, new Rectangle(treeViewItems.Location, treeViewItems.Size));

            treeViewItems.Visible = false;
            treeViewItems.SuspendLayout();
            treeViewItems.Nodes.Clear();
            foreach(Group g in groups.OrderBy(grp => grp.Name)) {
                AddGroupNode(g, null);
            }

            if(selectedNode != null) SelectNode(treeViewItems.Nodes, selectedNode.Text);
            SetExpandedNodes(treeViewItems.Nodes, expandedNodes);

            treeViewItems.ResumeLayout();
            treeViewItems.Visible = true;
            this.BackgroundImage = null;
        }

        private void SetExpandedNodes(TreeNodeCollection nodes, List<string> expandedNodes) {
            foreach(TreeNode n in nodes) {
                if(expandedNodes.Contains(n.Text)) n.Expand();
                SetExpandedNodes(n.Nodes, expandedNodes);
            }
        }

        private List<string> GetExpandedNodes(TreeNodeCollection nodes) {
            List<string> expandedNodes = new List<string>();

            foreach(TreeNode n in nodes) {
                if(n.IsExpanded) expandedNodes.Add(n.Text);
                expandedNodes.AddRange(GetExpandedNodes(n.Nodes).ToArray());
            }

            return expandedNodes;
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

        private TreeNode AddGroupNode(Group g, TreeNode parentNode) {
            TreeNode n;
            if(parentNode == null) {
                n = treeViewItems.Nodes.Add(g.Name);
            } else {
                n = parentNode.Nodes.Add(g.Name);
            }
            n.NodeFont = new Font(this.Font, FontStyle.Bold);
            n.Tag = g;

            foreach(Item i in g.Items.OrderBy(itm => itm.ToString())) {
                n.Nodes.Add(i.ToString()).Tag = i;
            }

            foreach(Group sg in g.Groups) {
                AddGroupNode(sg, n);
            }

            return n;
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
    }
}