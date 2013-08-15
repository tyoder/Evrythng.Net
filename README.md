Evrythng.Net
============

A wrapper for the Evrythng API written in C#

This wrapper was created using .NET version 4.5, which you must install or upgrade to in order to compile and run the code.  Much of the Json serialization and deserialization is done using the latest version of Json.Net which is included in the packages folder.

To run the unit tests and console application included in this Visual Studio solution, you must obtain an API Key from Evrythng.

Evrythng website: http://www.evrythng.com/

How to get started with an API key, etc:  https://dev.evrythng.com/documentation/start

The console application and the test project both contain app.config files.  When you have your own API key, place it in the appropriate node of the config files.  I would suggest running the console application first and then looking at the unit tests to get an understanding for how the Evrythng API works.  I am using a Service-Repository pattern in which the Repository classes are responsible for interacting with the API and the Service classes are responsible for checking nulls, empty strings, etc and enforcing any other rules which become necessary.  Essentially, the Repository layer relies on the Service layer to keep out bad requests.  Also, Service methods may combine Repository methods, if necessary, to return custom entities or collections.

This project currently includes the ability to operate on Thngs and Products.  Not yet included are Search and Collections, which I will work on next.

Please feel free to contact me if you would like to work on this project with me.
