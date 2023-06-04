## Principles (of this design)

* Anything we can do, they can do better - well at least they should be able to do
  * An alternate help system is needed to be sure ours can be extended. It could not.
* Context is for passing between layers and should have no internal content
* Data is distinct from layout is distinct from formatting is distinct from output location
    * This realization led to the creation of GetData() for JSON output.
   

After a bunch of struggle about where to put it, I decided that all customization of compound concepts (like GetUsage or second column description + default) should reside in the section. This makes reuse a little trickier but leaves all layout customization in a single location and avoids some layering problems. Honestly, I think I am stressing too much over this; if you change usage you are probably changing other things and will be customizing layout all up. This location allows Data to be available.

The intention is to cache the data collection. It is currently done multiple time (Synopsis, Usage for Command) and the individual items are created an additional time for their own section.

## Responsibilities

Configuration is for permanent things and factories for current run things
Context is for current run

Cli versions are generic, Help versions are specific


### CliOutputConfiguration
Sections prop: List<CliSection> --> This simplifies the common case, and a customer renderer can modify based on context
GetFormatter(OutputContext) --> default to console

### CliHelpConfiguration : CliOutputConfiguration

GetSymbolInspector(HelpContext)
*Defaults --> consider removing

*Try to collapse DefaultHelpConfiguration and HelpConfiguration (in help dll)*

### CliOutputContext

OutputConfiguration (for sections and formatter)
MaxWidth
Writer
~~Formatter~~ - Custom formatters must be done via the configuration override or in the OutputRenderer

### HelpContext : CliOutputContext

ParseResult
CliConfiguration
Command -> It is not clear when this would be used other than testing - where it is rather difficult to
           create a parseResult without excess scope sharing. And it is also a convenience.

### CliOutputRender (HelpBuilder)

Write(OutputContext)
protected virtual
* GetSections(outputContext) --> defaults to the one in outputContext.OutputConfiguration
* GetFormatter(outputContext) --> defaults to the method in outputContext.OutputConfiguration

### Cli[...]Section

GetData(outputContext) - For JSON, etc and to drive GetBody(), etc.
GetOpening(outputContext)
GetClosing(outputContext)
GetBody(outputContext)

### SymbolInspector

Abstract System.CommandLine primitives

### Issue

Where does GetUsage(), GetDefaultValue() etc go? They likely may need to be customized. Customization will also allow deprecation of aliases, which has been requested. 

* Section would combine them with layout responsibilities, but would make reuse hard
* SymbolInspector might make reuse easy, ut it has nothing to do with abstracting primitives
* Overridable methods on HelpConfiguration might be intuitive and allows reuse, but doesn't work due to layering
* Overridable methods on HelpDefaultConfiguration is less intuitive and allows reuse, but just feels odd

*Current plan: Have these virtual in each section and have utilities. If someone wants to change, they will need to override in each section and use their own utilities. Feels a bit snarky.













Inspector retrieves data from Symbols

* This could be located in the Parsing assembly because it may be useful in 
* May be a static class
 
The structure that is built is:

* Sections which contain blocks
  * Blocks are either tables or text
  * Text
    * Lambda expression that returns the text to display
    * Parameters to lambda are an inspector and a "formatter" (easily misunderstood term)
      * Formatter supports various output - HTML, markdown, etc.following a base class format
  * Table
    * Headers - which may be null - lambdas to allow formatting
    * Lambdas for column entries - this is the same as for text

There is no opening or closing as these would just be more sections

Issues:
* Who has resposiblity for formatting default values (like the separator for enumberables)?
  * GetDefaultValue is also available and we will have a helper for standard output
  * Inspector - let's get a default from the start
  * Configuration - let's have common place to set things
  * The lambda where it is used - combine all customizations 
* Similar question for usage
* Similar alias concatenation

Current answer:

Configuration will have a series of "Fallback...Text" methods.
* `Falback` chose over `Default` because it avoids `DefaultDefaultValueText`
* `Falback` chose over `Standard` as that has judgement
* `Fallback` is a poor choice because there is no actual fallback mechanism


