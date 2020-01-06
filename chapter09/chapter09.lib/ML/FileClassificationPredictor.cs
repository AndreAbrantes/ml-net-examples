﻿using System.IO;

using chapter09.lib.Data;
using chapter09.lib.Helpers;
using chapter09.lib.ML.Base;
using chapter09.lib.ML.Objects;

using Microsoft.ML;

namespace chapter09.lib.ML
{
    public class FileClassificationPredictor : BaseML
    {
        public FileClassificationResponseItem Predict(string fileName)
        {
            var bytes = File.ReadAllBytes(fileName);

            return Predict(new FileClassificationResponseItem(bytes));
        }

        public FileClassificationResponseItem Predict(FileClassificationResponseItem file)
        {
            ITransformer mlModel;

            using (var stream = new FileStream(Common.Constants.MODEL_PATH, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                mlModel = MlContext.Model.Load(stream, out _);
            }

            var predictionEngine = MlContext.Model.CreatePredictionEngine<FileData, FileDataPrediction>(mlModel);

            var prediction = predictionEngine.Predict(file.ToFileData());

            file.Confidence = prediction.Probability;
            file.IsMalicious = prediction.Label;

            return file;
        }
    }
}