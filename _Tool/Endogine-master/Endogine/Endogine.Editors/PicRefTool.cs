using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class PicRefTool : Form, IPicRefTool
    {
        string[] _selectedFileNames;

        public PicRefTool()
        {
            InitializeComponent();
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            string definition = null;
            string output = null;
            if (this.tbOutput.Text != "<default>")
                output = this.tbOutput.Text;

            if (this.cbxSource.Text.EndsWith(".txt"))
            {
                definition = Endogine.Files.FileReadWrite.Read(this.cbxSource.Text);
            }
            else
            {
                //Endogine.BitmapHelpers.BitmapHelper.PackBitmapFiles(sRegex, sOutputDir + "__packed.png");

                definition = "";
                definition += "Input" + "\r\n";

                if (this.cbxSource.Text == "<regex selection>" && this._selectedFileNames!=null)
                {
                    definition += "\tFiles" + "\r\n";
                    foreach (string fileName in this._selectedFileNames)
                    {
                        definition += "\t\t" + fileName + "\r\n";
                    }

                    //find longest string common to all filenames:
                    string[] names = new string[this._selectedFileNames.Length];
                    int shortestLength = 9999;
                    for (int i = 0; i < this._selectedFileNames.Length; i++)
                    {
                        string name = this._selectedFileNames[i];
                        names[i] = name.Remove(0, name.LastIndexOf("\\"));
                        shortestLength = shortestLength>names[i].Length?names[i].Length:shortestLength;
                    }

                    string commonName = names[0]+"Tx";
                    bool done = false;
                    for (int i = 0; i < shortestLength; i++)
                    {
                        char c = names[0][i];
                        foreach (string name in names)
                        {
                            if (name[i] != c)
                            {
                                done = true;
                                break;
                            }
                        }
                        if (done)
                        {
                            commonName = names[0].Substring(0, i);
                            break;
                        }
                    }

                    if (output == null)
                    {
                    //    output = this._selectedFileNames[0];
                    //    int index = output.LastIndexOf("\\");
                    //    output = output.Remove(index + 1) + "_" + output.Substring(index + 1);
                    //    output = output.Remove(output.LastIndexOf(".")); // +".png";
                        output = commonName;
                    }
                }
                else
                {
                    string sRegex = this.cbxSource.Text;
                    string sDir = sRegex.Substring(0, sRegex.LastIndexOf("\\") + 1);
                    sRegex = sRegex.Remove(0, sDir.Length);
                    definition += "\tPath\t" + sDir + "\r\n";
                    definition += "\tSearch\t" + sRegex + "\r\n";
                }

                definition += "NumbersMeanOrder\ttrue" + "\r\n";
                definition += "Passes" + "\r\n";
                definition += "\tA\t" + "1" + "\r\n";
                definition += "FrameSets" + "\r\n";
                definition += "\tAll\t" + "A\t" +"0-"+ "\r\n";
                definition += "Textures" + "\r\n";
                definition += "\tTx\t" + "All"+"\r\n";
            }

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            Endogine.BitmapHelpers.AnimationAssembler.Assemble(definition, output, new EPoint(512, 256));
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void btnCreateBrowse_Click(object sender, EventArgs e)
        {
            RegExFileRetriever dlg = new RegExFileRetriever();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this._selectedFileNames = dlg.FileNames;
                this.cbxSource.Text = "<regex selection>";
            }
        }
    }
}