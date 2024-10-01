# Fission .NET Environment

This project is an environment for [Fission](https://fission.io/), a serverless plugin for Kubernetes. It is based on Fission's official .NET environment but introduces support for more recent versions of the .NET framework, as the original version is still limited to .NET Core 2.0. This environment is specifically designed for **.NET 8 on Linux**.

## Main Features

- **Support for newer versions of .NET**: This environment is designed to work with **.NET 8 on Linux**, offering developers an updated and improved experience compared to the official environment.
  
- **Multi-assembly project management**: One of the key features of this environment is the ability to handle and run projects composed of multiple linked assemblies. The assemblies can be compressed into a ZIP file, simplifying the process of distribution and integration.

## Inspiration

This project is inspired by Fission's official environment for .NET Core 2.0 but focuses on improvements and updates requested by the community, allowing developers to work with newer versions of the .NET framework and complex projects that include multiple assemblies.

## Usage

1. **Add the environment to Fission**:
   To add this .NET 8 environment to Fission, use the following command:

   ```bash
   fission env create --name dotnet8 --image <your_custom_image>

2. **Project creation**: 
   - Create a **class library project** in .NET.
   - Add the **Fission NuGet package** to your project.
   - Create a class with the following function:
     
     ```csharp
     public object Execute(FissionContext input)
     {
         return "Hello World";
     }
     ```

3. **Compression**: Compress the assemblies and related files into a ZIP file.

4. **Deploy to Fission**: Use this environment to deploy your project to Fission, leveraging the ability to handle multiple linked assemblies.

## Requirements

- Kubernetes cluster
- Fission installed
- .NET SDK 8 for Linux

## Related NuGet Package

This project uses the **[Fission.DotNet.Common](https://www.nuget.org/packages/Fission.DotNet.Common/)** package. It provides essential functionality for interacting with the Fission environment in .NET, making it easier to work with serverless functions.
