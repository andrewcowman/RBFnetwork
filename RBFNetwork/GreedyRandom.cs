using System;

namespace RBFNetwork {

    /// <summary>
    /// Greedy random training algorithm.
    /// </summary>
    public class GreedyRandom {

        private Network _network; // network to perform greedy random on
        private Scorer _scorer; // scoring object
        private Random _rand; // random object

        private double _lastError; // error of last iteration

        /// <summary>
        /// Error of last iteration.
        /// </summary>
        public double LastError {
            get { return _lastError; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="net">Network to perform greedy random on.</param>
        /// <param name="score">Scoring object.</param>
        public GreedyRandom(Network net, Scorer score) {
            _network = net;
            _scorer = score;

            _lastError = double.MaxValue;
            _rand = new Random();
        }

        /// <summary>
        /// Run through an iteration of greedy random.
        /// 1. Randomize the long term memory.
        /// 2. Calculate the error.
        /// 3. Compare current error to previous error.
        /// </summary>
        public void Iteration() {

            int len = _network.LongTermMemory.Length; // length of long term memory

            // make copy of old state
            double[] oldState = new double[len];
            Array.Copy(_network.LongTermMemory, oldState, len);

            Randomize(_network.LongTermMemory); // randomize the memory

            double currError = _scorer.Score(_network); // score the memory

            if(currError < _lastError) { // if current error < previous error

                _lastError = currError; // set last error to the current error

            } else {

                Array.Copy(oldState, _network.LongTermMemory, len); // replace long term memory with old state

            }
        }

        /// <summary>
        /// Randomizes the long term memory.
        /// </summary>
        /// <param name="ltm">Long term memory.</param>
        private void Randomize(double[] ltm) {
            for(int i = 0; i < ltm.Length; i++) {
                ltm[i] = (_rand.NextDouble() * 20) + -10; // random double between -10 and 10
            }
        }
    }
}
