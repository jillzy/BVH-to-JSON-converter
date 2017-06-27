using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Converter
{
    class Bone
    {
        string name;
        Vector3 position;
        Vector3 orientation; //eventually W must be calculated

        public void setName(string n)
        {
            name = n;
        }

        public void setPosition(int x, int y, int z)
        {
            position.X = x;
            position.Y = y;
            position.Z = z;
        }

        public void setOrientation(int x, int y, int z)
        {
            orientation.X = x;
            orientation.Y = y;
            orientation.Z = z;
        }
        
        public string getName()
        {
            return name;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getOrientation()
        {
            return orientation;
        }

        public int calculateW(Vector3 v)
        {
            return 0;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            int braceCount = 0;
            string line = "";
            string prev = "";
            int numberFrames = 0;
            int numberChannels = 0;
            bool beginData = false;

            List<string> motionData = new List<string>();

            List<string> bones = new List<string>();

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\jy\\Desktop\\test.txt");

            while ((line = file.ReadLine()) != null)
            {

                /**********************************
                 *                                *
                 * Get rid of whitespace          *
                 *                                *
                 *********************************/
                line = new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());

                 /********************************
                 *                               *
                 * Find out number of frames     *
                 *                               *
                 *********************************/
                if (line.IndexOf("Frames:") == 0)
                { 
                    Int32.TryParse(line.Substring(line.IndexOf("Frames:") + 8,
                        line.Length - 8),
                        out numberFrames);
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
                if (line.IndexOf("CHANNELS") == 0)
                {
                    int n;
                    Int32.TryParse(new string(line.ToCharArray().Where(c => Char.IsDigit(c)).ToArray()),
                        out n);
                    numberChannels += n;
                }

                /********************************
                *                               *
                * Find where motion data begins *
                * Store data in its own list of *
                * strings                       *
                *                               *
                * ******************************/
                if (prev.IndexOf("FrameTime:") == 0)
                {
                    beginData = true;
                }
                
                if (beginData)
                {
                    motionData.Add(line);
                }




                foreach (char c in line)
                {
                    if (c == '{')
                    {

                        /********************************
                        *                               *
                        * Get names of each bone        *
                        *                               *
                        *********************************/
                        if (prev.IndexOf("ROOT") == 0)
                        {
                            bones.Add(prev.Substring(prev.IndexOf("ROOT") + 4,
                                prev.Length - 4));
                        }
                        if (prev.IndexOf("JOINT") == 0)
                        {
                            bones.Add(prev.Substring(prev.IndexOf("JOINT") + 5,
                                prev.Length - 5));
                        }
                        braceCount += 1;
                    }
                }
                Console.WriteLine(line);
                prev = line;
            }
            

            foreach (var bone in bones)
            {
                //instantiate bone object
                Bone b = new Bone();
                b.setName(bone);
                Console.WriteLine(b.getName());
            }

            Console.WriteLine("\nTotal number of channels -------> " + numberChannels + "\n");
            Console.WriteLine("\nTotal number of braces ------->: " + braceCount + "\n" );
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
