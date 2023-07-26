using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace AwesomeTechnologies.Utility.Quadtree
{  
    /// <summary>
    /// The QuadTreeNode
    /// </summary>
    public class QuadTreeNode<T> where T : IHasRect
    {
        /// <summary>
        /// Construct a quadtree node with the given rect 
        /// </summary>
        /// <param name="rect"></param>
        public QuadTreeNode(Rect rect)
        {
            _mRect = rect;                       
        }

        /// <summary>
        /// The area of this node
        /// </summary>
        Rect _mRect;

        /// <summary>
        /// The contents of this node.
        /// Note that the contents have no limit: this is not the standard way to impement a QuadTree
        /// </summary>
        readonly List<T> _mContents = new List<T>();

        /// <summary>
        /// The child nodes of the QuadTree
        /// </summary>
        readonly List<QuadTreeNode<T>> _mNodes = new List<QuadTreeNode<T>>(4);

        /// <summary>
        /// Is the node empty
        /// </summary>
        public bool IsEmpty { get { return _mRect.width == 0  || _mRect.height == 0 || _mNodes.Count == 0; } }

        /// <summary>
        /// Area of the quadtree node
        /// </summary>
        public Rect Rect { get { return _mRect; } }

        /// <summary>
        /// Total number of nodes in the this node and all SubNodes
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;

                foreach (QuadTreeNode<T> node in _mNodes)
                    count += node.Count;

                count += Contents.Count;

                return count;
            }
        }

        /// <summary>
        /// Return the contents of this node and all subnodes in the true below this one.
        /// </summary>
        public List<T> SubTreeContents
        {
            get
            {
                List<T> results = new List<T>();

                foreach (QuadTreeNode<T> node in _mNodes)
                    results.AddRange(node.SubTreeContents);

                results.AddRange(Contents);
                return results;
            }
        }

        public List<T> Contents { get { return _mContents; } }

        /// <summary>
        /// Query the QuadTree for items that are in the given area
        /// </summary>
        /// <param name="queryArea"></param>
        /// <returns></returns>
        public List<T> Query(Rect queryArea)
        {
            // create a list of the items that are found
            List<T> results = new List<T>();

            // this quad contains items that are not entirely contained by
            // it's four sub-quads. Iterate through the items in this quad 
            // to see if they intersect.
            foreach (T item in Contents)
            {
                if (queryArea.Overlaps(item.Rectangle))
                    results.Add(item);
            }

            foreach (QuadTreeNode<T> node in _mNodes)
            {
                if (node.IsEmpty)
                    continue;

                // Case 1: search area completely contained by sub-quad
                // if a node completely contains the query area, go down that branch
                // and skip the remaining nodes (break this loop)
                if (node.Rect.Contains(queryArea))
                {
                    results.AddRange(node.Query(queryArea));
                    break;
                }

                // Case 2: Sub-quad completely contained by search area 
                // if the query area completely contains a sub-quad,
                // just add all the contents of that quad and it's children 
                // to the result set. You need to continue the loop to test 
                // the other quads
                if (queryArea.Contains(node.Rect))
                {
                    results.AddRange(node.SubTreeContents);
                    continue;
                }

                // Case 3: search area intersects with sub-quad
                // traverse into this quad, continue the loop to search other
                // quads
                if (node.Rect.Overlaps(queryArea))
                {
                    results.AddRange(node.Query(queryArea));
                }
            }
            return results;
        }

        //public List<T> Query(Vector2 point)
        //{
        //    List<T> results = new List<T>();
        //    foreach (QuadTreeNode<T> node in m_nodes)
        //    {
        //        if (node.IsEmpty)
        //        {
        //            if (node.rect.Contains(point))
        //            {                               
        //                foreach (T item in this.Contents)
        //                {
        //                    if (item.Rectangle.Contains(point))
        //                    {
        //                        results.Add(item);
        //                    }
        //                }
        //            }
        //           continue;
        //        }                   

        //        if (node.rect.Contains(point))
        //        {
        //            results.AddRange(node.Query(point));
        //            break;
        //        }
        //    }
        //    return results;
        //}

        /// <summary>
        /// Insert an item to this node
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            // if the item is not contained in this quad, there's a problem
            if (!_mRect.Contains(item.Rectangle))
            {
                Trace.TraceWarning("feature is out of the rect of this quadtree node");
                return;
            }

            // if the subnodes are null create them. may not be sucessfull: see below
            // we may be at the smallest allowed size in which case the subnodes will not be created
            if (_mNodes.Count == 0)
                CreateSubNodes();

            // for each subnode:
            // if the node contains the item, add the item to that node and return
            // this recurses into the node that is just large enough to fit this item
            foreach (QuadTreeNode<T> node in _mNodes)
            {
                if (node.Rect.Contains(item.Rectangle))
                {
                    node.Insert(item);
                    return;
                }
            }

            // if we make it to here, either
            // 1) none of the subnodes completely contained the item. or
            // 2) we're at the smallest subnode size allowed 
            // add the item to this node's contents.
            Contents.Add(item);
        }

        //move quadtree node
        public void Move(UnityEngine.Vector2 offset)
        {
            foreach (QuadTreeNode<T> node in _mNodes)
            {
                node.Move(offset);
            }
            _mRect = new Rect(_mRect.xMin + offset.x, _mRect.yMin + offset.y, _mRect.width, _mRect.height);

            //this.m_rect.xMin += offset.x;
            //this.m_rect.yMin += offset.y;
            //this.m_rect.width += offset.x;
            //this.m_rect.height += offset.y;
        }

        public void LogData()
        {
            //UnityEngine.Debug.Log(_mRect.ToString() + " Count: " + SubTreeContents.Count.ToString());
            if (_mNodes.Count > 0)
            {
                _mNodes[0].LogData();
            }

            //foreach (QuadTreeNode<T> node in this.m_nodes)
            //{
            //    foreach (QuadTreeNode<T> node2 in node.m_nodes)
            //    {
            //        UnityEngine.Debug.Log(node2.m_rect.ToString() + " Count" + node2.SubTreeContents.Count.ToString());
            //    }
            //    UnityEngine.Debug.Log(node.m_rect);                
            //}
        }

        public void ForEach(QuadTree<T>.QTAction action)
        {
            action(this);

            // draw the child quads
            foreach (QuadTreeNode<T> node in _mNodes)
                node.ForEach(action);
        }

        /// <summary>
        /// Internal method to create the subnodes (partitions space)
        /// </summary>
        private void CreateSubNodes()
        {
            // the smallest subnode has an area 
            if ((_mRect.height * _mRect.width) <= 10)
                return;

            float halfWidth = (_mRect.width / 2f);
            float halfHeight = (_mRect.height / 2f);

            _mNodes.Add(new QuadTreeNode<T>(new Rect(new UnityEngine.Vector2(_mRect.xMin, _mRect.yMin), new UnityEngine.Vector2(halfWidth, halfHeight))));
            _mNodes.Add(new QuadTreeNode<T>(new Rect(new UnityEngine.Vector2(_mRect.xMin, _mRect.yMin + halfHeight), new UnityEngine.Vector2(halfWidth, halfHeight))));
            _mNodes.Add(new QuadTreeNode<T>(new Rect(new UnityEngine.Vector2(_mRect.xMin + halfWidth, _mRect.yMin), new UnityEngine.Vector2(halfWidth, halfHeight))));
            _mNodes.Add(new QuadTreeNode<T>(new Rect(new UnityEngine.Vector2(_mRect.xMin + halfWidth, _mRect.yMin + halfHeight), new UnityEngine.Vector2(halfWidth, halfHeight))));
        }
    }
}
