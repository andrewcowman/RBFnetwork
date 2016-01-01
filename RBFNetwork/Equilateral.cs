using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBFNetwork {

    /// <summary>
    /// This class is used to encode names using equilateral encoding.
    /// </summary>
    public class Equilateral {

        private double[][] _matrix; // encoding matrix

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numClasses">Number of classes that need encoding.</param>
        public Equilateral(int numClasses) {
            _matrix = CreateMatrix(numClasses); // create matrix for number of classes
        }

        /// <summary>
        /// Return row in encoded matrix.
        /// </summary>
        /// <param name="idx">Row to return.</param>
        /// <returns>Encoded row.</returns>
        public double[] Encode(int idx) {
            return _matrix[idx]; // return encoded row
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        public int Decode(double[] da) {
            double min = double.MaxValue;
            int minSet = -1;

            for(int i = 0; i < _matrix.Length; i++) {
                double dist = GetDistance(da, i);
                if(dist < min) {
                    min = dist;
                    minSet = i;
                }
            }
            return minSet;
        }

        private double[][] CreateMatrix(int numClasses) {
            double[][] temp = new double[numClasses][];
            for(int i = 0; i < numClasses; i++) {
                temp[i] = new double[numClasses - 1];
            }

            temp[0][0] = -1.0;
            temp[1][0] = 1.0;

            for(int i = 2; i < numClasses; i++) {
                double scale = Math.Sqrt(i * i - 1.0) / i;

                for(int j = 0; j < i; j++) {
                    for(int k = 0; k < i - 1; k++) {
                        temp[j][k] *= scale;
                    }
                }

                double recip = -1.0 / i;
                for(int j = 0; j < i; j++) {
                    temp[j][i - 1] = recip;
                }
                for(int j = 0; j < i - 1; j++) {
                    temp[i][j] = 0.0;
                }
                temp[i][i - 1] = 1.0;
            }

            foreach(double[] da in temp) {
                for(int i = 0; i < da.Length; i++) {
                    double min = -1, max = 1;
                    double nHigh = 1, nLow = 0;

                    da[i] = ((da[i] - min) / (max - min)) * (nHigh - nLow) + nLow;
                }
            }

            return temp;
        }

        private double GetDistance(double[] data, int set) {
            double temp = data.Select((t, i) => Math.Pow(t - _matrix[set][i], 2)).Sum();
            return Math.Sqrt(temp);
        }

    }
}
