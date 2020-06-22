# CheckBook: [DotVVM](https://github.com/riganti/dotvvm) Sample App

This is a sample application featuring the basic principles of [DotVVM](https://github.com/riganti/dotvvm), an open source MVVM framework for ASP.NET Core and OWIN.

This application demonstrates using **DotVVM 2.3.1** with **ASP.NET Core**, **Entity Framework Core** and **SQLite** database.

You can find an older version of this sample using ASP.NET 4.5 with OWIN in the `dotnetframework` branch.


## Running the sample app

1. Clone the repository using command `git clone https://github.com/riganti/dotvvm-samples-checkbook.git`

2. Open the solution (CheckBook.sln) in Visual Studio

3. Run `CheckBook.App` project


The SQLite database will be created upon first run of the application. You'll need to install .NET Core SDK (2.0 or newer).
The default user account is **smith@test.com** / **Pa$$w0rd**. It is also possible to configure **AAD** authentication using **OpenIdConnect**. For further details about such option, have a look at [this sample repository](https://github.com/riganti/dotvvm-samples-azuread-auth).

<br />

## Features Demonstrated in the Sample

* [Master Pages](https://www.dotvvm.com/docs/tutorials/basics-master-pages/latest) and [Markup Controls](https://www.dotvvm.com/docs/tutorials/control-development-markup-only-controls/latest)

* [Validation](https://www.dotvvm.com/docs/tutorials/basics-validation/latest)

* [Custom Resources](https://www.dotvvm.com/docs/tutorials/basics-javascript-and-css/latest)

* [PostBack Handlers](https://www.dotvvm.com/docs/tutorials/basics-postback-handlers/latest)

* [Authentication](https://www.dotvvm.com/docs/tutorials/advanced-owin-security/latest) and [Authorization](https://www.dotvvm.com/docs/tutorials/advanced-authentication-authorization/latest)

* [Presenters](https://www.dotvvm.com/docs/tutorials/advanced-custom-presenters/latest)

* Bootstrap and Modal Dialogs

<br />

The sample is a demonstration of a simple web app with common features like authentication. There are two projects in the application:

* **CheckBook.DataAccess** - Data Access Layer and Business Layer of the application

    * **Model** folder contains Entity Framework Core model
    
	* **Services** folder is a simple business layer. The services are registered in the `IServiceCollection` in the `Startup.cs` file.

    * **Data** contains objects that are passed from the **DataAccess** project to the **App** project. We don't use Entity Framework Core entities in the viewmodels because it would
cause serialization and other issues. We always transform them to these Data Transfer Objects.

* **CheckBook.App** - a DotVVM web application

    * **Controls** folder contains two markup controls referenced from the pages.
    
    * **ViewModels** folder contains viewmodels of all pages. Most of the viewmodels derive from the base class called **AppViewModelBase**.
    
    * **Views** folder contains DotHTML pages and a master page.
    
    * **Startup.cs** is a main application entry point.
    
    * **DotvvmStartup.cs** contains DotVVM route and resource configuration.
	
## Other resources

* [DotVVM.com](https://www.dotvvm.com)
* [DotVVM Blog](https://www.dotvvm.com/blog)
* [Documentation](https://www.dotvvm.com/docs)
* [Twitter @dotvvm](https://twitter.com/dotvvm)
* [Gitter Chat](https://gitter.im/riganti/dotvvm)
* [Samples](https://github.com/search?q=topic%3Adotvvm-sample+org%3Ariganti&type=Repositories)
* [Roadmap](https://github.com/riganti/dotvvm/blob/master/roadmap.md)
