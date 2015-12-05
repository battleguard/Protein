using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4.Models
{
//  COLUMNS DATA  TYPE FIELD          DEFINITION
//-------------------------------------------------------------
// 1 -  6       Record name   "CRYST1"
// 7 - 15       Real(9.3)     a a( Angstroms).
//16 - 24       Real(9.3)     b b( Angstroms).
//25 - 33       Real(9.3)     c c( Angstroms).
//34 - 40       Real(7.2)     alpha alpha( degrees).
//41 - 47       Real(7.2)     beta beta( degrees).
//48 - 54       Real(7.2)     gamma gamma( degrees).
//56 - 66       LString sGroup         Space group.
//67 - 70       Integer z              Z value.
  public struct Cryst
  {
    public Cryst(string line)
    {
      InitialString = line;
      // 1 -  6       Record name   "CRYST1"
      Name = line.Substring( 0, 6 );
      // 7 - 15       Real(9.3)     a a( Angstroms).
      A = double.Parse(line.Substring( 6, 7 ));
      //16 - 24       Real(9.3)     b b( Angstroms).
      B = double.Parse( line.Substring( 15, 7 ) );
      //25 - 33       Real(9.3)     c c( Angstroms).
      C = double.Parse( line.Substring( 24, 7 ) );
      //34 - 40       Real(7.2)     alpha alpha( degrees).
      Alpha = double.Parse( line.Substring( 33, 7 ) );
      //41 - 47       Real(7.2)     beta beta( degrees).
      Beta = double.Parse( line.Substring( 40, 7 ) );
      //48 - 54       Real(7.2)     gamma gamma( degrees).
      Gamma = double.Parse( line.Substring( 47, 7 ) );
      //56 - 66       LString sGroup         Space group.
      Space = line.Substring( 55, 11 );
      //67 - 70       Integer z              Z value.
      Z = int.Parse( line.Substring( 66, 4 ) );
    }

    public readonly string InitialString;
    public readonly string Name;
    public readonly double A;
    public readonly double B;
    public readonly double C;
    public readonly double Alpha;
    public readonly double Beta;
    public readonly double Gamma;
    public readonly string Space;
    public readonly int Z;
  }
}
