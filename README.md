# Azure-func-blob-storage
### Author(s): Dave Voyles | [@DaveVoyles](http://www.twitter.com/DaveVoyles)
### URL: [www.DaveVoyles.com][1]

Access Azure Blob Storage from an Azure Function
----------
### About
Manipulate blob storage from an Azure Function. Simply host this project in Azure App Service.


## Instructions
In AzureBlobManager.cs, set your:
* key
* account name
* connection string

### Passing an image to the function

This is done by making an HTTP POST request. Inside the body of the message, we want to pass in JSON content, with a key/value pair like this:

{
    "name" : "AddressOfTheImage"
}


  [1]: http://www.daveVoyles.com "My website"

