using System;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;

namespace Endogine.BitmapHelpers
{
    /// <summary>
    /// Summary description for AnimationAssembler.
    /// </summary>
    public class AnimationAssembler
    {
        public AnimationAssembler()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void Assemble(string definition, string savePathWithPrefix, EPoint firstTryTextureSize)
        {
            Hashtable passNumberToTexture = GenerateFileLists(definition, null);

            bool usingPasses = passNumberToTexture.Count > 1 ? true : false;
            //usingPasses = generate several bitmaps, based on different sequences

            foreach (DictionaryEntry dePass in passNumberToTexture)
            {
                int passNum = (int)dePass.Key;
                Hashtable textureNamesToFramesets = (Hashtable)dePass.Value;

                foreach (DictionaryEntry deTexture in textureNamesToFramesets)
                {
                    string textureName = (string)deTexture.Key;
                    Hashtable framesets = (Hashtable)deTexture.Value;

                    XmlDocument doc = new XmlDocument();
                    XmlNode xmlNode = doc.CreateElement("Animations");
                    doc.AppendChild(xmlNode);
                    XmlNode xmlFramesetNode;
                    XmlAttribute xmlAttr;

                    ArrayList allFiles = new ArrayList();
                    foreach (DictionaryEntry deAnim in framesets)
                    {
                        string framesetName = (string)deAnim.Key;
                        string[] filenames = (string[])deAnim.Value;

                        if (filenames.Length > 0)
                        {
                            //frameset can have same name as others (other characters etc)
                            //so it must be uniquely identifiable - add 
                            xmlFramesetNode = xmlNode.OwnerDocument.CreateElement(framesetName);
                            xmlNode.AppendChild(xmlFramesetNode);

                            string sAnimFrames = "0";
                            //								Create animation frame list: <Animations> tag
                            //if (usingPasses)
                            //{
                            int startFrame = allFiles.Count;
                            sAnimFrames = startFrame.ToString();
                            //}
                            sAnimFrames += " 0-" + (filenames.Length - 1).ToString();
                            //for (int i=0; i<filenames.Length;i++)
                            //	sAnimFrames+=i.ToString()+",";
                            //sAnimFrames = sAnimFrames.Remove(sAnimFrames.Length-1,1);

                            xmlAttr = xmlNode.OwnerDocument.CreateAttribute("value");
                            xmlFramesetNode.Attributes.Append(xmlAttr);
                            xmlAttr.InnerText = sAnimFrames;
                        }

                        foreach (string filename in filenames)
                            allFiles.Add(filename);
                    }

                    string[] all = new string[allFiles.Count];
                    for (int i = 0; i < allFiles.Count; i++)
                        all[i] = (string)allFiles[i];

                    string saveFilename = textureName;
                    if (usingPasses)
                        saveFilename += "_" + passNum.ToString().PadLeft(2, '0');

                    saveFilename = savePathWithPrefix + saveFilename;

                    //TODO: find texture size to start trying with.
                    //TODO: doesn't really matter in which texture the individual frames are... (in some cases at least)
                    EPoint trySize = firstTryTextureSize.Copy();
                    for (int tryNum = 0; tryNum < 3; tryNum++)
                    {
                        try
                        {
                            Endogine.BitmapHelpers.TexturePacking.TreePack(
                                new System.Drawing.Size(trySize.X, trySize.Y), all, saveFilename);
                        }
                        catch
                        {
                            if (trySize.X <= trySize.Y)
                                trySize.X *= 2;
                            else
                                trySize.Y *= 2;
                        }
                    }

                    //load the auto-created resource fork and add the animations to it:
                    string xmlFilename = saveFilename + ".xml";
                    XmlDocument resDoc = new XmlDocument();
                    resDoc.Load(xmlFilename);
                    if (resDoc["root"]["Animations"] == null)
                        Endogine.Serialization.XmlHelper.CreateAndAddElement(resDoc["root"], "Animations");
                    resDoc["root"]["Animations"].InnerXml = doc["Animations"].InnerXml;
                    //Endogine.Serialization.XmlHelper.Merge(resDoc["root"]["Animations"], doc["Animations"]);
                    resDoc.Save(xmlFilename);
                }
            }
        }

        //private static string GetFullSearchPathFromDefinition(string definition, string overridePath)
        //{
        //    Node rootNode = Endogine.Node.FromTabbed(definition);

        //    string path = rootNode["Input.Path"].Text;
        //    if (overridePath != null)
        //        path = overridePath;
        //    if (rootNode["Input.Search"] != null)
        //        path += rootNode["Input.Search"].Text;
        //    return path;
        //}

        /// <summary>
        /// search for both [] and [pad:<number>]. Padding means filling out on left side with zeroes.
        /// e.g. for @@actor0\w_isoview0[]_[pad:4]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static MatchCollection GetVariableMatchesInSearchPattern(string path)
        {
            MatchCollection ms = Regex.Matches(path, @"\[\]|\[pad:[0-9]+\]");
            return ms;
        }

        /// <summary>
        /// Generates a Hashtable with Hashtables with Hashtables: PassNumber:[TextureName:[AnimationName:[ArrayList FileNames]]]
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="pathWithPrefix"></param>
        /// <returns></returns>
        public static Hashtable GenerateFileLists(string definition, string overridePath) //TODO: Node overrides - can override anything
        {
            //Looks first at the Texture node to see what the textures should be called and which FrameSets they contain
            //For each texture, it loops through the FrameSets.
            //If the FrameSet should be included in the current pass, add it to the texure, else ignore it.

            Node rootNode = Endogine.Node.FromTabbed(definition);

            string path = null;
            string[] allFilesnames = null;
            if (rootNode["Input.Files"] != null)
            {
                allFilesnames = new string[rootNode["Input.Files"].ChildNodes.Count];
                Node nodeX = rootNode["Input.Files"].FirstChild;
                for (int i=0; i<rootNode["Input.Files"].ChildNodes.Count; i++)
                {
                    allFilesnames[i] = nodeX.Name;
                    nodeX = nodeX.NextSibling;
                }
            }
            else
            {
                path = rootNode["Input.Path"].Text;
                if (overridePath != null)
                    path = overridePath;
                if (rootNode["Input.Search"] != null)
                    path += rootNode["Input.Search"].Text;
            }

            //string path = GetFullSearchPathFromDefinition(definition, overridePath);

            //this will contain all passes (passNumber:textureHashList)
            Hashtable passNumberToTexture = new Hashtable();

            bool usingPasses = false;

            #region Find all IDs and numbers used in Passes section
            int highestPassNum = 1;
            Hashtable predefRanges = null; //contains rangeId:range definition.
            //TODO: predefRanges is essentially useless! remove and replace with predefRangeIdToPassNumbers!
            Hashtable predefRangeIdToPassNumbers = null; //contains rangeId:[pass numbers]
            ArrayList allPassNumbers = new ArrayList(); //contains all numbers that occur in all the pass specifications

            Node passesNode = rootNode["Passes"];
            if (passesNode != null)
            {
                usingPasses = true;
                predefRanges = new Hashtable();
                predefRangeIdToPassNumbers = new Hashtable();
                for (int i = 0; i < passesNode.ChildNodes.Count; i++)
                {
                    Node n = passesNode[i];
                    ArrayList nums = Endogine.Text.IntervalString.CreateArrayFromIntervalString(n.Text);
                    predefRanges.Add(n.Name, n.Text);
                    predefRangeIdToPassNumbers.Add(n.Name, nums);

                    foreach (int num in nums)
                    {
                        if (!allPassNumbers.Contains(num))
                        {
                            highestPassNum = num > highestPassNum ? num : highestPassNum;
                            allPassNumbers.Add(num);
                        }
                    }
                }
            }
            #endregion
            if (allPassNumbers.Count == 0)
                allPassNumbers.Add(1);

            Node texturesNode = rootNode["Textures"];
            Node framesNode = rootNode["FrameSets"];

            //Are the number specified for each frameset the actual numbers in the regex "variables" (eg [10-45])
            //or just the order index (alphabetical sorting)
            bool bNumbersMeanOrder = false;
            if (allFilesnames != null)
                bNumbersMeanOrder = true;
            else if (rootNode["NumbersMeanOrder"] != null && rootNode["NumbersMeanOrder"].Text.ToLower() == "true")
            {
                bNumbersMeanOrder = true;
                allFilesnames = Endogine.Files.FileFinder.GetNamesFromFiles(
                            Endogine.Files.FileFinder.GetFiles(path));
            }

            MatchCollection msVariableMatched = null;
            if (allFilesnames == null)
            {
                msVariableMatched = GetVariableMatchesInSearchPattern(path);
            }


            foreach (int passNum in allPassNumbers)
            {
                Hashtable textureNameToAnimations = new Hashtable();
                passNumberToTexture.Add(passNum, textureNameToAnimations);

                for (int texNum = 0; texNum < texturesNode.ChildNodes.Count; texNum++)
                {

                    //find which framesets to use for this texture:
                    Node node = texturesNode[texNum];
                    string[] frameSetNames = node.Text.Split(',');
                    if (frameSetNames[0] == "All")
                    {
                        frameSetNames = new string[framesNode.ChildNodes.Count];
                        for (int i = 0; i < framesNode.ChildNodes.Count; i++)
                            frameSetNames[i] = framesNode[i].Name;
                    }


                    Hashtable animationNameToFileLists = new Hashtable();

                    foreach (string frameSetName in frameSetNames)
                    {
                        string trimmedFrameSetName = frameSetName.Trim();
                        //TODO: rename range to pass!
                        string[] ranges = framesNode[trimmedFrameSetName].Text.Split('\t');

                        if (bNumbersMeanOrder)
                        {
                            string range = ranges[1];
                            if (range.EndsWith("-"))
                                range += allFilesnames.Length-1;
                            ArrayList indices = Endogine.Text.IntervalString.CreateArrayFromIntervalString(range);
                            string[] filenames = new string[indices.Count];
                            for (int i=0;i<indices.Count;i++)
                                filenames[i] = allFilesnames[(int)indices[i]];
                            animationNameToFileLists.Add(trimmedFrameSetName, filenames);
                            //animationNameToFileLists.Add(animationPrefix + trimmedFrameSetName, filenames);
                        }
                        else
                        {
                            //replace []'s in path with the ranges defined in the frameset:
                            //first is the pass, second is
                            string newPath = path;
                            bool includeThisFrameSet = true;
                            for (int i = 0; i < msVariableMatched.Count; i++)
                            {
                                //TODO: rename range to pass!
                                string find = msVariableMatched[i].Value;
                                string range = ranges[i];
                                string rangeID = range;
                                if (char.IsLetter(range, 0))
                                    range = (string)predefRanges[range];


                                //When doing passes, the first column is the one checked for pass numbers:
                                if (usingPasses && i == 0)
                                {
                                    //if pass is not included in the list, don't use in this pass
                                    if (char.IsLetter(range, 0))
                                    {
                                        if (!((ArrayList)predefRangeIdToPassNumbers[rangeID]).Contains(passNum))
                                            includeThisFrameSet = false;
                                    }
                                    else if (!Endogine.Text.IntervalString.CreateArrayFromIntervalString(range).Contains(passNum))
                                        includeThisFrameSet = false;

                                    if (!includeThisFrameSet)
                                        break;

                                    range = passNum.ToString();
                                }

                                //doesn't work with just one number, so make it number-number (13-13 instead of 13)
                                if (range.IndexOf("-") < 0 && range.IndexOf(",") < 0)
                                    range = range + "-" + range;
                                string replace = find.Insert(1, range);
                                newPath = Endogine.Text.StringHelpers.Replace(newPath, find, replace, 1);
                            }
                            if (!includeThisFrameSet)
                                continue;


                            //EH.Put("FS "+trimmedFrameSetName+" "+newPath);
                            string[] filenames = Endogine.Files.FileFinder.GetNamesFromFiles(
                                Endogine.Files.FileFinder.GetFiles(newPath));

                            if (filenames.Length == 0)
                                EH.Put("No files found for frameset " + trimmedFrameSetName);
                            else
                            {
                                string animationPrefix = "";
                                if (rootNode["AnimationPrefix"] != null)
                                    animationPrefix = rootNode["AnimationPrefix"].Text;
                                else
                                {
                                    //TODO: should be able to extract AnimationPrefix from newPath somehow...
                                    //First [] or [pad:X] is simply replaced with passNum, and at the next occurrence, remove the rest
                                    //NOPE: exacly what we DON'T want: then add .+ to find anything that starts with this
                                    //@@actor0[1-1]_portrait[1-10pad:4] -> actor01_portrait
                                    //@@actor0\w_isoview0[1-1]_[1-20pad:4] -> actor0\w_isoview01_
                                    //then look at the first found file that matches this:
                                    int indexStartFile = newPath.IndexOf("@@");
                                    animationPrefix = newPath.Remove(0, indexStartFile + 2);
                                    string regexPattern = Endogine.Files.FileFinder.GetRegexPatternForRanges();
                                    MatchCollection msRangeMatches = Regex.Matches(animationPrefix, regexPattern);
                                    if (msRangeMatches.Count > 0)
                                    {
                                        if (msRangeMatches.Count > 1)
                                            animationPrefix = animationPrefix.Substring(0, msRangeMatches[1].Index);
                                        //TODO: use my regex-like replacement. Now it doesn't care about padding.
                                        animationPrefix = Endogine.Text.StringHelpers.Replace(animationPrefix, msRangeMatches[0].Index, msRangeMatches[0].Length, passNum.ToString());
                                    }
                                    //now we have actor01_portrait.+
                                    Match m = Regex.Match(filenames[0], animationPrefix);
                                    if (m.Success)
                                        animationPrefix = m.Value;
                                }

                                //if (usingPasses) //the animation names should not be the same for the different passes - name after pass number
                                //	animationPrefix+=passNum.ToString().PadLeft(highestPassNum.ToString().Length, '0');
                                animationNameToFileLists.Add(animationPrefix + trimmedFrameSetName, filenames);
                            }
                        }
                    }

                    if (animationNameToFileLists.Count > 0)
                        textureNameToAnimations.Add(node.Name, animationNameToFileLists);
                    else
                        EH.Put("No files found for texture node " + node.Name);
                }
            }
            return passNumberToTexture;
        }
    }
}
