using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace BingSearchApisQuickstart
{

    class Program
    {
        // Add your Azure Bing Search V7 endpoint to your environment variables
        const string bingUriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";
        const string customVisionUriBase = "https://australiaeast.api.cognitive.microsoft.com/customvision/v3.0/training/projects/";

        class Subject
        {
            public string subjectName { get; set; }
            public string subjectTagId { get; set; }
        }
        // Number of images to select
        const int count = 5;

        // Offset number
        const int offset = 10;

        // A struct to return image search results seperately from headers
        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }

        static void Main()
        {

            List<Subject> subjectList = new List<Subject>
            {
                new Subject{ subjectName = "Apples"},
                new Subject{ subjectName = "Pears"}
            };

            //Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Enter Bing Subscription Key: ");
            string bingSubscriptionKey = Console.ReadLine();
            Console.WriteLine("Enter Custom Vision Key: ");
            string customVisionKey = Console.ReadLine();
            Console.WriteLine("Enter Custom Vision Project Id: ");
            string customVisionProjectId = Console.ReadLine();

            foreach (var subject in subjectList)
            {
                bool tagFlag = false;
                Console.WriteLine($"Searching images for: {subject.subjectName}"+ "\n");
                SearchResult result = BingImageSearch(bingSubscriptionKey, subject.subjectName);

                //deserialize the JSON response from the Bing Image Search API
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);
                Console.WriteLine(jsonObj);

                // Checking if tag already exists
                var httpWebRequestGetTags = (HttpWebRequest)WebRequest.Create(customVisionUriBase + customVisionProjectId + "/tags");
                httpWebRequestGetTags.Headers["Training-Key"] = customVisionKey;
                httpWebRequestGetTags.ContentLength = 0;
                httpWebRequestGetTags.Method = "GET";
                var httpResponseGetTags = (HttpWebResponse)httpWebRequestGetTags.GetResponse();
                using (var streamReader = new StreamReader(httpResponseGetTags.GetResponseStream()))
                {
                    var streamResult = streamReader.ReadToEnd();
                    dynamic tagJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(streamResult);

                    // If tag exists, getting tag ID for sending images to correct tag in Custom Vision 
                    if (tagJsonObj.Count != 0)                    
                    {
                        foreach (var tag in tagJsonObj)
                        {
                            if (subject.subjectName == (string)tag["name"])
                            {
                                tagFlag = true;
                                subject.subjectTagId = (string)tag["id"];
                                Console.WriteLine($"Tag already exists, setting tag flag for tag: {subject.subjectName} with tag ID: {subject.subjectTagId}");
                            }
                        }
                    }

                // If tag does not exist, create the tag
                }
                if (tagFlag == false)
                {
                    string weburi = customVisionUriBase + customVisionProjectId + "/tags/?name=" + subject.subjectName;
                    var httpWebRequestTagCreation = (HttpWebRequest)WebRequest.Create(customVisionUriBase + customVisionProjectId + "/tags/?name=" + subject.subjectName);
                    httpWebRequestTagCreation.Headers["Training-Key"] = customVisionKey;
                    httpWebRequestTagCreation.ContentLength = 0;
                    httpWebRequestTagCreation.Method = "POST";

                    var httpResponseTagCreation = (HttpWebResponse)httpWebRequestTagCreation.GetResponse();
                    using (var streamReader = new StreamReader(httpResponseTagCreation.GetResponseStream()))
                    {
                        var streamResult = streamReader.ReadToEnd();
                        dynamic tagJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(streamResult);
                        Console.WriteLine("Tag Creation Response: " + tagJsonObj);
                        Console.WriteLine("Tag Creation ID is: " + tagJsonObj["id"]);
                        subject.subjectTagId = tagJsonObj["id"];
                    }
                }

                // Iterating through each of the images returned in search result and sending to Custom Vision Project with relevant tag.
                foreach (var item in jsonObj["value"])
                {
                    Console.WriteLine($"Title for the first image result: { item["name"]}" + "\n");
                    //After running the application, copy the output URL into a browser to see the image. 
                    Console.WriteLine($"URL for the first image result: {item["webSearchUrl"]}" + "\n");

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(customVisionUriBase + customVisionProjectId + "/images/urls");
                    httpWebRequest.Headers["Training-Key"] = customVisionKey;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{" + "\"images\": [{" + "\"url\":\"" + item["contentUrl"] + "\"}]," + "\"tagIds\":[\"" + subject.subjectTagId + "\"]}";
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var streamResult = streamReader.ReadToEnd();
                    }
                }
            }
            Console.Write("\nPress Enter to exit ");
            Console.ReadLine();

        }

        // Function for performing a Bing Image search and returning the results as a SearchResult object.
        static SearchResult BingImageSearch(string bingSubscriptionKey, string searchQuery)
        {
            // Construct the URI of the search request
            var uriQuery = bingUriBase + "?q=" + Uri.EscapeDataString(searchQuery) + "&offset=" + offset + "&count=" + count;

            // Perform the Web request and get the response
            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = bingSubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Create result object for return
            var searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };

            // Extract Bing HTTP headers
            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                    searchResult.relevantHeaders[header] = response.Headers[header];
            }
            return searchResult;
        }

    }
}