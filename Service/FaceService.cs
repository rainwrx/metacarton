using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace MetaCarton
{
    public class FaceService
    {
        private const string FACE_LIST = "meta_carton";
        private static readonly string SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
        private static readonly string ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");
        public static readonly string RECOGNITION_MODEL1 = RecognitionModel.Recognition01; 
        public static readonly string RECOGNITION_MODEL2 = RecognitionModel.Recognition02;

        public static IFaceClient Authenticate()
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
        }

        public static async Task Delete(IFaceClient client)
        {
            var list = await client.FaceList.GetAsync(FACE_LIST);
            if (list != null) await client.FaceList.DeleteAsync(FACE_LIST);
        }

        public static async Task Create(IFaceClient client)
        {
            var list = await client.FaceList.GetAsync(FACE_LIST);
            await client.FaceList.CreateAsync(FACE_LIST);
        }

        public static async Task AddFace(IFaceClient client, Stream stream, string facesUrl)
        {
            var detectedFaces = await client.Face.DetectWithStreamAsync(stream, recognitionModel: RECOGNITION_MODEL1);
            foreach(var face in detectedFaces)
            {
                //await client.FaceList.AddFaceFromStreamAsync()
            }
        }

        public static async Task<IList<DetectedFace>> Detect(IFaceClient client, string imageFile, string recognitionModel)
        {
            IList<DetectedFace> detectedFaces = null;

            //using (var stream = File.Open(imageFile, FileMode.Open)) 
            //{
            //    detectedFaces = await client.Face.DetectWithStreamAsync(stream, recognitionModel: recognitionModel);
            //}
            try
            {
                detectedFaces = await client.Face.DetectWithUrlAsync(imageFile, recognitionModel: recognitionModel);

            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return detectedFaces;
        }
    }
}
