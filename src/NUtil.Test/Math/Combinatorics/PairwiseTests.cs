﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using NUtil.Math.Combinatorics.Pairwise;

namespace NUtil.Test.Math.Combinatorics
{
    [TestFixture]
    [SuppressMessage("ReSharper", "IteratorMethodResultIsIgnored")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PairwiseTests
    {
        [Test]
        public void TestAlgorithmPerformance_DimsWithIdenticalSizes()
        {
            Console.WriteLine("Name; Info; Sum Repetitions; Max Repetitions; pairProdCard; cartProdCard; rate(log10)");

            for (int dimAmount = 2; dimAmount < 10; dimAmount++)
            {
                for (int valAmount = 1; valAmount < 10; valAmount++)
                {
                    int[] dimSizes = Enumerable.Repeat(valAmount, dimAmount).ToArray();
                    TestAlgorithmPerformance_SingleRun(dimSizes, string.Format("IdenticalSize;  ({0}:{1})", dimAmount, valAmount));

                    dimSizes = Enumerable.Range(1, dimAmount).ToArray();
                    TestAlgorithmPerformance_SingleRun(dimSizes,
                                                       string.Format("IncreasingSize; ({0}:1-{1})", dimAmount, valAmount));

                    dimSizes = dimSizes.Reverse().ToArray();
                    TestAlgorithmPerformance_SingleRun(dimSizes,
                                                       string.Format("DecreasingSize; ({0}:{1}-1)", dimAmount, valAmount));
                }
            }
        }


        [Test]
        public void TestFourTuples()
        {
            int[] dimSizes = {2, 2, 1, 1};

            int[][] tuples = Generate(dimSizes);

            Assert.AreEqual(4, tuples.Length);
            Assert.Contains(new[] {0, 0, 0, 0}, tuples);
            Assert.Contains(new[] {1, 1, 0, 0}, tuples);
            Assert.Contains(new[] {0, 1, 0, 0}, tuples);
            Assert.Contains(new[] {1, 0, 0, 0}, tuples);
        }

        [Test]
        [TestCase(new int[] {})]
        [TestCase(new[] {0})]
        [TestCase(new[] {1})]
        [TestCase(new[] {5})]
        [TestCase(new[] {0, 0})]
        [TestCase(new[] {5, 0})]
        [TestCase(new[] {0, 5})]
        [TestCase(new[] {1, 1, 1, 1, 0})]
        public void TestLessThanTwoDimsOrAtLeastOneEmptyDim(int[] dimSizes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Generate(dimSizes));
        }

        [Test]
        public void TestManyTuples()
        {
            int[] dimSizes = {15, 10, 3, 1};

            int[][] tuples = Generate(dimSizes);

            // check that all pairs are covered
            for (int dim1 = 0; dim1 < 3; dim1++)
                for (int dim2 = dim1 + 1; dim2 < dimSizes.Length; dim2++)
                    for (int val1 = 0; val1 < dimSizes[dim1]; val1++)
                        for (int val2 = 0; val2 < dimSizes[dim2]; val2++)
                        {
                            int count = tuples.Count(t => t[dim1] == val1 && t[dim2] == val2);
                            Assert.Greater(count, 0);
                        }
        }

        [Test]
        public void TestNull()
        {
            Assert.Throws<ArgumentNullException>(() => Generate(null));
        }

        [Test]
        //[TestCase(new[] {1, 1})]
        [TestCase(new[] {1, 1, 1})]
        //[TestCase(new[] {1, 1, 1, 1, 1, 1, 1, 1})]
        public void TestSingleTuple(int[] dimSizes)
        {
            int[][] tuples = Generate(dimSizes);

            Assert.AreEqual(1, tuples.Length);
            Assert.AreEqual(Enumerable.Repeat(0, dimSizes.Length), tuples[0]);
        }


        [Test]
        [TestCase]
        public void TestThreeTuples()
        {
            int[] dimSizes = {1, 3, 1, 1, 1};
            int[][] tuples = Generate(dimSizes);

            Assert.AreEqual(3, tuples.Length);
            Assert.Contains(new[] {0, 0, 0, 0, 0}, tuples);
            Assert.Contains(new[] {0, 1, 0, 0, 0}, tuples);
            Assert.Contains(new[] {0, 2, 0, 0, 0}, tuples);
        }

        [Test]
        public void TestTwoTuples()
        {
            int[] dimSizes = {1, 2};

            int[][] tuples = Generate(dimSizes);

            Assert.AreEqual(2, tuples.Length);
            Assert.Contains(new[] {0, 0}, tuples);
            Assert.Contains(new[] {0, 1}, tuples);
        }

        [Test]
        [TestCase]
        public void TestTwoTuples2()
        {
            int[] dimSizes = {1, 2, 1, 1, 1};

            int[][] tuples = Generate(dimSizes);

            Assert.AreEqual(2, tuples.Length);
            Assert.Contains(new[] {0, 0, 0, 0, 0}, tuples);
            Assert.Contains(new[] {0, 1, 0, 0, 0}, tuples);
        }

        private static int[][] Generate(int[] dimSizes)
        {
            return new PairwiseGenerator().Generate(dimSizes).ToArray();
        }

        private void TestAlgorithmPerformance_SingleRun(int[] dimSizes, string name)
        {
            int sumRepetitions = 0;
            int maxRepetitions = 0;

            int[][] tuples = Generate(dimSizes);

            // check that all pairs are covered
            for (int dim1 = 0; dim1 < 3; dim1++)
                for (int dim2 = dim1 + 1; dim2 < dimSizes.Length; dim2++)
                    for (int val1 = 0; val1 < dimSizes[dim1]; val1++)
                        for (int val2 = 0; val2 < dimSizes[dim2]; val2++)
                        {
                            int amountOfPairOccurence = tuples.Count(t => t[dim1] == val1 && t[dim2] == val2);
                            Assert.Greater(amountOfPairOccurence, 0);
                            sumRepetitions += amountOfPairOccurence - 1;
                            maxRepetitions = System.Math.Max(maxRepetitions, amountOfPairOccurence - 1);
                        }

            long cartesianProductCardinal = dimSizes.Select(s => (long) s).Aggregate((acc, val) => acc*val);
            int pairwiseProductCardinal = tuples.Length;

            Console.WriteLine("{0};{1};{2};{3:E2};{4:E2};  {5}",
                              name,
                              sumRepetitions,
                              maxRepetitions,
                              pairwiseProductCardinal,
                              (double) cartesianProductCardinal,
                              System.Math.Log10((double) pairwiseProductCardinal/cartesianProductCardinal));
        }

  }
}