using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyDeskAB.Classes {
    public class Helpers {
        public static void SetExpandedNodes(TreeNodeCollection nodes, List<string> expandedNodes) {
            foreach(TreeNode n in nodes) {
                if(expandedNodes.Contains(n.Text)) n.Expand();
                SetExpandedNodes(n.Nodes, expandedNodes);
            }
        }

        public static List<string> GetExpandedNodes(TreeNodeCollection nodes) {
            List<string> expandedNodes = new List<string>();

            foreach(TreeNode n in nodes) {
                if(n.IsExpanded) expandedNodes.Add(n.Text);
                expandedNodes.AddRange(GetExpandedNodes(n.Nodes).ToArray());
            }

            return expandedNodes;
        }
    }
}