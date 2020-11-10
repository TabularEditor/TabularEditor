This article demonstrates how you can use the Advanced Scripting feature in Tabular Editor, to maintain DAX logic across several objects in a consistent way. In the [Useful Script Snippets article](/Useful-script-snippets), we already saw [how we can use Custom Actions to quickly generate many measures](/Useful-script-snippets#generate-time-intelligence-measures) with similar logic, which can be useful when creating Time Intelligence calculations, for example.

In this article, we're going to expand on this idea by creating a scripting "framework", which will allow us to centrally define all the calculations we need within a TSV file (Tabulator Separated Values). The advantages of using a TSV file is that it can be easily edited within Excel, while at the same time being easy to parse and load from within a script in Tabular Editor.

For this article, we will focus on the Internet Sales fact and related dimensions of good 'ol Adventure Works:

![image](https://user-images.githubusercontent.com/8976200/44193845-85cd5d80-a134-11e8-8f39-2da1380fdc63.png)

The fact table has a number of numeric columns which are simply aggregated up as seven simple `SUM` measures:

![image](https://user-images.githubusercontent.com/8976200/44196409-270be200-a13c-11e8-9994-0a8f2fa19e1a.png)

For the purposes of this article, we'll call these the **base measures**. In real life, the formula of the base measures could be more complex, but that does not matter in general, as we will see in a moment. The central idea, is that we will use our TSV file to define a set of formulas involving the base measures as well as filter contexts that will be applied outside the calculations.

*** TODO ***
 
, as long as the calculations we are trying to build can still be constructed from one or more base measures, evaluated within any valid filter context.

*** TODO ***