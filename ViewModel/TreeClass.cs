using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MatrycaKwalifikacji.ViewModels
{
    public class TreeNode
    {
        public int id { get; set; }
        public string text { get; set; }
        public string parent { get; set; }
    }
    public static class TreeBuilder
    {
        public static Tree BuildTree(IEnumerable<TreeNode> nodes)
        {
            if (nodes==null) return new Tree();
            var nodeList = nodes.ToList();
            var tree = FindTreeRoot(nodeList);
            BuildTree(tree, nodeList);
            return tree;
        }

        private static void BuildTree(Tree tree, IList<TreeNode> descendants)
        {
            var children = descendants.Where(node => node.parent == (tree.Id == null ? "#" : tree.Id.ToString())).ToArray();
            foreach (var child in children)
            {
                var branch = Map(child);
                tree.Add(branch);
                descendants.Remove(child);
            }
            foreach (var branch in tree.Children)
            {
                BuildTree(branch, descendants);
            }
        }

        private static Tree FindTreeRoot(IList<TreeNode> nodes)
        {
            var rootNodes = nodes.Where(node => node.parent.Equals("#") );
            if (rootNodes.Count()!=1) return new Tree();
            var rootNode = rootNodes.Single();
            nodes.Remove(rootNode);
            return Map(rootNode);
        }

        private static Tree Map(TreeNode node)
        {
            return new Tree
            {
                Id=node.id,
                Text=node.text
            };
        }
    }
    public static class TreeExtensions
    {

        public static Tree FindNode(Tree tree, int id)
        {
            if (tree==null) return null;
            if (tree.Id==id)
            {
                return tree;
            }
            foreach (Tree child in tree.Children)
            {
                if (child.Id==id)
                {
                    return child;
                }
                else
                {
                    if (child.Children.Any())
                    {
                        Tree x = FindNode(child, id);
                        if (x!=null) return x;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<Tree> Descendants(this Tree value)
        {
            // a descendant is the node self and any descendant of the children
            if (value==null) yield break;
            yield return value;
            // depth-first pre-order tree walker
            foreach (var child in value.Children)
                foreach (var descendantOfChild in child.Descendants())
                {
                    yield return descendantOfChild;
                }
        }

        public static IEnumerable<Tree> Ancestors(this Tree value)
        {
            // an ancestor is the node self and any ancestor of the parent
            var ancestor = value;
            // post-order tree walker
            while (ancestor!=null)
            {
                yield return ancestor;
                ancestor=ancestor.Parent;
            }
        }
    }

    public class Tree
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        protected List<Tree> _children;
        protected Tree _parent;

        public Tree()//dodano id
        {
            Text=string.Empty;

        }

        public Tree Parent { get { return _parent; } }
        public Tree Root { get { return _parent==null ? this : _parent.Root; } }
        public int Depth { get { return this.Ancestors().Count()-1; } }

        public IEnumerable<Tree> Children
        {
            get { return _children==null ? Enumerable.Empty<Tree>() : _children.ToArray(); }
        }

        public override string ToString()
        {
            return Text;
        }

        public void Add(Tree child)
        {
            if (child==null)
                throw new ArgumentNullException();
            if (child._parent!=null)
                throw new InvalidOperationException("A tree node must be removed from its parent before adding as child.");
            if (this.Ancestors().Contains(child))
            {
                const string Message = "A tree cannot be a cyclic graph.";
                throw new InvalidOperationException(Message);
            }

            if (_children==null)
            {
                _children=new List<Tree>();
            }
            Debug.Assert(!_children.Contains(child), "At this point, the node is definately not a child");
            child._parent=this;
            _children.Add(child);
        }

        public bool Remove(Tree child)
        {
            if (child==null)
                throw new ArgumentNullException();
            if (child._parent!=this)
                return false;
            Debug.Assert(_children.Contains(child), "At this point, the node is definately a child");
            child._parent=null;
            _children.Remove(child);
            if (!_children.Any())
                _children=null;
            return true;
        }
        public static void Wypisz(Tree lTree)
        {
            if (lTree.Children.Any())
            {
                Console.WriteLine("<LL{0}>", lTree.Depth.ToString());
            }
            Console.WriteLine(lTree.Text);
            foreach (Tree t in lTree.Children)
            {
                Wypisz(t);
            }
            if (lTree.Children.Any())
            {
                Console.WriteLine("</LL{0}>", lTree.Depth.ToString());
            }
        }
        public static void Wypisz2(Tree lTree)
        {
            if (lTree.Id!=null)
            {
                Console.WriteLine("<LL{0}> " + lTree.Id + '-' + lTree.Text, lTree.Depth.ToString());
                
                if (lTree.Children.Any())
                {
                    Console.WriteLine("Children: [");
                }
            }

            foreach (Tree t in lTree.Children)
            {
                Wypisz2(t);
            }
            if (lTree.Children.Any())
            {
                Console.WriteLine("]");
            }
            if (lTree.Id!=null)
            {
                Console.WriteLine("</LL{0}>", lTree.Depth.ToString());
            }
        }
    }
}
