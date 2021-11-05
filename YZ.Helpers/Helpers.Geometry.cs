﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace YZ {

    public static partial class Helpers {
        public static Rectangle ScaleToFit(this Rectangle src, Rectangle bounds) => src.ScaleToFit(bounds.Width, bounds.Height).Translate(bounds.X, bounds.Y);
        public static Rectangle ScaleToFit(this Rectangle src, Size bounds) => src.ScaleToFit(bounds.Width, bounds.Height);
        public static Rectangle ScaleToFit(this Rectangle src, int width, int height) {
            if (width == 0 || height == 0) return Rectangle.Empty;
            if (src.Height == 0) return new Rectangle(0, height / 2, width, 0);
            if (src.Width == 0) return new Rectangle(width / 2, 0, 0, height);
            var zoom = Math.Min((double)width / src.Width, (double)height / src.Height);
            if (zoom == 1) return src;

            //var asp = (double)src.Width / src.Height;
            var sz = new Size((int)(src.Width * zoom), (int)(src.Height * zoom));
            var offs = new Point((width - sz.Width) / 2, (height - sz.Height) / 2);
            return new Rectangle(offs, sz);
        }

        public static Rectangle Translate(this Rectangle src, Point offs) => new Rectangle(src.Location.Translate(offs), src.Size);
        public static Rectangle Translate(this Rectangle src, int x, int y) => new Rectangle(src.Location.Translate(x, y), src.Size);
        public static Point Translate(this Point src, Point offs) => new Point(src.X + offs.X, src.Y + offs.Y);
        public static Point Translate(this Point src, int x, int y) => new Point(src.X + x, src.Y + y);


        public static RectangleF ScaleToFit(this RectangleF src, RectangleF bounds) => src.ScaleToFit(bounds.Width, bounds.Height).Translate(bounds.X, bounds.Y);
        public static RectangleF ScaleToFit(this RectangleF src, Size bounds) => src.ScaleToFit(bounds.Width, bounds.Height);
        public static RectangleF ScaleToFit(this RectangleF src, float width, float height) {
            if (width == 0 || height == 0) return RectangleF.Empty;
            if (src.Height == 0) return new RectangleF(0, height / 2, width, 0);
            if (src.Width == 0) return new RectangleF(width / 2, 0, 0, height);
            var zoom = Math.Min(width / src.Width, height / src.Height);
            if (zoom == 1) return src;

            //var asp = (double)src.Width / src.Height;
            var sz = new SizeF(src.Width * zoom, src.Height * zoom);
            var offs = new PointF((width - sz.Width) / 2, (height - sz.Height) / 2);
            return new RectangleF(offs, sz);
        }

        public static RectangleF Translate(this RectangleF src, PointF offs) => new RectangleF(src.Location.Translate(offs), src.Size);
        public static RectangleF Translate(this RectangleF src, float x, float y) => new RectangleF(src.Location.Translate(x, y), src.Size);
        public static PointF Translate(this PointF src, PointF offs) => new PointF(src.X + offs.X, src.Y + offs.Y);
        public static PointF Translate(this PointF src, float x, float y) => new PointF(src.X + x, src.Y + y);

        /// <summary>
        /// Check if the given point is inside the shape
        /// </summary>
        /// <param name="shape">bounding shape</param>
        /// <param name="pt">point</param>
        /// <returns>true if inside</returns>
        public static bool IsInside(this PointF pt, IEnumerable<PointF> shape) => shape.Count() >= 3 && shape.Aggregate((inside: false, prev: shape.Last()), (acc, p) => (p.Y < pt.Y && acc.prev.Y >= pt.Y || acc.prev.Y < pt.Y && p.Y >= pt.Y) && p.X + (pt.Y - p.Y) / (acc.prev.Y - p.Y) * (acc.prev.X - p.X) < pt.X ? (!acc.inside, p) : (acc.inside, p)).inside;

    }

}