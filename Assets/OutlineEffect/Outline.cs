/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using JetBrains.Annotations;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OutlineEffect
{
    public enum OutlineColor
    {
        Blue    = 0,
        Yellow  = 1,
        Red     = 2,
        Erase   = 3,
        Default = 4
    }

    public class OutlineHelper
    {
        public static OutlineColor OutlineColorFromIndex(int index)
        {
            if ((index >= 0) && (index < 5))
            {
                return (OutlineColor)(index);
            }
            else
            {
                return OutlineColor.Default;
            }
        }
    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Renderer Renderer { get; private set; }
        public Func<OutlineColor> ColorDelegate;

        public OutlineColor color;
        public OutlineColor Color
        {
            get
            {
                if (ColorDelegate != null)
                    return ColorDelegate();
                return color;
            }
        }

        public bool eraseRenderer;
        private Coroutine _routine;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
        }

        void OnEnable()
        {
            IEnumerable<OutlineEffect> effects = Camera.allCameras
                .Select(c => c.GetComponent<OutlineEffect>())
                .Where(e => e != null);

            foreach (OutlineEffect effect in effects)
                effect.AddOutline(this);
        }

        void OnDisable()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = null;

            IEnumerable<OutlineEffect> effects = Camera.allCameras
                .Select(c => c.GetComponent<OutlineEffect>())
                .Where(e => e != null);

            foreach (OutlineEffect effect in effects)
            {
                effect.RemoveOutline(this);
            }
        }
    }
}