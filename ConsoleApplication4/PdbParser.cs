using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleApplication4.Models;

namespace ConsoleApplication4
{
  public static class PdbParser
  {
    public static IEnumerable<Atom> GetAtomsFromPdb( string[] pdbfile )
    {
      return pdbfile.Where( line => line.StartsWith( "ATOM" ) || line.StartsWith( "HETATM" ) ).Select( Atom.Parse );
    }

    public static Cryst GetCrystFromPdb( string[] pdbfile )
    {
      return new Cryst(pdbfile.First( line => line.Contains( "CRYST1" ) ));
    }
  }
}
