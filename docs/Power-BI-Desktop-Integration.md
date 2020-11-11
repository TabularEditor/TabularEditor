# Power BI Desktop Integration

As of July 2020, [Power BI Desktop adds support for External Tools](https://docs.microsoft.com/da-dk/power-bi/create-reports/desktop-external-tools). This allows Tabular Editor to perform certain modelling operations when working with Imported or DirectQuery data in Desktop.

![image](https://user-images.githubusercontent.com/8976200/87296924-dcea3180-c507-11ea-9cf9-2f647d26a2a9.png)

## Prerequisites

- [July 2020 version of Power BI Desktop](https://www.microsoft.com/en-us/download/details.aspx?id=58494) (or newer)
- [Latest version of Tabular Editor](https://github.com/otykier/TabularEditor/releases/latest)
- Enable [Enhanced Metadata](https://docs.microsoft.com/en-us/power-bi/connect-data/desktop-enhanced-dataset-metadata) under Power BI Desktop's Preview Features

Also, it is highly recommended that [automatic date/time](https://docs.microsoft.com/en-us/power-bi/transform-model/desktop-auto-date-time) is **disabled** (Power BI Desktop setting under "Data Load").

## Supported Modelling Operations

By default, Tabular Editor will only let you edit a limited number of objects and properties when connected to a Power BI Desktop model. These are:

- Measures (add/remove/edit any property)
- Calculation Groups and Calculation Items (add/remove/edit any property)
- Perspectives (add/remove/edit any property)
- Translations (add/remove)
  - You can apply metadata translations to any object in the model, although be aware that Power BI Desktop does not yet support translations to the default model culture.

**Note:** If you enable the "Allow unsupported Power BI features (experimental)" option under Tabular Editor's File > Preferences dialog, Tabular Editor will let you edit **any** object and property, potentially causing model changes that are not supported by Power BI Desktop, which may cause a crash or a corrupt .pbix file. In this case, Microsoft Support will not be able to help you, so use at your own risk, and keep a backup of your .pbix file just in case.