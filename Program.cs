using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HeartDisease
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            //list for all the data
            List<double[]> data = new List<double[]>();

            //list for whether patient has heart disease
            List<double> answer = new List<double>();

            //read the values from the .csv into a comma delimited string
            var reader = new StreamReader(File.OpenRead(@"heart.csv"));

            //holds values from .csv file
            List<string> listRows = new List<string>();

            //retrieves the information
            while (!reader.EndOfStream)
            {
                listRows.Add(reader.ReadLine());
            }//end while

            //remove the header row
            listRows.RemoveAt(0);


            for (int i = 0; i < listRows.Count; i++)
            {
                //holds values for rows from the .csv file
                double[] holder = new double[14];

                //splits the comma delimited rows
                string[] values = listRows[i].Split(',');

                //parses all values to doubles
                for (int j = 0; j < holder.Length; j++)
                {
                    holder[j] = double.Parse(values[j]);
                }
                //adds the row (array of doubles) just found to the data
                data.Add(holder);

                //adds whether the patient has heart disease or not
                answer.Add(double.Parse(values[13]));
            }

            //neural network (learningrate, input nodes, hidden nodes, output nodes
            //relu trains by relu as activation function
            NeuralNetwork Relu = new NeuralNetwork(.00001, 13, 2, 1);

            //sigmoid trains by sigmoid as activation function
            NeuralNetwork Sigmoid = new NeuralNetwork(.00001, 13, 2, 1);

            //tanh rains by relu as activaion function
            NeuralNetwork Tanh = new NeuralNetwork(.00001, 13, 2, 1);

            //user choice
            int choice = 0;

            //menu operations
            while (choice != 6)
            {
                Menu();
                choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    //Train Network
                    case 1:
                        //prompt user for how many times they'd like to train the network
                        int num = 0;
                        while(num == 0)
                        {
                            Console.WriteLine("How many times would you like to train the network?");
                            try
                            {
                                num = int.Parse(Console.ReadLine());
                            }
                            catch
                            {
                                Console.WriteLine("Please enter an integer value.");
                            }
                            
                        }
                        for (int i = 0; i < num; i++)
                        {
                            //trains the neural networks
                            Relu.TrainRelu(data, answer);
                            Sigmoid.TrainSigmoid(data, answer);
                            Tanh.TrainTanh(data, answer);

                            //clears the answer and shuffles the data
                            answer.Clear();
                            data = data.OrderBy(x => random.Next()).ToList();

                            //adds all the shuffled answers in order
                            for (int j = 0; j < data.Count; j++)
                            {
                                answer.Add(data[j][13]);
                            }
                        }
                        break;
                    //Retrain Network
                    case 2:
                        Tanh = new NeuralNetwork(.00001, 13, 2, 1);
                        Relu = new NeuralNetwork(.00001, 13, 2, 1);
                        Sigmoid = new NeuralNetwork(.00001, 13, 2, 1);
                        for (int i = 0; i < 100; i++)
                        {
                            //trains the neural networks
                            Relu.TrainRelu(data, answer);
                            Sigmoid.TrainSigmoid(data, answer);
                            Tanh.TrainTanh(data, answer);

                            //clears the answer and shuffles the data
                            answer.Clear();
                            data = data.OrderBy(x => random.Next()).ToList();

                            //adds all the shuffled answers in order
                            for (int j = 0; j < data.Count; j++)
                            {
                                answer.Add(data[j][13]);
                            }
                        }
                        break;

                        //read in a network saved previously
                    case 3:
                        Read(ref Tanh, ref Sigmoid, ref Relu);
                        break;

                        //print the average accuracy of the Neural Network
                    case 4:
                        double tanhAverage = 0;
                        double reluAverage = 0;
                        double sigmoidAverage = 0;
                        for (int i = 0; i < data.Count; i++)
                        {
                            double d = data[i][13];
                            double t = Tanh.Test(data[i]);
                            double r = Relu.Test(data[i]);
                            double s = Sigmoid.Test(data[i]);
                            tanhAverage += d - t;
                            reluAverage += d - r;
                            sigmoidAverage += d - s;
                        }
                        Console.WriteLine("Tanh=" + tanhAverage / 303);
                        Console.WriteLine("Relu=" + reluAverage / 303);
                        Console.WriteLine("Sigmoid=" + sigmoidAverage / 303);
                        break;

                        //saves the current network
                    case 5:
                        Print(Tanh, Sigmoid, Relu);
                        break;

                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            }
        }

        //Menu for program
        static void Menu()
        {
            Console.WriteLine("\t\t-----MENU-----");
            Console.WriteLine("\n\t1. Train Network(s)");
            Console.WriteLine("\t2. Retrain Network(s)");
            Console.WriteLine("\t3. Read Network(s) In From File");
            Console.WriteLine("\t4. Print Average Error");
            Console.WriteLine("\t5. Save Network(s) To File");
            Console.WriteLine("\t6. Exit");
            Console.WriteLine("\n\t\t-----MENU-----");
        }

        //method for reading the values for the neural network(s)
        static void Read(ref NeuralNetwork tanh, ref NeuralNetwork sigm, ref NeuralNetwork relu)
        {
            //list for data retrieved from .txt file
            List<double> data2 = new List<double>();
            //counter for the list of data
            int counter = 0;
            //will hold initial values retrieved from the .txt files
            string[] text;
            try
            {
                text = File.ReadAllText("sigmInput.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < sigm.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < sigm.InputWeights.Columns; j++)
                    {
                        sigm.InputWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }

                //reset
                counter = 0;
                data2.Clear();
                text = File.ReadAllText("sigmHidden.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < sigm.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < sigm.HiddenWeights.Columns; j++)
                    {
                        sigm.HiddenWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }

                //reset
                counter = 0;
                data2.Clear();
                text = File.ReadAllText("tanhHidden.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < tanh.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < tanh.HiddenWeights.Columns; j++)
                    {
                        tanh.HiddenWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }

                //reset
                counter = 0;
                data2.Clear();
                text = File.ReadAllText("tanhInput.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < tanh.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < tanh.InputWeights.Columns; j++)
                    {
                        tanh.InputWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }

                //reset
                counter = 0;
                data2.Clear();
                text = File.ReadAllText("reluInput.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < relu.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < relu.InputWeights.Columns; j++)
                    {
                        relu.InputWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }

                //reset
                counter = 0;
                data2.Clear();
                text = File.ReadAllText("reluHidden.txt").Trim().Split('\n');
                foreach (string g in text)
                {
                    data2.Add(double.Parse(g));
                }
                for (int i = 0; i < relu.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < relu.HiddenWeights.Columns; j++)
                    {
                        relu.HiddenWeights.m[i, j] = data2[counter];
                        counter++;
                    }
                }
            }
            catch
            {

            }
        }

        //method for writing the current network(s) to a .txt file
        static void Print(NeuralNetwork tanh, NeuralNetwork sigm, NeuralNetwork relu)
        {
            string docPath = @"C:\Users\morga\Desktop\Personal Projects\HeartDisease\HeartDisease\bin\Debug";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "sigmInput.txt"), false))
            {
                for (int i = 0; i < sigm.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < sigm.InputWeights.Columns; j++)
                    {
                        outputFile.WriteLine(sigm.InputWeights.m[i, j]);
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "sigmHidden.txt"), false))
            {
                for (int i = 0; i < sigm.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < sigm.HiddenWeights.Columns; j++)
                    {
                        outputFile.WriteLine(sigm.HiddenWeights.m[i, j]);
                    }
                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "tanhInput.txt"), false))
            {
                for (int i = 0; i < tanh.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < tanh.InputWeights.Columns; j++)
                    {
                        outputFile.WriteLine(tanh.InputWeights.m[i, j]);
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "tanhHidden.txt"), false))
            {
                for (int i = 0; i < tanh.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < tanh.HiddenWeights.Columns; j++)
                    {
                        outputFile.WriteLine(tanh.HiddenWeights.m[i, j]);
                    }
                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "reluInput.txt"), false))
            {
                for (int i = 0; i < relu.InputWeights.Rows; i++)
                {
                    for (int j = 0; j < relu.InputWeights.Columns; j++)
                    {
                        outputFile.WriteLine(relu.InputWeights.m[i, j]);
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "reluHidden.txt"), false))
            {
                for (int i = 0; i < relu.HiddenWeights.Rows; i++)
                {
                    for (int j = 0; j < relu.HiddenWeights.Columns; j++)
                    {
                        outputFile.WriteLine(relu.HiddenWeights.m[i, j]);
                    }
                }
            }

        }
    }
}
