using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFCogServices.Services;

namespace XFCogServices.Analyzer
{
    class ClientHelper
    {
        readonly VisionServiceClient visionClient = new VisionServiceClient(ServiceKeys.CognitiveServiceKey);

        public async Task<List<ClientModel>> GetImageDescriptionFromUrl(string imageUri, List<ClientModel> clientModelList)
        {
            clientModelList = new List<ClientModel>();

            VisualFeature[] features = {
                VisualFeature.Tags,
                VisualFeature.Categories,
                VisualFeature.Description,
                VisualFeature.Adult,
                VisualFeature.Faces,
                VisualFeature.ImageType,
                VisualFeature.Color };
            string[] details = { "Celebrities", "Landmarks" };

            AnalysisResult result = await visionClient.AnalyzeImageAsync(imageUri, features.ToList(), details);

            return ProcessVisionResult(result);

        }

        private List<ClientModel> ProcessVisionResult(AnalysisResult result)
        {
            try
            {
                List<ClientModel> clientModelList = new List<ClientModel>();


                

                clientModelList.Add(new ClientModel
                {
                    faceRectangels = result.Faces.Select(x => x.FaceRectangle).ToArray(),
                    genders = result.Faces.Select(x => x.Gender).ToArray(),
                    faceAges = result.Faces.Select(x => x.Age).ToArray(),
                    imageFormat = result.Metadata.Format,
                    imageDimensions = result.Metadata.Width + " x " + result.Metadata.Height,
                    clipArtType = result.ImageType.ClipArtType,
                    lineDrawingType = result.ImageType.LineDrawingType,
                    isBlackAndWhite = result.Color.IsBWImg,
                    isAdultContent = result.Adult.IsAdultContent,
                    adultScore = result.Adult.AdultScore,
                    isRacyContent = result.Adult.IsRacyContent,
                    racyScore = result.Adult.RacyScore,
                    dominantColorBackground = result.Color.DominantColorBackground,
                    dominantColorForeground = result.Color.DominantColorForeground,
                    accentColor = result.Color.AccentColor,
                    dominantColors = result.Color.DominantColors,
                    categories = result.Categories.Select(x => x.Name).ToArray(),
                    tags = result.Tags.Select(x => x.Name).ToArray(),
                    captions = result.Description.Captions.Select(x => x.Text).ToArray()
                });

                return clientModelList;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace.ToString());
                return null;
            }

        }


        public async Task<List<ClientModel>> GetImageDescription(Stream imageStream, List<ClientModel> clientModelList)
        {
            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description, VisualFeature.Adult, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Color };
            string[] details = { "celebrities", "landmarks" };

            AnalysisResult result = await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), details);
            return ProcessVisionResult(result);
        }

        public List<ClientItemModel> GetResultList(List<ClientModel> clientModelList, List<ClientItemModel> clientModelItemList)
        {
            clientModelItemList = new List<ClientItemModel>();
            var item = clientModelList[0];
            string caps = null;
            string categories = null;
            string dominantColors = null;
            string ages = null;
            string genders = null;
            string tags = null;
            foreach (string s in item.captions)
            {
                caps += s;
            }
            clientModelItemList.Add(new ClientItemModel { itemName = "Image Desc", itemDesc = caps });
            foreach (string s in item.tags)
            {
                tags += "#" + s + " ";
            }
            clientModelItemList.Add(new ClientItemModel { itemName = "Tags", itemDesc = tags });

            var faceIndex = 0;
            foreach (int s in item.faceAges)
            {
                faceIndex++;
                ages += "Person " + (faceIndex) + " : " + s + ", ";
            }
            if (faceIndex != 0)
                clientModelItemList.Add(new ClientItemModel { itemName = "Ages", itemDesc = ages });
            var genderIndex = 0;
            foreach (string s in item.genders)
            {
                genderIndex++;
                genders += "Person " + (genderIndex) + " : " + s + ", ";
            }
            if (genderIndex != 0)
                clientModelItemList.Add(new ClientItemModel { itemName = "Genders", itemDesc = genders });

            foreach (string s in item.categories)
            {
                categories += s;
            }
            clientModelItemList.Add(new ClientItemModel { itemName = "Categories", itemDesc = categories + ", " });

            clientModelItemList.Add(new ClientItemModel { itemName = "Is Adult Content", itemDesc = item.isAdultContent.ToString() });
            clientModelItemList.Add(new ClientItemModel { itemName = "Adult Score", itemDesc = item.adultScore.ToString() });
            clientModelItemList.Add(new ClientItemModel { itemName = "Is Racy Content", itemDesc = item.isRacyContent.ToString() });
            clientModelItemList.Add(new ClientItemModel { itemName = "Racy Score", itemDesc = item.racyScore.ToString() });
            clientModelItemList.Add(new ClientItemModel { itemName = "Accent Color", itemDesc = "#" + item.accentColor });
            clientModelItemList.Add(new ClientItemModel { itemName = "Dominant Color Background", itemDesc = item.dominantColorBackground });
            clientModelItemList.Add(new ClientItemModel { itemName = "Dominant Color Foreground", itemDesc = item.dominantColorForeground });
            foreach (string s in item.dominantColors)
            {
                dominantColors += s + ", ";
            }
            clientModelItemList.Add(new ClientItemModel { itemName = "Dominant Colors", itemDesc = dominantColors });
            clientModelItemList.Add(new ClientItemModel { itemName = "Image Dimensions", itemDesc = item.imageDimensions });
            clientModelItemList.Add(new ClientItemModel { itemName = "Image Format", itemDesc = item.imageFormat });
            clientModelItemList.Add(new ClientItemModel { itemName = "Clip Art Type", itemDesc = item.clipArtType.ToString() });


            return clientModelItemList;
        }
    }
}
