using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ConsoleApplication4.Models;

namespace ConsoleApplication4
{
  internal class Runner
  {
    public static IEnumerable<BoundingBox> CreateSlices( BoundingBox box, int count )
    {
      var sliceHeight = box.Depth / count;
      for ( var i = 1; i <= count; i++ )
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

    public static BoundingBox CreateBoundingBox( IEnumerable<Atom> atoms )
    {
      var box = new BoundingBox();
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
      atoms = atoms.Where( atom => atom.Vdw != -1.0 ).ToArray();

      var box = CreateBoundingBox( atoms );
      // y z plane
//
      const int numOfSlices = 1000;
//      BoundingBox[] slices = CreateSlices( box, numOfSlices ).ToArray();
//
//      foreach ( var atom in atoms )
//      {
//        var slice = slices.First( s => s.Contains( atom ) );
//        slice.Atoms.Add( atom );
//      }

      var densities = GetGaussianDensities( atoms, 0.93 * 0.93 );
      var dielectrics = SmoothWithDielectrics( densities, 4, 80.4 );

      Console.WriteLine( "My formula" );
      Console.WriteLine( "Your formula" );

      Console.ReadLine();
      // 4 reference dialectric
    }

    private static double[] SmoothWithDielectrics( double[] densities, double referenceDielectricValue, double waterDielectricValue )
    {
      return densities.Select( d => SmoothDielectric( d, referenceDielectricValue, waterDielectricValue ) ).ToArray();
    }

    private static double SmoothDielectric( double density, double referenceDielectricValue, double waterDielectricValue )
    {
      return density * referenceDielectricValue + ( 1 - density ) * waterDielectricValue;
    }

    private static double[] GetGaussianDensities( Atom[] atoms, double varianceSquared )
    {
      var densities = new double[atoms.Length];

      for ( var i = 0; i < atoms.Length; i++ )
      {
        densities[i] = 1.0;
        for ( var j = 0; j < atoms.Length; j++ )
        {
          if ( i == j )
            continue;
          densities[i] *= GetGaussianDensity( atoms[i], atoms[j], varianceSquared );
        }
      }
      return densities;
    }

    private static double GetGaussianDensity( Atom atom1, Atom atom2, double varianceSquared )
    {
      var diffx = atom1.X - atom2.X;
      var diffy = atom1.Y - atom2.Y;
      var diffz = atom1.Z - atom2.Z;

      var distance = ( diffx * diffx ) + ( diffy * diffy ) + ( diffz * diffz );
      return 1.0 - Math.Exp( ( -1.0 * distance ) / ( ( varianceSquared ) * ( atom2.VdwSquared ) ) );
    }
  }
}