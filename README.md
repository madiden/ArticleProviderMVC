# ArticleProvider
This repository contains the implementation of the assesment project and this readme includes related information with the project.

## Technologies Utilized
 - ASP.NET MVC for backend
 - Entity Framework (code first) with MS SQL Server Express for data persistence.
     - EF 6 version has been used since it supports mocking. Thus, abstraction for data access was not necessary.
 - Bootstrap for frontend.
 - Javascript and JQuery with ajax functions
 - Microsoft Testing and Moq for mocking
 - Unity for Dependency Injection
 
 ## Some Key points within the project.
 - The project is prepared with MS Visual Studio 2015 Community Edition.
 - The project can be cloned with Visual Studio Git Provider and can be directly run. The repository also includes Database files and the Web.config file includes a connection string which attaches the db files in the location to local SQL Server Express instance. The repository was cloned from different locations and was runned successfully. 
 - When the project is first compiled, it restores required NuGet packages automatically.
 - For user identity management, built in Asp.Net identity system is used. It contains required tables within its own context. 
 - Within the user data, a boolean __IsEditor__ field is added for discriminating authors and employees. When a user attempts for registration, a check box on the registration form is provided for determining a user as an editor.
 - The password policy is not strict. It accepts passwords greater or equal to 6 characters.
 - The number of likes for a user in a day is stored in Web.Config file. An abstraction interface which includes methods for supplying web  config values is prepared. Real implementation returns web.config file and the stub the in test project simply returns 5. (Last committed web.config file also has the value of 5 for this setting. The site informs with a modal dialog if the user has exceeded the number of likes for the day) A unit test was also prepared for this feature. Controller was modified so that it accepts this interface. Default implementation of the interface is registered to Unity's IOC (inversion of control) container.
 - A user can only edit and delete his/her own article. But all users can view all of the articles.
 - Any user can make unlimited number of comments to any article. Empty comment texts are not allowed and the user is informed with a modal dialog about this.
 - Like feature is implemented with JQuery asynchronous request and the Comment button is implemented with using form submit.
 - Each of the business rules are both implemented in the backend and illustrated at the frontend. For example Like button is disabled for a user if it was liked before, if form submission is simulated backend action does not allow re-liking same article as well.
 
 _Delivery time of the project is not the exact time the project was finished. Commit times may be observed within this repository to evaluate commits and their respective times._ 
 

