using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFCogServices.Services;

namespace XFCogServices.Analyzer
{
    public class EmotionAnalysis
    {
        public static async Task<Emotion[]> GetHappinessAsync(Stream stream)
        {
            var emotionClient = new EmotionServiceClient(ServiceKeys.Emotion);
            var emotionResults = await emotionClient.RecognizeAsync(stream);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        public static async Task<Emotion[]> GetHappinessUrlAsync(string imageUrl)
        {
            var emotionClient = new EmotionServiceClient(ServiceKeys.Emotion);
            var emotionResults = await emotionClient.RecognizeAsync(imageUrl);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }


        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScoreAsync(Stream stream)
        {
            Emotion[] emotionResults = await GetHappinessAsync(stream);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Count();
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }

}
