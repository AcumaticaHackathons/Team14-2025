using Amazon;
using Amazon.CloudSearchDomain.Model;
using Amazon.CloudWatch;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using MeshAPIUI;
using PX.Data;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AcuPhotoBooth
{
    public static class StaticObjects
    {
        public class FileProcessResult
        {
            public Stream STLFile { get; set; }
            public Stream GCodeFile { get; set; }
            public Stream PreviewFile { get; set; }
        }
        public static FileProcessResult ProcessFile(string executablePath, string configFile, int maxSize, Stream fromFile,string fileID,string OrderType,string OrderNbr,Guid OrigFileID)
        {
            var graph = PXGraph.CreateInstance<SOOrderEntry>();
            var retVal = new FileProcessResult();
            var bucket = "imagefiles-us-east-2";
            var region = "us-east-2";
            ThreeDProgress test =PXSelect<ThreeDProgress,Where<ThreeDProgress.orderType,Equal<Required<ThreeDProgress.orderType>>,And<ThreeDProgress.orderNbr,Equal<Required<ThreeDProgress.orderNbr>>,And<ThreeDProgress.fileid,Equal<Required<ThreeDProgress.fileid>>>>>>.Select(graph,OrderType,OrderNbr,OrigFileID);
            if(test==null)
            {
                PXDatabase.Insert<ThreeDProgress>(new PXDataFieldAssign<ThreeDProgress.progress1>(0)
                        , new PXDataFieldAssign<ThreeDProgress.progress2>(0)
                        , new PXDataFieldAssign<ThreeDProgress.orderType>(OrderType)
                        , new PXDataFieldAssign<ThreeDProgress.orderNbr>(OrderNbr)
                        , new PXDataFieldAssign<ThreeDProgress.fileid>(OrigFileID));
            }
            else
            {
                PXDatabase.Update<ThreeDProgress>(new PXDataFieldAssign<ThreeDProgress.progress1>(0)
                        , new PXDataFieldAssign<ThreeDProgress.progress2>(0)
                        , new PXDataFieldRestrict<ThreeDProgress.orderType>(OrderType)
                        , new PXDataFieldRestrict<ThreeDProgress.orderNbr>(OrderNbr)
                        , new PXDataFieldRestrict<ThreeDProgress.fileid>(OrigFileID));
            }
            var awsConfig = new AmazonS3Config()
            {
                ServiceURL = $"https://s3.{region}.amazonaws.com",
                //RegionEndpoint = RegionEndpoint.USEast1,
                SignatureMethod = SigningAlgorithm.HmacSHA256,
                SignatureVersion = "4",
                //AuthenticationRegion= "us-east-2",


            };


            var aws = AWSClientFactory.CreateAmazonS3Client("AKIAVYV52DDRVVV4GXMH", "0o1KZ3Pc9uKrG28qjeztGX2tOhlaFWe5UykaipuV", awsConfig);

            var result = UploadFile(aws, "imagefiles-us-east-2", fileID, fromFile);
            if(result)
            {
                var filename = $"https://{bucket}.s3.{region}.amazonaws.com/{fileID}";
                SendImageModel ImageModel = new SendImageModel(filename);
                string imageID = SendImageProcessing(JsonSerializer.Serialize(ImageModel));
                bool isReadyToProcess = false;
                while (!isReadyToProcess)
                {
                    ThreeDTaskModel ThreeDTask = Retrieve3DTask(imageID);
                    PXDatabase.Update<ThreeDProgress>(new PXDataFieldAssign<ThreeDProgress.progress1>(ThreeDTask.progress)
                        , new PXDataFieldRestrict<ThreeDProgress.orderType>(OrderType)
                        , new PXDataFieldRestrict<ThreeDProgress.orderNbr>(OrderNbr)
                        , new PXDataFieldRestrict<ThreeDProgress.fileid>(OrigFileID));
                    //textBox3.Text = ThreeDTask.progress.ToString();
                    //textBox5.Text = DateTime.Now.Subtract(start).TotalSeconds.ToString();
                    //Application.DoEvents();
                    if (ThreeDTask.status == "SUCCEEDED")
                    {
                        isReadyToProcess = true;
                        //textBox3.Text = ThreeDTask.id;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                RemeshRequestModel remeshModel = new RemeshRequestModel(imageID);
                var remeshID = RemeshProcessing(JsonSerializer.Serialize(remeshModel));
                System.Diagnostics.Debug.Print(remeshID);
                bool isRemeshComplete = false;
                while (!isRemeshComplete)
                {
                    RemeshModel RemeshResult = RetrieveRemeshTask(remeshID);
                    PXDatabase.Update<ThreeDProgress>(new PXDataFieldAssign<ThreeDProgress.progress2>(RemeshResult.progress)
    , new PXDataFieldRestrict<ThreeDProgress.orderType>(OrderType)
    , new PXDataFieldRestrict<ThreeDProgress.orderNbr>(OrderNbr)
    , new PXDataFieldRestrict<ThreeDProgress.fileid>(OrigFileID));
                    //textBox4.Text = RemeshResult.progress.ToString();
                    //textBox5.Text = DateTime.Now.Subtract(start).TotalSeconds.ToString();
                    //Application.DoEvents();
                    if (RemeshResult.status == "SUCCEEDED")
                    {
                        isRemeshComplete = true;
                        var client = new WebClient();
                        client.DownloadFile(RemeshResult.thumbnail_url, string.Format("c:\\temp\\{0}.stl.jpg", fileID)); 
                        client.DownloadFile(RemeshResult.model_urls.stl, string.Format("c:\\temp\\{0}.stl",fileID));
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                var stlFile = string.Format("c:\\temp\\{0}.stl", fileID);
                var outFile = string.Format("c:\\temp\\{0}.gcode", fileID); //Path.GetFullPath(stlFile) + Path.GetFileNameWithoutExtension(stlFile) + ".gcode";
                Convert_STL("C:\\Program Files\\Prusa3D\\PrusaSlicer", stlFile, outFile, "c:\\temp\\ender-3v3.ini", maxSize);
                SendFile(Path.GetFileName(outFile),  System.IO.File.ReadAllBytes(outFile));
                retVal.STLFile=new MemoryStream(File.ReadAllBytes(stlFile));
                retVal.GCodeFile = new MemoryStream(File.ReadAllBytes(outFile));
                retVal.PreviewFile = new MemoryStream(File.ReadAllBytes(stlFile+".jpg"));
            }
            return retVal;  
        }
        public static bool Convert_STL(string executablePath, string fromFile, string toFile, string configFile, int maxSize)
        {
            var info = new ProcessStartInfo(Path.Combine(executablePath, "prusa-slicer"), $"-g --load \"{configFile}\" {fromFile} --scale-to-fit {maxSize},{maxSize},{maxSize} --output \"{toFile}\"");
            var process = Process.Start(info);
            process.WaitForExit();
            return File.Exists(toFile);
        }
        public static string SendFile(string name, byte[] data)
        {
            WebResponse response = null;
            try
            {
                string sWebAddress = $"http://192.168.228.203/upload/{name}";

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(sWebAddress);
                wr.Accept = "application/json, text/plain, */*";
                //wr.Connection = "keep-alive";
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
                Stream stream = wr.GetRequestStream(); //new MemoryStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                stream.Write(boundarybytes, 0, boundarybytes.Length);
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file", name, "application/octet-stream");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);

                byte[] formitembytes = data; //System.Text.Encoding.UTF8.GetBytes(filePath);
                stream.Write(formitembytes, 0, formitembytes.Length);
                stream.Write(boundarybytes, 0, boundarybytes.Length);

                stream.Write(data, 0, data.Length);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                stream.Write(trailer, 0, trailer.Length);

                response = wr.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string responseData = streamReader.ReadToEnd();
                return responseData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

       
        public static bool UploadFile(
                IAmazonS3 client,
                string bucketName,
                string objectName,
                Stream inputStream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                //FilePath = filePath,
                InputStream = inputStream
            };
            var response = client.PutObject(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                System.Diagnostics.Debug.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }

       

        public static string SendImageProcessing(string sendImageModelasString)
        {
            var client = new WebClient();
            //client.Headers.Add("Authorization", "msy_OrODMURN4rFn64eVyqM6pQwTsGwq2GOWbntw");
            client.Headers.Add("Authorization", "msy_krH3TRsitb0qchcoeiPLJBSEV2rVr6rpAFnU");
            
            try
            {
                var response = client.UploadData("https://api.meshy.ai/openapi/v1/image-to-3d", "POST", System.Text.Encoding.ASCII.GetBytes(sendImageModelasString));

                SendImageResultModel returnsModel = JsonSerializer.Deserialize<SendImageResultModel>(System.Text.Encoding.ASCII.GetString(response), JsonSerializerOptions.Default);

                return returnsModel.result;
            }
            catch (Exception ex)
            {
                return "";
            }
            finally { }
        }

        public static ThreeDTaskModel Retrieve3DTask(string sendImageID)
        {

            var client = new WebClient();
            client.Headers.Add("Authorization", "msy_krH3TRsitb0qchcoeiPLJBSEV2rVr6rpAFnU");
            var response = client.DownloadData("https://api.meshy.ai/openapi/v1/image-to-3d/" + sendImageID);

            string respResult = System.Text.Encoding.ASCII.GetString(response);
            ThreeDTaskModel result = JsonSerializer.Deserialize<ThreeDTaskModel>(respResult, JsonSerializerOptions.Default);
            /// Need to decide what this should pass
            return result;
        }

        public static string RemeshProcessing(string remeshModel)
        {
            var client = new WebClient();
            client.Headers.Add("Authorization", "msy_krH3TRsitb0qchcoeiPLJBSEV2rVr6rpAFnU");
            try
            {
                var response = client.UploadData("https://api.meshy.ai/openapi/v1/remesh", System.Text.Encoding.ASCII.GetBytes(remeshModel));

                string result = System.Text.Encoding.ASCII.GetString(response);
                RemeshResultModel returnModel = JsonSerializer.Deserialize<RemeshResultModel>(result, JsonSerializerOptions.Default);
                string returns = returnModel.result;
                return returns;
            }
            catch (Exception ex)
            {
                return "";
            }
            finally { }
        }

        public static RemeshModel RetrieveRemeshTask(string remeshID)
        {
            var client = new WebClient();
            client.Headers.Add("Authorization", "msy_krH3TRsitb0qchcoeiPLJBSEV2rVr6rpAFnU");
            var response = client.DownloadData("https://api.meshy.ai/openapi/v1/remesh/" + remeshID);

            string respResult = System.Text.Encoding.ASCII.GetString(response);

            RemeshModel result = JsonSerializer.Deserialize<RemeshModel>(respResult, JsonSerializerOptions.Default);
            return result;
        }
    }
}
