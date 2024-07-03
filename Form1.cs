using System.Text;

namespace ElmaAutoGravityApples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Show the OpenFileDialog
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // This line will be executed if the user clicks OK in the dialog
            }
        }


        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get the selected file path
            string filePath = openFileDialog1.FileName;
            List<int> expectedAnimationTypeAsInts = new List<int>();
            List<int> newAnimationTypeAsInts = new List<int>();
            List<int> updateOffsetsAsInts = new List<int>();

            // Do something with the file path (e.g., display a message)
            //MessageBox.Show("You selected: " + filePath);

            // Open the file in binary read mode
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
            {
                int totalBytesRead = 0;
                int offset;
                int bytesToSkip = 7 + 4 + 8 + 8 + 8 + 8; // skippable data
                reader.ReadBytes(bytesToSkip);
                totalBytesRead += bytesToSkip;

                // level name
                // Read the actual data based on the total length
                byte[] data = reader.ReadBytes(51);
                totalBytesRead += 51;
                byte[] levelNameBytes = data.Take(51).ToArray();
                string levelName = Encoding.ASCII.GetString(levelNameBytes);
                levelName = levelName.TrimEnd('\0');
                //MessageBox.Show("Level Name: " + levelName);

                bytesToSkip = 16 + 10 + 10; // skippable data
                reader.ReadBytes(bytesToSkip);
                totalBytesRead += bytesToSkip;

                // get polygons count
                data = reader.ReadBytes(8);
                totalBytesRead += 8;
                double polygonCountAsDouble = BitConverter.ToDouble(data);
                int polygonsAsInt = (int)polygonCountAsDouble;  // Casting might cause data loss if not whole number
                //MessageBox.Show("Number of Polygons: " + polygonsAsInt);

                foreach (int polygonIndex in Enumerable.Range(0, polygonsAsInt))
                {
                    data = reader.ReadBytes(4);
                    totalBytesRead += 4;
                    int grassAsInt = BitConverter.ToInt32(data);
                    //MessageBox.Show("Is Grass (Grass == 1): " + grassAsInt);

                    data = reader.ReadBytes(4);
                    totalBytesRead += 4;
                    int verticesAsInt = BitConverter.ToInt32(data);
                    //MessageBox.Show("Number of Vertices: " + verticesAsInt);

                    foreach (int vertexIndex in Enumerable.Range(0, verticesAsInt))
                    {
                        bytesToSkip = 8 + 8; // skippable data: vertex coordinates
                        reader.ReadBytes(bytesToSkip);
                        totalBytesRead += bytesToSkip;
                    }
                }

                // get objects count
                data = reader.ReadBytes(8);
                totalBytesRead += 8;
                double objectCountAsDouble = BitConverter.ToDouble(data);
                int objectsAsInt = (int)objectCountAsDouble;  // Casting might cause data loss if not whole number
                //MessageBox.Show("Number of Objects: " + objectsAsInt);

                foreach (int objectIndex in Enumerable.Range(0, objectsAsInt))
                {
                    bytesToSkip = 8 + 8; // skippable data: objects coordinates
                    reader.ReadBytes(bytesToSkip);
                    totalBytesRead += bytesToSkip;

                    data = reader.ReadBytes(4);
                    totalBytesRead += 4;
                    int objectTypeAsInt = BitConverter.ToInt32(data);
                    //MessageBox.Show("Object Type: " + objectTypeAsInt);

                    data = reader.ReadBytes(4);
                    totalBytesRead += 4;
                    int gravityTypeAsInt = BitConverter.ToInt32(data);
                    //MessageBox.Show("Gravity Type: " + gravityTypeAsInt);

                    data = reader.ReadBytes(4);
                    offset = totalBytesRead;
                    totalBytesRead += 4;
                    int animationTypeAsInt = BitConverter.ToInt32(data);
                    //if (gravityTypeAsInt != 0) MessageBox.Show("Animation Type: " + animationTypeAsInt);

                    if (objectTypeAsInt == 2)
                    {
                        //MessageBox.Show("Should update animation type to " + gravityTypeAsInt + " after " + offset + " bytes");
                        expectedAnimationTypeAsInts.Add(animationTypeAsInt);
                        // no gravity apple: gravityType == 0 // anim number stored value == 0 // anim number displayed value in elma editor == 1 // qfood1.pcx
                        // gravity up: gravityType == 1 // anim number stored value == 1 // anim number displayed value in elma editor == 2 // qfood2.pcx
                        // gravity down: gravityType == 2 // anim number stored value == 2 // anim number displayed value in elma editor == 3 // qfood3.pcx
                        // gravity left: gravityType == 3 // anim number stored value == 3 // anim number displayed value in elma editor == 4 // qfood4.pcx
                        // gravity right: gravityType == 4 // anim number stored value == 4 // anim number displayed value in elma editor == 5 // qfood5.pcx
                        newAnimationTypeAsInts.Add(gravityTypeAsInt);
                        updateOffsetsAsInts.Add(offset);
                    }
                }

            }

            // verify data before writing
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Use the file stream to read data from the file
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    byte[] data;
                    for (int i = 0; i < expectedAnimationTypeAsInts.Count; i++)
                    {
                        if (i == 0) reader.ReadBytes(updateOffsetsAsInts[i]);
                        else
                        {
                            // offset is counted from 0, so subtract what's already read
                            reader.ReadBytes(updateOffsetsAsInts[i] - updateOffsetsAsInts[i - 1] - 4);
                        }
                        data = reader.ReadBytes(4);
                        int animationTypeAsInt = BitConverter.ToInt32(data);
                        if (animationTypeAsInt != expectedAnimationTypeAsInts[i])
                        {
                            MessageBox.Show("Verification Failed: " + animationTypeAsInt + " != " + expectedAnimationTypeAsInts[i]);
                            this.Close();  // Closes the main form
                        }
                    }

                }
            }

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                try
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                {
                        for (int i = 0; i < expectedAnimationTypeAsInts.Count; i++)
                        {
                            //MessageBox.Show("Writing iteration: " + i + ", writing value: " + newAnimationTypeAsInts[i]);
                            // Seek to the desired offset position in the stream
                            writer.BaseStream.Seek(updateOffsetsAsInts[i], SeekOrigin.Begin);
                            writer.Write(newAnimationTypeAsInts[i]);
                            // Important: Flush the writer to ensure data is written to the stream
                        }
                }
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Error writing data to file: " + ex.Message);
                    // Handle the exception (e.g., log error or notify user)
                }
            }

            MessageBox.Show("Done.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}