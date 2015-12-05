using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication4.Models;

namespace ConsoleApplication4
{
  public class BoundingBox
  {
    public List<Atom> Atoms = new List<Atom>();
    public double Depth;
    public double Height;
    public double MaxX;
    public double MaxY;
    public double MaxZ;
    public double MinX;
    public double MinY;
    public double MinZ;
    public double Width;

    public BoundingBox Copy()
    {
      var box = new BoundingBox();
      box.Width = Width;
      box.Height = Height;
      box.Depth = Depth;
      box.MinX = MinX;
      box.MinY = MinY;
      box.MinZ = MinZ;
      box.MaxX = MaxX;
      box.MaxY = MaxY;
      box.MaxZ = MaxZ;
      return box;
    }

    public bool Contains( Atom atom )
    {
      return atom.X > MinX && atom.X < MaxX &&
             atom.Y > MinY && atom.Y < MaxY &&
             atom.Z > MinZ && atom.Z < MaxZ;
    }

    public bool Contains( Vec3 vector )
    {
      return vector.X > MinX && vector.X < MaxX &&
             vector.Y > MinY && vector.Y < MaxY &&
             vector.Z > MinZ && vector.Z < MaxZ;
    }

    public override string ToString()
    {
      return $"MinX {MinX} MaxX {MaxX}" +
             $"MinY {MinY} MaxY {MaxY}" +
             $"MinZ {MinZ} MaxZ {MaxZ}" +
             $"Width {Width} Height {Height} Depth {Depth} Atoms {Atoms.Count}";
    }
  }
}