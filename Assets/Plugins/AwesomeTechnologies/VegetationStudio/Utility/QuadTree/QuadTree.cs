using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace AwesomeTechnologies.Utility.Quadtree
{
    public class QuadTree<T> where T : IHasRect
    {
        /// <summary>
        /// The root QuadTreeNode
        /// </summary>
        QuadTreeNode<T> m_root;

        /// <summary>
        /// The bounds of this QuadTree
        /// </summary>
        Rect m_rect;

        /// <summary>
        /// An delegate that performs an action on a QuadTreeNode
        /// </summary>
        /// <param name="obj"></param>
        public delegate void QTAction(QuadTreeNode<T> obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        public QuadTree(Rect rect)
        {
            m_rect = rect;
            m_root = new QuadTreeNode<T>(m_rect);
        }

        /// <summary>
        /// Get the count of items in the QuadTree
        /// </summary>
        public int Count { get { return m_root.Count; } }

        /// <summary>
        /// Insert the feature into the QuadTree
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            m_root.Insert(item);
        }

        public void Move(UnityEngine.Vector2 offset)
        {
            this.m_rect = new Rect(m_rect.xMin + offset.x, this.m_rect.yMin + offset.y, this.m_rect.width, this.m_rect.height);
            m_root.Move(offset);
        }

        public void LogData()
        {
            m_root.LogData();
        }

        /// <summary>
        /// Query the QuadTree, returning the items that are in the given area
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<T> Query(Rect area)
        {
            return m_root.Query(area);
        }

        //public List<T> Query(Vector2 point)
        //{
        //    //TUDO there is a bug in the result
        //    return m_root.Query(point);
        //}

        /// <summary>
        /// Do the specified action for each item in the quadtree
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(QTAction action)
        {
            m_root.ForEach(action);
        }
    }
}
