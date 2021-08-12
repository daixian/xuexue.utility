using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xuexue.utility;

namespace UnitTest
{
    [TestClass]
    public class HierarchyTreeTest
    {
        [TestMethod]
        public void CreateTest()
        {
            HierarchyTree tree = new HierarchyTree();

            tree.GetOrCreateNode(new string[] { "数学" });
            tree.GetOrCreateNode(new string[] { "数学", "一年级" });
            HierarchyTreeNode node = tree.GetOrCreateNode(new string[] { "数学", "一年级", "上册" });
            tree.GetOrCreateNode(new string[] { "物理", "一年级" });

            Assert.IsTrue(tree.root.children.Count == 2);

            HierarchyTreeNode[] arr = tree.root.GetAllChild();
            Assert.IsTrue(arr.Length == 5);

            Assert.IsNotNull(tree.GetNode(new string[] { "数学", "一年级", "上册" }));
            HierarchyTreeNode node1 = tree.GetNode(new string[] { "数学", "一年级", "上册" });
            Assert.IsTrue(node1.IsValid());
            Assert.IsTrue(node1.Path.Length == 3);

            Assert.IsNotNull(tree.GetNode(new string[] { "数学", "一年级" }));
            tree.GetNode(new string[] { "数学", "一年级" }).IsValid();


            Assert.IsNull(tree.GetNode(new string[] { "数学123", "一年级" }));

            //移除一个节点
            tree.RemoveNode(new string[] { "数学", "一年级", "上册" });
            Assert.IsNull(tree.GetNode(new string[] { "数学", "一年级", "上册" }));
            Assert.IsFalse(node1.IsValid());

        }
    }


}