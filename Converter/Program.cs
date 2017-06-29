﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Converter
{

    class Bone
    {
        public string name;
        public string type;
        public Bone parent;


        public Vector3 offset;
        public Vector3 position;
        public Vector3 rotation;

        public List<Bone> bones = new List<Bone>();
        public List<string> channels = new List<string>();
        public List<Bone> children = new List<Bone>();
        //public List<Dictionary<string, float>> frameData = new List<Dictionary<string, float>>();
        public List<Dictionary<string, string>> frameData = new List<Dictionary<string, string>>();

        public void print()
        {
            Console.WriteLine(name);
            foreach (var child in children)
            {
                child.print();
            }


        }

    }
    class Frame
    {
        public string name = "";
        public List<Bone> bones = new List<Bone>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            string line = "";

            Bone currentBone = null;

            int frameTotal = 0;
            
            List<Bone> roots = new List<Bone>();
            List<string[]> splitData = new List<string[]>();
            List<string> motionData = new List<string>();
            List<Frame> frames = new List<Frame>();
            List<Bone> bones = new List<Bone>();

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\jy\\Desktop\\test.txt");


            while ((line = file.ReadLine()) != null)
            {

                /********************************
                *                               *
                * Find out number of frames     *
                *                               *
                *********************************/
                if (line.Contains("Frames:"))
                {
                    Int32.TryParse(line.Substring(line.IndexOf("Frames:") + 7,
                        line.Length - 7),
                        out frameTotal);
                }


                else if (line.Contains("ROOT"))
                {
                    Bone b = new Bone();
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string name = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    b.name = name;
                    b.type = ("root");
                    roots.Add(b);
                    bones.Add(b);
                }
                else if (line.Contains("JOINT"))
                {
                    Bone b = new Bone();
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string name = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    b.name = name;
                    b.type = "joint";
                    currentBone.children.Add(b);
                    bones.Add(b);
                    b.parent = currentBone;
                }
                else if (line.Contains("End Site"))
                {
                    Bone b = new Bone();
                    b.type = "End Site";
                    b.parent = currentBone;
                    currentBone.children.Add(b);
                }

                else if (line.Contains("{"))
                {
                    if (currentBone == null)
                    {
                        currentBone = roots.Last();
                    }
                    else
                    {
                        currentBone = currentBone.children.Last();
                    }

                } else if (line.Contains("OFFSET"))
                {
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string[] offsets = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < offsets.Length; i++)
                    {
                        float offset = float.Parse(offsets[i]);
                        switch (i) {
                            case 1:
                                currentBone.offset.X = offset;
                                break;
                            case 2:
                                currentBone.offset.Y = offset;
                                break;
                            case 3:
                                currentBone.offset.Z = offset;
                                break;
                        }
                    }
                    
                }
                else if (line.Contains("}"))
                {
                    currentBone = currentBone.parent;
                }

                else if (line.Contains("CHANNELS"))
                {
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string[] keys = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);


                    for (int i = 2; i < keys.Length; i++) //but for some reason not getting the last key (eg. y position)
                    {
                        currentBone.channels.Add(keys[i]);

                    }
                }

                else if (line.Contains("Frame Time:"))

                {
                    int frameCount = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        motionData.Add(line);
                    }
                    //split by numbers
                    foreach (var md in motionData)
                    {
                        char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                        string[] data = md.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                        splitData.Add(data);                  

                    }
                    //for each frame
                    for (int frameIter = 0; frameIter < motionData.Count; frameIter++)
                    {
                        Frame frame = new Frame();
                        frame.name = "frame" + frameCount;
                        int dataIter = 0;
                            
                        //go through each bone in order
                        foreach (Bone b in bones)
                        {
                            Dictionary<string, string> tmp = new Dictionary<string, string>();
                            int channelIter = 0;
                            //for each of its channels
                            while(channelIter < b.channels.Count)
                            {
                                //add the chanel key and corresponding data to a dict
                                tmp.Add(b.channels[channelIter], splitData[frameIter][dataIter]);


                                channelIter += 1;
                                dataIter += 1;
                            }

                            
                            //add dict to a list of frameData
                            b.frameData.Add(tmp);
                            
                        }


                        frameCount += 1;


                    }

                    //each bone
                    foreach (Bone b in bones)
                    {
                        //Console.WriteLine("\n\n\n\n" + b.name);
                        //each frame
                        foreach (var fd in b.frameData)
                        {
                            {
                                //each channel
                                foreach (var k in fd.Keys)
                                {
                                    //Console.WriteLine(k + ": " + fd[k]);
                                }
                            }
                            //Console.WriteLine();
                        }
                    }


                }










            }
            file.Close();
            foreach (Bone bone in roots) {
                //bone.print();
            }
            
            // Suspend the screen.
            Console.ReadLine();

            
        }

    }
}
