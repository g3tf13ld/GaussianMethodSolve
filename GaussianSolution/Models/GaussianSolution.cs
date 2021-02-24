using System;
using GaussSolution.Models;

namespace GaussianSolution {
    public class LinearSystem {
        
        private double[,] initialAMatrix;
        private double[,] aMatrix;
        private double[] initialBVector;
        private double[] bVector;
        private double[] xVector;
        
        private double eps;
        private int size;
        
        public double[] XVector => xVector;

        public LinearSystem(double[,] aMatrix, double[] bVector)
            : this(aMatrix, bVector, 0.0001) {
        }

        private LinearSystem(double[,] aMatrix, double[] bVector, double eps) {
            if (aMatrix == null || bVector == null)
                throw new ArgumentNullException("One or more params are null.");

            var bLength = bVector.Length;
            var aLength = aMatrix.Length;
            if (aLength != bLength * bLength)
                throw new ArgumentException("The rows and columns number in A-matrix must match the elements number in B-vector");
            
            // Save the initial A-matrix.
            initialAMatrix = aMatrix;  
            // A-matrix copy for use in calculations.
            this.aMatrix = (double[,])aMatrix.Clone(); 
            // Save the initial B-vector.
            initialBVector = bVector;  
            // B-vector copy for use in calculations.
            this.bVector = (double[])bVector.Clone();  
            xVector = new double[bLength];
            size = bLength;
            this.eps = eps;

            GaussianSolve();
        }

        //  Column indexes array initializing.
        private int[] InitIndex() {
            var index = new int[size];
            for (var i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        // Main matrix item finding.
        private double FindR(int row, int[] index) {
            var maxIndex = row;
            var max = aMatrix[row, index[maxIndex]];
            var maxAbs = Math.Abs(max);
            
            for (var curIndex = row + 1; curIndex < size; ++curIndex) {
                var cur = aMatrix[row, index[curIndex]];
                var curAbs = Math.Abs(cur);
                if (curAbs > maxAbs) {
                    maxIndex = curIndex;
                    max = cur;
                    maxAbs = curAbs;
                }
            }

            if (maxAbs < eps) {
                if (Math.Abs(bVector[row]) > eps)
                    throw new GaussianSolutionNotFoundException("The system of equations is inconsistent.");
                else
                    throw new GaussianSolutionNotFoundException("The system of equations has many solutions.");
            }

            // Swap column indexes.
            var temp = index[row];
            index[row] = index[maxIndex];
            index[maxIndex] = temp;

            return max;
        }

        // Finding solution of linear equations system using Gaussian method.
        private void GaussianSolve() {
            var index = InitIndex();
            GaussianForwardStroke(index);
            GaussianBackwardStroke(index);
        }
        
        private void GaussianForwardStroke(int[] index) {
            // Move through each row from top to bottom.
            for (var i = 0; i < size; ++i) {
                // 1) Change the main item.
                var r = FindR(i, index);

                // 2) Current row of A-matrix transformation.
                for (var j = 0; j < size; ++j)
                    aMatrix[i, j] /= r;

                // 3) The i-th element of B-vector transformation.
                bVector[i] /= r;

                // 4) Subtracting the current line from all the lines below.
                for (var k = i + 1; k < size; ++k) {
                    var p = aMatrix[k, index[i]];
                    for (var j = i; j < size; ++j)
                        aMatrix[k, index[j]] -= aMatrix[i, index[j]] * p;
                    bVector[k] -= bVector[i] * p;
                    aMatrix[k, index[i]] = 0.0;
                }
            }
        }

        private void GaussianBackwardStroke(int[] index) {
            // Move through each row from bottom to top.
            for (var i = size - 1; i >= 0; --i) {
                // 1) Set the initial value of the x-element.
                var xI = bVector[i];

                // 2) Adjusting the value.
                for (var j = i + 1; j < size; ++j)
                    xI -= xVector[index[j]] * aMatrix[i, index[j]];
                xVector[index[i]] = xI;
            }
        }
    }
}
