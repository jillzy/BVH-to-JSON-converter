using System;
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

        public int numberChannels;

        public Vector3 position;
        public Vector3 rotation;

        public List<Bone> children = new List<Bone>();
        public Dictionary<string, float> data = new Dictionary<string, float>();

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
            string prev = "";

            Bone currentBone = null;

            int frameTotal = 0;
            int channelTotal = 0;

            List<Bone> roots = new List<Bone>();

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






                if (line.Contains("ROOT"))
                {
                    Bone b = new Bone();
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string name = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    b.name = name;
                    b.type = ("root");
                    roots.Add(b);
                }
                else if (line.Contains("JOINT"))
                {
                    Bone b = new Bone();
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string name = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    b.name = name;
                    b.type = "joint";
                    currentBone.children.Add(b);
                    b.parent = currentBone;
                } else if (line.Contains("End Site"))
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

                    //add channel data to current

                }
                else if (line.Contains("}"))
                {
                    currentBone = currentBone.parent;
                }

                


                /********************************
                *                               *
                * Find out total number of      *
                * channels:                     *
                *                               *
                * "flexibility to allow for     *
                * segments which have any       *
                * number of channels which can  *
                * appear in any order."         *
                *                               *
                *********************************/
                //    if (line.Contains("CHANNELS"))
                //    {
                //        int n;

                //        line = new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                //        Int32.TryParse(new string(line.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()),
                //            out n);
                //        //set channels for current bone
                //        //add channel keys into dictionary
                //        foreach (var bone in bones)
                //        {
                //            if (bone.getName() == currentBone)
                //            {
                //                Console.WriteLine("*********************** THE BONE IS: " + currentBone);
                //                if (line.Contains("position"))
                //                {
                //                    bone.addKeys("Xposition");
                //                    bone.addKeys("Yposition");
                //                    bone.addKeys("Zposition");

                //                }
                //                if (line.Contains("rotation"))
                //                {
                //                    bone.addKeys("Xrotation");
                //                    bone.addKeys("Yrotation");
                //                    bone.addKeys("Zrotation");
                //                }

                //                bone.setNumberChannels(n);
                //            }
                //        }
                //        channelTotal += n;
                //    }


                //    //Console.WriteLine(line);
                //    prev = line;
                //}







                ////    for (int i = 0; i < bone.getNumberChannels(); i++)
                ////    {
                ////        /*int j = 0;
                ////        while (j < frameTotal)
                ////        {
                ////            frame.setName("frame" + j);
                ////            Console.WriteLine(frame.getName());
                ////            frames.Add(frame);
                ////            j += 1;
                ////        }
                ////        //read an item from motion data*/
                ////    }
                ////    Console.WriteLine(bone.getName());
                ////    Console.WriteLine(bone.getType());
                ////    Console.WriteLine(bone.getNumberChannels());

                ////}

                ////Console.WriteLine("\nTotal number of channels -------> " + channelTotal + "\n");
                ////Console.WriteLine("\nTotal number of frames -------> " + frameTotal + "\n");
                ////Console.WriteLine("\nMotion Data:");
                //foreach (var l in motionData)
                //{
                //    Console.WriteLine(l);
                //}
            }
            file.Close();
            foreach (Bone bone in roots) {
                bone.print();
            }
            // Suspend the screen.
            Console.ReadLine();

            
        }

    }
}
