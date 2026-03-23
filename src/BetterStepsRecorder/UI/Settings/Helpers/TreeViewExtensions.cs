using System.Linq;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings.Helpers
{
    /// <summary>
    /// Extension methods for TreeView to reduce boilerplate code.
    /// </summary>
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Finds a TreeNode by name recursively using LINQ.
        /// </summary>
        public static TreeNode FindNodeByName(this TreeView treeView, string name)
        {
            return treeView.Nodes.Cast<TreeNode>()
                .SelectMany(node => GetNodeAndDescendants(node))
                .FirstOrDefault(node => node.Name == name);
        }

        /// <summary>
        /// Recursively gets all descendant nodes including the parent.
        /// </summary>
        private static System.Collections.Generic.IEnumerable<TreeNode> GetNodeAndDescendants(TreeNode node)
        {
            yield return node;
            foreach (TreeNode child in node.Nodes)
            {
                foreach (var descendant in GetNodeAndDescendants(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
