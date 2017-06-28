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
        string name;
        string type;

        int numberChannels;

        Vector3 position;
        Vector3 rotation;

        Dictionary<string, float> data = new Dictionary<string, float>();

        public void setName(string s)
        {
            name = s;
        }

        public void setType(string s)
        {
            type = s;
        }

        public void setNumberChannels(int i)
        {
            numberChannels = i;
        }

        public void setPosition(int x, int y, int z)
        {
            position.X = x;
            position.Y = y;
            position.Z = z;
        }

        public void setOrientation(int x, int y, int z)
        {
            rotation.X = x;
            rotation.Y = y;
            rotation.Z = z;
        }
        
        public string getName()
        {
            return name;
        }

        public string getType()
        {
            return type;
        }

        public int getNumberChannels()
        {
            return numberChannels;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getRotation()
        {
            return rotation;
        }

        public void addKeys(string s)
        {
            data.Add(s, 0);
        }

        public int calculateW(Vector3 v)
        {
            return 0;
        }

    }
    class Frame
    {
        string name = "";
        List<Bone> bones = new List<Bone>();

        public void addBone(Bone b)
        {
            bones.Add(b);
        }
        public void setName(string s)
        {
            name = s;
        }
        public string getName()
        {
            return name;
        }
        public List<Bone> getBones()
        {
            return bones;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string line = "";
            string prev = "";
            string currentBone = "";

            int frameTotal = 0;
            int channelTotal = 0;

            bool beginData = false;

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



                /********************************
                *                               *
                * Find where motion data begins *
                * Store data in its own list of *
                * strings                       *
                *                               *
                * ******************************/
                if (prev.Contains("Frame Time:"))
                {
                    beginData = true;
                }
                
                if (beginData)
                {
                    //collapse white space
                    //line = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ");

                    motionData.Add(line);
                }




                foreach (char ch in line)
                {
                    Bone b = new Bone();
                    string name = "";
                    string type = "";
                    if (ch == '{')
                    {
                        /********************************
                        *                               *
                        * Get name and type of          *
                        * each bone                     *
                        *                               *
                        *********************************/
                        prev = new string(prev.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                        if (prev.Contains("ROOT"))
                        {
                            name = prev.Substring(prev.IndexOf("ROOT") + 4,
                                prev.Length - 4);
                            type = "root";
                           
                        }
                        else if (prev.Contains("JOINT"))
                        {
                            name = prev.Substring(prev.IndexOf("JOINT") + 5,
                                prev.Length - 5);
                            type = "joint";
                            
                        } else
                        {
                            break;
                        }
                        currentBone = name;
                        b.setName(name);
                        b.setType(type);
                        bones.Add(b);
                    }
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
                if (line.Contains("CHANNELS"))
                {
                    int n;

                    line = new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                    Int32.TryParse(new string(line.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()),
                        out n);
                    //set channels for current bone
                    //add channel keys into dictionary
                    foreach (var bone in bones)
                    {
                        if (bone.getName() == currentBone)
                        {
                            Console.WriteLine("*********************** THE BONE IS: " + currentBone);
                            if (line.Contains("position"))
                            {
                                bone.addKeys("Xposition");
                                bone.addKeys("Yposition");
                                bone.addKeys("Zposition");

                            }
                            if (line.Contains("rotation"))
                            {
                                bone.addKeys("Xrotation");
                                bone.addKeys("Yrotation");
                                bone.addKeys("Zrotation");
                            }

                            bone.setNumberChannels(n);
                        }
                    }
                    channelTotal += n;
                }


                //Console.WriteLine(line);
                prev = line;
            }




            //logic
            foreach (var bone in bones)
            {

                Frame frame = new Frame();
                foreach (var data in motionData)
                {
                    string[] dataItems = Regex.Split(data, @"\s+");
                    int i = 0;
                    foreach (var d in dataItems)
                    {
                        if (d != "") {
                            Console.WriteLine("Item " + i++ + ": " + d);
                        }
                    }

                }
                //todo: dont assumesorder xyz

                
                for (int i = 0; i < bone.getNumberChannels(); i++)
                {
                    /*int j = 0;
                    while (j < frameTotal)
                    {
                        frame.setName("frame" + j);
                        Console.WriteLine(frame.getName());
                        frames.Add(frame);
                        j += 1;
                    }
                    //read an item from motion data*/
                }
                Console.WriteLine(bone.getName());
                Console.WriteLine(bone.getType());
                Console.WriteLine(bone.getNumberChannels());
               
            }

            Console.WriteLine("\nTotal number of channels -------> " + channelTotal + "\n");
            Console.WriteLine("\nTotal number of frames -------> " + frameTotal + "\n");
            Console.WriteLine("\nMotion Data:");
            foreach (var l in motionData)
            {
                Console.WriteLine(l);
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();

        }
    }
}
