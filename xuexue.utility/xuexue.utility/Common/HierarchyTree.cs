using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.utility
{
    /// <summary>
    /// HierarchyTree的子节点
    /// </summary>
    public class HierarchyTreeNode
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <param name="rank"></param>
        public HierarchyTreeNode(string name, Object user = null, int rank = 0)
        {
            this.name = name;
            this.user = user;
            this.rank = rank;
        }

        /// <summary>
        /// 节点名
        /// </summary>
        /// <returns></returns>
        public string name;

        /// <summary>
        /// 是否整个激活的
        /// </summary>
        /// <returns></returns>
        public bool isActive = true;

        /// <summary>
        /// 是否展开
        /// </summary>
        /// <returns></returns>
        public bool isExpand = false;

        /// <summary>
        /// 父节点
        /// </summary>
        public HierarchyTreeNode parent;

        /// <summary>
        /// 子节点
        /// </summary>
        public List<HierarchyTreeNode> children;

        /// <summary>
        /// 这个节点的排名优先级
        /// </summary>
        public int rank = 0;

        /// <summary>
        /// 一个关联的引用
        /// </summary>
        public Object user = null;

        /// <summary>
        /// 向上寻找还原这个节点的Path
        /// </summary>
        public string[] Path
        {
            get
            {
                Stack<string> result = new Stack<string>();
                HierarchyTreeNode curNode = this;
                //迭代200层
                for (int i = 0; i < 200; i++)
                {
                    if (curNode == null)
                    {
                        return null;//无效队列
                    }
                    if (curNode.isSceneRoot())
                    {
                        return result.ToArray();
                    }
                    else
                    {
                        result.Push(curNode.name);
                    }
                    curNode = curNode.parent;
                }
                return null;//这个是死循环
            }
        }

        /// <summary>
        /// 向上寻找还原这个节点的Path
        /// </summary>
        public HierarchyTreeNode[] PathNode
        {
            get
            {
                Stack<HierarchyTreeNode> result = new Stack<HierarchyTreeNode>();
                HierarchyTreeNode curNode = this;
                //迭代200层
                for (int i = 0; i < 200; i++)
                {
                    if (curNode == null)
                    {
                        return null;//无效队列
                    }
                    if (curNode.isSceneRoot())
                    {
                        return result.ToArray();
                    }
                    else
                    {
                        result.Push(curNode);
                    }
                    curNode = curNode.parent;
                }
                return null;//这个是死循环
            }
        }

        /// <summary>
        /// 得到子节点的个数
        /// </summary>
        public int childCount
        {
            get
            {
                if (children == null)
                {
                    return 0;
                }
                return children.Count;
            }
        }

        /// <summary>
        /// 得到一个子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HierarchyTreeNode GetChild(int index)
        {
            {
                if (children == null || index >= children.Count)
                {
                    return null;
                }
                return children[index];
            }
        }

        /// <summary>
        /// 按名字寻找一个子节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HierarchyTreeNode FindChild(string name)
        {
            if (children == null)
                return null;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].name == name)
                {
                    return children[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 递归得到所有的子节点
        /// </summary>
        /// <returns></returns>
        public HierarchyTreeNode[] GetAllChild()
        {
            if (children == null)
                return null;

            Queue<HierarchyTreeNode> result = new Queue<HierarchyTreeNode>();
            Queue<HierarchyTreeNode> queue = new Queue<HierarchyTreeNode>();//BFS队列
            for (int i = 0; i < children.Count; i++)
            {
                queue.Enqueue(children[i]);
            }

            while (queue.Count > 0)
            {
                HierarchyTreeNode front = queue.Dequeue();
                result.Enqueue(front);
                if (front.children != null)
                {
                    for (int i = 0; i < front.children.Count; i++)
                    {
                        queue.Enqueue(front.children[i]);
                    }
                }
            }
            return result.ToArray();
        }


        /// <summary>
        /// 选中或添加一个节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HierarchyTreeNode GetOrCreateChild(string name)
        {
            if (children == null)
                children = new List<HierarchyTreeNode>();

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].name == name)
                {
                    return children[i];
                }
            }
            HierarchyTreeNode child = new HierarchyTreeNode(name);
            children.Add(child);
            child.parent = this;
            return child;
        }

        /// <summary>
        /// 移除一个节点
        /// </summary>
        /// <param name="name"></param>
        public void RemoveChild(string name)
        {
            if (children == null)
                return;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].name == name)
                {
                    HierarchyTreeNode moveChild = children[i];
                    moveChild.Break();
                    children.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 是否是有效的(未断开)
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            HierarchyTreeNode curNode = this;
            //迭代200层
            for (int i = 0; i < 200; i++)
            {
                if (curNode == null)
                {
                    return false;
                }
                if (curNode.isSceneRoot())
                {
                    return true;
                }

                curNode = curNode.parent;
            }
            return false;
        }

        /// <summary>
        /// 断开这节点
        /// </summary>
        public void Break()
        {
            parent = null;
            children = null;
        }

        /// <summary>
        /// 是否这个点是场景的根节点了
        /// </summary>
        /// <returns></returns>
        public bool isSceneRoot()
        {
            if (this.parent == null)
            {
                if (this.name == HierarchyTree.HierarchyTreeSceneRootName)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 层级树结构,使用于一些层级UI
    /// </summary>
    public class HierarchyTree
    {
        /// <summary>
        /// 具有这个名字的节点是整个树的根节点,使用这个长的名字避免重复
        /// </summary>
        public static string HierarchyTreeSceneRootName = "XX_HierarchyTree_SceneRoot";

        /// <summary>
        /// 整个树的根列表
        /// </summary>
        public HierarchyTreeNode root = new HierarchyTreeNode(HierarchyTreeSceneRootName);

        /// <summary>
        /// 选中或添加一个节点
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns>添加成功返回结果节点</returns>
        public HierarchyTreeNode GetOrCreateNode(string[] hierarchyPath)
        {
            if (hierarchyPath == null || hierarchyPath.Length == 0)
            {
                throw new ArgumentException();
            }

            //根节点的第一层里先搜索
            HierarchyTreeNode curNode = root;
            //开始从第一个Node出发开始搜索
            for (int i = 0; i < hierarchyPath.Length; i++)
            {
                if (!string.IsNullOrEmpty(hierarchyPath[i]))
                {
                    string nodeName = hierarchyPath[i];

                    curNode = curNode.GetOrCreateChild(nodeName);
                }
            }
            return curNode;
        }

        /// <summary>
        /// 得到这个节点,不会创建
        /// </summary>
        /// <returns></returns>
        public HierarchyTreeNode GetNode(string[] hierarchyPath)
        {
            if (hierarchyPath == null || hierarchyPath.Length == 0)
            {
                throw new ArgumentException();
            }

            //根节点的第一层里先搜索
            HierarchyTreeNode curNode = root;
            //开始从第一个Node出发开始搜索
            for (int i = 0; i < hierarchyPath.Length; i++)
            {
                if (!string.IsNullOrEmpty(hierarchyPath[i]))
                {
                    string nodeName = hierarchyPath[i];

                    curNode = curNode.FindChild(nodeName);
                    if (curNode == null)
                    {
                        return null;//无法找到只能中断
                    }
                }
            }
            return curNode;
        }

        /// <summary>
        /// 移除一个节点
        /// </summary>
        /// <param name="hierarchyPath"></param>
        public void RemoveNode(string[] hierarchyPath)
        {
            if (hierarchyPath == null || hierarchyPath.Length == 0)
            {
                throw new ArgumentException();
            }

            HierarchyTreeNode node = GetNode(hierarchyPath);
            if (node != null)
            {
                node.parent.RemoveChild(node.name);
            }
        }

        /// <summary>
        /// 得到这个节点(未实现)
        /// </summary>
        /// <returns></returns>
        public HierarchyTreeNode GetNode(Object user)
        {

            return null;
        }

        /// <summary>
        /// 得到这个节点在层级中的最终是否是活动的
        /// </summary>
        /// <param name="node"></param>
        public void NodeActiveHierarchy(HierarchyTreeNode node)
        {


        }
    }
}
