


//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using Alea.CUDA;
//using Alea.CUDA.Utilities;
//using System.Diagnostics;
//using System.Threading;
//
//namespace ProteinDielectricCalculator
//{
//  public class Form1
//  {
//    private string pdbpath;
//    List<Atom> atoms;
//    private double[] pdbbounds = new double[6];
//
//    public struct GridPoint
//    {
//      public double x;
//      public double y;
//      public double z;
//
//      public GridPoint( double x, double y, double z )
//      {
//        this.x = x;
//        this.y = y;
//        this.z = z;
//      }
//    }
//
//    public struct GPUAtom
//    {
//      public int resid;
//      public double x;
//      public double y;
//      public double z;
//      public double vdw;
//
//      public GPUAtom( int resid, double x, double y, double z, double vdw )
//      {
//        this.resid = resid;
//        this.x = x;
//        this.y = y;
//        this.z = z;
//        this.vdw = vdw;
//      }
//    }
//
//    class Atom
//    {
//      public int serial;
//      public string name;
//      public char altLoc;
//      public string resName;
//      public char chainID;
//      public int resSeq;
//      public char iCode;
//      public double x;
//      public double y;
//      public double z;
//      public double occupancy;
//      public double tempFactor;
//      public string element;
//      public string charge;
//      public double vdw;
//      public double density;
//      public double dielectric;
//
//      public Atom( int serial, string name, char altLoc, string resName, char chainID, int resSeq, char iCode, double x, double y, double z, double occupancy, double tempFactor, string element, string charge, double vdw )
//      {
//        this.serial = serial;
//        this.name = name;
//        this.altLoc = altLoc;
//        this.resName = resName;
//        this.chainID = chainID;
//        this.resSeq = resSeq;
//        this.iCode = iCode;
//        this.x = x;
//        this.y = y;
//        this.z = z;
//        this.occupancy = occupancy;
//        this.tempFactor = tempFactor;
//        this.element = element;
//        this.charge = charge;
//        this.vdw = vdw;
//      }
//
//      public string getTabbedInfo()
//      {
//        return serial + "\t" + name + "\t" + altLoc + "\t" + resName + "\t" + chainID + "\t" + resSeq + "\t" + iCode + "\t" + x + "\t" + y + "\t" + z + "\t" + occupancy + "\t" + tempFactor + "\t" + element + "\t" + charge;
//      }
//
//      public GPUAtom getGPUAtom()
//      {
//        return new GPUAtom( this.resSeq, this.x, this.y, this.z, this.vdw );
//      }
//    }
//
//    private void buttonBrowse_Click( object sender, EventArgs e )
//    {
//      //Opens file dialog and loads selected .pdb file
//      OpenFileDialog openFileDialog1 = new OpenFileDialog();
//      openFileDialog1.Filter = "PDB File|*.pdb";
//      openFileDialog1.Multiselect = false;
//      DialogResult result = openFileDialog1.ShowDialog();
//      if ( result == DialogResult.OK )
//      {
//        pdbpath = openFileDialog1.FileName;
//        textBoxPDBPath.Text = pdbpath;
//        using ( var filestream = openFileDialog1.OpenFile() )
//        {
//          if ( filestream == null )  //Exit if file was never selected
//          {
//            toolStripStatusLabel.Text = "ERROR: No file loaded!";
//            return;
//          }
//          if ( filestream.Length == 0 ) //Exit if .pdb is empty
//          {
//            toolStripStatusLabel.Text = "ERROR: File has no data!";
//            return;
//          }
//          if ( !( checkBoxATOM.Checked || checkBoxHETATM.Checked ) ) //Exit if no coordinates will be found
//          {
//            toolStripStatusLabel.Text = "ERROR: No records selected!";
//            return;
//          }
//          string[] pdbfile = File.ReadAllLines( pdbpath );
//          atoms = getAtomsFromPDB( pdbfile );
//          pdbbounds[0] = Double.MaxValue;
//          pdbbounds[2] = Double.MaxValue;
//          pdbbounds[4] = Double.MaxValue;
//          pdbbounds[1] = Double.MinValue;
//          pdbbounds[3] = Double.MinValue;
//          pdbbounds[5] = Double.MinValue;
//          for ( int i = 0; i < atoms.Count; i++ )
//          {
//            if ( atoms[i].x < pdbbounds[0] )
//              pdbbounds[0] = atoms[i].x;
//            if ( atoms[i].y < pdbbounds[2] )
//              pdbbounds[2] = atoms[i].y;
//            if ( atoms[i].z < pdbbounds[4] )
//              pdbbounds[4] = atoms[i].z;
//            if ( atoms[i].x > pdbbounds[1] )
//              pdbbounds[1] = atoms[i].x;
//            if ( atoms[i].y > pdbbounds[3] )
//              pdbbounds[3] = atoms[i].y;
//            if ( atoms[i].z > pdbbounds[5] )
//              pdbbounds[5] = atoms[i].z;
//          }
//        }
//        toolStripStatusLabel.Text = String.Format( "Ready to calculate( Size[x,y,z] = [{0},{1},{2}] ).", pdbbounds[1] - pdbbounds[0], pdbbounds[3] - pdbbounds[2], pdbbounds[5] - pdbbounds[4] );
//      }
//    }
//
//    private void radioButtonDefined_CheckedChanged( object sender, EventArgs e )
//    {
//      if ( radioButtonDefined.Checked )
//      {
//        textBoxSpecifiedAtoms.ReadOnly = false;
//      }
//      else
//      {
//        textBoxSpecifiedAtoms.ReadOnly = true;
//
//      }
//    }
//
//    private void textBoxSpecifiedAtoms_KeyPress( object sender, KeyPressEventArgs e )
//    {
//      if ( ( e.KeyChar < 48 || e.KeyChar > 57 ) && ( e.KeyChar != 8 && e.KeyChar != 44 ) )
//      {
//        e.Handled = true;
//      }
//    }
//
//    private void textBoxVariance_KeyPress( object sender, KeyPressEventArgs e )
//    {
//      if ( ( e.KeyChar < 48 || e.KeyChar > 57 ) && ( e.KeyChar != 8 && e.KeyChar != 46 ) )
//      {
//        e.Handled = true;
//      }
//    }
//
//    private void textBoxReferenceDielectric_KeyPress( object sender, KeyPressEventArgs e )
//    {
//      if ( ( e.KeyChar < 48 || e.KeyChar > 57 ) && ( e.KeyChar != 8 && e.KeyChar != 46 ) )
//      {
//        e.Handled = true;
//      }
//    }
//
//    private double getVDWR( string element )
//    {
//      switch ( element )
//      {
//        case "H":
//          return 1.2;
//        case "ZN":
//          return 1.39;
//        case "F":
//          return 1.47;
//        case "O":
//          return 1.52;
//        case "N":
//          return 1.55;
//        case "C":
//          return 1.7;
//        case "S":
//          return 1.8;
//        default:
//          return -1.0;
//      }
//    }
//
//
//    private double calculateDistance( double x, double y, double z, double i, double j, double k )
//    {
//      return Math.Sqrt( Math.Pow( x - i, 2 ) + Math.Pow( y - j, 2 ) + Math.Pow( z - k, 2 ) );
//    }
//
//    private double calculateAtomDensity( List<Atom> atoms, double x, double y, double z, double variance )
//    {
//      double density = 1.0;
//      for ( int i = 0; i < atoms.Count; i++ )
//      {
//        double distance = calculateDistance( x, y, z, atoms[i].x, atoms[i].y, atoms[i].z );
//        //double distance = Math.Sqrt(Math.Pow(x - atoms[i].x,2) + Math.Pow(y - atoms[i].y,2) + Math.Pow(z - atoms[i].z,2));
//        density *= ( 1.0 - Math.Exp( ( -1.0 * Math.Pow( distance, 2 ) ) / ( Math.Pow( variance, 2 ) * Math.Pow( atoms[i].vdw, 2 ) ) ) );
//      }
//      return ( 1.0 - density );
//    }
//
//    private double calculateAtomDensity( List<Atom> atoms, double x, double y, double z, double variance, int ignoreIndex )
//    {
//      double density = 1.0;
//      for ( int i = 0; i < atoms.Count; i++ )
//      {
//        if ( i != ignoreIndex )
//        {
//          double distance = calculateDistance( x, y, z, atoms[i].x, atoms[i].y, atoms[i].z );
//          double posdensity = Math.Exp( ( -1.0 * Math.Pow( distance, 2 ) ) / ( Math.Pow( variance, 2 ) * Math.Pow( atoms[i].vdw, 2 ) ) );
//          density *= 1.0 - posdensity;
//        }
//      }
//      return ( 1.0 - density );
//    }
//
//    private void buttonRunCalculation_Click( object sender, EventArgs e )
//    {
//      if ( string.IsNullOrWhiteSpace( textBoxReferenceDielectric.Text ) )
//      {
//        toolStripStatusLabel.Text = "ERROR: No reference dielectric value!";
//        return;
//      }
//      if ( string.IsNullOrWhiteSpace( textBoxVariance.Text ) )
//      {
//        toolStripStatusLabel.Text = "ERROR: No relative variance value!";
//        return;
//      }
//      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
//      saveFileDialog1.Filter = "Log file|*.log";
//      DialogResult result = saveFileDialog1.ShowDialog();
//
//      if ( result != DialogResult.OK )  //Exit if logfile wasn't OK'd
//      {
//        return;
//      }
//      //Setup initial log file
//      string logfile = saveFileDialog1.FileName;
//      File.WriteAllLines( logfile, logheader );
//
//      //Find all atoms
//      if ( radioButtonDefined.Checked )
//      {
//        //TODO: Parse textbox code, confirm no numbers are out of bounds, and run selective analysis.
//      }
//
//      double variance = double.Parse( textBoxVariance.Text );
//      double refdielectric = double.Parse( textBoxReferenceDielectric.Text );
//
//      if ( checkBoxGPUAcceleration.Checked )
//      {
//        File.AppendAllText( logfile, "(GPU) Calculating densities at all atom centers...\n" );
//        toolStripStatusLabel.Text = "(GPU) Calculating densities at all atom centers...";
//        GPUAtom[] gpuatoms = new GPUAtom[atoms.Count];
//        for ( int i = 0; i < gpuatoms.Length; i++ )
//        {
//          gpuatoms[i] = atoms[i].getGPUAtom();
//        }
//
//        using ( var dinputs = worker.Malloc( gpuatoms ) )
//        using ( var ddensitymatrix = worker.Malloc<double>( gpuatoms.Length * gpuatoms.Length ) )
//        using ( var doutputs = worker.Malloc<double>( gpuatoms.Length ) )
//        {
//          int blockDim = (int)Math.Sqrt( worker.Device.Attributes.MAX_THREADS_PER_BLOCK );
//          var blockSize = new dim3( blockDim, blockDim );
//          var gridSize = new dim3( ( blockDim - 1 + gpuatoms.Length ) / blockDim, ( blockDim - 1 + gpuatoms.Length ) / blockDim );
//          var lp = new LaunchParam( gridSize, blockSize );
//          var blockSize2 = worker.Device.Attributes.MAX_THREADS_PER_BLOCK;
//          var gridSize2 = Math.Min( 16 * worker.Device.Attributes.MULTIPROCESSOR_COUNT, Common.divup( gpuatoms.Length, blockSize2 ) );
//          var lp2 = new LaunchParam( gridSize2, blockSize2 );
//          if ( checkBoxIgnoreResidueAtoms.Checked )
//          {
//            worker.Launch( atomDensityKernelIR, lp, ddensitymatrix.Ptr, dinputs.Ptr, Double.Parse( textBoxVariance.Text ), gpuatoms.Length );
//            worker.Launch( dielectricKernelIR, lp2, doutputs.Ptr, ddensitymatrix.Ptr, dinputs.Ptr, Double.Parse( textBoxReferenceDielectric.Text ), 80.4, gpuatoms.Length );
//          }
//          else
//          {
//            worker.Launch( atomDensityKernel, lp, ddensitymatrix.Ptr, dinputs.Ptr, Double.Parse( textBoxVariance.Text ), gpuatoms.Length );
//            worker.Launch( dielectricKernel, lp2, doutputs.Ptr, ddensitymatrix.Ptr, Double.Parse( textBoxReferenceDielectric.Text ), 80.4, gpuatoms.Length );
//          }
//          var gpuout = doutputs.Gather();
//          for ( int i = 0; i < atoms.Count; i++ )
//          {
//            atoms[i].dielectric = gpuout[i];
//          }
//        }
//      }
//      else
//      {
//        //Calculate density at each atom center
//        File.AppendAllText( logfile, "Calculating densities at all atom centers...\n" );
//        toolStripStatusLabel.Text = "Calculating densities at all atom centers...";
//        toolStripProgressBar.Minimum = 0;
//        toolStripProgressBar.Maximum = atoms.Count - 1;
//        statusStrip1.Update();
//
//        for ( int i = 0; i < atoms.Count; i++ )
//        {
//          toolStripProgressBar.Value = i;
//          statusStrip1.Update();
//          atoms[i].density = calculateAtomDensity( atoms, atoms[i].x, atoms[i].y, atoms[i].z, variance, i );
//          atoms[i].dielectric = ( atoms[i].density * refdielectric ) + ( ( 1.0 - atoms[i].density ) * 80.4 );
//        }
//      }
//
//      //Write output file
//      double avgdielectric = 0.0;
//      double maxdielectric = 0.0;
//      double mindielectric = double.MaxValue;
//      using ( StreamWriter file = new StreamWriter( logfile, true ) )
//      {
//        for ( int i = 0; i < atoms.Count; i++ )
//        {
//          file.Write( "Atom " + atoms[i].serial + "(" + atoms[i].element + ") dielectric: \t\t" + atoms[i].dielectric + "\n" );
//          if ( atoms[i].dielectric > maxdielectric )
//          {
//            maxdielectric = atoms[i].dielectric;
//          }
//          if ( atoms[i].dielectric < mindielectric )
//          {
//            mindielectric = atoms[i].dielectric;
//          }
//          avgdielectric += atoms[i].dielectric;
//        }
//        avgdielectric = avgdielectric / (double)atoms.Count;
//        file.Write( "Average dielectric: " + ( avgdielectric / atoms.Count ) + "\nMinimum value: " + mindielectric + "\nMaximum value: " + maxdielectric + "\n" );
//      }
//      toolStripProgressBar.Value = 0;
//      statusStrip1.Update();
//
//      if ( checkBoxWritePYMOLScript.Checked )
//      {
//        File.AppendAllText( logfile, "Writing PYMOL coloring script...\n" );
//        File.AppendAllText( logfile, "Heatmap Key: BLUE (e= " + mindielectric + ") -> CYAN -> GREEN -> YELLOW -> RED (e= " + maxdielectric + ")" );
//        toolStripStatusLabel.Text = "Writing PYMOL coloring script...";
//        statusStrip1.Update();
//        float[] hmap = new float[3];
//        string pymolfile = Path.GetDirectoryName( logfile ) + "\\" + Path.GetFileNameWithoutExtension( logfile ) + "_colorscript.py";
//        if ( File.Exists( pymolfile ) )
//        {
//          File.Delete( pymolfile );
//        }
//        using ( StreamWriter file = new StreamWriter( pymolfile, true ) )
//        {
//          for ( int i = 0; i < atoms.Count; i++ )
//          {
//            toolStripProgressBar.Value = i / atoms.Count;
//            statusStrip1.Update();
//            var percentage = ( maxdielectric - atoms[i].dielectric ) / ( maxdielectric - mindielectric );
//            hmap = getHeatMapColor( ( maxdielectric - atoms[i].dielectric ) / ( maxdielectric - mindielectric ) );
//            file.WriteLine( "cmd.set_color(\"atom" + atoms[i].serial + "\",[" + hmap[0] + "," + hmap[1] + "," + hmap[2] + "])" );
//            file.WriteLine( "cmd.color(\"atom" + atoms[i].serial + "\",\"v. and id " + atoms[i].serial + "\")" );
//          }
//        }
//        Bitmap bmp = new Bitmap( 600, 50 );
//        float[] heatmap = new float[3];
//        for ( double i = 0.0; i < 600.0; i += 1.0 )
//        {
//          heatmap = getHeatMapColor( i / 600.0 );
//          for ( int j = 0; j < 50; j++ )
//          {
//            bmp.SetPixel( (int)i, j, Color.FromArgb( (int)( heatmap[0] * 255.0 ), (int)( heatmap[1] * 255.0 ), (int)( heatmap[2] * 255.0 ) ) );
//          }
//        }
//        bmp.Save( Path.GetDirectoryName( logfile ) + "\\heatmap_key.bmp", ImageFormat.Bmp );
//        bmp.Dispose();
//      }
//      toolStripStatusLabel.Text = "Done!";
//      toolStripProgressBar.Value = 0;
//      statusStrip1.Update();
//    }
//
//    private float[] getHeatMapColor( double value )
//    {
//      const int NUM_COLORS = 5;
//      float[,] color = new float[,] { { 0.0f, 0.0f, 1.0f }, { 0.0f, 1.0f, 1.0f }, { 0.0f, 1.0f, 0.0f }, { 1.0f, 1.0f, 0.0f }, { 1.0f, 0.0f, 0.0f } };
//      int idx1;
//      int idx2;
//      float fractBetween = 0;
//      if ( value <= 0 )
//      {
//        idx1 = 0;
//        idx2 = 0;
//      }
//      else
//      {
//        if ( value >= 1 )
//        {
//          idx1 = NUM_COLORS - 1;
//          idx2 = NUM_COLORS - 2;
//        }
//        else
//        {
//          value = value * ( NUM_COLORS - 1 );
//          idx1 = (int)Math.Floor( value );
//          idx2 = idx1 + 1;
//          fractBetween = (float)value - (float)( idx1 );
//        }
//      }
//      float[] heatmap = new float[3];
//      heatmap[0] = ( color[idx2, 0] - color[idx1, 0] ) * fractBetween + color[idx1, 0];
//      heatmap[1] = ( color[idx2, 1] - color[idx1, 1] ) * fractBetween + color[idx1, 1];
//      heatmap[2] = ( color[idx2, 2] - color[idx1, 2] ) * fractBetween + color[idx1, 2];
//      return heatmap;
//    }
//
//    private Worker worker;
//    private void checkBoxGPUAcceleration_CheckedChanged( object sender, EventArgs e )
//    {
//
//      if ( checkBoxGPUAcceleration.Checked )
//      {
//        worker = Worker.Default;
//        textBoxGPUInfo.Text = worker.Device.Name + " (Mem: " + ( worker.Device.TotalMemory / 1048576.0 ) + " MB, Cores: " + worker.Device.Cores + ")";
//        groupBoxSliceMethod.Enabled = true;
//      }
//      else
//      {
//        textBoxGPUInfo.Text = string.Empty;
//        groupBoxSliceMethod.Enabled = false;
//      }
//    }
//
//    [AOTCompile]
//    private static void atomDensityKernel( deviceptr<double> doutputs, deviceptr<GPUAtom> dinputs, double variance, int n )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      var j = blockIdx.y * blockDim.y + threadIdx.y;
//      var stridex = gridDim.x * blockDim.x;
//      var stridey = gridDim.y * blockDim.y;
//      if ( i < n && j < n && i != j )
//      {
//        var diffx = dinputs[i].x - dinputs[j].x;
//        var diffy = dinputs[i].y - dinputs[j].y;
//        var diffz = dinputs[i].z - dinputs[j].z;
//        var distance = ( diffx * diffx ) + ( diffy * diffy ) + ( diffz * diffz );
//        doutputs[( j * n ) + i] = 1.0 - Math.Exp( ( -1.0 * distance ) / ( ( variance * variance ) * ( dinputs[j].vdw * dinputs[j].vdw ) ) );
//      }
//    }
//
//    [AOTCompile]
//    private static void dielectricKernel( deviceptr<double> doutputs, deviceptr<double> dinputs, double refdielectric, double outdielectric, int n )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      if ( i >= n )
//        return;
//      var moldensity = 1.0;
//      for ( var j = 0; j < n; j++ )
//      {
//        if ( i != j )
//          moldensity *= dinputs[( j * n ) + i];
//      }
//      doutputs[i] = ( ( 1.0 - moldensity ) * refdielectric ) + ( moldensity * outdielectric );
//    }
//
//    [AOTCompile]
//    private static void atomDensityKernelIR( deviceptr<double> doutputs, deviceptr<GPUAtom> dinputs, double variance, int n )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      var j = blockIdx.y * blockDim.y + threadIdx.y;
//      var stridex = gridDim.x * blockDim.x;
//      var stridey = gridDim.y * blockDim.y;
//      if ( i < n && j < n && dinputs[i].resid != dinputs[j].resid )
//      {
//        var diffx = dinputs[i].x - dinputs[j].x;
//        var diffy = dinputs[i].y - dinputs[j].y;
//        var diffz = dinputs[i].z - dinputs[j].z;
//        var distance = ( diffx * diffx ) + ( diffy * diffy ) + ( diffz * diffz );
//        doutputs[( j * n ) + i] = 1.0 - Math.Exp( ( -1.0 * distance ) / ( ( variance * variance ) * ( dinputs[j].vdw * dinputs[j].vdw ) ) );
//      }
//    }
//
//    [AOTCompile]
//    private static void dielectricKernelIR( deviceptr<double> doutputs, deviceptr<double> dinputmatrix, deviceptr<GPUAtom> dinputs, double refdielectric, double outdielectric, int n )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      if ( i >= n )
//        return;
//      var moldensity = 1.0;
//      for ( var j = 0; j < n; j++ )
//      {
//        if ( dinputs[i].resid != dinputs[j].resid )
//          moldensity *= dinputmatrix[( j * n ) + i];
//      }
//      // ref dialectric = 4
        // outdielectric = 80.4
        // output = (1.0 - density) * 4 + (density * 80.4)
//      doutputs[i] = ( ( 1.0 - moldensity ) * refdielectric ) + ( moldensity * outdielectric );
//    }
//
//    private void Form1_KeyDown( object sender, KeyEventArgs e )
//    {
//      if ( ( e.Modifiers == Keys.Control ) && ( e.KeyCode == Keys.D ) )
//      {
//        DebugForm debugform = new DebugForm();
//        debugform.Show();
//      }
//    }
//
//    private List<Atom> getAtomsFromPDB( string[] pdbfile )
//    {
//      List<Atom> atoms = new List<Atom>();
//      foreach ( string line in pdbfile )
//      {
//        if ( ( line.Contains( "ATOM" ) && checkBoxATOM.Checked ) || ( line.Contains( "HETATM" ) && checkBoxHETATM.Checked ) )
//        {
//          int serial;
//          if ( !int.TryParse( line.Substring( 6, 5 ), out serial ) )
//            serial = 0;
//          string name = line.Substring( 12, 4 ).Trim();
//          char altLoc = line[16];
//          string resName = line.Substring( 17, 3 ).Trim();
//          char chainID = line[21];
//          int resSeq;
//          if ( !int.TryParse( line.Substring( 22, 4 ), out resSeq ) )
//            resSeq = 0;
//          char iCode = line[26];
//          double x;
//          if ( !double.TryParse( line.Substring( 30, 8 ), out x ) )
//            x = 0;
//          double y;
//          if ( !double.TryParse( line.Substring( 38, 8 ), out y ) )
//            y = 0;
//          double z;
//          if ( !double.TryParse( line.Substring( 46, 8 ), out z ) )
//            z = 0;
//          double occupancy;
//          if ( !double.TryParse( line.Substring( 54, 6 ), out occupancy ) )
//            occupancy = 0;
//          double tempFactor;
//          if ( !double.TryParse( line.Substring( 60, 6 ), out tempFactor ) )
//            tempFactor = 0;
//          string element = line.Substring( 76, 2 ).Trim();
//          string charge = line.Substring( 78, 2 ).Trim();
//          double vdw = getVDWR( element );
//          if ( vdw != -1.0 )
//          {
//            atoms.Add( new Atom( serial, name, altLoc, resName, chainID, resSeq, iCode, x, y, z, occupancy, tempFactor, element, charge, vdw ) );
//          }
//        }
//      }
//      return atoms;
//    }
//
//    private void numericAxisResolution_ValueChanged( object sender, EventArgs e )
//    {
//      int iterationsRequired = (int)Math.Ceiling( ( numericAxisResolution.Value * numericAxisResolution.Value ) / 10000.0m );
//      textBoxIterationsRequired.Text = String.Format( "{0}", iterationsRequired );
//    }
//
//    private void numericSlices_ValueChanged( object sender, EventArgs e )
//    {
//      int iterationsRequired = (int)Math.Ceiling( ( numericAxisResolution.Value * numericAxisResolution.Value ) / 10000.0m );
//      textBoxIterationsRequired.Text = String.Format( "{0}", iterationsRequired );
//    }
//
//    private void buttonMakeSlices_Click( object sender, EventArgs e )
//    {
//      if ( string.IsNullOrWhiteSpace( textBoxReferenceDielectric.Text ) )
//      {
//        toolStripStatusLabel.Text = "ERROR: No reference dielectric value!";
//        return;
//      }
//      if ( string.IsNullOrWhiteSpace( textBoxVariance.Text ) )
//      {
//        toolStripStatusLabel.Text = "ERROR: No relative variance value!";
//        return;
//      }
//      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
//      saveFileDialog1.Filter = "Log file|*.log";
//      DialogResult result = saveFileDialog1.ShowDialog();
//
//      if ( result != DialogResult.OK )  //Exit if logfile wasn't OK'd
//      {
//        return;
//      }
//      //Setup initial log file
//      string logfile = saveFileDialog1.FileName;
//      File.WriteAllLines( logfile, logheader );
//
//      double variance = double.Parse( textBoxVariance.Text );
//      double refdielectric = double.Parse( textBoxReferenceDielectric.Text );
//      int resint = (int)numericAxisResolution.Value;
//
//      int iterationsRequired = (int)Math.Ceiling( ( numericSlices.Value * numericAxisResolution.Value ) / 10000.0m );
//      GridPoint[] gridpoints = new GridPoint[(int)( numericAxisResolution.Value * numericAxisResolution.Value )];
//      double xspan = ( pdbbounds[0] - pdbbounds[1] ) * 1.1;
//      double yspan = ( pdbbounds[2] - pdbbounds[3] ) * 1.1;
//      double zspan = ( pdbbounds[4] - pdbbounds[5] ) * 1.1;
//      double[] boxcenter = new double[3];
//      boxcenter[0] = pdbbounds[0] - ( xspan / 2.0 );
//      boxcenter[1] = pdbbounds[2] - ( yspan / 2.0 );
//      boxcenter[2] = pdbbounds[4] - ( yspan / 2.0 );
//      for ( int i = 0; i < numericSlices.Value; i++ )
//      {
//        if ( radioButtonSliceX.Checked )
//        {
//          double pointstep = Math.Max( yspan, zspan ) / (double)numericAxisResolution.Value;
//          for ( int y = 0; y < numericAxisResolution.Value / 2; y++ )
//          {
//            for ( int z = 0; z < numericAxisResolution.Value / 2; z++ )
//            {
//
//              gridpoints[( y * resint ) + z] = new GridPoint( i, y, z );
//            }
//          }
//        }
//        if ( radioButtonSliceY.Checked )
//        {
//          double pointstep = Math.Max( xspan, zspan ) / (double)numericAxisResolution.Value;
//
//
//        }
//        if ( radioButtonSliceZ.Checked )
//        {
//          double pointstep = Math.Max( yspan, xspan ) / (double)numericAxisResolution.Value;
//
//        }
//      }
//    }
//
//    [AOTCompile]
//    private static void sliceDensityKernel( deviceptr<double> doutputmatrix, deviceptr<GPUAtom> dinputatoms, deviceptr<GridPoint> dgridpoints, double variance, int atomcount, int gridcount )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      var j = blockIdx.y * blockDim.y + threadIdx.y;
//      if ( i < gridcount && j < atomcount && i != j )
//      {
//        var diffx = dgridpoints[i].x - dinputatoms[j].x;
//        var diffy = dgridpoints[i].y - dinputatoms[j].y;
//        var diffz = dgridpoints[i].z - dinputatoms[j].z;
//        var distance = ( diffx * diffx ) + ( diffy * diffy ) + ( diffz * diffz );
//        doutputmatrix[( j * gridcount ) + i] = 1.0 - Math.Exp( ( -1.0 * distance ) / ( ( variance * variance ) * ( dinputatoms[j].vdw * dinputatoms[j].vdw ) ) );
//      }
//    }
//
//    [AOTCompile]
//    private static void sliceDielectricKernel( deviceptr<double> doutputs, deviceptr<double> dinputs, double refdielectric, double outdielectric, int n )
//    {
//      var i = blockIdx.x * blockDim.x + threadIdx.x;
//      if ( i >= n )
//        return;
//      var moldensity = 1.0;
//      for ( var j = 0; j < n; j++ )
//      {
//        if ( i != j )
//          moldensity *= dinputs[( j * n ) + i];
//      }
//      doutputs[i] = ( ( 1.0 - moldensity ) * refdielectric ) + ( moldensity * outdielectric );
//    }
//  }
//}