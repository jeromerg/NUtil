using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUtil.Linq;

namespace NUtil.Math.Combinatorics.Pairwise
{
    public class PairwiseGenerator : IPairwiseGenerator
    {
        public IEnumerable<int[]> Generate(int[] dimSizes)
        {
            if (dimSizes == null)
                throw new ArgumentNullException("dimSizes");

            if (dimSizes.Length < 2)
                throw new ArgumentOutOfRangeException("dimSizes", "dimSizes.length must be greater or equal to 2");

            if (dimSizes.Any(s => s <= 0))
                throw new ArgumentOutOfRangeException("dimSizes", "Some dimension has no value");

            // Generates all existing pairs
            var pairSet = new PairSet(dimSizes);

            // Assigns pairs to the first generation
            // the first generation contains all pairs, that have not be used yet
            var pairGenerations = new List<PairSet> {pairSet};

            // Generates the tuples one by one until all pairs have been removed from the first generation
            // ReSharper disable once PossibleNullReferenceException
            while (pairGenerations.First().Any())
                yield return GenerateNextTuple(dimSizes, pairGenerations);
        }

        private int[] GenerateNextTuple([NotNull] int[] dimSizes, [NotNull] List<PairSet> pairGenerations)
        {
            var tuple = new Tuple(dimSizes.Length);

            while (tuple.FreeDims.Any())
                FreezeOneOrTwoDims(pairGenerations, tuple);

            return tuple.Result;
        }

        private void FreezeOneOrTwoDims([NotNull, ItemNotNull] List<PairSet> generations, [NotNull] Tuple tuple)
        {
            for (int generationIndex = 0; generationIndex < generations.Count; generationIndex++)
            {
                PairSet pairGeneration = generations[generationIndex];

                // ReSharper disable once AssignNullToNotNullAttribute
                Pair pair = TryPeakBestPair(pairGeneration, tuple);
                if (pair == null)
                    continue; // try to freeze dims in next generation

                MoveAppearingPairsToNextGeneration(generations, tuple, pair, generationIndex);

                // Add the newly frozen dimensions to tuple
                tuple.Add(pair.Dim1, pair.Val1);
                tuple.Add(pair.Dim2, pair.Val2);

                return;
            }
            throw new ArgumentException("This case should never happen");
        }

        private void MoveAppearingPairsToNextGeneration([NotNull, ItemNotNull] List<PairSet> generations,
                                                        [NotNull] Tuple tuple,
                                                        [NotNull] Pair pair,
                                                        int generationIndex)
        {
            bool isDim1New = tuple.Result[pair.Dim1] == -1;
            bool isDim2New = tuple.Result[pair.Dim2] == -1;

            if(isDim1New && isDim2New)
                MovePairToNextGeneration(pair, generations, generationIndex);

            foreach (DimValue dimValue in tuple.FrozenDimValues)
            {
                if (isDim1New)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var p1 = new Pair(dimValue.Dim, dimValue.Val, pair.Dim1, pair.Val1);
                    MovePairToNextGeneration(p1, generations, generationIndex);
                }

                if (isDim2New)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var p2 = new Pair(dimValue.Dim, dimValue.Val, pair.Dim2, pair.Val2);
                    MovePairToNextGeneration(p2, generations, generationIndex);
                }
            }
        }

        private void MovePairToNextGeneration([NotNull] Pair pair,
                                              [NotNull, ItemNotNull] List<PairSet> generations,
                                              int generationIndexStart)
        {
            for (int generationIndex = generationIndexStart; generationIndex < generations.Count; generationIndex++)
            {
                PairSet pairGeneration = generations[generationIndex];

                // ReSharper disable once PossibleNullReferenceException
                bool removed = pairGeneration.Remove(pair);
                if (!removed)
                    continue;

                PairSet nextGeneration = GetOrCreateNextGeneration(generations, generationIndex);
                // ReSharper disable once PossibleNullReferenceException
                nextGeneration.Add(pair);
                return;
            }

            throw new ArgumentException("pair was not found within the generations >= " + generationIndexStart);
        }

        [CanBeNull]
        private Pair TryPeakBestPair([NotNull] PairSet pairs, [NotNull] Tuple tuple)
        {
            return TryPeakPairInFreeDims(pairs, tuple)
                   ?? TryPeakPairInFreeAndFrozenDims(pairs, tuple);
        }

        [CanBeNull]
        private Pair TryPeakPairInFreeDims([NotNull] PairSet pairs, [NotNull] Tuple tuple)
        {
            return tuple.FreeDims
                        .TriangularProductWithoutDiagonal((dim1, dim2) => pairs.FirstOrDefault(dim1, dim2))
                        .FirstOrDefault(pair => pair != null);
        }

        [CanBeNull]
        private Pair TryPeakPairInFreeAndFrozenDims([NotNull] PairSet pairs, [NotNull] Tuple tuple)
        {
            // ReSharper disable once PossibleNullReferenceException
            return tuple
                .FreeDims
                .CartesianProduct(tuple.FrozenDimValues, (dim1, val1) => pairs.FirstOrDefault(val1.Dim, val1.Val, dim1))
                .FirstOrDefault(pair => pair != null);
        }

        [NotNull]
        private static PairSet GetOrCreateNextGeneration([NotNull, ItemNotNull] List<PairSet> generations, int generationIndex)
        {
            PairSet newGeneration;
            if (generationIndex == generations.Count - 1)
            {
                newGeneration = new PairSet();
                generations.Add(newGeneration);
            }
            else
            {
                newGeneration = generations[generationIndex + 1];
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            return newGeneration;
        }
    }
}