using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication4.Models
{
//  COLUMNS DATA  TYPE FIELD        DEFINITION
//-------------------------------------------------------------------------------------
// 1 -  6        Record name   "ATOM  "
// 7 - 11        Integer serial       Atom serial number.
//13 - 16        Atom name         Atom name.
//17             Character altLoc       Alternate location indicator.
//18 - 20        Residue name  resName Residue name.
//22             Character chainID      Chain identifier.
//23 - 26        Integer resSeq       Residue sequence number.
//27             AChar iCode        Code for insertion of residues.
//31 - 38        Real(8.3)     x Orthogonal coordinates for X in Angstroms.
//39 - 46        Real(8.3)     y Orthogonal coordinates for Y in Angstroms.
//47 - 54        Real(8.3)     z Orthogonal coordinates for Z in Angstroms.
//55 - 60        Real(6.2)     occupancy Occupancy.
//61 - 66        Real(6.2)     tempFactor Temperature  factor.
//77 - 78        LString(2)    element Element symbol, right-justified.
//79 - 80        LString(2)    charge Charge  on the atom.

  public struct Atom
  {
    public char AltLoc;
    public char ChainId;
    public string Charge;
    public double Density;
    public double Dielectric;
    public string Element;
    public char ICode;
    public string InitialString;
    public string Name;
    public double Occupancy;
    public string RecordName;
    public string ResidueName;
    public int ResSeq;
    public int SerialNumber;
    public double TempFactor;
    public double Vdw;
    public double X;
    public double Y;
    public double Z;

    public static Atom Parse( string data )
    {
      var atom = new Atom();
      atom.InitialString = data;

      // 1 -  6        Record name   "ATOM  "
      atom.RecordName = data.Substring( 0, 5 );
      // 7 - 11        Integer serial       Atom serial number.
      atom.SerialNumber = int.Parse( data.Substring( 6, 5 ) );
      //13 - 16        Atom name         Atom name.
      atom.Name = data.Substring( 12, 4 );
      //17             Character altLoc       Alternate location indicator.
      atom.AltLoc = data[16];
      //18 - 20        Residue name  resName Residue name.
      atom.ResidueName = data.Substring( 17, 3 );
      //22             Character chainID      Chain identifier.
      atom.ChainId = data[21];
      //23 - 26        Integer resSeq       Residue sequence number.
      atom.ResSeq = int.Parse( data.Substring( 22, 4 ) );
      //27             AChar iCode        Code for insertion of residues.
      atom.ICode = data[26];
      //31 - 38        Real(8.3)     x Orthogonal coordinates for X in Angstroms.
      atom.X = double.Parse( data.Substring( 30, 8 ) );
      //39 - 46        Real(8.3)     y Orthogonal coordinates for Y in Angstroms.
      atom.Y = double.Parse( data.Substring( 38, 8 ) );
      //47 - 54        Real(8.3)     z Orthogonal coordinates for Z in Angstroms.
      atom.Z = double.Parse( data.Substring( 46, 8 ) );
      //55 - 60        Real(6.2)     occupancy Occupancy.
      atom.Occupancy = double.Parse( data.Substring( 54, 6 ) );
      //61 - 66        Real(6.2)     tempFactor Temperature  factor.
      atom.TempFactor = double.Parse( data.Substring( 60, 6 ) );
      //77 - 78        LString(2)    element Element symbol, right-justified.
      atom.Element = data.Substring( 76, 2 );
      //79 - 80        LString(2)    charge Charge  on the atom.
      atom.Charge = data.Substring( 78, 2 );
      atom.Vdw = GetVdwr( atom.Element );
      return atom;
    }

    private static double GetVdwr( string element )
    {
      switch ( element )
      {
        case "H":
          return 1.2;
        case "ZN":
          return 1.39;
        case "F":
          return 1.47;
        case "O":
          return 1.52;
        case "N":
          return 1.55;
        case "C":
          return 1.7;
        case "S":
          return 1.8;
        default:
          return -1.0;
      }
    }

    public override string ToString()
    {
      return InitialString;
    }

    public GpuAtom ToGpuAtom()
    {
      return new GpuAtom( ResSeq, X, Y, Z );
    }
  }
}