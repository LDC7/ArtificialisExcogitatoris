namespace FaceAdder
{
  using Emgu.CV;
  using Emgu.CV.Structure;
  using System;
  using System.Drawing;
  using System.IO;

  public class FaceWrapper : IDisposable
  {
    public Image<Rgba, byte> Image { get; }
    public Rectangle Rectangle { get; }
    public bool CanBeRotated { get; }
    public double Rotation { get; }
    public string Description { get; }

    public FaceWrapper(string path)
    {
      this.Image = new Image<Rgba, byte>($"{path}.png");
      using (var stream = new FileStream(path, FileMode.Open))
      using (var reader = new StreamReader(stream))
      {
        var fileString = reader.ReadLine();
        var array = fileString.Split(' ');

        this.Rectangle = new Rectangle(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
        if (double.TryParse(array[4], out double degree))
        {
          this.CanBeRotated = true;
          this.Rotation = degree;
        }
        else
        {
          this.CanBeRotated = false;
        }

        this.Description = reader.ReadLine();
      }
    }

    public void Dispose()
    {
      this.Image.Dispose();
    }
  }
}
