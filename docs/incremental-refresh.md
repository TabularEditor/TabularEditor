# Incremental Refresh

Datasets hosted in the Power BI service can have [Incremental Refresh](https://docs.microsoft.com/en-us/power-bi/connect-data/incremental-refresh-overview) set up on one or more tables. To configure or modify Incremental Refresh on a Power BI dataset, you can either use the [XMLA endpoint of the Power BI service directly](https://docs.microsoft.com/en-us/power-bi/connect-data/incremental-refresh-xmla), or you can use Tabular Editor connected to the XMLA endpoint, as described below:

## Setting up Incremental Refresh from scratch with Tabular Editor

<ol>
<li>Connect to the Power BI XMLA R/W endpoint of your workspace, and open the dataset on which you want to configure Incremental Refresh.</li>
  <li>Incremental refresh requires the <code>RangeStart</code> and <code>RangeEnd</code> parameters to be created (<a href="https://docs.microsoft.com/en-us/power-bi/connect-data/incremental-refresh-configure#create-parameters">more information</a>), so let's start by adding two new Shared Expressions in Tabular Editor:

![](https://user-images.githubusercontent.com/8976200/121341006-8906e900-c920-11eb-97af-ee683ff40609.png)</li>
<li>Name them <code>RangeStart</code> and <code>RangeEnd</code> respectively, set their <code>Kind</code> property to "M" and set their expression to the following (the actual date/time value you specify doesn't matter, as it will be set by the PBI service when starting the data refresh):

```M
#datetime(2021, 6, 9, 0, 0, 0) meta [IsParameterQuery=true, Type="DateTime", IsParameterQueryRequired=true]
```
![](https://user-images.githubusercontent.com/8976200/121342389-dc2d6b80-c921-11eb-8848-b67950e55e36.png)</li>
<li>Next, select the table on which you want to enable incremental refresh</li>
<li>Set the <code>EnableRefreshPolicy</code> property on the table to "true":
  
![](https://user-images.githubusercontent.com/8976200/121339872-3842c080-c91f-11eb-8e63-a051b34fb36f.png)</li>
<li>Configure the remaining properties according to the incremental refresh policy you need. Remember to specify an M expression for the <code>SourceExpression</code> property (this is the expression that will be added to partititions created by the incremental refresh policy, which should use the <code>RangeStart</code> and <code>RangeEnd</code> parameters to filter the data in the source).
  
![](https://user-images.githubusercontent.com/8976200/121342866-5f4ec180-c922-11eb-8a7a-cef44d3a407b.png)</li>
<li>Save your model (Ctrl+S).</li>
<li>Right-click on the table and choose "Apply Refresh Policy"
  
![](https://user-images.githubusercontent.com/8976200/121342947-78577280-c922-11eb-82b5-a517fbe86c3e.png)</li>
</ol>


That's it! At this point, you should see that the Power BI service has automatically generated the partitions on your table, based on the policy you specified.

![](https://user-images.githubusercontent.com/8976200/121343417-eef47000-c922-11eb-8731-1ac4dde916ef.png)

The next step is to refresh the data in the partitions. You can use the Power BI service for that, or you can refresh the partitions in batches using [XMLA/TMSL through SQL Server Management Studio](https://docs.microsoft.com/en-us/power-bi/connect-data/incremental-refresh-xmla#refresh-management-with-sql-server-management-studio-ssms), or even using [Tabular Editor's scripting](https://www.elegantbi.com/post/datarefreshintabulareditor).

## Modifying existing refresh policies

You can also use Tabular Editor to modify existing refresh policies that has been set up using Power BI Desktop. Simply follow step 6-8 above in this case.

## Applying refresh policies with `EffectiveDate`

If you want to generate partitions while overriding the current date (for purposes of generating different rolling window ranges), you can use a small script in Tabular Editor to apply the refresh policy with the [EffectiveDate](https://docs.microsoft.com/en-us/analysis-services/tmsl/refresh-command-tmsl?view=asallproducts-allversions#optional-parameters) parameter.

With the incremental refresh table selected, run the following script in Tabular Editor's "Advanced Scripting" pane, in place of step 8 above:

```csharp
var effectiveDate = new DateTime(2020, 1, 1);  // Todo: replace with your effective date
Selected.Table.ApplyRefreshPolicy(effectiveDate);
```

![](https://user-images.githubusercontent.com/8976200/121344362-f9633980-c923-11eb-916c-44a35cf03a36.png)
