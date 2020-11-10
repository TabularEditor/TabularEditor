# Frequently Asked Questions

## What is Tabular Editor?
Essentially, Tabular Editor provides a UI for editing the metadata making up an Analysis Services Tabular Model. The main difference between using Tabular Editor for editing a model versus using Visual Studio, is that Tabular Editor does not load any _data_ - only _metadata_. This means that no validations or calculations are performed when you create and modify measures, display folders, etc. Validations and calculations are performed only when the user chooses to persist the changes to the database. This provides a better developer experience for medium to large sized models, which tend to be slow to work with in Visual Studio.

Additionally, Tabular Editor has a lot of [features](/Features-at-a-glance) that will generally boost your productivity and make certain tasks easier.

## Why do we need yet another tool for SSAS Tabular?
Working with Analysis Services Tabular, you may already be familiar with SQL Server Data Tools (Visual Studio), [DAX Editor](https://www.sqlbi.com/tools/dax-editor/), [DAX Studio](https://www.sqlbi.com/tools/dax-studio/), [BISM Normalizer](http://bism-normalizer.com/) and [BIDSHelper](https://bidshelper.codeplex.com/). These are all excellent tools, each with their own purposes. Tabular Editor is not intended to replace any of these tools, but should rather be seen as a supplement to them. Please view the [Features at a glance](/Features-at-a-glance) article, to see why Tabular Editor is justified.

## Why isn't Tabular Editor available as a plug-in for Visual Studio?
While a better user experience for working with Tabular Models inside Visual Studio would definitely be appreciated, a stand-alone tool provides some benefits over a plug-in: First of all, you **don't need a Visual Studio/SSDT installation to use Tabular Editor**. Tabular Editor only requires the AMO libraries, which is quite a small installation compared to VS. Secondly, TabularEditor.exe can be executed with command-line options for deployment, scripting, etc., which would not be possible in a .vsix (plug-in) project.

Also worth mentioning: Tabular Editor can be downloaded as a [standalone .zip file](https://github.com/otykier/TabularEditor/releases/latest/download/TabularEditor.Portable.zip), meaning you do not need to install anything. In other words, you can run Tabular Editor without having admin rights on your Windows machine. Simply download the zip file, extract it, and run TabularEditor.exe.

## What features are planned for upcoming releases?
You can view the current roadmap [here](/Roadmap).
