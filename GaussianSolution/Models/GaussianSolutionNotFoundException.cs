using System;

namespace GaussSolution.Models
{
    public class GaussianSolutionNotFoundException : Exception {
        public GaussianSolutionNotFoundException(string msg)
            : base("Solution couldn't be found: \r\n" + msg) {
        }

        GaussianSolutionNotFoundException() : base()
        {
        }

        GaussianSolutionNotFoundException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}