﻿using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Providers.Base
{
    public abstract class MahAppsIconProviderModuleBase<TKind, TControl> : IIconProviderModule<TKind>
        where TControl : PackIconControlBase, new()
    {
        public bool Handle(IconDescription iconDescription)
        {
            return iconDescription.Key is TKind;
        }

        public Control GetControl(IconDescription iconDescription, int size)
        {
            var control = new TControl
            {
                Foreground = iconDescription.Brush,
                Width = size - iconDescription.Margin * 2,
                Height = size - iconDescription.Margin * 2,
                Focusable = false
            };

            AssignKindToControl(control, (TKind)iconDescription.Key);
            return control;
        }

        public Bitmap GetBitmap(IconDescription iconDescription, int size)
        {
            using (var memoryStream = new MemoryStream())
            {
                WriteRenderToStream(memoryStream, iconDescription, size);
                return new Bitmap(memoryStream);
            }
        }

        public Uri GetUri(IconDescription iconDescription, int size)
        {
            RuntimeIconsFolder.CreateIfMissing();

            string hash = iconDescription.GetHashCode().ToString("X");
            string filePath = Path.Combine(RuntimeIconsFolder.Path, Path.ChangeExtension(hash, "png"));
            if (File.Exists(filePath))
                return new Uri(filePath);

            using (FileStream fileStream = File.Create(filePath))
            {
                WriteRenderToStream(fileStream, iconDescription, size);
            }

            return new Uri(filePath);
        }

        private void WriteRenderToStream(Stream stream, IconDescription iconDescription, int size)
        {
            string geometryPath = GetGeometryPath((TKind)iconDescription.Key);
            Geometry geometry = Geometry.Parse(geometryPath);

            var geometryDrawing = new GeometryDrawing
            {
                Geometry = geometry,
                Brush = iconDescription.Brush
            };

            double geometryOriginalSize = Math.Max(geometry.Bounds.Width, geometry.Bounds.Height);
            double geometryDesiredSize = size - iconDescription.Margin * 2;

            double geometryRescale = geometryDesiredSize / geometryOriginalSize;
            var scaleTransform = new ScaleTransform(geometryRescale, geometryRescale);

            var drawingGroup = new DrawingGroup
            {
                Children = { geometryDrawing },
                Transform = scaleTransform
            };

            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var rect = new Rect(size / 2.0 - drawingImage.Width / 2, size / 2.0 - drawingImage.Height / 2, drawingImage.Width, drawingImage.Height);
                drawingContext.DrawImage(drawingImage, rect);
            }

            var bitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
        }

        protected abstract void AssignKindToControl(TControl control, TKind kind);
        protected abstract string GetGeometryPath(TKind iconKind);
    }
}