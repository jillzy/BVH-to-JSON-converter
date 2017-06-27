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
        Vector3 rotation;
        //fields: name, orientation, position
    }
    class Program
    {
        static void Main(string[] args)
        {
            int braceCount = 0;
            string line = "";
            string prev = "";
            int numberFrames = 0;
            List<String> bones = new List<String>();

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
                
                

                foreach (char c in line)
                {
                    if (c == '{')
                    {

                        /********************************
                        *                               *
                        * Get names of bones            *
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
                Console.WriteLine(bone);
            }


            Console.WriteLine("bracecount: " + braceCount);
            file.Close();

            // Suspend the screen.
            Console.ReadLine();

        }
    }
}
