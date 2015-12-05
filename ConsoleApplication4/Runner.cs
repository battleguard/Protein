using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alea.CUDA;
using ConsoleApplication4.Models;

namespace ConsoleApplication4
{
  class Runner
  {
    public static IEnumerable<BoundingBox> CreateSlices(BoundingBox box, int count)
    {
      double sliceHeight = box.Depth / count;
      for ( int i  = 1; i <= count; i++ )
      {
        var slice = box.Copy();
        if ( i == 1 )
        {
          slice.MinZ = box.MinZ;
          slice.MaxZ = slice.MinZ + sliceHeight;
        }
        else if ( i == count )
        {
          slice.MaxZ = box.MaxX;
          slice.MinZ = slice.MaxZ - sliceHeight;
        }
        else
        {
          slice.MinZ = box.MinZ + ( sliceHeight * i );
          slice.MaxZ = slice.MinZ + sliceHeight;
        }
        slice.Depth = slice.MaxZ - slice.MinZ;
        yield return slice;
      }
    } 

    public static BoundingBox CreateBoundingBox( IEnumerable<Atom> atoms)
    {
      BoundingBox box = new BoundingBox();
      box.MinX = box.MinY = box.MinZ = double.MaxValue;
      box.MaxX = box.MaxY = box.MaxZ = double.MinValue;
      foreach ( var atom in atoms )
      {
        if ( atom.X < box.MinX )
          box.MinX = atom.X;
        if ( atom.Y < box.MinY )
          box.MinY = atom.Y;
        if ( atom.Z < box.MinZ )
          box.MinZ = atom.Z;

        if ( atom.X > box.MaxX )
          box.MaxX = atom.X;
        if ( atom.Y > box.MaxY )
          box.MaxY = atom.Y;
        if ( atom.Z > box.MaxZ )
          box.MaxZ = atom.Z;
      }
      box.MinX -= .000001;
      box.MinY -= .000001;
      box.MinZ -= .000001;
      box.MaxX += .000001;
      box.MaxY += .000001;
      box.MaxZ += .000001;
      box.Width = box.MaxX - box.MinX;
      box.Height = box.MaxY - box.MinY;
      box.Depth = box.MaxZ - box.MinZ;
      return box;
    }

    public static void Main()
    {
      var lines = File.ReadAllLines( @"C:\workspaces\protein\pdb1smd.ent" );
      var sw = Stopwatch.StartNew();
      var atoms = PdbParser.GetAtomsFromPdb( lines ).ToArray();

      var box = CreateBoundingBox( atoms );

      const int numOfSlices = 1000;
      BoundingBox[] slices = CreateSlices( box, numOfSlices ).ToArray();

      foreach ( var atom in atoms )
      {
        var slice = slices.First( s => s.Contains( atom ) );
        slice.Atoms.Add( atom );
      }

      double[] densities = new double[atoms.Length];
      AtomDensityKernelIr( atoms, 0.93, densities );

      Console.WriteLine( sw.ElapsedMilliseconds );
      Console.ReadLine();
      // 4 reference dialectric
    }

        private static void AtomDensityKernelIr( Atom[] atoms, double variance, double[] output )
        {

          Parallel.For( 0, atoms.Length, i =>
          {
            var curAtom = atoms[i];
            for ( int j = 0; j < atoms.Length && i != j; j++ )
            {
              var otherAtom = atoms[j];
              var diffx = curAtom.X - otherAtom.X;
              var diffy = curAtom.Y - otherAtom.Y;
              var diffz = curAtom.Z - otherAtom.Z;
              var distance = ( diffx * diffx ) + ( diffy * diffy ) + ( diffz * diffz );
              output[i] = 1.0 - Math.Exp( ( -1.0 * distance ) / ( ( variance * variance ) * ( otherAtom.Vdw * otherAtom.Vdw ) ) );
            }
          } );
        }
  }
}


