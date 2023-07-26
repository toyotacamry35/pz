using Assets.Src.Lib.Extensions;
using Assets.Src.RubiconAI;
using Assets.Src.RubiconAI.BehaviourTree;
using Assets.Src.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Detective.Editor
{

    class TickStrategyDrawer : EventDrawer<TickStrategyEvent>
    {
        TreeNodeModel<BehaviourStatus> _tree;
        Rect treePanel;

        public static float NODE_WIDTH { get; private set; } = 100;
        public static float NODE_MARGIN_X { get; private set; } = 50;
        public static int NODE_HEIGHT { get; private set; } = 100;
        public static int NODE_MARGIN_Y { get; private set; } = 20;
        BehaviourStatus _selectedStatus = null;
        Vector2 _scroll = Vector2.zero;
        protected override void Draw(InvestigatorWindow w, TickStrategyEvent e)
        {
            GUI.skin.button.wordWrap = true;
            if (UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.keyCode == KeyCode.Mouse1)
                _selectedStatus = null;
            if (_selectedStatus != null)
            {
                foreach (var tickedNode in e.TickedNodes)
                {
                    var node = tickedNode as BehaviourNode;
                    if (node != null)
                        GUILayout.Label($"{node._def.____GetDebugShortName()}");
                }
            }
            else
            {

                if (e.Strategy.TerminatedByEvent)
                {
                    GUILayout.Label($"Terminated by event {e.Strategy.Def}");
                }
                else
                {
                    GUILayout.Label($"Strategy: {e?.Strategy?.Def?.____GetDebugShortName()}");
                    var rootTreeNode = new TreeNodeModel<BehaviourStatus>(e.Strategy.RootNodeTick, null);
                    FillTreeNode(rootTreeNode, e);
                    _tree = rootTreeNode;
                    var size = TreeHelpers<BehaviourStatus>.CalculateNodePositions(rootTreeNode);
                    GUILayoutUtility.GetRect(size.x, size.y);
                    DrawNode(rootTreeNode);
                }
            }
        }

        private TreeNodeModel<BehaviourStatus> FillTreeNode(TreeNodeModel<BehaviourStatus> rootTreeNode, TickStrategyEvent e)
        {
            if (rootTreeNode.Item.SubStrategyStatus != null)
            {
                if (rootTreeNode.Item.SubStrategyStatus.RootNodeTick != null)
                    rootTreeNode.Children.Add(FillTreeNode(new TreeNodeModel<BehaviourStatus>(rootTreeNode.Item.SubStrategyStatus.RootNodeTick, rootTreeNode), e));
            }
            else
                foreach (var subNode in rootTreeNode.Item.SubNodesTicks)
                    rootTreeNode.Children.Add(FillTreeNode(new TreeNodeModel<BehaviourStatus>(subNode, rootTreeNode), e));

            return rootTreeNode;
        }


        private void CalculateControlSize()
        {
            // tree sizes are 0-based, so add 1
            var treeWidth = _tree.Width + 1;
            var treeHeight = _tree.Height + 1;

            treePanel.size = new Vector2(
                Convert.ToInt32((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X)),
                (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y));
        }

        private void DrawNode(TreeNodeModel<BehaviourStatus> node)
        {
            // rectangle where node will be positioned
            var nodeRect = new Rect(
                Convert.ToInt32(NODE_MARGIN_X + (node.X * (NODE_WIDTH + NODE_MARGIN_X))),
                NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y))
                , NODE_WIDTH, NODE_HEIGHT);

            GUI.color = Color.white;
            switch (node.Item.Result)
            {
                case ScriptResultType.Running:
                    GUI.color = Color.green;
                    break;
                case ScriptResultType.Succeeded:
                    GUI.color = Color.yellow;
                    break;
                case ScriptResultType.Failed:
                    GUI.color = Color.red;
                    break;
            }

            // draw box
            if (GUI.Button(nodeRect,
                new GUIContent($"{node.Item.Node.GetType().Name} {(node.Item.Strategy != null ? node.Item.Strategy : "")} {node.Item.Node._def.Comment} {node.Item.StatusDescription}"
                )))
            {
                _selectedStatus = node.Item;
            }
            /*if(_selectedStatus == node.Item)
            {
                GUILayout.BeginArea();
                _scroll = GUILayout.BeginScrollView(_scroll);
                DrawNodeStatusDescription(node.Item);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }*/

            if (!node.Item.Ticked)
            {
                GUI.color = Color.gray;
                GUI.Box(new Rect(nodeRect.position + new Vector2(NODE_WIDTH - 20, 0), new Vector2(50, 30)), "X");
            }
            if (node.Item.Started)
            {
                GUI.color = Color.blue;
                GUI.Box(new Rect(nodeRect.position + new Vector2(NODE_WIDTH - 20, 0), new Vector2(50, 30)), "Started");
            }
            if (node.Item.Terminated)
            {
                GUI.color = Color.cyan;
                GUI.Box(new Rect(nodeRect.position + new Vector2(NODE_WIDTH - 20, 0) + new Vector2(0, 30), new Vector2(50, 30)), "Terminated");
            }
            if (node.Item.Finished)
            {
                GUI.color = Color.magenta;
                GUI.Box(new Rect(nodeRect.position + new Vector2(NODE_WIDTH - 20, 0) + new Vector2(0, 60), new Vector2(50, 30)), "Finished");
            }
            if (node.Item.ParentStrategy != null)
            {
                GUI.color = Color.white;
                GUI.Box(new Rect(nodeRect.position + new Vector2(NODE_WIDTH - 20, 0) + new Vector2(0, 90), new Vector2(100, 30)), node.Item.ParentStrategy.Result.ToString());
            }
            // draw content

            // draw line to parent
            if (node.Parent != null)
            {
                var nodeTopMiddle = new Vector2(nodeRect.x + (nodeRect.width / 2), nodeRect.y);
                var otherPoint = new Vector2(nodeTopMiddle.x, nodeTopMiddle.y - (NODE_MARGIN_Y / 2));
                Handles.DrawLine(nodeTopMiddle, otherPoint);
            }

            // draw line to children
            if (node.Children.Count > 0)
            {
                var nodeBottomMiddle = new Vector2(nodeRect.x + (nodeRect.width / 2), nodeRect.y + nodeRect.height);
                Handles.DrawLine(nodeBottomMiddle, new Vector2(nodeBottomMiddle.x, nodeBottomMiddle.y + (NODE_MARGIN_Y / 2)));


                // draw line over children
                if (node.Children.Count > 1)
                {
                    var childrenLineStart = new Vector2(
                        Convert.ToInt32(NODE_MARGIN_X + (node.GetRightMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
                        nodeBottomMiddle.y + (NODE_MARGIN_Y / 2));
                    var childrenLineEnd = new Vector2(
                        Convert.ToInt32(NODE_MARGIN_X + (node.GetLeftMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
                        nodeBottomMiddle.y + (NODE_MARGIN_Y / 2));

                    Handles.DrawLine(childrenLineStart, childrenLineEnd);
                }
            }


            foreach (var item in node.Children)
            {
                DrawNode(item);
            }
        }

        private void DrawNodeStatusDescription(BehaviourStatus item)
        {
        }
    }

    class MemoryStatusDrawer : EventDrawer<LegionaryKnowledgeAndMemoryStatus>
    {
        HashSet<RememberedLegionary> _drawnInWorldSpace = new HashSet<RememberedLegionary>();
        protected override void Draw(InvestigatorWindow w, LegionaryKnowledgeAndMemoryStatus e)
        {
            _drawnInWorldSpace.Clear();
            GUILayout.BeginVertical();

            GUILayout.Label("Cooldowns");
            foreach (var cooldown in e.Cooldowns.Cooldowns)
            {
                GUILayout.Label($"{cooldown.Key} {cooldown.Value}");
            }
            foreach (var known in e.KnowledgeSnapshot.Legionaries)
            {
                GUILayout.Label(known.Key.____GetDebugShortName());
                foreach (var legionary in known.Value)
                {
                    UnityEngine.Random.InitState(legionary.GetHashCode());
                    var color = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    DrawRememberedLegionary(legionary, color);
                    GUILayout.EndHorizontal();
                }
            }
            DrawMemorySnapshot(e.MemorySnapshot);
            GUILayout.EndVertical();
        }

        private void DrawMemorySnapshot(MemorySnapshot memorySnapshot)
        {
            foreach (var mem in memorySnapshot.RememberedLegionaries)
            {
                UnityEngine.Random.InitState(mem.Key.GetHashCode());
                var color = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1);
                DrawRememberedLegionary(mem.Key, color);
                foreach (var stat in mem.Value.Stats)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label($"{stat.Key.____GetDebugShortName()} = {stat.Value.Value}");
                    GUILayout.EndHorizontal();
                }
            }
            if (memorySnapshot.LegionMemory != null)
                DrawMemorySnapshot(memorySnapshot.LegionMemory);
        }

        private void DrawRememberedLegionary(RememberedLegionary legionary, Color color)
        {

            GUILayout.Label(legionary.WasNamed);
            if (UnityEngine.Event.current.type == EventType.Layout)
            {
                var rect = GUILayoutUtility.GetLastRect();
                var colorRect = new Rect(rect.xMax, rect.yMin, 10, 10);
                GUIExtensions.DrawRectangle(colorRect, color);
            }
            return;
            
        }
    }

    public static class TreeHelpers<T>
    where T : class
    {
        private static int nodeSize = 1;
        private static float siblingDistance = 0.0F;
        private static float treeDistance = 0.0F;
        private static float _maxX;
        private static float _maxY;
        public static Vector2 CalculateNodePositions(TreeNodeModel<T> rootNode)
        {
            // initialize node x, y, and mod values
            InitializeNodes(rootNode, 0);

            // assign initial X and Mod values for nodes
            CalculateInitialX(rootNode);

            // ensure no node is being drawn off screen
            CheckAllChildrenOnScreen(rootNode);

            // assign final X values to nodes
            CalculateFinalPositions(rootNode, 0);
            return new Vector2(_maxX, _maxY);
        }

        // recusrively initialize x, y, and mod values of nodes
        private static void InitializeNodes(TreeNodeModel<T> node, int depth)
        {
            node.X = -1;
            node.Y = depth;
            node.Mod = 0;
            if (_maxY < (node.Y + 1) * (TickStrategyDrawer.NODE_HEIGHT + TickStrategyDrawer.NODE_MARGIN_Y))
                _maxY = (node.Y + 1) * (TickStrategyDrawer.NODE_HEIGHT + TickStrategyDrawer.NODE_MARGIN_Y);
            foreach (var child in node.Children)
                InitializeNodes(child, depth + 1);
        }

        private static void CalculateFinalPositions(TreeNodeModel<T> node, float modSum)
        {
            node.X += modSum;
            modSum += node.Mod;

            foreach (var child in node.Children)
                CalculateFinalPositions(child, modSum);

            if (node.Children.Count == 0)
            {
                node.Width = node.X;
                node.Height = node.Y;
            }
            else
            {
                node.Width = node.Children.OrderByDescending(p => p.Width).First().Width;
                node.Height = node.Children.OrderByDescending(p => p.Height).First().Height;
            }
            if (_maxX < (node.X + 1) * (TickStrategyDrawer.NODE_WIDTH + TickStrategyDrawer.NODE_MARGIN_X))
                _maxX = (node.X + 1) * (TickStrategyDrawer.NODE_WIDTH + TickStrategyDrawer.NODE_MARGIN_X);
        }

        private static void CalculateInitialX(TreeNodeModel<T> node)
        {
            foreach (var child in node.Children)
                CalculateInitialX(child);

            // if no children
            if (node.IsLeaf())
            {
                // if there is a previous sibling in this set, set X to prevous sibling + designated distance
                if (!node.IsLeftMost())
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                else
                    // if this is the first node in a set, set X to 0
                    node.X = 0;
            }
            // if there is only one child
            else if (node.Children.Count == 1)
            {
                // if this is the first node in a set, set it's X value equal to it's child's X value
                if (node.IsLeftMost())
                {
                    node.X = node.Children[0].X;
                }
                else
                {
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - node.Children[0].X;
                }
            }
            else
            {
                var leftChild = node.GetLeftMostChild();
                var rightChild = node.GetRightMostChild();
                var mid = (leftChild.X + rightChild.X) / 2;

                if (node.IsLeftMost())
                {
                    node.X = mid;
                }
                else
                {
                    node.X = node.GetPreviousSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - mid;
                }
            }

            if (node.Children.Count > 0 && !node.IsLeftMost())
            {
                // Since subtrees can overlap, check for conflicts and shift tree right if needed
                CheckForConflicts(node);
            }

        }

        private static void CheckForConflicts(TreeNodeModel<T> node)
        {
            var minDistance = treeDistance + nodeSize;
            var shiftValue = 0F;

            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);

            var sibling = node.GetLeftMostSibling();
            while (sibling != null && sibling != node)
            {
                var siblingContour = new Dictionary<int, float>();
                GetRightContour(sibling, 0, ref siblingContour);

                for (int level = node.Y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < minDistance)
                    {
                        shiftValue = minDistance - distance;
                    }
                }

                if (shiftValue > 0)
                {
                    node.X += shiftValue;
                    node.Mod += shiftValue;

                    CenterNodesBetween(node, sibling);

                    shiftValue = 0;
                }

                sibling = sibling.GetNextSibling();
            }
        }

        private static void CenterNodesBetween(TreeNodeModel<T> leftNode, TreeNodeModel<T> rightNode)
        {
            var leftIndex = leftNode.Parent.Children.IndexOf(rightNode);
            var rightIndex = leftNode.Parent.Children.IndexOf(leftNode);

            var numNodesBetween = (rightIndex - leftIndex) - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

                int count = 1;
                for (int i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.Parent.Children[i];

                    var desiredX = rightNode.X + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.X;
                    middleNode.X += offset;
                    middleNode.Mod += offset;

                    count++;
                }

                CheckForConflicts(leftNode);
            }
        }

        private static void CheckAllChildrenOnScreen(TreeNodeModel<T> node)
        {
            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);

            float shiftAmount = 0;
            foreach (var y in nodeContour.Keys)
            {
                if (nodeContour[y] + shiftAmount < 0)
                    shiftAmount = (nodeContour[y] * -1);
            }

            if (shiftAmount > 0)
            {
                node.X += shiftAmount;
                node.Mod += shiftAmount;
            }
        }

        private static void GetLeftContour(TreeNodeModel<T> node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Min(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetLeftContour(child, modSum, ref values);
            }
        }

        private static void GetRightContour(TreeNodeModel<T> node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Max(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetRightContour(child, modSum, ref values);
            }
        }
    }
    public class TreeNodeModel<T>
        where T : class
    {
        public float X { get; set; }
        public int Y { get; set; }
        public float Mod { get; set; }
        public TreeNodeModel<T> Parent { get; set; }
        public List<TreeNodeModel<T>> Children { get; set; }

        public float Width { get; set; }
        public int Height { get; set; }

        public T Item { get; set; }

        public TreeNodeModel(T item, TreeNodeModel<T> parent)
        {
            this.Item = item;
            this.Parent = parent;
            this.Children = new List<TreeNodeModel<T>>();
        }

        public bool IsLeaf()
        {
            return this.Children.Count == 0;
        }

        public bool IsLeftMost()
        {
            if (this.Parent == null)
                return true;

            return this.Parent.Children[0] == this;
        }

        public bool IsRightMost()
        {
            if (this.Parent == null)
                return true;

            return this.Parent.Children[this.Parent.Children.Count - 1] == this;
        }

        public TreeNodeModel<T> GetPreviousSibling()
        {
            if (this.Parent == null || this.IsLeftMost())
                return null;

            return this.Parent.Children[this.Parent.Children.IndexOf(this) - 1];
        }

        public TreeNodeModel<T> GetNextSibling()
        {
            if (this.Parent == null || this.IsRightMost())
                return null;

            return this.Parent.Children[this.Parent.Children.IndexOf(this) + 1];
        }

        public TreeNodeModel<T> GetLeftMostSibling()
        {
            if (this.Parent == null)
                return null;

            if (this.IsLeftMost())
                return this;

            return this.Parent.Children[0];
        }

        public TreeNodeModel<T> GetLeftMostChild()
        {
            if (this.Children.Count == 0)
                return null;

            return this.Children[0];
        }

        public TreeNodeModel<T> GetRightMostChild()
        {
            if (this.Children.Count == 0)
                return null;

            return this.Children[Children.Count - 1];
        }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
