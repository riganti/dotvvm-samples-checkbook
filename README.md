![Screenshot](https://github.com/riganti/dotvvm-samples-checkbook/blob/master/images/CheckBookSample001.png?raw=true)


# CheckBook: [DotVVM](https://github.com/riganti/dotvvm) Sample App

This is a sample application featuring the basic principles of [DotVVM](https://github.com/riganti/dotvvm), an open source MVVM framework for ASP.NET Core and OWIN.

This application demonstrates using **DotVVM 4.1.0** with **ASP.NET Core**, **Entity Framework Core** and **SQLite** database.

You can find an older version of this sample using ASP.NET 4.5 with OWIN in the `dotnetframework` branch.


### Prerequisites
* Make sure you have installed [DotVVM for Visual Studio](https://www.dotvvm.com/install)


### How to run the sample

1. [Open the GitHub repo in Visual Studio](git-client://clone/?repo=https%3A%2F%2Fgithub.com%2Friganti%2Fdotvvm-samples-checkbook)
or 
`git clone https://github.com/riganti/dotvvm-samples-checkbook.git`

2. Open `src/CheckBook.sln` 
![Open the solution file](https://github.com/riganti/dotvvm-samples-checkbook/blob/master/images/CheckBookSample002.png?raw=true)

3. Right-click the `Checkbook.App` project and select **View > View in Browser**
![View CheckBook in Browser](https://github.com/riganti/dotvvm-samples-checkbook/blob/master/images/CheckBookSample003.png?raw=true)

4. The default user account is **smith@test.com** / **Pa$$w0rd**.

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
