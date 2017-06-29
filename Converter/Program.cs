using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace Converter
{


    class Bone
    {
        const string xpos = "Xposition";
        const string ypos = "Yposition";
        const string zpos = "Zposition";
        const string xrot = "Xrotation";
        const string yrot = "Yrotation";
        const string zrot = "Zrotation";

        public string name;
        public string type;
        public Bone parent;

        public Vector3 offset;
        
        public List<Bone> bones = new List<Bone>();
        public List<string> channels = new List<string>();
        public List<Bone> children = new List<Bone>();
        public List<Dictionary<string, float>> frameData = new List<Dictionary<string, float>>();

        public Matrix4x4 getWorldTransform(int frameNumber)
        {
            var matrix = Matrix4x4.Identity;

            matrix.Translation = offset;
            matrix = Matrix4x4.Transform(matrix, getQuaternion(frameNumber));

            if (parent != null)
            {
                matrix = Matrix4x4.Multiply(parent.getWorldTransform(frameNumber), matrix);
            }

            return matrix;
        }
        
        public Vector3 getWorldPositionForFrame(int frameNumber)
        {
            Vector3 scale;
            Vector3 translation;
            Quaternion rotation;
            Matrix4x4.Decompose(getWorldTransform(frameNumber), out scale, out rotation, out translation);

            return translation;
        }

        public Quaternion getWorldRotationForFrame(int frameNumber)
        {
            Vector3 scale;
            Vector3 translation;
            Quaternion rotation;
            Matrix4x4.Decompose(getWorldTransform(frameNumber), out scale, out rotation, out translation);

            return rotation;
        }

        public Quaternion getQuaternion(int frameNumber)
        {
            Quaternion rotation = new Quaternion();
            float x = frameData[frameNumber][xrot];
            float y = frameData[frameNumber][yrot];
            float z = frameData[frameNumber][zrot];

            rotation = Quaternion.CreateFromYawPitchRoll(y, x, z);

            return rotation;
        }

        public void print()
        {
            Console.WriteLine(name);
            foreach (var child in children)
            {
                child.print();
            }


        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: converter.exe [input .bvh path] [output file path]");
                Console.ReadLine();
            }
            string input = args[0];
            string output = args[1];
            string line = "";

            Bone currentBone = null;

            int frameTotal = 0;

            List<Bone> roots = new List<Bone>();
            List<string[]> splitData = new List<string[]>();
            List<string> motionData = new List<string>();
            List<Bone> bones = new List<Bone>();

            // Read the file and display it line by line.
            StreamReader file = new StreamReader(input);


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

                }
                else if (line.Contains("OFFSET"))
                {
                    char[] splitChars = new Char[] { ' ', '\t', '\n', '\r', '\f' };
                    string[] offsets = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < offsets.Length; i++)
                    {
                        float offset = float.Parse(offsets[i]);
                        switch (i)
                        {
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
                        int dataIter = 0;

                        //go through each bone in order
                        foreach (Bone b in bones)
                        {
                            Dictionary<string, float> tmp = new Dictionary<string, float>();
                            int channelIter = 0;
                            //for each of its channels
                            while (channelIter < b.channels.Count)
                            {
                                //add the chanel key and corresponding data to a dict
                                tmp.Add(b.channels[channelIter], float.Parse(splitData[frameIter][dataIter]));


                                channelIter += 1;
                                dataIter += 1;
                            }


                            //add dict to a list of frameData
                            b.frameData.Add(tmp);

                        }



                    }

        

                }










            }
            file.Close();

            Dictionary<string,Dictionary<string,Dictionary<string, float[]>>> frameJSON
                = new Dictionary<string, Dictionary<string, Dictionary<string, float[]>>>();

            for (int i = 0; i < motionData.Count; i++)
            {
                frameJSON.Add("Frame" + i,
                    new Dictionary<string, Dictionary<string, float[]>>());

                foreach (Bone b in bones)
                {
                    frameJSON["Frame" + i].Add(b.name, new Dictionary<string, float[]>());
                    Vector3 scale;
                    Vector3 translation;
                    Quaternion rotation;
                    Matrix4x4.Decompose(b.getWorldTransform(i), out scale, out rotation, out translation);

                    frameJSON["Frame" + i][b.name].Add("Position",
                        new float[] { translation.X, translation.Y, translation.Z });

                    frameJSON["Frame" + i][b.name].Add("Rotation",
                        new float[] { rotation.W, rotation.X, rotation.Y, rotation.Z });


                }

            }

            JsonSerializer js = JsonSerializer.Create();
            StreamWriter outputFile = new StreamWriter(new FileStream(output, FileMode.Create));
            js.Serialize(outputFile, frameJSON);
            outputFile.Flush();
            Console.WriteLine("Successfully converted file.");

            

        }

    }
}
