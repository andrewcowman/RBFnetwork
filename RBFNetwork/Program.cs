using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RBFNetwork {
    class Program {

        static void Main(string[] args) {
            Program prg = new Program();
            prg.Run();
        }

        public void Run() {

            // get iris file from resource stream
            Assembly assembly = Assembly.GetExecutingAssembly();
            var f = assembly.GetManifestResourceStream("RBFNetwork.Resources.iris.csv");
            StreamReader sr = new StreamReader(f);

            // load all records into new dataset
            DataSet ds = DataSet.Load(sr);
            sr.Close();

            // normalize the data for columns
            ds.Normalize(0); // sepal len
            ds.Normalize(1); // sepal wid
            ds.Normalize(2); // petal len
            ds.Normalize(3); // petal wid

            // get all species names and assign them an index
            Dictionary<string, int> specieNames = ds.EncodeEquilateral(4);

            // get a list of records with data and encoded class name
            List<Record> trainingData = ds.ExtractSupervised(0, 4, 4, 2);

            Network net = new Network(4, 4, 2); // new RBF network
            Scorer score = new Scorer(trainingData); //  new scorer
            GreedyRandom gr = new GreedyRandom(net, score); // new greedy random training algorithm

            Iterate(gr, 100000, 0.01); // iterate through the greedy random algorithm

            QueryEquilateral(gr, net, trainingData, specieNames); // 
        }

        /// <summary>
        /// Iterate through a greedy random algorithm a certain number of times.
        /// Tries to minimize the score.
        /// </summary>
        /// <param name="gr">Algorithm to iterate through.</param>
        /// <param name="numIterations">Number of times to iterate.</param>
        /// <param name="minScore">Score to attempt to reach.</param>
        public void Iterate(GreedyRandom gr, int numIterations, double minScore) {
            int iterationNumber = 0; // number of iterations
            bool done = false; // is the algorithm done?

            // while not done...
            do {
                iterationNumber++; // increase number of iterations
                gr.Iteration(); // run an iteration

                // if iteration reaches limit or score reaches min score
                if(iterationNumber >= numIterations || gr.LastError < minScore) {
                    done = true; // set done = true
                }

                // write out iteration # and score
                Console.WriteLine("Iteration #" + iterationNumber + ", Score=" + gr.LastError + " (Minimize)");
            } while(!done);
        }

        /// <summary>
        /// Goes through each record in the training set and displays the ideal output and the actual output.
        /// </summary>
        /// <param name="net">RBF network.</param>
        /// <param name="trainingData"></param>
        /// <param name="specieNames"></param>
        public void QueryEquilateral(GreedyRandom gr, Network net, List<Record> trainingData, Dictionary<string, int> specieNames) {

            // invert the specie list
            Dictionary<int, string> invSpecies = new Dictionary<int, string>();
            foreach(string key in specieNames.Keys) {
                int value = specieNames[key];
                invSpecies[value] = key;
            }

            Equilateral eq = new Equilateral(specieNames.Count); // create encoding matrix for specie names
            
            // for each training record
            foreach(Record rec in trainingData) {

                double[] output = net.ComputeRegression(rec.Input); // run the record through the RBF network

                int idealIndex = eq.Decode(rec.Ideals); // find the ideal specie name for the record
                int actualIndex = eq.Decode(output); // find the calculated specie name for the record

                Console.WriteLine("Guess: " + invSpecies[actualIndex]
                        + ", Ideal: " + invSpecies[idealIndex]);
            }

            Console.WriteLine("Final error: " + gr.LastError);
        }
    }
}
