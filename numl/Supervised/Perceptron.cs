﻿using System;
using System.Linq;
using numl.Math.LinearAlgebra;
using System.Collections.Generic;

namespace numl.Supervised
{
    public class PerceptronGenerator : Generator
    {
        public bool Normalize { get; set; }

        public PerceptronGenerator()
        {
            Normalize = true;
        }

        public PerceptronGenerator(bool normalize)
        {
            Normalize = normalize;
        }

        public override IModel Generate(Matrix X, Vector Y)
        {
            Vector w = Vector.Zeros(X.Cols);
            Vector a = Vector.Zeros(X.Cols);

            double wb = 0;
            double ab = 0;

            int n = 1;

            if (Normalize)
            {
                for (int i = 0; i < X.Rows; i++)
                {
                    var norm = X.Row(i).Norm(2);
                    for (int j = 0; j < X.Cols; j++)
                        X[i, j] = X[i, j] / norm;
                }
            }

            // repeat 10 times for *convergence*
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < X.Rows; j++)
                {
                    var x = X[j];
                    var y = Y[j];

                    // perceptron update
                    if (y * (w.Dot(x) + wb) <= 0)
                    {
                        w = w + y * x;
                        wb += y;
                        a = (a + y * x) + n;
                        ab += y * n;
                    }

                    n += 1;
                }
            }

            return new PerceptronModel
            {
                W = w - (a / n),
                B = wb - (ab / n),
                Normalized = Normalize
            };
        }
    }

    public class PerceptronModel : Model
    {
        public Vector W { get; set; }
        public double B { get; set; }
        public bool Normalized { get; set; }

        public override double Predict(Vector y)
        {
            if (Normalized)
                y = y / y.Norm();

            return W.Dot(y) + B;
        }
    }
}