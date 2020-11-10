Are you a Tabular Editor pro?

Test your knowledge of Tabular Editor's Advanced Scripting and Dynamic LINQ filter expressions. All the questions here may be answered using just one line of code.

If you're new to these features, the solutions presented here (both the C# and Dynamic LINQ version), provides a lot of useful information on how this stuff works, so make sure to check them out.

***

#### Question #1) Total number of measures
* How would you obtain the total number of measures in your model?

<details><summary><i>C# script solution</i></summary>
<pre><code>Model.AllMeasures.Count().Output();</code></pre>
<b>Explanation:</b> The <code>Model</code> object represents the root of the <a href="https://docs.microsoft.com/en-us/sql/analysis-services/tabular-model-programming-compatibility-level-1200/introduction-to-the-tabular-object-model-tom-in-analysis-services-amo?view=sql-server-2017#tabular-object-model-hierarchy">TOM tree</a>. It supports most of the properties found in the <a href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.analysisservices?redirectedfrom=MSDN&view=sqlserver-2016">API documentation</a> with the addition of a number of extra properties and methods, that are only available inside Tabular Editor. The <code>AllMeasures</code> property is one of these extra properties, added for convenience. It simply returns a collection of all measures across all tables in the model. All collections (or more precisely, <i>enumerables</i>) support the powerful <a href="https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable?view=netframework-4.7.2#methods">.NET LINQ methods</a>. <code>Count()</code> is one such method, which simply returns the number of elements in the collection as an integer. Once we have that, the only thing left is to <code>Output()</code> it.<br/><br/>
</details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:ObjectType="Measure"</code></pre>
<b>Explanation:</b> When you put a ':' as the first character of the Filter textbox, you enable Dynamic LINQ filtering. What that means is, that Tabular Editor evaluates the expression after the ':' character against every object in the TOM tree, returning only those objects where the expression evaluates to true. Putting the expression above into the Filter textbox, will have Tabular Editor display all objects whose <code>ObjectType</code> property is "Measure". The search result count at the bottom of the screen, should then tell you how many measures you have in total.<br/><br/>
</details>

***

#### Question #2) Find all measures with "TODO" in their expression

* What's the easiest way to find all measures that contain the word "TODO" inside their Expression property?

<details><summary><i>C# script solution</i></summary>
<pre><code>Model.AllMeasures.Where(m => m.Expression.Contains("TODO")).Output();</code></pre>
<b>Explanation:</b> The first part of this script is the same as in question 1. <code>Where(x =&gt; y)</code> is another .NET LINQ method, that filters the preceding collection based on a so-called <i>predicate</i>. The predicate is expressed using the special C# Lambda notation <code>x =&gt; y</code>. On the left side of the arrow, you declare a variable with a name of your choice. The expression to the right of the arrow will be evaluated for every object in the collection, using the variable on the left to represent individual objects. This expression can be any valid C# expression that evaluates to a boolean value (true or false). Thus, the <code>Where</code> method simply filters the collection to return only those objects where the Lambda expression evaluates to true. So in the example above, we decide to use <code>m</code> as the name of our variable, which will represent the individual measures of our model. But we only want to keep measures whose <code>Expression</code> property <code>Contains</code> the word "TODO". Makes sense?<br/><br/>
</details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:ObjectType="Measure" and Expression.Contains("TODO")</code></pre>
<b>Explanation:</b> The first part of this Dynamic LINQ expression is identical to question 1. Dynamic LINQ lets you use <a href="https://github.com/kahanu/System.Linq.Dynamic/wiki/Dynamic-Expressions#operators">many different operators</a> such as <code>and</code> or <code>or</code> to express complex logic. Notice how the second part of the expression is similar to the C# Lambda expression used above, except that we don't declare a variable to represent the measure. Since Dynamic LINQ is evaluated against every object in the TOM tree, any property or method name we add to the expression will implicitly be evaluated against the current object. Since different types of objects have different properties, no error is produced if the Filter box contains an invalid expression. However, when writing Dynamic LINQ expressions within the <a href="/Best-Practice-Analyzer">Best Practice Analyzer</a>, an error will be shown if you try to access a property or method that doesn't exist on the chosen object types.
</details>

***

#### Question #3) Count the number of direct measure dependencies
* How can we know the number of measures that directly reference the currently selected measure? You can always check your answer against the "Show dependencies" dialog.

<details><summary><i>C# script solution</i></summary>
<pre><code>Selected.Measure.ReferencedBy.Measures.Count().Output();</code></pre>
<b>Explanation:</b> <code>Selected.Measure</code> refers to the currently selected measure in the explorer tree. All objects that can be referenced through DAX (measures, tables, columns, KPIs) have the <code>ReferencedBy</code> property, which is a special collection of objects that directly reference the former. Although we could use the LINQ-method <code>.OfType&lt;Measure&gt;()</code> to filter the collection to measures only, this particular collection contains a set of convenient properties that does this for us. One of them, is <code>Measures</code>.<br/><br/>
</details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:ObjectType="Measure" and DependsOn.Measures.Any(Name="Reseller Total Sales")</code></pre>
<b>Explanation:</b> It's not possible to create a Dynamic LINQ filter expression based on the current selection, so instead we consider a specific measure in this example, [Reseller Total Sales]. The example here, will return all those objects who have a direct dependency on a measure named "Reseller Total Sales". The reason we're using "DependsOn" instead of "ReferencedBy" here, is that search filter expressions are evaluated against every single object in the model. That's the opposite of what we're doing in the C# script, where we already have a handle to a specific measure and want to obtain the list of measures referencing that measure.
</details>

***

#### Question #4) Recursively count the number of measure dependencies
* Let's go deeper. How would you obtain the number of measures that depend recursively on the currently selected measure?

<details><summary><i>C# script solution</i></summary>
<pre><code>Selected.Measure.ReferencedBy.Deep().OfType&lt;Measure&gt;().Count().Output();</code></pre>
Here, we add the <code>Deep()</code> method to recursively traverse the dependency tree, to get a collection of all objects that reference the original measure either directly, or indirectly through other objects. We have to manually filter this collection to objects of type "Measure", to avoid seeing Calculated Columns, RLS Expressions, etc. The only thing left then, is to <code>Count()</code> the number of items in this result and <code>Output()</code> it to the screen.<br/><br/>By the way, if we wanted to display a list of these measures instead of just the count, we could write:
<pre><code>Selected.Measure.ReferencedBy.Deep().OfType<Measure>().Output();</code></pre>
</details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:ObjectType="Measure" and DependsOn.Deep().Any(Name="Reseller Total Sales")</code></pre>
<b>Explanation:</b> All methods that can be called using C# may also be called using Dynamic LINQ. So just like we did above, we're calling the <code>Deep()</code> method to recursively traverse the dependency tree upwards, to find all objects that have a dependency on an object named "Reseller Total Sales". Strictly speaking, this is not exactly the same as the C# expression above, as we would also get a positive hit on non-measure type objects with the name "Reseller Total Sales". To work around that, we could either explicitly state that we only want to consider measures...
<pre><code>:DependsOn.Deep().Any(Name="Reseller Total Sales" and ObjectType="Measure")</code></pre>
...or we could use the <code>DaxObjectFullName</code> property to check for a hit (column names would be fully qualified, and measures must be uniquely named across the entire model):
<pre><code>:DependsOn.Deep().Any(DaxObjectFullName="[Reseller Total Sales]")</code></pre>
</details>

***

#### Question #5) List all related dimensions
* Given a fact table `'Reseller Sales'`, how do we obtain a list of all related dimension tables?

<details><summary><i>C# script solution</i></summary>
<pre><code>var t = Model.Tables["Reseller Sales"];<br/>
t.UsedInRelationships.Where(r => r.FromTable == t).Select(r => r.ToTable).Output();</code></pre>
<b>Explanation:</b> Okay, I admit, this one is a little tricky and because I used a variable to hold the given table, we end up with 2 lines of code instead of one. The naïve approach would be to simply write <code>t.RelatedTables.Output();</code>, but since the question specifically asked us to output only related <i>dimension</i> tables, we need to consider only those relationships where our given table is on the "From" side. That is the purpose of <code>t.UsedInRelationships.Where(r => r.FromTable == t)</code>. If we just wanted the list of outgoing relationships, we'd be done here, but since we want a list of the <i>tables</i> pointed to by those relationships, we need to project this list to get the `ToTable` property of each relationship. That's exactly what <code>.Select(r => r.ToTable)</code> does. Makes sense? Now check out the Dynamic LINQ solution below.<br/><br/></details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:UsedInRelationships.Any(ToTable=current and FromTable.Name = "Reseller Sales")</code></pre>
<b>Explanation:</b> Let's read this expression from left to right, keeping in mind that this is evaluated for every object in the model. <code>UsedInRelationships</code> is a list of relationships in which the current object participates. At this point, we've ruled out anything that's not a table or a column object, as these are the only ones that have the <code>UsedInRelationships</code> property. To filter anything that's not a dimension table, we only want to consider relationships pointing <i>to</i> the current object, <i>from</i> the table in question. <code>.Any( ... )</code> evaluates to true if at least one of the relationships satisfies the condition: <code>ToTable=current and FromTable.Name = "Reseller Total Sales"</code>. The special keyword <code>current</code> refers to the current object being evaluated. As we're equating this with the <code>ToTable</code> property of the relationship, we're ruling out columns from the search result, as this property can only be of type Table. <code>FromTable.Name = ...</code> is self-explanatory.
</details>

***

#### Question #6) Find all objects with the words "Total" and "Amount" (in that order) in their name

![image](https://user-images.githubusercontent.com/8976200/44931220-c2dd4680-ad15-11e8-9e52-29ec07f1edb6.png)

Hint: The regular expression for that would be `Total.*Amount`

<details><summary><i>C# script solution</i></summary>
<pre><code>Model.AllMeasures.Where(m => System.Text.RegularExpressions.Regex.IsMatch(m.Name, "Total.*Amount")).Output();</code></pre>
<b>Explanation:</b> This one is actually quite annoying to do in the Advanced Script tab. Strictly speaking, we would actually have to search all the collections (Tables, AllMeasures, AllColumns, AllHierarchies, ...) and then concatenate the result, if we wanted to see them all in one view. Additionally, since the <code>System.Text.RegularExpressions</code> namespace is not in scope by default, the script is not really that typing-friendly. Check out the Dynamic LINQ solution instead.<br/><br/></details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:Regex.IsMatch(Name, "Total.*Amount")</code></pre>
Beautiful, isn't it?
</details>

***

#### Question #7) Same as #6 but with a case-*in*sensitive search

<details><summary><i>C# script solution</i></summary>
<pre><code>Model.AllMeasures.Where(m => System.Text.RegularExpressions.Regex.IsMatch(m.Name, "Total.*Amount", RegexOptions.IgnoreCase)).Output();</code></pre></details>

<details><summary><i>Dynamic LINQ solution</i></summary>
<pre><code>:Regex.IsMatch(Name, "Total.*Amount", "IgnoreCase")</code></pre></details>

#### Stay tuned for more...