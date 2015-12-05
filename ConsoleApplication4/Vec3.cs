using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication4
{
  public struct Vec3
  {
    public Vec3( double x, double y, double z )
    {
      X = x;
      Y = y;
      Z = z;
    }

    public double X;
    public double Y;
    public double Z;
  }
}