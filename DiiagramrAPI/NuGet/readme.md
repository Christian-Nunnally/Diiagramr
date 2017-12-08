# Diiagramr Library SDK

## Contents
- Create-Node Powershell Command
- Publish-Library Powershell Command
- DiiagramrAPI.dll

## Instructions

#### Creating a node
1. Go to Tools -> NuGet Package Manager -> Package Manager Console
2. In the console, enter command "cd <Project Name>" to enter project directory
3. In the console, enter command "Create-Node"
4. Type the desired name of your node (must not include spaces, and should be CamelCase)
5. Right click your project and click Add -> Existing Item
6. Add <Your Node Name>View.xaml and <Your Node Name>ViewModel.cs to your project

#### Publishing your library
1. Go to Tools -> NuGet Package Manager -> Package Manager Console
2. In the console, enter command "cd <Project Name>" to enter project directory
3. In the console, enter command "Publish-Library"
4. Enter prompted information about your package and press enter

## Wiki
For more information on howto program your own nodes, refer to the following wiki:

https://christiannunnally.visualstudio.com/Diiagramr/Diiagramr%20Team/_wiki
