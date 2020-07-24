# customvision-bingsearch

[Custom Vision](https://azure.microsoft.com/en-us/services/cognitive-services/custom-vision-service/) is a service apart of the Azure Cognitive Services suite that lets users train customized computer vision models to fit their scenario. To create these models, a lot time is usually spent collecting & organizing your data for your model, especially for scenarios where you want to spin up a relevant demo but do not have any existing datasets to work with. To simplify this process and reduce the time to MVP, we can utilize the [Azure Bing Image Search](https://azure.microsoft.com/en-us/services/cognitive-services/bing-image-search-api/) REST APIs to gather images relevant to our search terms, tag and send them to our Custom Vision project - saving a lot of time in what otherwise can be a tedious process. 

The search criteria for your images retured from the Bing Image Search API can also have additional query parameters configured to refine your results and get the images most accurate for the computer vision model being trained. [Click here](https://docs.microsoft.com/en-us/rest/api/cognitiveservices-bingsearch/bing-images-api-v7-reference#query-parameters) to see the filter query parameters in further detail. 

As part of this demo, we will be using this solution to create a classification model that identifies whether a fruit is an apple or an orange. 

## Pre-requisites:
- Azure subscription with a resource group to deploy the solution into 

## Deployment Steps: 

1. Clone this repository locally

2. Go to the Azure landing page, click the side bar on top the left corner and click the "Create a resource" button. 
![createimage](https://taeyhcontent.blob.core.windows.net/images/createresource.PNG?st=2020-07-23T06%3A05%3A34Z&se=2025-07-24T06%3A13%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=JwML6DQ8M84%2BZXOA0B1YMdA3SPNfksE%2FtsdS%2B4pt2j8%3D)

3. Search for "bing search" and click it when it appears on the dropdown

4. Click "Create" and configure your details for the resource. Populate resource name, pricing tier and resource group. For the purpose of doing a simple demo, choose the "Free F1" pricing tier (Note: If conducting multiple searches above the free tier offerings, please upgrade to a paid tier). Should look similar to example below: 
![createbingimage](https://taeyhcontent.blob.core.windows.net/images/createbingresource.PNG?st=2020-07-23T06%3A17%3A16Z&se=2025-07-24T06%3A17%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=T6dtmumK1VIkH6adl3MAa1cSeEOaldXtQ8Bv8SmWITE%3D)

5. Head to the newly created Bing Search resource and click on the tab on the left hand side labelled "Keys and Endpoint". Copy the value of "KEY 1" and save it to your clipboard. This value will be required later to authorize against the Bing Image Search APIs.
![createbingimage](https://taeyhcontent.blob.core.windows.net/images/keybingresource.PNG?st=2020-07-23T06%3A27%3A18Z&se=2025-07-24T06%3A27%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=V89OQ3u5lQLHKFL69UvyFL9WUMo5b%2FLQo5b3XKgQsbo%3D)

6. Repeat step 2, but this time search for a Custom Vision resource type and click create. For the purpose of this demo, set the "Create options" parameter as both. Fill in the other parameters and set the pricing tiers for both the Training resource and Prediction resource at "Free F0" and create the resource.

7. Head over to [Custom Vision Web Interface](https://www.customvision.ai/) and sign in using your credentials for your Azure account that has the above resources deployed in.

8. Once you are logged in, create a new project by clicking the "NEW PROJECT" button. Set the name and description as your choice and set the Resource as the name of the Custom Vision resource created in step 5. Set the project type as "Classification" and Classification types as "Multiclass". For Domains select "Food". After all these values are filled in, create the project.

9. Once the project is created, you should be directed into the newly created project. Once this view is visible, click the settings button at the top right corner. From this page, note down the Project ID and key of your Custom Vision resource which will be required.  
![customvisionsettings](https://taeyhcontent.blob.core.windows.net/images/customvisionprojectsettings.PNG?st=2020-07-23T13%3A47%3A54Z&se=2025-07-24T13%3A47%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=mqIWfLZHr2GeRqmHLrt3QWN71VGTkQqd22WhII5GZUc%3D)

10. Now that all the neccessary Azure resources are deployed, head over the root of the cloned repository and open the .sln file to open the solution in Visual Studio. From here open the file named Program.cs to edit parameters.  

11. At the start of the static void Main() function, we can see a List of type Subject which contains a parameter for a subject name (subjectName) and subject search term (subjectSearchTerm). The subject name refers to the name of the tag on Custom Vision and the subject search term refers to the term that is searched in Bing Images. For this example, the subject Apples is searched with a search term of Granny Smith Apple and the subject Oranges is searched with a search term of Naval Orange. The number of subjects in the list as well as the type of subjects being used to create the model can be altered later to train different models.  
![codesnippet1](https://taeyhcontent.blob.core.windows.net/images/codesubject.PNG?st=2020-07-24T04%3A30%3A16Z&se=2025-07-25T04%3A30%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=cYfQawtxagdQwrBVxxjknh5O66JsJ%2BXQwv3zlEUY4T4%3D)

12. Build and run the solution which will open a console application. When prompted, set the value for Offset as 0 and Count as 75. Rest of the values for Bing Search key, Custom Vision key and Custom Vision Project ID should be values saved earlier to your clipboard.  

13. Once this is done, head over to your custom vision project created earlier and you will see that the project has been populated with tagged images of apples and oranges. From here we now train the computer vision model by clicking the Train button and selecting Quick Training as the type for the purpose of this demo. Click "Train" to confirm and begin model training.
![customvisiontrain](https://taeyhcontent.blob.core.windows.net/images/customvisiontraining.PNG?st=2020-07-24T06%3A25%3A27Z&se=2025-07-25T06%3A25%3A00Z&sp=rl&sv=2018-03-28&sr=b&sig=Y3jADxixh89BiSYgUw9ti0WYdPd%2FgaAnk70r13b2YaI%3D)

14. Once training of the model is complete, we can do a quick test on our newly created model to classify between an orange and apple by using some of the sample images in the "images" folder in the root directory. These images were not used for training the model iteslf. 

