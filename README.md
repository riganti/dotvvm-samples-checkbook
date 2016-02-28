# CheckBook: [DotVVM](https://github.com/riganti/dotvvm) Sample App

## Before you start...

This sample waits until the DotVVM 1.0 is released. Until then, it references pre-release version of DotVVM which is not on the official Nuget feed.
You can create it on your own package from the **future** branch in the [DotVVM](https://github.com/riganti/dotvvm) repository.



## Running The Sample

If you are using _SQL Server Express_, just open the project in Visual Studio and run. The database will be created automatically.

If you use another database, you have to change the connection string in the **web.config** file first. You don't have to create an empty database, it will be created automatically.

The default user account is **smith@test.com** / **Pa$$w0rd**.

<br />

## Features Demonstrated in the Sample

* [Master Pages](https://www.dotvvm.com/docs/tutorials/basics-master-pages) and [Markup Controls](https://www.dotvvm.com/docs/tutorials/control-development-markup-controls)

* [Validation](https://www.dotvvm.com/docs/tutorials/basics-validation)

* [Custom Resources](https://www.dotvvm.com/docs/tutorials/basics-javascript-and-css)

* [PostBack Handlers](https://www.dotvvm.com/docs/tutorials/basics-postback-handlers)

* [Authentication](https://www.dotvvm.com/docs/tutorials/advanced-owin-security) and [Authorization](https://www.dotvvm.com/docs/tutorials/advanced-authentication-authorization)

* [Presenters](https://www.dotvvm.com/docs/tutorials/advanced-presenters)

* Bootstrap and Modal Dialogs

<br />

The sample is a demonstration of a simple web app with common features like authentication. There are two projects in the application:

* **CheckBook.DataAccess** - Data Access Layer and Business Layer of the application

    * **Model** folder contains Entity Framework model
    
	* **Services** folder is a simple business layer. To make things simple and understandable for beginners, we don't use dependency injection and stuff like AutoMapper. 
All business layer methods are static.

    * **Data** contains objects that are passed from the **DataAccess** project to the **App** project. We don't use Entity Framework entities in the viewmodels because it would
cause serialization and other issues.

* **CheckBook.App** - a DotVVM web application

    * **Controls** folder contains two markup controls referenced from the pages.
    
    * **ViewModels** folder contains viewmodels of all pages. Most of the viewmodels derive from the base class called **AppViewModelBase**.
    
    * **Views** folder contains DOTHTML pages and a master page.
    
    * **Startup.cs** is a main application entry point.
    
    * **DotvvmStartup.cs** contains DotVVM route and resource configuration.
