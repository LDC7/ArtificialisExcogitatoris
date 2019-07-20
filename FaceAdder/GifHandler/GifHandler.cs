namespace FaceAdder
{
    using Emgu.CV;
    using Emgu.CV.Structure;
    using Gifed;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class GifHandler : IDisposable
    {
        private const double sameScenePercent = 0.2;
        private readonly List<GifSceneWrapper> gifScenes;

        internal GifHandler(AnimatedGif gif)
        {
            this.gifScenes = new List<GifSceneWrapper>();
            var bufScene = new GifSceneWrapper();
            bufScene.AddFrame(gif[0]);

            for (int i = 1; i < gif.FrameCount; i++)
            {
                if (SameScene(bufScene.Frames[bufScene.Frames.Count - 1].Frame, gif[i]))
                {
                    bufScene.AddFrame(gif[i]);
                }
                else
                {
                    this.gifScenes.Add(bufScene);
                    bufScene = new GifSceneWrapper();
                    bufScene.AddFrame(gif[i]);
                }
            }
            this.gifScenes.Add(bufScene);

            Console.WriteLine($"GIF: Scenes count: {this.gifScenes.Count}");
        }

        private static bool SameScene(GifFrame frame1, GifFrame frame2)
        {
#warning СЛИШКОМ ДОЛГО. НУЖЕН БЫСТРЫЙ СПОСОБ

            double sum = 0;
            using (var image1 = new Image<Rgba, byte>((Bitmap)frame1.Image))
            using (var image2 = new Image<Rgba, byte>((Bitmap)frame2.Image))
            {
                for (int w = 0; w < image1.Width; w++)
                {
                    for (int h = 0; h < image1.Height; h++)
                    {
                        sum += (Math.Abs(GetPixelSum(image1[h, w]) - GetPixelSum(image2[h, w])) / 255);
                    }
                }
                var dispersion = sum / (image1.Width * image1.Height * 3);
                return dispersion < sameScenePercent;
            }
        }

        private static double GetPixelSum(Rgba pixel)
        {
            return pixel.Red + pixel.Green + pixel.Blue;
        }

        internal AnimatedGif GetFacedGif()
        {
            this.DeleteRandomFaces();
            this.PlotFaces();
            this.AddFaceToScenes();
            return this.UniteScenes();
        }

        private void DeleteRandomFaces()
        {
            foreach (var scene in this.gifScenes)
            {
                scene.DeleteExcessFaces();
            }
        }

        private void PlotFaces()
        {
            foreach (var scene in this.gifScenes)
            {
                scene.AddBoundFaces();
            }
        }

        private void AddFaceToScenes()
        {
            foreach (var scene in this.gifScenes)
            {
                scene.AddFaceToScene();
            }
        }

        private AnimatedGif UniteScenes()
        {
            var gif = new AnimatedGif();
            bool sizeChanging = false;
            Size size = this.gifScenes[0].Frames[0].Frame.Image.Size;
            if (size.Width > 500)
            {
                double coof = 500 / size.Width;
                size = new Size((int)(size.Width * coof), (int)(size.Height * coof));
                sizeChanging = true;
            }
            if (size.Height > 500)
            {
                double coof = 500 / size.Height;
                size = new Size((int)(size.Width * coof), (int)(size.Height * coof));
                sizeChanging = true;
            }

            foreach (var scene in this.gifScenes)
            {
                foreach (var frame in scene.Frames)
                {
                    if (sizeChanging)
                    {
                        gif.AddFrames(new GifFrame(new Bitmap(frame.Frame.Image, size), frame.Frame.Delay));
                    }
                    else
                    {
                        gif.AddFrames(frame.Frame);
                    }                    
                }
            }

            return gif;
        }

        public void Dispose()
        {
            foreach (var scene in this.gifScenes)
            {
                scene.Dispose();
            }
        }
    }
}
