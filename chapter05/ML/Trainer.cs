﻿using System;
using System.IO;

using chapter05.Common;
using chapter05.ML.Base;
using chapter05.ML.Objects;

using Microsoft.ML;

namespace chapter05.ML
{
    public class Trainer : BaseML
    {
        public void Train(string trainingFileName, string testFileName)
        {
            if (!System.IO.File.Exists(trainingFileName))
            {
                Console.WriteLine($"Failed to find training data file ({trainingFileName}");

                return;
            }

            if (!System.IO.File.Exists(testFileName))
            {
                Console.WriteLine($"Failed to find test data file ({testFileName}");

                return;
            }

            var trainingDataView = MlContext.Data.LoadFromTextFile<FileData>(trainingFileName, ',', hasHeader: false);

            IEstimator<ITransformer> dataProcessPipeline = MlContext.Transforms.Concatenate("Features",
                typeof(File).ToPropertyList<FileData>(nameof(FileData.Label)))
                .Append(MlContext.Transforms.NormalizeMeanVariance(inputColumnName: "Features",
                    outputColumnName: "FeaturesNormalizedByMeanVar"));

            var trainer = MlContext.Clustering.Trainers.KMeans();

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            var trainedModel = trainingPipeline.Fit(trainingDataView);
            MlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath);

            var testDataView = MlContext.Data.LoadFromTextFile<Objects.FileData>(testFileName, ',', hasHeader: false);

            var modelMetrics = MlContext.Clustering.Evaluate(data: testDataView,
                labelColumnName: nameof(FileData.Label),
                scoreColumnName: "Score");

            Console.WriteLine($"Accuracy: {modelMetrics.AverageDistance}");
            Console.WriteLine($"Area Under Curve: {modelMetrics.DaviesBouldinIndex}");
            Console.WriteLine($"Area under Precision recall Curve: {modelMetrics.NormalizedMutualInformation}");
        }
    }
}