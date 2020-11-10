# SQL Server 2017 Support

Starting from version 2.3, Tabular Editor now also supports SQL Server 2017 (Compatibility Level 1400). This means that the Tabular Editor UI exposes some of the new functionality described [here](https://blogs.msdn.microsoft.com/analysisservices/2017/04/19/whats-new-in-sql-server-2017-ctp-2-0-for-analysis-services/).

Please note, however, that you need to download the [proper build of Tabular Editor](https://github.com/otykier/TabularEditor/releases/tag/2.5-CL1400) to use these features. This is because a new set of client libraries are provided by Microsoft for SQL Server 2017 / SSDT 17.0, and these libs are incompatible with the SQL Server 2016-build of Tabular Editor. The new libraries can be obtained through the new [version of SSDT](https://docs.microsoft.com/en-us/sql/ssdt/download-sql-server-data-tools-ssdt) (requires Visual Studio 2015).

If you don't need Compatibility Level 1400 features, you can still use the SQL Server 2016-build of [Tabular Editor](https://github.com/otykier/TabularEditor/releases/tag/2.5).

Here is a quick rundown of how the new features are used in Tabular Editor:

## Date Relationships
All relationships now expose the "Join on Date Behavior" property in the property grid:

![image](https://cloud.githubusercontent.com/assets/8976200/25297821/9dd46be0-26f0-11e7-92bf-10a921ed20dc.png)

## Variations (column/hierarchy reuse)
You can set up variations on a column, by expanding the "Variations" property in the property grid:

![image](https://cloud.githubusercontent.com/assets/8976200/25297845/c69ecc5a-26f0-11e7-93af-b7a2a0cc9310.png)

Note that you can also specify **Object Level Security** at the column level.

Clicking the ellipsis button opens the Variations Collection Editor, from where you can set up how columns and hierarchies are resurfaced in Power BI:

![image](https://cloud.githubusercontent.com/assets/8976200/25297884/fd4faf58-26f0-11e7-9a1a-df7a1b05f663.png)

Remember to set the "Show As Variations Only" property to "True" at the table level:

![image](https://cloud.githubusercontent.com/assets/8976200/25297917/2c1e4b64-26f1-11e7-8ce6-a62aef2b7d8a.png)

**Detail Row Expressions** can be set directly on tables and measures. At this time, however, no syntax highlighting or IntelliSense is available.

Hierarchy objects exposes a new **Hide Members** property that is useful for ragged hierarchies.