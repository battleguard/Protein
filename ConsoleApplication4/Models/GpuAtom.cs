using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4.Models
{
  public struct GpuAtom
  {
    public double ReqSeq;
    public double X;
    public double Y;
    public double Z;

    public GpuAtom( double reqSeq, double x, double y, double z )
    {
      ReqSeq = reqSeq;
      X = x;
      Z = z;
      Y = y;
    }
  }
}
